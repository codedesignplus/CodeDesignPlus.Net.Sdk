# CodeDesignPlus.Net.EFCore
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.EFCore&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.EFCore)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.EFCore&metric=bugs)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.EFCore)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.EFCore&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.EFCore)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.EFCore&metric=coverage)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.EFCore)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.EFCore&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.EFCore)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.EFCore&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.EFCore)


## Description
The `CodeDesignPlus.Net.EFCore` project provides an abstraction layer for handling common Entity Framework Core operations in .NET Core applications. This library simplifies data access by offering interfaces and classes to perform CRUD operations, manage options, and configure claims for user information retrieval.

## Table of Contents
- [About The Project](#about-the-project)
- [Installation](#installation)
- [Usage](#usage)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## About The Project
The `CodeDesignPlus.Net.EFCore` project provides an abstraction layer for handling common Entity Framework Core operations in .NET Core applications. This library simplifies data access by offering interfaces and classes to perform CRUD operations, manage options, and configure claims for user information retrieval.

### Key Features
- EFCoreOptions: Configuration options for EFCore, including claims identity settings.
- ClaimsOption: Defines claims for obtaining user information such as name, ID, email, and role.
- OperationBase: Provides base operations for creating, updating, and deleting records in the repository, while assigning information to the transversal properties of the entity.
- RepositoryBase: Abstract class implementing common methods for interacting with the database.
- EFCoreException: Custom exception class for handling EFCore-specific errors.

## Installation
To install the package, run the following command:
```bash
dotnet add package CodeDesignPlus.Net.EFCore
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

Project Link: [CodeDesignPlus.Net.EFCore](https://github.com/codedesignplus/CodeDesignPlus.Net.Sdk/tree/main/packages/CodeDesignPlus.Net.EFCore)
