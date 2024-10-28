# CodeDesignPlus.Net.Security

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Security&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Security)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Security&metric=bugs)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Security)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Security&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Security)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Security&metric=coverage)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Security)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Security&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Security)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Security&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Security)


## Description
The `CodeDesignPlus.Net.Security` library is a component of the CodeDesignPlus.Net SDK, designed to provide robust security features for .NET applications. This library encompasses various aspects of security, including authentication, authorization, and user context management, ensuring that applications can securely manage user identities and permissions.

## Table of Contents
- [About The Project](#about-the-project)
- [Installation](#installation)
- [Usage](#usage)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## About The Project
The `CodeDesignPlus.Net.Security` library is a component of the CodeDesignPlus.Net SDK, designed to provide robust security features for .NET applications. This library encompasses various aspects of security, including authentication, authorization, and user context management, ensuring that applications can securely manage user identities and permissions.

### Key Features
- User Context Management: The library provides an interface (IUserContext) and a concrete implementation (UserContext) for managing user information such as ID, name, email, and claims. This allows applications to easily access authenticated user details during a request.

- Authentication and Authorization: The library includes extension methods for setting up security services in an IServiceCollection and configuring authentication in an IApplicationBuilder. This ensures seamless integration of JWT Bearer authentication and authorization in .NET applications.

- Claims and Headers Handling: It offers utilities to handle claims and headers within user context, enabling the extraction of specific user information from JWT tokens and HTTP headers.

- Security Configuration: The SecurityOptions class provides comprehensive configuration options for security settings, including authority, client ID, certificate paths, and validation parameters, ensuring that applications can be securely configured according to their requirements.

- Error Handling: Custom extensions for handling authentication failures are provided, allowing applications to respond appropriately to various security token exceptions.

These features make the `CodeDesignPlus.Net.Security` library an essential tool for integrating comprehensive security mechanisms in .NET applications, providing flexibility, reliability, and ease of use.

## Installation
To install the package, run the following command:
```bash
dotnet add package CodeDesignPlus.Net.Security
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

Project Link: [CodeDesignPlus.Net.Security](https://github.com/codedesignplus/CodeDesignPlus.Net.Sdk/tree/main/packages/CodeDesignPlus.Net.Security)
