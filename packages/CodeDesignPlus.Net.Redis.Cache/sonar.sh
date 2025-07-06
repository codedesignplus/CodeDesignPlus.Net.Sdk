#!/bin/sh
echo "Install dotnet-sonarscanner ----------------------------------------------------------------------------------------------------------------------"
dotnet tool install --global dotnet-sonarscanner 

echo "Start Sonarscanner -------------------------------------------------------------------------------------------------------------------------------"

org=codedesignplus
key=CodeDesignPlus.Net.Redis.Cache.Key
csproj=tests/CodeDesignPlus.Net.Redis.Cache.Test/CodeDesignPlus.Net.Redis.Cache.Test.csproj
report=tests/CodeDesignPlus.Net.Redis.Cache.Test/coverage.opencover.xml
server=https://sonarcloud.io

dotnet test $csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
dotnet sonarscanner begin /o:$org /k:$key /d:sonar.host.url=$server /d:sonar.coverage.exclusions="**Tests*.cs" /d:sonar.cs.opencover.reportsPaths=$report
dotnet build
dotnet sonarscanner end /d:sonar.token=sqa_44c22c45e0ee901f36b1d6b446b17ad7be3f589e
