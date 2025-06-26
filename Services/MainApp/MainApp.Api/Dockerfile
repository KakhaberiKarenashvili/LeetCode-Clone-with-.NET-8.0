FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV ASPNETCORE_HTTP_PORTS=5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/MainApp/MainApp.Api/MainApp.Api.csproj", "Services/MainApp/MainApp.Api/"]
COPY ["BuildingBlocks/BuildingBlocks.Messaging/BuildingBlocks.Messaging.csproj", "BuildingBlocks/BuildingBlocks.Messaging/"]
COPY ["BuildingBlocks/BuildingBlocks.Common/BuildingBlocks.Common.csproj", "BuildingBlocks/BuildingBlocks.Common/"]
COPY ["Services/MainApp/MainApp.Application/MainApp.Application.csproj", "Services/MainApp/MainApp.Application/"]
COPY ["Services/MainApp/MainApp.Domain/MainApp.Domain.csproj", "Services/MainApp/MainApp.Domain/"]
COPY ["Services/MainApp/MainApp.Infrastructure/MainApp.Infrastructure.csproj", "Services/MainApp/MainApp.Infrastructure/"]
RUN dotnet restore "Services/MainApp/MainApp.Api/MainApp.Api.csproj"
COPY . .
WORKDIR "/src/Services/MainApp/MainApp.Api"
RUN dotnet build "./MainApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MainApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MainApp.Api.dll"]
