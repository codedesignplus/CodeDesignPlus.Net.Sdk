Write-Host "Install dotnet-sonarscanner ----------------------------------------------------------------------------------------------------------------------"
dotnet tool install --global dotnet-sonarscanner 

Write-Host "Start Sonarscanner -------------------------------------------------------------------------------------------------------------------------------"

$org = "codedesignplus"
$key = "CodeDesignPlus.Net.Core.Key"
$csproj = "tests/CodeDesignPlus.Net.Core.Test/CodeDesignPlus.Net.Core.Test.csproj"
$report = "tests/CodeDesignPlus.Net.Core.Test/coverage.opencover.xml"
$server = "https://sonarcloud.io"

dotnet test $csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
dotnet sonarscanner begin /o:$org /k:$key /d:sonar.host.url=$server /d:sonar.coverage.exclusions="**Tests*.cs" /d:sonar.cs.opencover.reportsPaths=$report
dotnet build
dotnet sonarscanner end 