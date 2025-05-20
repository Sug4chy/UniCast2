FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UniCast.Migrator/UniCast.Migrator.csproj", "UniCast.Migrator/"]
COPY ["UniCast.Infrastructure.Persistence/UniCast.Infrastructure.Persistence.csproj", "UniCast.Infrastructure.Persistence/"]
COPY ["UniCast.Application/UniCast.Application.csproj", "UniCast.Application/"]
COPY ["UniCast.Domain/UniCast.Domain.csproj", "UniCast.Domain/"]
RUN dotnet restore "UniCast.Migrator/UniCast.Migrator.csproj"
COPY . .
WORKDIR "/src/UniCast.Migrator"
RUN dotnet build "./UniCast.Migrator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UniCast.Migrator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UniCast.Migrator.dll"]
