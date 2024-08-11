# Define the path to the solution file and the packages directory
$solutionPath = "F:\Repos\CodeDesignPlus.Net.Sdk\CodeDesignPlus.Net.Sdk.sln"
$packagesDirectory = "F:\Repos\CodeDesignPlus.Net.Sdk\packages"

# Recorrer la carpeta "packages" en busca de archivos .csproj
$projectFiles = Get-ChildItem -Path $packagesDirectory -Recurse -Filter *.csproj

# Agregar cada archivo .csproj a la solución
foreach ($projectFile in $projectFiles) {
    & dotnet sln $solutionPath add $projectFile.FullName
}

Write-Host "Se han agregado todos los archivos .csproj a la solución."
