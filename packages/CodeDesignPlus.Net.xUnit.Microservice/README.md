# CodeDesignPlus.Net.xUnit.Microservice

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.xUnit.Microservice&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.xUnit.Microservice)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.xUnit.Microservice&metric=bugs)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.xUnit.Microservice)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.xUnit.Microservice&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.xUnit.Microservice)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.xUnit.Microservice&metric=coverage)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.xUnit.Microservice)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.xUnit.Microservice&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.xUnit.Microservice)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.xUnit.Microservice&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.xUnit.Microservice)


## Description
`CodeDesignPlus.Net.xUnit.Microservice` provides a comprehensive logging framework for .NET Core applications. This library simplifies the process of logging application events, errors, and diagnostics, enabling developers to build robust and maintainable logging solutions.

## Table of Contents
- [About The Project](#about-the-project)
- [Installation](#installation)
- [Usage](#usage)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## About The Project
The `CodeDesignPlus.Net.xUnit.Microservice` project is a testing framework designed to streamline the development and testing of microservices within .NET applications. It provides a robust set of tools and services to facilitate the creation, configuration, and management of test environments, ensuring comprehensive test coverage and efficient test execution.

### Key Features
- Server Management: Classes like `Server<TProgram>` and `ServerBase<TProgram>` for configuring and managing web application servers for testing purposes.
- In-Memory Logging: Integration with in-memory logging providers (`InMemoryLoggerProvider`) to capture and analyze log messages during tests.
- Docker Compose Support: Classes like DockerCompose and `ServerCompose` for managing Docker Compose configurations and services, enabling isolated test environments.
- Custom Attributes: Attributes such as `CommandAttribute<TAssemblyScan>` for providing data to test methods that validate commands, supporting dynamic test data generation.
- Middleware Configuration: Support for adding authentication schemes and other middleware components to the test server configuration, ensuring realistic test scenarios.
- gRPC Channels: Creation and management of gRPC channels (`GrpcChannel`) for making gRPC calls to the test server, supporting comprehensive integration tests.

## Installation
To install the package, run the following command:
```bash
dotnet add package CodeDesignPlus.Net.xUnit.Microservice
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

Project Link: [CodeDesignPlus.Net.xUnit.Microservice](https://github.com/codedesignplus/CodeDesignPlus.Net.Sdk/tree/main/packages/CodeDesignPlus.Net.xUnit.Microservice)
