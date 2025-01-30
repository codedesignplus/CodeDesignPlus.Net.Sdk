$proyectos = Get-ChildItem -Path "./../packages/" -Recurse -Filter "*.csproj"

foreach ($proyecto in $proyectos) 
{
    Write-Host "Update $($proyecto.FullName)" -ForegroundColor Green

    dotnet list $proyecto.FullName package --vulnerable --include-transitive
}