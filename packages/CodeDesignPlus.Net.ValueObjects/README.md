# CodeDesignPlus.Net.ValueObjects

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.ValueObjects&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.ValueObjects)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.ValueObjects&metric=bugs)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.ValueObjects)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.ValueObjects&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.ValueObjects)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.ValueObjects&metric=coverage)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.ValueObjects)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.ValueObjects&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.ValueObjects)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.ValueObjects&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.ValueObjects)

## Description
The `CodeDesignPlus.Net.ValueObjects` project is a Domain-Driven Design (DDD) Shared Kernel library that provides enterprise-grade, immutable Value Objects. It is designed to ensure domain consistency, financial precision, and structural integrity across all distributed microservices. By centralizing these core domain primitives, this library prevents logic duplication and guarantees a ubiquitous language across your ecosystem.

## Table of Contents
- [CodeDesignPlus.Net.ValueObjects](#codedesignplusnetvalueobjects)
  - [Description](#description)
  - [Table of Contents](#table-of-contents)
  - [About The Project](#about-the-project)
    - [Key Features](#key-features)
  - [Installation](#installation)
  - [Usage](#usage)
  - [Roadmap](#roadmap)
  - [Roadmap](#roadmap-1)
  - [License](#license)
  - [Contact](#contact)

## About The Project
In microservice architectures, dealing with concepts like Money, Currency, and Geographical Locations repetitively can lead to severe data anomalies and precision loss (e.g., floating-point rounding errors). The `CodeDesignPlus.Net.ValueObjects` library solves this by offering strictly encapsulated, mathematically safe, and context-agnostic objects that act as the foundational building blocks for your aggregates and entities.

### Key Features
- **Money Pattern**: Encapsulates monetary amounts with their respective currencies to prevent cross-currency mathematical operations and financial precision loss (ISO 4217 compliant).
- **Standardized Domain Primitives**: Includes robust representations for `Currency`, `Location`, and other cross-cutting geographical and financial concepts.
- **Strict Immutability**: All Value Objects are completely immutable. Once created, their state cannot be altered, ensuring thread safety and historical accuracy (Snapshot pattern).
- **Value Equality**: Implements `IEquatable<T>` and overrides operators (`==`, `!=`) so objects are compared by their structural values, not their memory references.
- **Fail-Fast Validation**: Built-in Domain Guards ensure that invalid states (like negative taxes or invalid ISO codes) are rejected at the exact moment of creation.

## Installation
To install the package, run the following command:
```bash
dotnet add package CodeDesignPlus.Net.ValueObjects
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
CodeDesignPlus - @CodeDesignPlus - wliscano@codedesignplus.com

Project Link: [CodeDesignPlus.Net.ValueObjects](https://github.com/codedesignplus/CodeDesignPlus.Net.Sdk/tree/main/packages/CodeDesignPlus.Net.ValueObjects)