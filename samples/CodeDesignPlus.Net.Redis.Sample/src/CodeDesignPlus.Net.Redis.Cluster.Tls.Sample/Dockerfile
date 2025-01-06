FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY ./packages/CodeDesignPlus.Net.Redis /app/packages/CodeDesignPlus.Net.Redis
COPY ./packages/CodeDesignPlus.Net.Core /app/packages/CodeDesignPlus.Net.Core
COPY ./packages/CodeDesignPlus.Net.Serializers /app/packages/CodeDesignPlus.Net.Serializers
COPY ./examples/CodeDesignPlus.Net.Redis.Sample /app/examples/CodeDesignPlus.Net.Redis.Sample

# # Restore as distinct layers
RUN dotnet restore /app/examples/CodeDesignPlus.Net.Redis.Sample/src/CodeDesignPlus.Net.Redis.Cluster.Tls.Sample/

# Build and publish a release
RUN dotnet publish /app/examples/CodeDesignPlus.Net.Redis.Sample/src/CodeDesignPlus.Net.Redis.Cluster.Tls.Sample/ -c Release -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "CodeDesignPlus.Net.Redis.Cluster.Tls.Sample.dll"]
