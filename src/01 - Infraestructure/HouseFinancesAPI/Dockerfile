#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["src/01 - Infraestructure/HouseFinancesAPI/HouseFinancesAPI.csproj", "src/01 - Infraestructure/HouseFinancesAPI/"]
COPY ["src/02 - Application/Application/Application.csproj", "src/02 - Application/Application/"]
COPY ["src/01 - Infraestructure/Data/Data.csproj", "src/01 - Infraestructure/Data/"]
COPY ["src/03 - Domain/Domain/Domain.csproj", "src/03 - Domain/Domain/"]

RUN dotnet restore "./src/01 - Infraestructure/HouseFinancesAPI/HouseFinancesAPI.csproj"

COPY . .
WORKDIR "/src/src/01 - Infraestructure/HouseFinancesAPI"

RUN dotnet build "./HouseFinancesAPI.csproj" -c PRD -o /app/build

FROM build AS publish

RUN dotnet publish "./HouseFinancesAPI.csproj" -c PRD -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HouseFinancesAPI.dll"]