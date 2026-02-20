FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files for restore layer caching (preserve directory structure for project references)
COPY kernel/project/ViajesAltairis.Domain/ViajesAltairis.Domain.csproj kernel/project/ViajesAltairis.Domain/
COPY kernel/project/ViajesAltairis.Application/ViajesAltairis.Application.csproj kernel/project/ViajesAltairis.Application/
COPY kernel/project/ViajesAltairis.Data/ViajesAltairis.Data.csproj kernel/project/ViajesAltairis.Data/
COPY kernel/project/ViajesAltairis.Infrastructure/ViajesAltairis.Infrastructure.csproj kernel/project/ViajesAltairis.Infrastructure/
COPY admin-api/project/ViajesAltairis.AdminApi/ViajesAltairis.AdminApi.csproj admin-api/project/ViajesAltairis.AdminApi/

RUN dotnet restore admin-api/project/ViajesAltairis.AdminApi/ViajesAltairis.AdminApi.csproj

# Copy source and publish
COPY kernel/project/ kernel/project/
COPY admin-api/project/ViajesAltairis.AdminApi/ admin-api/project/ViajesAltairis.AdminApi/

RUN dotnet publish admin-api/project/ViajesAltairis.AdminApi/ViajesAltairis.AdminApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ViajesAltairis.AdminApi.dll"]
