FROM mcr.microsoft.com/dotnet/aspnet:latest AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:latest AS build

WORKDIR /src
COPY ["src/01 - Infraestructure/FamilyFinanceApi/FamilyFinanceApi.csproj", "01 - Infraestructure/FamilyFinanceApi/"]

RUN dotnet restore "01 - Infraestructure/FamilyFinanceApi/FamilyFinanceApi.csproj"

COPY . .
WORKDIR "/src/01 - Infraestructure/FamilyFinanceApi"

RUN dotnet build "FamilyFinanceApi.csproj" -c Release -o /app/build

FROM build AS publish

RUN dotnet publish "FamilyFinanceApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FamilyFinanceApi.dll"]