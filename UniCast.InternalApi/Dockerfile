FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UniCast.InternalApi/UniCast.InternalApi.csproj", "UniCast.InternalApi/"]
COPY ["UniCast.Application.InternalApi/UniCast.Application.InternalApi.csproj", "UniCast.Application.InternalApi/"]
COPY ["UniCast.Application/UniCast.Application.csproj", "UniCast.Application/"]
COPY ["UniCast.Domain/UniCast.Domain.csproj", "UniCast.Domain/"]
COPY ["UniCast.Infrastructure.Persistence/UniCast.Infrastructure.Persistence.csproj", "UniCast.Infrastructure.Persistence/"]
COPY ["UniCast.Infrastructure.Telegram/UniCast.Infrastructure.Telegram.csproj", "UniCast.Infrastructure.Telegram/"]
RUN dotnet restore "UniCast.InternalApi/UniCast.InternalApi.csproj"
COPY . .
WORKDIR "/src/UniCast.InternalApi"
RUN dotnet build "UniCast.InternalApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UniCast.InternalApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UniCast.InternalApi.dll"]
