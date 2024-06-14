#!/bin/sh
echo "Install dotnet-sonarscanner ----------------------------------------------------------------------------------------------------------------------"
dotnet tool install --global dotnet-sonarscanner 

echo "Start Sonarscanner -------------------------------------------------------------------------------------------------------------------------------"

org=codedesignplus
key=CodeDesignPlus.Net.RabitMQ.Key
csproj=tests/CodeDesignPlus.Net.RabitMQ.Test/CodeDesignPlus.Net.RabitMQ.Test.csproj
report=tests/CodeDesignPlus.Net.RabitMQ.Test/coverage.opencover.xml
server=https://sonarcloud.io

dotnet test $csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
dotnet sonarscanner begin /o:$org /k:$key /d:sonar.host.url=$server /d:sonar.coverage.exclusions="**Tests*.cs" /d:sonar.cs.opencover.reportsPaths=$report
dotnet build
dotnet sonarscanner end
