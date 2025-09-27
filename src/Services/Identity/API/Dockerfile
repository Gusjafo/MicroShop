# syntax=docker/dockerfile:1.7

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY MicroShop.sln ./
COPY src ./src
COPY tests ./tests
RUN dotnet restore MicroShop.sln
RUN dotnet publish src/Services/Identity/API/MicroShop.Services.Identity.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "MicroShop.Services.Identity.API.dll"]
