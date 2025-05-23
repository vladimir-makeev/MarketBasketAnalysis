FROM mcr.microsoft.com/dotnet/aspnet:9.0-azurelinux3.0-distroless AS base
USER app
WORKDIR /app/data
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MarketBasketAnalysis.Server.API/MarketBasketAnalysis.Server.API.csproj", "MarketBasketAnalysis.Server.API/"]
RUN dotnet restore "./MarketBasketAnalysis.Server.API/MarketBasketAnalysis.Server.API.csproj"
COPY . .
WORKDIR "/src/MarketBasketAnalysis.Server.API"
RUN dotnet build "./MarketBasketAnalysis.Server.API.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN GRPC_HEALTH_PROBE_VERSION=v0.4.13 && \
    wget -qO/bin/grpc_health_probe https://github.com/grpc-ecosystem/grpc-health-probe/releases/download/${GRPC_HEALTH_PROBE_VERSION}/grpc_health_probe-linux-amd64 && \
    chmod +x /bin/grpc_health_probe

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MarketBasketAnalysis.Server.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /bin/grpc_health_probe /bin
ENTRYPOINT dotnet MarketBasketAnalysis.Server.API.dll