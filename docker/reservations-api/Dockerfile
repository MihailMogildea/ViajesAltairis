FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY kernel/project/ViajesAltairis.Domain/ViajesAltairis.Domain.csproj kernel/project/ViajesAltairis.Domain/
COPY kernel/project/ViajesAltairis.Application/ViajesAltairis.Application.csproj kernel/project/ViajesAltairis.Application/
COPY kernel/project/ViajesAltairis.Data/ViajesAltairis.Data.csproj kernel/project/ViajesAltairis.Data/
COPY kernel/project/ViajesAltairis.Infrastructure/ViajesAltairis.Infrastructure.csproj kernel/project/ViajesAltairis.Infrastructure/
COPY reservations-api/project/ViajesAltairis.ReservationsApi/ViajesAltairis.ReservationsApi.csproj reservations-api/project/ViajesAltairis.ReservationsApi/
RUN dotnet restore reservations-api/project/ViajesAltairis.ReservationsApi/ViajesAltairis.ReservationsApi.csproj

COPY kernel/project/ kernel/project/
COPY reservations-api/project/ reservations-api/project/
RUN dotnet publish reservations-api/project/ViajesAltairis.ReservationsApi/ViajesAltairis.ReservationsApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ViajesAltairis.ReservationsApi.dll"]
