# CodeDesignPlus.Net.Criteria

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Criteria&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Criteria)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Criteria&metric=bugs)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Criteria)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Criteria&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Criteria)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Criteria&metric=coverage)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Criteria)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Criteria&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Criteria)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Criteria&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Criteria)


## Description
The `CodeDesignPlus.Net.Criteria` project provides a robust framework for building and managing dynamic query criteria in .NET Core applications. This library simplifies the creation of complex queries by offering a flexible and extensible criteria system, enabling developers to construct queries dynamically at runtime.

## Table of Contents
- [About The Project](#about-the-project)
- [Installation](#installation)
- [Usage](#usage)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## About The Project
The `CodeDesignPlus.Net.Criteria` project provides a robust framework for building and managing dynamic query criteria in .NET Core applications. This library simplifies the creation of complex queries by offering a flexible and extensible criteria system, enabling developers to construct queries dynamically at runtime.

### Key Features
- Criteria Parsing: The project includes a `Parser` class that converts a list of tokens into an Abstract Syntax Tree (AST), facilitating the interpretation of dynamic queries.
- Tokenization: The `Tokenizer` class tokenizes input strings into logical and comparison operators, enabling structured query parsing.
- Expression Evaluation: The `Evaluator` class evaluates AST nodes and builds expressions to represent the evaluation, supporting complex query logic.
- Exception Management: Custom exceptions like `CriteriaException` handle errors related to criteria processing, providing detailed error information.
- Extensions: Extension methods in `CriteriaExtensions` offer convenient ways to generate filter and sort expressions based on criteria objects.

## Installation
To install the package, run the following command:
```bash
dotnet add package CodeDesignPlus.Net.Criteria
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

Project Link: [CodeDesignPlus.Net.Criteria](https://github.com/codedesignplus/CodeDesignPlus.Net.Sdk/tree/main/packages/CodeDesignPlus.Net.Criteria)
