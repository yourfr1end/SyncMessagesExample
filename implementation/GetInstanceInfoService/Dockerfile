FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GetInstanceInfoService/GetInstanceInfoService.csproj", "GetInstanceInfoService/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["Common.Infrastructure/Common.Infrastructure.csproj", "Common.Infrastructure/"]
RUN dotnet restore "GetInstanceInfoService/GetInstanceInfoService.csproj"
COPY . .
WORKDIR "/src/GetInstanceInfoService"
RUN dotnet build "GetInstanceInfoService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GetInstanceInfoService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GetInstanceInfoService.dll"]
