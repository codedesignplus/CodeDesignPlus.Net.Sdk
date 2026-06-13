# CodeDesignPlus.Net.Resilience

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Resilience&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Resilience)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Resilience&metric=bugs)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Resilience)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Resilience&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Resilience)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Resilience&metric=coverage)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Resilience)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Resilience&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Resilience)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Resilience&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Resilience)

## Description

The `CodeDesignPlus.Net.Resilience` library provides transversal resilience patterns (retry, circuit breaker, timeout) for HTTP clients and soft error retry pipelines with OpenTelemetry integration. Built on top of Microsoft.Extensions.Http.Resilience (Polly 8).

## Table of Contents

- [About The Project](#about-the-project)
- [Installation](#installation)
- [Usage](#usage)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## About The Project

The `CodeDesignPlus.Net.Resilience` library provides a standardized way to add resilience patterns to HTTP clients across all CodeDesignPlus microservices.

### Key Features

- **Transport Resilience**: Retry with exponential backoff + circuit breaker for HTTP 5xx and timeouts via `AddResiliencePolicies()`.
- **Soft Error Retry**: Body-inspection retry pipeline for cases where HTTP 200 contains an application-level error (e.g., payment gateway returning `code=ERROR`).
- **OpenTelemetry Integration**: Automatic Polly meter and trace source registration via `AddResilience()`.
- **Configurable per Provider**: `ResilienceOptions` class embeddable in any provider-specific options.

## Installation

```bash
dotnet add package CodeDesignPlus.Net.Resilience
```

## Usage

```csharp
// Register OTEL telemetry for Polly
services.AddResilience();

// Add transport resilience to a named HttpClient
services.AddHttpClient("PaymentProvider", client =>
{
    client.BaseAddress = new Uri("https://api.provider.com");
})
.AddResiliencePolicies(providerOptions.Resilience);

// Build a soft error retry pipeline in your adapter
var pipeline = SoftErrorPipelineFactory.Create(options.Resilience, loggerFactory);
```

## Roadmap

Refer to the [roadmap](https://github.com/codedesignplus/CodeDesignPlus.Net.Sdk/issues) for planned features and improvements.

## Contributing

Contributions are welcome! Please read the [contributing guidelines](https://github.com/codedesignplus/CodeDesignPlus.Net.Sdk/blob/main/CONTRIBUTING.md) before submitting pull requests.

## License

Distributed under the LGPL License. See `LICENSE.md` for more information.

## Contact

CodeDesignPlus - support@codedesignplus.com
