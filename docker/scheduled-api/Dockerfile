FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

COPY kernel/project/ kernel/project/
COPY scheduled-api/project/ scheduled-api/project/

WORKDIR /src/scheduled-api/project
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ViajesAltairis.ScheduledApi.dll"]
