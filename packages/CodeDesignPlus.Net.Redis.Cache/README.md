# CodeDesignPlus.Net.Redis.Cache

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Redis.Cache&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Redis.Cache)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Redis.Cache&metric=bugs)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Redis.Cache)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Redis.Cache&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Redis.Cache)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Redis.Cache&metric=coverage)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Redis.Cache)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Redis.Cache&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Redis.Cache)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.Redis.Cache&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=CodeDesignPlus.Net.Redis.Cache)

## Description

The `CodeDesignPlus.Net.Redis.Cache` project provides a concrete implementation of the `ICacheManager` interface using Redis as the underlying caching mechanism. This library allows .NET applications to seamlessly leverage Redis for efficient caching, improving performance and scalability. It’s part of the CodeDesignPlus ecosystem, designed to work with the core caching abstractions provided in `CodeDesignPlus.Net.Cache`.

## Table of Contents

- [CodeDesignPlus.Net.Redis.Cache](#codedesignplusnetrediscache)
  - [Description](#description)
  - [Table of Contents](#table-of-contents)
  - [About The Project](#about-the-project)
    - [Key Features](#key-features)
  - [Installation](#installation)
  - [Usage](#usage)
  - [Roadmap](#roadmap)
  - [Contributing](#contributing)
  - [License](#license)
  - [Contact](#contact)

## About The Project

`CodeDesignPlus.Net.Redis.Cache` provides a reliable and high-performance way to use Redis as a caching provider. Built on top of `CodeDesignPlus.Net.Cache`, this library provides a direct way to integrate Redis caching into your application, taking full advantage of Redis' advanced features, while still keeping a clear separation of concerns.

### Key Features

-   **Redis Implementation:** Provides a concrete implementation of the `ICacheManager` interface using Redis.
-   **Serialization:**  Uses `Newtonsoft.Json` or similar for serializing and deserializing objects to store in Redis.
-   **Asynchronous Operations:** All methods are asynchronous, ensuring non-blocking I/O and improving application responsiveness.
-   **Configuration:** Supports configuration via connection strings.
-   **Integration with `CodeDesignPlus.Net.Cache`:** Seamlessly integrates with the abstract caching layer for easy switching between caching mechanisms.

## Installation

To install the package, run the following command:

```bash
dotnet add package CodeDesignPlus.Net.Redis.Cache
```

## Usage

Here's an example of how to use `RedisCacheManager` with the `ICacheManager` abstraction.

First, register the RedisCacheManager in the dependency injection container:

```csharp
// In Startup.cs or your DI setup:
using CodeDesignPlus.Net.Redis.Cache.Extensions;
using Microsoft.Extensions.DependencyInjection;

public void ConfigureServices(IServiceCollection services)
{
    // ... other service registrations

    services.AddCache(Configuration);

    // ... other service registrations
}
```

Then, inject the `ICacheManager` into your services:

```csharp
using CodeDesignPlus.Net.Cache;

public class MyService
{
    private readonly ICacheManager _cacheManager;

    public MyService(ICacheManager cacheManager)
    {
        _cacheManager = cacheManager;
    }

    public async Task<MyData> GetDataAsync(string key)
    {
        var cachedData = await _cacheManager.GetAsync<MyData>(key);
        if (cachedData != null)
        {
            return cachedData;
        }

        var data = await GetDataFromSourceAsync(key); // Some function to retrieve data

        await _cacheManager.SetAsync(key, data, TimeSpan.FromMinutes(10));

        return data;
    }

  // Assume this method return a task with the result
    private async Task<MyData> GetDataFromSourceAsync(string key)
    {
      // Your implementation here
       await Task.Delay(100);
       return new MyData() { Id=key, Name = "Example " + key};
    }
}

// A simple Model
public class MyData {
  public string Id { get; set;}
  public string Name { get; set; }
}
```

For more detailed information, please refer to our documentation at [CodeDesignPlus Doc](https://doc.codedesignplus.com).

## Roadmap

Refer to [issues](https://github.com/codedesignplus/CodeDesignPlus.Net.Sdk/issues) for a list of proposed features and known issues.

## Contributing

1.  Fork the Project
2.  Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3.  Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4.  Push to the Branch (`git push origin feature/AmazingFeature`)
5.  Open a Pull Request

## License

Distributed under the MIT License. See `LICENSE` for more information.

## Contact

CodeDesignPlus - @CodeDesignPlus - codedesignplus@outlook.com

Project Link: [CodeDesignPlus.Net.Redis.Cache](https://github.com/codedesignplus/CodeDesignPlus.Net.Sdk/tree/main/packages/CodeDesignPlus.Net.Redis.Cache)