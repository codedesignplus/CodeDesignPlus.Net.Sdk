#!/bin/bash

# Install dotnet outdated tool
dotnet tool install --global dotnet-outdated-tool

# Find and update all .Abstractions.csproj files
find ./../packages/ -type f -name "*.Abstractions.csproj" | while read -r proyecto; do
    echo -e "\e[32mUpdate Package $(basename "$proyecto")\e[0m"
    dotnet outdated -u "$proyecto"
done

# Find and update all .csproj files
find ./../packages/ -type f -name "*.csproj" | while read -r proyecto; do
    echo -e "\e[32mUpdate Package $(basename "$proyecto")\e[0m"
    dotnet outdated -u "$proyecto"
done