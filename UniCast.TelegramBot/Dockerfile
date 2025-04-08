FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UniCast.TelegramBot/UniCast.TelegramBot.csproj", "UniCast.TelegramBot/"]
COPY ["UniCast.Application.TelegramBot/UniCast.Application.TelegramBot.csproj", "UniCast.Application.TelegramBot/"]
COPY ["UniCast.Application/UniCast.Application.csproj", "UniCast.Application/"]
COPY ["UniCast.Domain/UniCast.Domain.csproj", "UniCast.Domain/"]
COPY ["UniCast.Infrastructure.Persistence/UniCast.Infrastructure.Persistence.csproj", "UniCast.Infrastructure.Persistence/"]
COPY ["UniCast.Infrastructure.Telegram/UniCast.Infrastructure.Telegram.csproj", "UniCast.Infrastructure.Telegram/"]
COPY ["UniCast.Infrastructure.Moodle/UniCast.Infrastructure.Moodle.csproj", "UniCast.Infrastructure.Moodle/"]
RUN dotnet restore "UniCast.TelegramBot/UniCast.TelegramBot.csproj"
COPY . .
WORKDIR "/src/UniCast.TelegramBot"
RUN dotnet build "UniCast.TelegramBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UniCast.TelegramBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UniCast.TelegramBot.dll"]
