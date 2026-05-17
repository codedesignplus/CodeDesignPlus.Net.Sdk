<!-- PROJECT LOGO -->
<br />
<p align="center">
  <a href="https://github.com/codedesignplus/CodeDesignPlus.Net.Sdk/tree/main/packages/CodeDesignPlus.Net.Hangfire">
    <img src="https://i.imgur.com/PwbGy0o.png" alt="Logo">
  </a>

  <h3 align="center">CodeDesignPlus.Net.Hangfire</h3>

  <p align="center">
    Wrapper de Hangfire para el ecosistema CodeDesignPlus: Redis como storage, auto-descubrimiento de jobs y dashboard opcional.
    <br />
    <a href="https://codedesignplus.com">
      <strong>Explore the docs »</strong>
    </a>
    <br />
    <br />
    <a href="https://github.com/codedesignplus/CodeDesignPlus.Net.Sdk/issues">
      <img src="https://img.shields.io/github/issues/codedesignplus/CodeDesignPlus.Net.Sdk?color=0088ff&style=for-the-badge&logo=github" alt="issues"/>
    </a>
    <a href="https://github.com/codedesignplus/CodeDesignPlus.Net.Sdk/pulls">
      <img src="https://img.shields.io/github/issues-pr/codedesignplus/CodeDesignPlus.Net.Sdk?color=0088ff&style=for-the-badge&logo=github" alt="pull requests"/>
    </a>
  </p>
</p>

## About The Project

`CodeDesignPlus.Net.Hangfire` es un paquete NuGet del SDK de CodeDesignPlus que integra [Hangfire](https://www.hangfire.io/) como motor de background jobs en microservicios .NET 9.

### Key Features

- **Redis Storage**: Usa `IRedisFactory` del ecosistema CodeDesignPlus para obtener la conexión Redis sin configuración adicional.
- **Auto-descubrimiento de jobs**: Detecta automáticamente clases que implementan `IRecurrentJob` en el ensamblado del microservicio.
- **Atributo declarativo**: Decora tus jobs con `[RecurringJobOptions("0 6 * * *")]` para definir el cron directamente en la clase.
- **Dashboard opcional**: Habilita el panel de Hangfire con autenticación básica vía `Dashboard.Enable = true`.
- **Options Pattern**: Configuración centralizada en la sección `Hangfire` de `appsettings.json`.

## Getting Started

### Configuración en appsettings.json

```json
{
  "Hangfire": {
    "Enable": true,
    "Prefix": "hangfire:ms-invoicing:",
    "WorkerCount": 2,
    "Queues": ["default", "critical"],
    "Dashboard": {
      "Enable": true,
      "Username": "admin",
      "Password": "your-secure-password"
    }
  }
}
```

### Registro en Program.cs

```csharp
// Registra Hangfire con Redis y auto-descubre los jobs del ensamblado
builder.Services.AddHangfire<Program>(builder.Configuration);

// En el pipeline HTTP
app.UseHangfireDashboard<Program>(app.Configuration);
```

### Definir un job recurrente

```csharp
[RecurringJobOptions("0 6 * * *", jobId: "daily-report", timezone: "America/Bogota")]
public class DailyReportJob : IRecurrentJob
{
    public Task ExecuteAsync(IJobCancellationToken cancellationToken)
    {
        // lógica del job
        return Task.CompletedTask;
    }
}
```

## License

Distributed under the LGPL-3.0 License. See [LICENSE](LICENSE.md) for more information.

## Contact

CodeDesignPlus - support@codedesignplus.com

Project Link: [CodeDesignPlus.Net.Sdk](https://github.com/codedesignplus/CodeDesignPlus.Net.Sdk)
