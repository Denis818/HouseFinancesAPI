FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/04 - UI/HouseFinancesAPI/HouseFinancesAPI.csproj", "src/04 - UI/HouseFinancesAPI/"]
COPY ["src/03 - Infraestructure/DIContainer/DIContainer.csproj", "src/03 - Infraestructure/DIContainer/"]
COPY ["src/01 - Domain/Domain/Domain.csproj", "src/01 - Domain/Domain/"]
COPY ["src/02 - Application/Application/Application.csproj", "src/02 - Application/Application/"]
COPY ["src/03 - Infraestructure/Data/Data.csproj", "src/03 - Infraestructure/Data/"]
RUN dotnet restore "./src/04 - UI/HouseFinancesAPI/HouseFinancesAPI.csproj"

COPY . .

WORKDIR "/src/src/04 - UI/HouseFinancesAPI"
RUN dotnet build "./HouseFinancesAPI.csproj" -c PRD -o /app/build

FROM build AS publish

RUN dotnet publish "./HouseFinancesAPI.csproj" -c PRD -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HouseFinancesAPI.dll"]