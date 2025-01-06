dotnet tool install --global dotnet-outdated-tool

$proyectos = Get-ChildItem -Path "./../samples/" -Recurse -Filter "*.Abstractions.csproj"

foreach ($proyecto in $proyectos) 
{
    Write-Host "Update Package $($proyecto.Name)" -ForegroundColor Green

    dotnet outdated -u $proyecto.FullName 
}

$proyectos = Get-ChildItem -Path "./../samples/" -Recurse -Filter "*.csproj"

foreach ($proyecto in $proyectos) 
{
    Write-Host "Update Package $($proyecto.Name)" -ForegroundColor Green

    dotnet outdated -u $proyecto.FullName 
}