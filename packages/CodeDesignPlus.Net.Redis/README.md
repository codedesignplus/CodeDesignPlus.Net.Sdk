# CodeDesignPlus.Net.Redis

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Redis&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Redis)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Redis&metric=bugs)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Redis)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Redis&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Redis)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Redis&metric=coverage)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Redis)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Redis&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Redis)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Redis&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Redis)


## Description
The `CodeDesignPlus.Net.Redis` library is a component of the CodeDesignPlus.Net SDK, designed to facilitate the integration of Redis within .NET applications. This library offers a streamlined interface for configuring and managing Redis connections, enhancing performance and reliability in distributed systems through efficient caching and data storage solutions.

## Table of Contents
- [About The Project](#about-the-project)
- [Installation](#installation)
- [Usage](#usage)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## About The Project
The `CodeDesignPlus.Net.Redis` library is a component of the CodeDesignPlus.Net SDK, designed to facilitate the integration of Redis within .NET applications. This library offers a streamlined interface for configuring and managing Redis connections, enhancing performance and reliability in distributed systems through efficient caching and data storage solutions.

### Key Features
- Flexible Redis Configuration: The library provides comprehensive configuration options for Redis, including settings for connection strings, SSL certificates, and thread priorities. This is managed through the RedisOptions and Instance classes, which ensure that Redis instances can be customized to meet specific application needs.

- Connection Management: Robust handling of Redis connections, including automatic retries and connection event management, is implemented to ensure high availability and resilience. The RedisService class orchestrates these connections, including event handling for configuration changes, connection failures, and restorations.

- Service Factory: The RedisServiceFactory class facilitates the creation and management of Redis services, ensuring that each Redis instance is correctly initialized and maintained. This factory pattern helps in managing multiple Redis instances efficiently.

- Error Handling: Detailed error handling mechanisms, including custom exceptions such as RedisException, provide clarity and control over Redis operation failures, enabling developers to implement graceful degradation and recovery strategies.

- Dependency Injection Support: The library integrates seamlessly with .NET's dependency injection framework, allowing for easy setup and management of Redis services within an application's service collection. This is achieved through extension methods provided in the ServiceCollectionExtensions class.

These features make the CodeDesignPlus.Net.Redis library a powerful tool for developers seeking to integrate Redis into their .NET applications, providing flexibility, reliability, and ease of use.

## Installation
To install the package, run the following command:
```bash
dotnet add package CodeDesignPlus.Net.Redis
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

Project Link: [CodeDesignPlus.Net.Redis](https://github.com/codedesignplus/CodeDesignPlus.Net.Sdk/tree/main/packages/CodeDesignPlus.Net.Redis)
