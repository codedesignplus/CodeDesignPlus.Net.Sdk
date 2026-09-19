# CodeDesignPlus.Net.ServiceBus

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.ServiceBus&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.ServiceBus)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.ServiceBus&metric=bugs)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.ServiceBus)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.ServiceBus&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.ServiceBus)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.ServiceBus&metric=coverage)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.ServiceBus)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.ServiceBus&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.ServiceBus)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.ServiceBus&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.ServiceBus)

## Description

`CodeDesignPlus.Net.ServiceBus` implementa el contrato de publicación y suscripción del SDK sobre Azure Service Bus. Es un transporte alternativo a `CodeDesignPlus.Net.RabbitMQ`: los handlers, los eventos de dominio y los atributos `EventKey` y `QueueName` no cambian, solo cambia por dónde viajan los mensajes.

## Tabla de contenido

- [Sobre el proyecto](#sobre-el-proyecto)
- [Cómo se traduce el modelo de RabbitMQ](#cómo-se-traduce-el-modelo-de-rabbitmq)
- [Instalación](#instalación)
- [Uso](#uso)
- [Autenticación](#autenticación)
- [Reintentos y dead-letter](#reintentos-y-dead-letter)
- [Pruebas](#pruebas)
- [Límites que conviene conocer](#límites-que-conviene-conocer)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## Sobre el proyecto

La librería implementa `IMessage` de `CodeDesignPlus.Net.PubSub`. Todo lo que rodea al transporte —el descubrimiento de handlers, los servicios en segundo plano que abren las suscripciones, el seguimiento de cuáles están listas y la cola en memoria opcional— lo sigue aportando `CodeDesignPlus.Net.PubSub` sin cambios.

## Cómo se traduce el modelo de RabbitMQ

| RabbitMQ | Azure Service Bus |
|---|---|
| Exchange `fanout` por tipo de evento | Topic, con el mismo nombre derivado de `EventKey` |
| Una cola por handler, enlazada al exchange | Una suscripción por handler |
| Exchange y cola `.dlx` declarados a mano | DLQ nativa, `<topic>/Subscriptions/<sub>/$DeadLetterQueue` |
| `x-delivery-limit` | `MaxDeliveryCount` |
| `x-message-ttl` | `DefaultMessageTimeToLive` |
| `BasicAck` / `BasicNack` | `CompleteMessageAsync` / `AbandonMessageAsync` / `DeadLetterMessageAsync` |
| Cabecera `x-delivery-count` | `ServiceBusReceivedMessage.DeliveryCount` |

El nombre del topic es el mismo que el del exchange, así que el inventario de eventos no cambia. **El nombre de la suscripción sí**: Azure lo limita a 50 caracteres y casi todos los nombres de cola actuales los superan. La librería lo resuelve sola, sin tocar los `[QueueName]`:

```
subscription = "{appName}.{action}"                  si cabe en 50
             = prefijo[41] + "-" + sha256(logico)[8] si no cabe
```

El resumen se calcula sobre el nombre lógico completo (negocio, versión y entidad incluidos), de modo que dos handlers cuyo prefijo coincide tras el recorte siguen resolviendo a suscripciones distintas.

## Instalación

```bash
dotnet add package CodeDesignPlus.Net.ServiceBus
```

## Uso

```csharp
builder.Services.AddServiceBus<Program>(builder.Configuration);
```

```json
{
  "Core": { "AppName": "ms-invoicing", "Business": "kappali", "Version": "v1" },
  "ServiceBus": {
    "Enable": true,
    "FullyQualifiedNamespace": "sb-kappali-stg.servicebus.windows.net",
    "UseQueue": false,
    "EnableDiagnostic": true
  }
}
```

| Opción | Valor por defecto | Para qué sirve |
|---|---|---|
| `Enable` | `false` | Con `false` no se registra nada y no se abre conexión |
| `FullyQualifiedNamespace` | — | Namespace, cuando se autentica con Entra ID |
| `ConnectionString` | `null` | Alternativa a lo anterior; tiene prioridad si se define |
| `ManagementConnectionString` | `null` | Solo para el emulador, que separa el plano de gestión |
| `AutoProvisionEntities` | `true` | Crear topics y suscripciones al vuelo |
| `MaxRetry` | `10` | Reintentos antes de mandar el mensaje a la DLQ |
| `RetryIntervalMs` / `MaxRetryIntervalMs` | `2000` / `60000` | Base y techo del backoff exponencial |
| `MaxConcurrentCalls` | `4` | Mensajes procesados a la vez por suscripción |
| `PrefetchCount` | `0` | Sin prelectura: la espera del backoff retiene el bloqueo |
| `LockDurationSeconds` | `300` | Bloqueo del mensaje, máximo que admite Service Bus |
| `MaxAutoLockRenewalMinutes` | `10` | Debe superar a `MaxRetryIntervalMs`; se valida |
| `MessageTimeToLiveHours` | `48` | Caducidad del mensaje en la suscripción |
| `RegisterHealthCheck` | `true` | Registra los chequeos con etiqueta `ready` |

## Autenticación

Por defecto se usa `DefaultAzureCredential` contra `FullyQualifiedNamespace`, lo que en AKS se traduce en Workload Identity. La identidad necesita el rol **`Azure Service Bus Data Owner`**, porque `AutoProvisionEntities` crea entidades; con las entidades ya creadas y `AutoProvisionEntities` en `false` bastan `Data Sender` y `Data Receiver`.

`ConnectionString` existe para el desarrollo local y el emulador, que no admiten Entra ID.

## Reintentos y dead-letter

```
error de negocio (CodeDesignPlusException)  ->  DLQ inmediata, motivo "BusinessError"
error de infraestructura, quedan reintentos ->  espera creciente y AbandonMessageAsync
error de infraestructura, agotados          ->  DLQ, motivo "MaxRetriesExceeded"
```

La espera crece de forma exponencial desde `RetryIntervalMs` hasta `MaxRetryIntervalMs`, con una dispersión de hasta el 20% para que varias réplicas que fallaron a la vez no reintenten a la vez. **Transcurre en proceso, con el bloqueo del mensaje retenido y renovándose solo.** La alternativa —completar el mensaje y reprogramar una copia— libera el bloqueo antes, pero completar y reprogramar no son operaciones atómicas y una caída entre ambas pierde el evento.

La suscripción se crea con `MaxDeliveryCount = MaxRetry + 1`: decide primero el consumidor, que es quien sabe distinguir un error de negocio, y el broker queda como red de seguridad para el caso de que el proceso muera antes de decidir.

## Pruebas

Las pruebas de integración usan el emulador oficial (`mcr.microsoft.com/azure-messaging/servicebus-emulator` más su SQL Server), levantado por `ServiceBusCollectionFixture` de `CodeDesignPlus.Net.xUnit`.

El emulador **impone topes que Azure no tiene** y que obligan a bajar los valores de producción en las pruebas:

| | Emulador | Azure |
|---|---|---|
| `MaxDeliveryCount` | 1 a 10 | hasta 2.147.483.647 |
| `DefaultMessageTimeToLive` | 1 s a 1 h | hasta `TimeSpan.MaxValue` |
| `UserMetadata` de un topic | se descarta | se conserva |
| Plano de gestión | puerto aparte (5300) | mismo extremo |

## Límites que conviene conocer

- **256 KB por mensaje** en el nivel Standard, propiedades incluidas. La librería comprueba el tamaño antes de enviar y lanza un `ServiceBusPubSubException` que nombra el evento y su tamaño.
- **1.000 operaciones por segundo** por namespace en Standard.
- Un mensaje publicado en un topic con N suscripciones son **N entregas facturables**.
- 10.000 topics por namespace y 2.000 suscripciones por topic.

## Contributing

Please read [CONTRIBUTING](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE.md) file for details.

## Contact

CodeDesignPlus - [@CodeDesignPlus](https://www.codedesignplus.com) - custom.software@codedesignplus.com
