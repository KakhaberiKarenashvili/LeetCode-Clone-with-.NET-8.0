FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5276
EXPOSE 5341

ENV ASPNETCORE_HTTP_PORTS=5276

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["informaticsge/informaticsge.csproj", "informaticsge/"]
RUN dotnet restore "informaticsge/informaticsge.csproj"
COPY . .
WORKDIR "/src/informaticsge"
RUN dotnet build "informaticsge.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "informaticsge.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "informaticsge.dll"]
