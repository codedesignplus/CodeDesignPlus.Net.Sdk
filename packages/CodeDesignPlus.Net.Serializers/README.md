# CodeDesignPlus.Net.Serializers

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Serializers&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Serializers)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Serializers&metric=bugs)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Serializers)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Serializers&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Serializers)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Serializers&metric=coverage)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Serializers)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Serializers&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Serializers)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Serializers&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Serializers)


## Description
The `CodeDesignPlus.Net.Serializers` library is a component of the CodeDesignPlus.Net SDK, designed to provide serialization and deserialization capabilities for .NET applications. This library offers a standardized approach to handling JSON data, ensuring that objects can be easily converted to and from JSON format with customizable settings and contract resolvers.

## Table of Contents
- [About The Project](#about-the-project)
- [Installation](#installation)
- [Usage](#usage)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## About The Project
The `CodeDesignPlus.Net.Serializers` library is a component of the CodeDesignPlus.Net SDK, designed to provide serialization and deserialization capabilities for .NET applications. This library offers a standardized approach to handling JSON data, ensuring that objects can be easily converted to and from JSON format with customizable settings and contract resolvers.

### Key Features
- JSON Serialization and Deserialization: The library includes methods for serializing objects to JSON strings and deserializing JSON strings to objects. It supports various customization options through settings and formatting parameters.

- Event Contract Resolver: A specialized EventContractResolver class is provided for the serialization of domain events. This class allows developers to specify properties to ignore during serialization, ensuring that sensitive or unnecessary data is excluded.

- Custom Exceptions: The library defines a SerializersException class to handle errors that occur during serialization and deserialization processes, providing detailed error messages and custom error collections.

- Extensible and Configurable: The library supports extending and configuring serialization settings, making it adaptable to different application requirements and ensuring consistent data handling across the application.

These features make the `CodeDesignPlus.Net.Serializers` library a powerful tool for managing JSON data in .NET applications, providing flexibility, reliability, and ease of use.

## Installation
To install the package, run the following command:
```bash
dotnet add package CodeDesignPlus.Net.Serializers
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

Project Link: [CodeDesignPlus.Net.Serializers](https://github.com/codedesignplus/CodeDesignPlus.Net.Sdk/tree/main/packages/CodeDesignPlus.Net.Serializers)
