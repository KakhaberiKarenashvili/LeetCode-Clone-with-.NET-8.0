FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5144
EXPOSE 5341

ENV ASPNETCORE_HTTP_PORTS=5144

RUN apt-get update

RUN apt-get install -y g++ && apt-get install -y python3 


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Compilation-Service/Compilation-Service.csproj", "Compilation-Service/"]
RUN dotnet restore "Compilation-Service/Compilation-Service.csproj"
COPY . .
WORKDIR "/src/Compilation-Service"
RUN dotnet build "Compilation-Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Compilation-Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Compilation-Service.dll"]
