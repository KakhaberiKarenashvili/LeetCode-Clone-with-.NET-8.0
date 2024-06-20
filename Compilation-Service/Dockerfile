# Use a base image with Linux and .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# Set the working directory
WORKDIR /app

# Copy the .NET project files and restore dependencies
COPY Compilation-Service.csproj ./
RUN dotnet restore

# Copy the remaining project files and build the application
COPY . ./
RUN dotnet build -c Release -o out

# Switch to a runtime image with Linux and .NET runtime (slimmer)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory
WORKDIR /app

# Copy the built application from the build stage
COPY --from=build-env /app/out .

ENV ASPNETCORE_HTTP_PORTS=5144

RUN apt-get update

RUN apt-get install -y g++

RUN g++ --version
# Expose port for the service
EXPOSE 80
EXPOSE 443
EXPOSE 5144

# Run the application when the container starts
ENTRYPOINT ["dotnet", "Compilation-Service.dll"]