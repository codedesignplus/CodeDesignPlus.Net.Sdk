# CodeDesignPlus.Net.EventStore
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.EventStore&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.EventStore)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.EventStore&metric=bugs)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.EventStore)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.EventStore&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.EventStore)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.EventStore&metric=coverage)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.EventStore)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.EventStore&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.EventStore)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.EventStore&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.EventStore)


## Description
The `CodeDesignPlus.Net.EventStore` project is a robust library designed to facilitate event sourcing in .NET applications using EventStore. It provides abstractions, services, and utilities for efficiently managing events, maintaining event streams, and ensuring data consistency. The project aims to simplify the implementation of event-driven architectures, enabling scalable and maintainable applications.

## Table of Contents
- [About The Project](#about-the-project)
- [Installation](#installation)
- [Usage](#usage)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## About The Project
`CodeDesignPlus.Net.EventStore` provides a comprehensive framework for implementing event sourcing in .NET Core applications. This library simplifies the process of capturing, storing, and replaying events, enabling developers to build robust and scalable event-driven systems.

### Key Features
- Event Store Service: Implements `IEventStoreService` for managing event streams and interactions with EventStore.
- Exception Handling: Custom `EventStoreException` for handling errors specific to event store operations.
- Event Management: Methods for appending events, retrieving event versions, and loading events for specific categories and aggregate IDs.
- Serialization: Utilizes JSON serialization for event data and metadata, ensuring efficient storage and retrieval.
- Integration with EventStore: Uses `EventStore.ClientAPI` for seamless communication with EventStore, supporting operations like appending to streams and reading events.
- Configuration Constants: Provides constants like `EventStoreFactoryConst.Core` for standardized configuration management.

## Installation
To install the package, run the following command:
```bash
dotnet add package CodeDesignPlus.Net.EventStore
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

Project Link: [CodeDesignPlus.Net.EventStore](https://github.com/codedesignplus/CodeDesignPlus.Net.Sdk/tree/main/packages/CodeDesignPlus.Net.EventStore)
