dotnet tool install -g upgrade-assistant

$proyectos = Get-ChildItem -Path "./../samples/" -Recurse -Filter "*.csproj"

foreach ($proyecto in $proyectos) 
{
    Write-Host "Update $($proyecto.FullName)" -ForegroundColor Green

    upgrade-assistant upgrade --operation Inplace --targetFramework net9.0  --non-interactive $proyecto.FullName 
}