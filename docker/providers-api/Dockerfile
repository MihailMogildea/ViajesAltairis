# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY project/src/ViajesAltairis.ProvidersApi/ViajesAltairis.ProvidersApi.csproj src/ViajesAltairis.ProvidersApi/
RUN dotnet restore src/ViajesAltairis.ProvidersApi/ViajesAltairis.ProvidersApi.csproj

COPY project/src/ src/
RUN dotnet publish src/ViajesAltairis.ProvidersApi/ViajesAltairis.ProvidersApi.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ViajesAltairis.ProvidersApi.dll"]
