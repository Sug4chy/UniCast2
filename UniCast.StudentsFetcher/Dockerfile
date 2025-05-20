FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UniCast.StudentsFetcher/UniCast.StudentsFetcher.csproj", "UniCast.StudentsFetcher/"]
COPY ["UniCast.Infrastructure.Moodle/UniCast.Infrastructure.Moodle.csproj", "UniCast.Infrastructure.Moodle/"]
COPY ["UniCast.Application/UniCast.Application.csproj", "UniCast.Application/"]
COPY ["UniCast.Domain/UniCast.Domain.csproj", "UniCast.Domain/"]
COPY ["UniCast.Infrastructure.Persistence/UniCast.Infrastructure.Persistence.csproj", "UniCast.Infrastructure.Persistence/"]
RUN dotnet restore "UniCast.StudentsFetcher/UniCast.StudentsFetcher.csproj"
COPY . .
WORKDIR "/src/UniCast.StudentsFetcher"
RUN dotnet build "./UniCast.StudentsFetcher.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UniCast.StudentsFetcher.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UniCast.StudentsFetcher.dll"]
