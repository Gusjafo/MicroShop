# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Services/Identity/API/MicroShop.Services.Identity.API.csproj", "src/Services/Identity/API/"]
COPY ["src/Services/Identity/Application/MicroShop.Services.Identity.Application.csproj", "src/Services/Identity/Application/"]
COPY ["src/Services/Identity/Infrastructure/MicroShop.Services.Identity.Infrastructure.csproj", "src/Services/Identity/Infrastructure/"]
COPY ["src/Services/Identity/Domain/MicroShop.Services.Identity.Domain.csproj", "src/Services/Identity/Domain/"]
COPY ["src/BuildingBlocks/Abstractions/MicroShop.BuildingBlocks.Abstractions.csproj", "src/BuildingBlocks/Abstractions/"]
COPY ["src/BuildingBlocks/Contracts/MicroShop.BuildingBlocks.Contracts.csproj", "src/BuildingBlocks/Contracts/"]
RUN dotnet restore "src/Services/Identity/API/MicroShop.Services.Identity.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Identity/API"
RUN dotnet publish "MicroShop.Services.Identity.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MicroShop.Services.Identity.API.dll"]
