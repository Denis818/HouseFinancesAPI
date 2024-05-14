FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["src/01 - Presentation/CasaFinanceiroApi/CasaFinanceiroApi.csproj", "src/01 - Presentation/CasaFinanceiroApi/"]
COPY ["src/02 - Infraestructure/DIContainer/DIContainer.csproj", "src/02 - Infraestructure/DIContainer/"]
COPY ["src/03 - Core/Application/Application.csproj", "src/03 - Core/Application/"]
COPY ["src/03 - Core/Domain/Domain.csproj", "src/03 - Core/Domain/"]
COPY ["src/02 - Infraestructure/Data/Data.csproj", "src/02 - Infraestructure/Data/"]
RUN dotnet restore "./src/01 - Presentation/CasaFinanceiroApi/CasaFinanceiroApi.csproj"

COPY . .

WORKDIR "/src/src/01 - Presentation/CasaFinanceiroApi"
RUN dotnet build "./CasaFinanceiroApi.csproj" -c PRD -o /app/build

FROM build AS publish

RUN dotnet publish "./CasaFinanceiroApi.csproj" -c PRD -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CasaFinanceiroApi.dll"]