# CodeDesignPlus.Net.RabbitMQ

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.RabbitMQ&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.RabbitMQ)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.RabbitMQ&metric=bugs)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.RabbitMQ)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.RabbitMQ&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.RabbitMQ)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.RabbitMQ&metric=coverage)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.RabbitMQ)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.RabbitMQ&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.RabbitMQ)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.RabbitMQ&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.RabbitMQ)


## Description
The `CodeDesignPlus.Net.RabbitMQ` library is part of the CodeDesignPlus.Net SDK, designed to provide a robust and flexible implementation for integrating RabbitMQ messaging into .NET applications. This library abstracts the complexities of RabbitMQ, offering a simplified interface for configuring and managing RabbitMQ connections, channels, and queues, which are essential for developing scalable and reliable message-driven applications.

## Table of Contents
- [About The Project](#about-the-project)
- [Installation](#installation)
- [Usage](#usage)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## About The Project
The `CodeDesignPlus.Net.RabbitMQ` library is part of the CodeDesignPlus.Net SDK, designed to provide a robust and flexible implementation for integrating RabbitMQ messaging into .NET applications. This library abstracts the complexities of RabbitMQ, offering a simplified interface for configuring and managing RabbitMQ connections, channels, and queues, which are essential for developing scalable and reliable message-driven applications.

## Key Features
- Flexible Configuration: 
  - The library provides detailed configuration options for RabbitMQ, including settings for host, port, username, password, retry intervals, and more. This allows developers to fine-tune their RabbitMQ setup to meet specific application requirements.

  - Configuration options are encapsulated in the RabbitMQOptions class, which supports validation and default values.

- Queue Management: 
  - It includes comprehensive support for managing RabbitMQ queues, including defining message TTL, queue expiration, maximum length, and priority settings.

  - Configurations are managed through the QueueArguments class, which validates and compiles these settings into a dictionary of arguments for RabbitMQ.

- Connection Handling: 
  - The library handles RabbitMQ connections efficiently, with retry mechanisms to ensure robust connectivity.
  - The RabbitConnection class manages the lifecycle of RabbitMQ connections, including creating and disposing of connections, and handling retries on failure.

- Channel Provisioning: 
  - It supports dynamic provisioning of RabbitMQ channels for publishing and consuming messages, ensuring that each domain event type has its dedicated channel.

  - The ChannelProvider class manages the creation and retrieval of channels based on event types, ensuring efficient message routing.

- Event Handling: 
  - The library simplifies the process of declaring exchanges and managing consumer tags for domain events, facilitating smooth event-driven architecture implementations.

  - Methods in ChannelProvider allow for declaring exchanges and retrieving channels for both publishing and consuming events.

- Dead-Letter Exchange (DLX) Support: 
  - Includes configuration for dead-letter exchanges and queues, helping to manage message failures and retries.

  - The RabbitPubSubService class provides methods for configuring standard and DLX queues, ensuring proper handling of undeliverable messages.
  
These features make the CodeDesignPlus.Net.RabbitMQ library a powerful tool for integrating RabbitMQ in .NET applications, providing flexibility, reliability, and ease of use.

## Installation
To install the package, run the following command:
```bash
dotnet add package CodeDesignPlus.Net.RabbitMQ
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

Project Link: [CodeDesignPlus.Net.RabbitMQ](https://github.com/codedesignplus/CodeDesignPlus.Net.Sdk/tree/main/packages/CodeDesignPlus.Net.RabbitMQ)
