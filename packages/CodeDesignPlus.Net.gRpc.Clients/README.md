<!-- PROJECT LOGO -->
<br />
<p align="center">
  <a href="https://github.com/codedesignplus/CodeDesignPlus.Net.gRpc.Clients/README">
    <img src="https://i.imgur.com/PwbGy0o.png" alt="Logo">
  </a>

  <h3 align="center">CodeDesignPlus.Net.gRpc.Clients</h3>

  <p align="center">
    .NET Core archetype for efficient development, unit testing, and continuous integration of NuGet libraries. 
    <br />
    <a href="https://codedesignplus.com">
      <strong>Explore the docs »</strong>
    </a>
    <br />
    <br />
    <a href="https://github.com/codedesignplus/CodeDesignPlus.Net.gRpc.Clients/issues">
      <img src="https://img.shields.io/github/issues/codedesignplus/CodeDesignPlus.Net.gRpc.Clients?color=0088ff&style=for-the-badge&logo=github" alt="codedesignplus/CodeDesignPlus.Net.gRpc.Clients's issues"/>
    </a>
    <a href="https://github.com/codedesignplus/CodeDesignPlus.Net.gRpc.Clients/pulls">
      <img src="https://img.shields.io/github/issues-pr/codedesignplus/CodeDesignPlus.Net.gRpc.Clients?color=0088ff&style=for-the-badge&logo=github"  alt="codedesignplus/CodeDesignPlus.Net.gRpc.Clients's pull requests"/>
    </a>
    <br />    
    <br />
    <img alt="sonarcloud" src="https://sonarcloud.io/images/project_badges/sonarcloud-white.svg" width="100">
    <br />
    <img alt="Quality Gate Status" src="https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.gRpc.Clients.Key&metric=alert_status" />    
    <img alt="Security Rating" src="https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.gRpc.Clients.Key&metric=security_rating"/>
    <img alt="Reliability Rating" src="https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.gRpc.Clients.Key&metric=reliability_rating" />
    <img alt="Vulnerabilities" src="https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.gRpc.Clients.Key&metric=vulnerabilities" />
    <img alt="Bugs" src="https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.gRpc.Clients.Key&metric=bugs" />
    <img alt="Code Smells" src="https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.gRpc.Clients.Key&metric=code_smells" />
    <img alt="Coverage" src="https://sonarcloud.io/api/project_badges/measure?project=CodeDesignPlus.Net.gRpc.Clients.Key&metric=coverage" />
  </p>
</p>



