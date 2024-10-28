# CodeDesignPlus.Net.Redis.PubSub

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Redis.PubSub&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Redis.PubSub)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Redis.PubSub&metric=bugs)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Redis.PubSub)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Redis.PubSub&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Redis.PubSub)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Redis.PubSub&metric=coverage)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Redis.PubSub)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Redis.PubSub&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Redis.PubSub)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Redis.PubSub&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Redis.PubSub)


## Description
`CodeDesignPlus.Net.Redis.PubSub` provides a comprehensive logging framework for .NET Core applications. This library simplifies the process of logging application events, errors, and diagnostics, enabling developers to build robust and maintainable logging solutions.

## Table of Contents
- [About The Project](#about-the-project)
- [Installation](#installation)
- [Usage](#usage)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## About The Project
The `CodeDesignPlus.Net.Redis.PubSub` library is a component of the CodeDesignPlus.Net SDK, designed to streamline the implementation of Redis Pub/Sub (Publish/Subscribe) messaging within .NET applications. This library provides an efficient and scalable framework for managing message-based communication, enabling applications to publish and subscribe to events with ease.

### Key Features
- Comprehensive Configuration: The library offers detailed configuration options for Redis Pub/Sub, encapsulated in the RedisPubSubOptions class. This includes enabling/disabling the Pub/Sub functionality and integrating with the application configuration.

- Service Integration: Seamlessly integrates with .NET's dependency injection framework, making it straightforward to add Redis Pub/Sub services to the service collection. The ServiceCollectionExtensions class facilitates this integration, ensuring that all necessary services and configurations are properly registered.

- Event Publishing: Provides robust methods for publishing domain events asynchronously. The RedisPubSubService class manages the publication process, including serializing events and notifying subscribers efficiently.

- Event Subscription: Supports subscribing to domain events asynchronously, allowing applications to react to specific events as they occur. The RedisPubSubService class handles the subscription logic, including deserializing messages and invoking event handlers.

- Error Handling: Implements custom exceptions, such as RedisPubSubException, to manage errors specific to Redis Pub/Sub operations, ensuring that developers can handle and debug issues effectively.

- Extensibility: Designed to be easily extensible, allowing developers to implement custom event handlers and extend the core functionality to suit specific application needs.

These features make the `CodeDesignPlus.Net.Redis.PubSub` library a powerful tool for integrating Redis Pub/Sub messaging in .NET applications, providing flexibility, reliability, and ease of use.

## Installation
To install the package, run the following command:
```bash
dotnet add package CodeDesignPlus.Net.Redis.PubSub
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

Project Link: [CodeDesignPlus.Net.Redis.PubSub](https://github.com/codedesignplus/CodeDesignPlus.Net.Sdk/tree/main/packages/CodeDesignPlus.Net.Redis.PubSub)
