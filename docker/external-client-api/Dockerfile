FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY kernel/project/ViajesAltairis.Domain/ViajesAltairis.Domain.csproj kernel/project/ViajesAltairis.Domain/
COPY kernel/project/ViajesAltairis.Application/ViajesAltairis.Application.csproj kernel/project/ViajesAltairis.Application/
COPY kernel/project/ViajesAltairis.Data/ViajesAltairis.Data.csproj kernel/project/ViajesAltairis.Data/
COPY kernel/project/ViajesAltairis.Infrastructure/ViajesAltairis.Infrastructure.csproj kernel/project/ViajesAltairis.Infrastructure/
COPY external-client-api/project/ViajesAltairis.ExternalClientApi.csproj external-client-api/project/
RUN dotnet restore external-client-api/project/ViajesAltairis.ExternalClientApi.csproj

COPY kernel/project/ kernel/project/
COPY external-client-api/project/ external-client-api/project/
RUN dotnet publish external-client-api/project/ViajesAltairis.ExternalClientApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ViajesAltairis.ExternalClientApi.dll"]
