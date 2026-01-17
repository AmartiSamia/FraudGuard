# FraudDetectionAPI Dockerfile
# Multi-stage build for optimized production image

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["FraudDetectionAPI.csproj", "./"]
RUN dotnet restore "FraudDetectionAPI.csproj"

# Copy source code and build
COPY . .
RUN dotnet build "FraudDetectionAPI.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "FraudDetectionAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=publish /app/publish .

# Environment variables
ENV ASPNETCORE_URLS=http://+:5203
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5203/api/health || exit 1

# Expose port
EXPOSE 5203

# Entry point
ENTRYPOINT ["dotnet", "FraudDetectionAPI.dll"]
