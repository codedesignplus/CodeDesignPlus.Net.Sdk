
# Define the root path
$rootPath = "G:\Repos\CodeDesignPlus.Net.Sdk"
$packagesPath = "$rootPath\packages"
$examplesPath = "$rootPath\examples"

# Create examples directory if it doesn't exist
if (-Not (Test-Path -Path $examplesPath)) {
    New-Item -ItemType Directory -Path $examplesPath
}

# Get all .csproj files in the packages directory
$csprojFiles = Get-ChildItem -Path $packagesPath -Recurse -Filter *.csproj

Write-Output "Creating example projects and adding them to the solution..."

foreach ($csproj in $csprojFiles) {

    Write-Output "Creating example project for $csproj"

    # Get the project name without extension
    $projectName = [System.IO.Path]::GetFileNameWithoutExtension($csproj.FullName)
    # Define the example project path
    $exampleProjectPath = "$examplesPath\$projectName.Sample"

    # Create the example project
    dotnet new console -n "$projectName.Sample" -o $exampleProjectPath

    # Add the example project to the solution
    dotnet sln $rootPath\CodeDesignPlus.Net.Sdk.sln add "$exampleProjectPath\$projectName.Sample.csproj"
}

Write-Output "Example projects created and added to the solution."