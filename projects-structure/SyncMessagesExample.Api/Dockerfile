FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SyncMessagesExample.Api/SyncMessagesExample.Api.csproj", "SyncMessagesExample.Api/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["Common.Infrastructure/Common.Infrastructure.csproj", "Common.Infrastructure/"]
RUN dotnet restore "SyncMessagesExample.Api/SyncMessagesExample.Api.csproj"
COPY . .
WORKDIR "/src/SyncMessagesExample.Api"
RUN dotnet build "SyncMessagesExample.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SyncMessagesExample.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SyncMessagesExample.Api.dll"]
