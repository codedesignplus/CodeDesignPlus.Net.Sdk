# CodeDesignPlus.Net.Vault

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Vault&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Vault)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Vault&metric=bugs)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Vault)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Vault&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Vault)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Vault&metric=coverage)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Vault)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Vault&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Vault)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Vault&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Vault)


## Description
The `CodeDesignPlus.Net.Vault` project provides a comprehensive set of tools and services for integrating with HashiCorp Vault. It facilitates secure storage, management, and access to secrets and sensitive information within .NET applications. The project includes configurations, service implementations, and extensions to streamline the integration process with Vault, ensuring robust security practices.

## Table of Contents
- [About The Project](#about-the-project)
- [Installation](#installation)
- [Usage](#usage)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## About The Project
The `CodeDesignPlus.Net.Vault` project provides a comprehensive set of tools and services for integrating with HashiCorp Vault. It facilitates secure storage, management, and access to secrets and sensitive information within .NET applications. The project includes configurations, service implementations, and extensions to streamline the integration process with Vault, ensuring robust security practices.

### Key Features
- Vault Exception Handling: Custom exception classes like VaultException to handle errors within the Vault integration.
- Vault Client Factory: A factory class for creating instances of IVaultClient with various configuration options, including support for Kubernetes.
- Configuration Providers: Classes like VaultConfigurationProvider to load configuration settings from the Vault service.
- Service Extensions: Extension methods for adding Vault services (`AddVault`) and configurations (`AddVault`) to .NET's IServiceCollection and `IConfigurationBuilder`.
- Transit Operations: Interfaces (`IVaultTransit`) and implementations (`VaultTransit`) for Vault Transit operations, providing methods for encryption and decryption of data.
- Options Classes: Various options classes (`VaultOptions`, `Transit`, `KeyVault`) to configure different aspects of the Vault integration, such as address, role, secret ID, and specific service settings.

## Installation
To install the package, run the following command:
```bash
dotnet add package CodeDesignPlus.Net.Vault
```

## Usage
For more information regarding the library, you can visit our documentation at [CodeDesignPlus Doc](https://doc.codedesignplus.com)

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
CodeDesignPlus - @CodeDesignPlus - codedesignplus@outlook.com

Project Link: [CodeDesignPlus.Net.Vault](https://github.com/codedesignplus/CodeDesignPlus.Net.Sdk/tree/main/packages/CodeDesignPlus.Net.Vault)
