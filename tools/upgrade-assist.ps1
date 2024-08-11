dotnet tool install -g upgrade-assistant

$proyectos = Get-ChildItem -Path "./../packages/" -Recurse -Filter "*.csproj"

foreach ($proyecto in $proyectos) 
{
    Write-Host "Update $($proyecto.FullName)" -ForegroundColor Green

    upgrade-assistant upgrade --targetFramework LTS --non-interactive $proyecto.FullName 
}