# CodeDesignPlus.Net.xUnit

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.xUnit&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.xUnit)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.xUnit&metric=bugs)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.xUnit)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.xUnit&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.xUnit)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.xUnit&metric=coverage)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.xUnit)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.xUnit&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.xUnit)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.xUnit&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.xUnit)


## Description
The `CodeDesignPlus.Net.xUnit` project provides a set of helper classes and tools for managing Docker containers for various services commonly used in integration testing. It leverages Docker Compose to streamline the setup and teardown of these services, ensuring a consistent and isolated test environment.

## Table of Contents
- [About The Project](#about-the-project)
- [Installation](#installation)
- [Usage](#usage)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## About The Project
The `CodeDesignPlus.Net.xUnit` project provides a set of helper classes and tools for managing Docker containers for various services commonly used in integration testing. It leverages Docker Compose to streamline the setup and teardown of these services, ensuring a consistent and isolated test environment.

### Key Features
1. SQL Server Container: Manages a Docker container for SQL Server, including configuration and port settings.
2. MongoDB Container: Manages a Docker container for MongoDB with Docker Compose configurations.
3. Kafka Container: Sets up a Kafka container, including dynamic port allocation and environment configurations.
4. Vault Container: Handles a Docker container for Vault, including credential management and service initialization.
5. Redis Container: Manages a Redis container and provides options for instances with and without PFX passwords.
6. In-Memory Exporters: Provides in-memory exporters for tracing activities, log records, and metrics for testing purposes.

## Installation
To install the package, run the following command:
```bash
dotnet add package CodeDesignPlus.Net.xUnit
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

Project Link: [CodeDesignPlus.Net.xUnit](https://github.com/codedesignplus/CodeDesignPlus.Net.Sdk/tree/main/packages/CodeDesignPlus.Net.xUnit)
