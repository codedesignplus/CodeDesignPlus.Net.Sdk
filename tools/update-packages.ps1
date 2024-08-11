dotnet tool install -g upgrade-assistant

$proyectos = Get-ChildItem -Path "./../packages/" -Recurse -Filter "*.Abstractions.csproj"

foreach ($proyecto in $proyectos) 
{
    Write-Host "Update Package $($proyecto.Name)" -ForegroundColor Green

    dotnet outdated -u $proyecto.FullName 
}

$proyectos = Get-ChildItem -Path "./../packages/" -Recurse -Filter "*.csproj"

foreach ($proyecto in $proyectos) 
{
    Write-Host "Update Package $($proyecto.Name)" -ForegroundColor Green

    dotnet outdated -u $proyecto.FullName 
}