<!-- TABLE OF CONTENTS -->
## Table of Contents
- [About The Project](#about-the-project)
  * [Key Features](#key-features)
  * [Scripts and Dependencies](#scripts-and-dependencies)
  * [Built With](#built-with)
- [Getting Started](#getting-started)
  * [For Visual Studio Community 2022](#for-visual-studio-community-2022)
  * [For VS Code](#for-vs-code)
- [Usage](#usage)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)
- [Acknowledgements](#acknowledgements)


<!-- ABOUT THE PROJECT -->
## About The Project

About Me
Hello! I am an archetype designed to assist you in creating .NET Core libraries distributed as NuGet packages. I've been crafted with the best practices and common patterns in mind to make your life easier.

### Key Features:
- **Options Pattern:** I facilitate configuration and customization of your libraries using the options pattern.

- **Dependency Injection:** I employ the mechanism of dependency injection to ensure effective decoupling and efficient service management. This not only eases testing and maintainability of your code but also promotes a modular and flexible structure.

- **Abstractions:** With the interfaces and abstract classes I offer, you can develop decoupled and maintainable components.

- **Unit Tests:** I include a dedicated project for unit tests, equipped with tools to validate data annotations, loggers, and more.

- **Node Utilities:** I come packed with several Node.js utilities, like linters and Commitizen, to assist you in adhering to commit conventions.

### Scripts and Dependencies:
In the package.json, I offer several handy scripts:

- **Tools:** Install global .NET tools like dotnet-sonarscanner.
- **Installation:** Restore necessary dependencies for your project.
- **Tests:** Execute all unit tests.
- **Compilation:** Compile your solution.
- **Formatting:** Format your code to keep it neat and consistent.
- **Preparation:** Set up husky, a tool to manage git hooks.

Furthermore, I integrate various dependencies and devDependencies to streamline `version control`, `linting`, and `commit management`, such as `commitizen`, `husky`, and `lint-staged`.

### Built With

Efficient and maintainable software creation not only demands coding skills but also the right tools that fit our needs and streamline our workflow. The tools and technologies we employ often mirror our priorities in terms of efficiency, security, collaboration, and other pivotal aspects of software development. Below are the cornerstone tools and technologies that underpin this archetype:

- **.NET SDK 7.0**: The necessary SDK for building applications in .NET Core 7.0.
- **Node.js**: A JavaScript runtime environment that enables server-side application development.
- **Visual Studio Community 2022**: A comprehensive IDE tailored for .NET development.
- **VS Code**: A lightweight yet powerful code editor with extensive extensibility capabilities.

This archetype comes with a preconfigured continuous integration pipeline, streamlining the automation of crucial tasks such as testing, building, packaging, and deployment. Thanks to this pipeline:

- **Full Automation**: Tests and builds are automatically run with each push, ensuring the code meets quality standards.
- **Semantic Versioning Control**: Using codedesignplus/semver-git-version, versions are automatically generated based on commits, ensuring coherent and predictable version management.
- **Code Quality**: With integrated SonarQube, the code is analyzed for quality and security issues.
- **Automatic Publishing**: Packages are automatically published to NuGet and GitHub Package Registry under certain conditions, making distribution straightforward.
- **Change Management**: With tools like mikepenz/release-changelog-builder-action, a changelog is automatically created for each new version.

Beyond the continuous integration pipeline, this archetype brings several benefits that enhance and standardize the development and collaboration process:

- **Predefined Issue Templates**: They simplify the creation of bug reports, new feature requests, security reports, and more, ensuring all necessary information is provided in a structured manner when creating a new issue.
- **Standardized Contribution**: The CONTRIBUTING.md file offers clear guidelines for those wishing to contribute to the project.
- **Code of Conduct**: CODE_OF_CONDUCT.md sets the norms to ensure a respectful and productive collaboration environment.
- **Code Owners**: The CODEOWNERS file outlines those responsible for reviewing and approving changes.
- **Pull Request Template**: Every new pull request will follow a defined structure, simplifying the review process.
- **Security Guidelines**: SECURITY.md provides guidelines for reporting security vulnerabilities.
- **Bot Configuration**: With files like issue_label_bot.yaml, tasks like auto-labeling issues based on their content can be automated.
- **License**: LICENSE.md details how others can use or contribute to the project.

<!-- GETTING STARTED -->
## Getting Started

To dive into this archetype and set up your development environment, follow these steps:

### For Visual Studio Community 2022:

  1. Open the .sln file with Visual Studio Community 2022.
  2. Select `Build > Restore NuGet Packages` from the menu to restore dependencies.
  3. Compile the project by selecting `Build > Build Solution`.
  4. Run the unit tests by selecting `Test > Run All Tests`.
  5. To package, right-click on the project you wish to package and select Pack.

### For VS Code:

1. Open the project folder in VS Code.

2. Launch the integrated terminal (Ctrl + ~).

3. Install the Recommended Extensions:

    ```bash
    code --install-extension amazonwebservices.aws-toolkit-vscode
    code --install-extension dbaeumer.vscode-eslint
    code --install-extension eamodio.gitlens
    code --install-extension esbenp.prettier-vscode
    code --install-extension github.vscode-github-actions
    code --install-extension ms-azuretools.vscode-docker
    code --install-extension ms-dotnettools.csharp
    code --install-extension ms-dotnettools.vscode-dotnet-runtime
    code --install-extension ms-vscode-remote.remote-containers
    code --install-extension ms-vscode-remote.remote-wsl
    code --install-extension ms-vscode.cpptools
    code --install-extension ms-vscode.cpptools-extension-pack
    code --install-extension ms-vscode.cpptools-themes
    code --install-extension nrwl.angular-console
    code --install-extension PKief.material-icon-theme
    code --install-extension SonarSource.sonarlint-vscode
    code --install-extension Tyriar.lorem-ipsum
    code --install-extension vivaxy.vscode-conventional-commits
    ```
4. Build and Management Procedures:
    ```bash
    dotnet tool install --global dotnet-sonarscanner
    dotnet restore
    dotnet build
    dotnet test
    dotnet format
    npm install
    npm run prepare
    dotnet pack -c Release /p:Version=1.0.0  # Ensure to set the appropriate version
    ```

5. Recommended Extensions for VS Code: These extensions enrich the development experience within VS Code:

   - AWS Toolkit
   - ESLint
   - GitLens
   - Prettier
   - GitHub Actions
   - Docker
   - C#
   - .NET Runtime
   - Remote Development (for containers, WSL, etc.)
   - CMake Tools & Extensions
   - Angular Console
   - Material Icon Theme
   - SonarLint
   - Lorem Ipsum
   - Conventional Commits

<!-- USAGE EXAMPLES -->
## Usage

For more information regarding the library, you can visit our documentation at <a target="_blank" href="https://codedesignplus.com">CodeDesignPlus Doc</a>

<!-- ROADMAP -->
## Roadmap

Refer to [issues](https://github.com/codedesignplus/CodeDesignPlus.Net.gRpc.Clients/issues) for a list of proposed features and known issues.

<!-- CONTRIBUTING -->
## Contributing

1. Fork the Project
2. Create your Feature Branch (`git checkout -b features/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<!-- LICENSE -->
## License

Distributed under the MIT License. See [LICENSE](LICENSE.md) for more information.

<!-- CONTACT -->
## Contact

CodeDesignPlus - [@CodeDesignPlus](https://www.facebook.com/Codedesignplus-115087913695067) - codedesignplus@outlook.com

Project Link: [CodeDesignPlus.Net.gRpc.Clients](https://github.com/codedesignplus/CodeDesignPlus.Net.gRpc.Clients)



<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements

We want to thank each member of the Latin development community in which we participate, contributing content daily to continue growing together.

* [Asp.Net Core en Español](https://www.facebook.com/groups/291405831518163/?multi_permalinks=670205453638197)
* [Asp.Net Core](https://www.facebook.com/groups/aspcore/?multi_permalinks=3454898711268798)
* [Asp.net Core -MVC Group](https://www.facebook.com/groups/2400659736836389/?ref=group_browse)
* [Asp.Net MVC (Español)](https://www.facebook.com/groups/180056992071066/?ref=group_browse)
* [.Net Core](https://www.facebook.com/groups/1547819181920312/?ref=group_browse)
* [.NET En Español PROGRAMADORES](https://www.facebook.com/groups/1537580353178689/?ref=group_browse)
* [ASP.Net Core/C#/MVC/API/Jquery/Html/Sql/Angular/Bootstrap.](https://www.facebook.com/groups/302195073639460/?ref=group_browse)
* [.NET en Español](https://www.facebook.com/groups/1191799410855661/?ref=group_browse)
* [Blazor - ASP.NET Core](https://www.facebook.com/groups/324620021830833/?ref=group_browse)
* [C# (.NET)](https://www.facebook.com/groups/354915134536797/?ref=group_browse)
* [ASP.NET MVC(C#)](https://www.facebook.com/groups/663936840427220/?ref=group_browse)
* [Programación C# .Net Peru](https://www.facebook.com/groups/559287427442678/?ref=group_browse)
* [ASP.NET and ASP.NET Core](https://www.facebook.com/groups/160807057346964/?ref=group_browse)
* [ASP.NET AND .NET CORE](https://www.facebook.com/groups/147648562098634/?ref=group_browse)
* [C#, MVC & .NET CORE 3.1](https://www.facebook.com/groups/332314354403273/?ref=group_browse)
* [.NET Core Community](https://www.facebook.com/groups/2128178990740761/?ref=group_browse)
* [Desarrolladores .Net, C#, React](https://www.facebook.com/groups/2907866402565621/?ref=group_browse)
* [Programadores C#](https://www.facebook.com/groups/304179163001281/?ref=group_browse)
* [.NET Core](https://www.facebook.com/groups/136495930173074/?ref=group_browse)
* [ASP.NET EN ESPAÑOL](https://www.facebook.com/groups/507683892666901/?ref=group_browse)
* [Desarrolladores Microsoft.Net](https://www.facebook.com/groups/169250349939705/?ref=group_browse)
* [ASP.NET Core](https://www.facebook.com/groups/141597583026616/?ref=group_browse)
* [Grupo de Desarrolladores .Net de Microsoft](https://www.facebook.com/groups/15270556519/?ref=group_browse)



<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
