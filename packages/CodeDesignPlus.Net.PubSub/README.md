# CodeDesignPlus.Net.PubSub

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.PubSub&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.PubSub)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.PubSub&metric=bugs)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.PubSub)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.PubSub&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.PubSub)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.PubSub&metric=coverage)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.PubSub)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.PubSub&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.PubSub)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.PubSub&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.PubSub)


## Description
`CodeDesignPlus.Net.PubSub` provides a robust framework for implementing publish-subscribe patterns in .NET Core applications. This library simplifies the process of publishing and subscribing to messages, enabling developers to build scalable and decoupled event-driven systems.

## Table of Contents
- [About The Project](#about-the-project)
- [Installation](#installation)
- [Usage](#usage)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## About The Project
`CodeDesignPlus.Net.PubSub` provides a robust framework for implementing publish-subscribe patterns in .NET Core applications. This library simplifies the process of publishing and subscribing to messages, enabling developers to build scalable and decoupled event-driven systems.

### Key Features
1. Ease of Use: Simplifies the implementation of publish-subscribe patterns.
2. Scalability: Designed to support scalable and decoupled event-driven architectures.
3. Integration: Seamlessly integrates with other CodeDesignPlus.Net libraries.
4. Comprehensive Documentation: Includes extensive documentation and support for common tasks.

## Installation
To install the package, run the following command:
```bash
dotnet add package CodeDesignPlus.Net.PubSub
```

## Usage
For more information regarding the library, you can visit our documentation at [CodeDesignPlus Doc](https://doc.codedesignplus.com)

## Sincronización inicial de proyecciones (fotos)

Una copia local que se alimenta de eventos solo recibe lo que pasa desde que existe su cola. Para que nazca completa,
la copia **pide una foto** al arrancar y el dueño **contesta** con un evento de foto por agregado, dirigido a quien la
pidió y paginado. La copia aplica con upsert y marca la fuente como sincronizada cuando aplicó todas las fotos que
anunció el dueño.

### Lado del dueño

1. Declare el evento de foto del agregado, con su estado completo, heredando de `SnapshotDomainEvent`:

   ```csharp
   [EventKey<AssemblyAggregate>(1, "AssemblySnapshotDomainEvent")]
   public class AssemblySnapshotDomainEvent(Guid aggregateId, string title, Instant scheduledDate, Instant? completedAt, bool isCancelled, Guid tenant, Guid? eventId = null, Instant? occurredAt = null, Dictionary<string, object>? metadata = null)
       : SnapshotDomainEvent(aggregateId, tenant, eventId, occurredAt, metadata) { ... }
   ```

2. Implemente `ISnapshotSource<TSnapshot>`: devuelve una página de fotos con orden estable (por `Id`), de una
   copropiedad o de todas. Póngala en el ensamblado del worker. El SDK la descubre y se suscribe solo a
   `SnapshotRequestedDomainEvent`; contesta las peticiones que nombran su `Core:AppName`, el agregado y el nombre de
   esa foto. Un agregado puede tener varias fotos (una por vista que copian otros), cada una con su fuente.

   ```csharp
   public class AssemblySnapshotSource(IAssemblyRepository repository) : ISnapshotSource<AssemblySnapshotDomainEvent>
   {
       public async Task<IReadOnlyList<AssemblySnapshotDomainEvent>> GetPageAsync(Guid? tenant, int page, int pageSize, CancellationToken cancellationToken)
       {
           var assemblies = await repository.GetPageAsync(tenant, page, pageSize, cancellationToken);

           return [.. assemblies.Select(x => new AssemblySnapshotDomainEvent(x.Id, ...))];
       }
   }
   ```

El tamaño de página es `PubSub:SnapshotPageSize` (100 por defecto).

### Lado de la copia

1. Declare el gemelo del evento de foto: la clave nombra al dueño (aplicación) y al agregado (entidad). De ahí sale a
   quién se le pide.

   ```csharp
   [EventKey("AssemblyAggregate", 1, "AssemblySnapshotDomainEvent", "ms-assemblies", "kappali")]
   public class AssemblySnapshotDomainEvent(...) : SnapshotDomainEvent(...)
   ```

2. Consuma la foto heredando de `SnapshotHandler<TSnapshot>` e implemente `ApplyAsync` como un upsert idempotente.
   Para no pisar un evento en vivo más nuevo, la copia guarda el `OccurredAt` del último evento del dueño que aplicó
   (un campo nuevo fuera del constructor) y solo reemplaza si `snapshot.Supersedes(copia.SourceChangedAt)`.

   ```csharp
   [QueueName<AssemblyProjectionAggregate>("AssemblySnapshotHandler")]
   public class AssemblySnapshotHandler(IAssemblyProjectionRepository repository, IOptions<CoreOptions> coreOptions)
       : SnapshotHandler<AssemblySnapshotDomainEvent>(coreOptions)
   {
       protected override async Task ApplyAsync(AssemblySnapshotDomainEvent snapshot, CancellationToken cancellationToken) { ... }
   }
   ```

3. Nada más. Con un `SnapshotHandler` en el ensamblado, el SDK:
   - espera a que estén suscritos todos los consumidores (una foto publicada antes de que exista la cola se pierde);
   - mira en `IProjectionSyncStore` si esa foto ya está sincronizada (un documento por dueño y evento de foto) (`CodeDesignPlus.Net.Mongo` lo registra:
     colección `ProjectionSyncState`); si no, publica `SnapshotRequestedDomainEvent` y lo anota;
   - cuenta cada foto que `ApplyAsync` aplica sin lanzar (una vez por mensaje, aunque el bus la reentregue) y, al llegar
     `SnapshotCompletedDomainEvent` de esa petición, anota cuántas anunció el dueño. La fuente queda sincronizada solo
     cuando las dos cifras coinciden, lo vea primero el cierre o la última foto: llegan por colas distintas y en
     cualquier orden;
   - si el dueño cerró y faltan fotos (alguna acabó en la cola de descartados), la deja **incompleta**. Al siguiente
     arranque, o pasado `PubSub:SnapshotRetryInterval` (una hora por defecto; `00:00:00` lo desactiva) desde la última
     petición, la vuelve a pedir y deja una advertencia en el log con cuántas aplicó de cuántas. Lo mismo si el dueño
     no contestó.

Para reparar una copropiedad a mano, inyecte `ISnapshotRequester` y llame a
`RequestAsync<AssemblySnapshotDomainEvent>(tenant, token)` desde un endpoint de soporte. Para volver a pedir la foto
completa, borre el documento de esa foto en `ProjectionSyncState` y reinicie el micro.

La cuenta vive en dos colecciones más de la base de la copia: `ProjectionSnapshotProgress` (una por petición: aplicadas y
anunciadas) y `ProjectionSnapshotReceipt` (una por foto aplicada, para no contar dos veces una reentrega). Se borran al
quedar sincronizada la foto. Una marca de `ProjectionSyncState` sin `Applied` se puso antes de que se contaran las fotos:
no prueba que la copia esté completa y la foto se vuelve a pedir una vez.

### Lo que hay que saber

- Las peticiones y los cierres son contratos de plataforma con clave fija (`codedesignplus.ms-snapshots.v1.projectionsnapshot.*`):
  todas las aplicaciones publican y escuchan en el mismo tema y descartan lo que no las nombra.
- Las fotos van al tema del evento de foto de cada pareja; si dos copias del mismo agregado sincronizan, cada una
  descarta la que pidió la otra (`RequestedBy`).
- Pedir dos veces (dos réplicas, un reinicio antes del cierre) no duplica: la copia aplica con upsert.
- Se cuenta una foto por mensaje y se asume un solo `SnapshotHandler` por evento de foto en cada aplicación.
- Todo es aditivo: un consumidor desplegado que no conoce los eventos nuevos no cambia.

## Roadmap
Refer to [issues](https://github.com/codedesignplus/CodeDesignPlus.Net.Sdk/issues) for a list of proposed features and known issues.

## Roadmap
1. Fork the Project
2. Create your Feature Branch (git checkout -b features/AmazingFeature)
3. Commit your Changes (git commit -m 'Add some AmazingFeature')
4. Push to the Branch (git push origin feature/AmazingFeature)
5. Open a Pull Request

## License
Distributed under the MIT License. See LICENSE for more information.

## Contact
CodeDesignPlus - @CodeDesignPlus - wliscano@codedesignplus.com

Project Link: [CodeDesignPlus.Net.PubSub](https://github.com/codedesignplus/CodeDesignPlus.Net.Sdk/tree/main/packages/CodeDesignPlus.Net.PubSub)
