FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER root
WORKDIR /app

RUN apt-get update

RUN apt-get install -y g++ && apt-get install -y python3 

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Compilation/Compilation.Application/Compilation.Application.csproj", "Services/Compilation/Compilation.Application/"]
COPY ["BuildingBlocks/BuildingBlocks.Common/BuildingBlocks.Common.csproj", "BuildingBlocks/BuildingBlocks.Common/"]
COPY ["BuildingBlocks/BuildingBlocks.Messaging/BuildingBlocks.Messaging.csproj", "BuildingBlocks/BuildingBlocks.Messaging/"]
RUN dotnet restore "Services/Compilation/Compilation.Application/Compilation.Application.csproj"
COPY . .
WORKDIR "/src/Services/Compilation/Compilation.Application"
RUN dotnet build "./Compilation.Application.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Compilation.Application.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Compilation.Application.dll"]
