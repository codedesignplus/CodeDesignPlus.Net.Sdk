# Define el nombre del archivo JSON de salida
$jsonPath = ".\packages.json"

# Busca recursivamente todos los archivos .csproj en el directorio actual y subdirectorios, omitiendo el directorio node_modules
$csprojFiles = Get-ChildItem -Path "../" -Filter *.csproj -Recurse -Exclude *node_modules*

# Prepara los datos para el JSON
$jsonData = @()

foreach ($file in $csprojFiles) {
    [xml]$csprojContent = Get-Content $file.FullName

    $packageReferences = $csprojContent.Project.ItemGroup.PackageReference
    foreach ($package in $packageReferences) {
        $jsonData += @{
            FilePath = $file.FullName
            FileName = $file.Name
            ReferenceType = "PackageReference"
            ReferenceName = $package.Include
            Version = $package.Version
        }
    }

    $projectReferences = $csprojContent.Project.ItemGroup.ProjectReference
    foreach ($project in $projectReferences) {
        $jsonData += @{
            FilePath = $file.FullName
            FileName = $file.Name
            ReferenceType = "ProjectReference"
            ReferenceName = [System.IO.Path]::GetFileNameWithoutExtension($project.Include)
            Version = ""
        }
    }
}

# Convertir los datos a JSON y exportar a un archivo
$jsonData | ConvertTo-Json -Depth 5 | Set-Content -Path $jsonPath

Write-Host "Archivo JSON creado en: $jsonPath"