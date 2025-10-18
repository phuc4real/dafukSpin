# Multi-stage build for dafukSpin ASP.NET Core Web API
# Optimized for production deployment with security and performance best practices

# Base runtime image with non-root user for security
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app

# Create a non-root user for running the application
RUN addgroup --system --gid 1001 dafukspin && \
    adduser --system --uid 1001 --ingroup dafukspin --shell /bin/false dafukspin

# Set up proper ownership and permissions
RUN chown -R dafukspin:dafukspin /app
USER dafukspin

# Expose ports (HTTP and HTTPS)
EXPOSE 8080
EXPOSE 8081

# Build stage with SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution-level package management files (required for Central Package Management)
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

# Copy project files for dependency resolution
COPY ["src/dafukSpin/dafukSpin.csproj", "src/dafukSpin/"]
COPY ["test/dafukSpin.Tests/dafukSpin.Tests.csproj", "test/dafukSpin.Tests/"]

# Restore dependencies in a separate layer for better caching
RUN dotnet restore "src/dafukSpin/dafukSpin.csproj" \
    --runtime linux-musl-x64

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/src/dafukSpin"
RUN dotnet build "dafukSpin.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build \
    --runtime linux-musl-x64 \
    --self-contained false \
    --no-restore

# Test stage (optional - can be skipped in CI/CD if tests run separately)
FROM build AS test
WORKDIR /src
RUN dotnet test "test/dafukSpin.Tests/dafukSpin.Tests.csproj" \
    --configuration $BUILD_CONFIGURATION \
    --no-build \
    --verbosity normal \
    --collect:"XPlat Code Coverage"

# Publish stage - create optimized release build
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/src/dafukSpin"

RUN dotnet publish "dafukSpin.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --runtime linux-musl-x64 \
    --self-contained false \
    --no-restore \
    /p:PublishTrimmed=false \
    /p:PublishSingleFile=false

# Final runtime stage
FROM base AS final
WORKDIR /app

# Copy published application from publish stage
COPY --from=publish --chown=dafukspin:dafukspin /app/publish .

# Add health check using wget (available in Alpine)
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

# Set environment variables for production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_HTTP_PORTS=8080
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_EnableDiagnostics=0

# Labels for better container management
LABEL maintainer="dafukSpin Team" \
      description="dafukSpin API - MyAnimeList integration service" \
      version="1.0" \
      org.opencontainers.image.source="https://github.com/phuc4real/dafukSpin" \
      org.opencontainers.image.description="ASP.NET Core Web API for anime data via MyAnimeList Official API"

# Use non-root user
USER dafukspin

# Start the application
ENTRYPOINT ["dotnet", "dafukSpin.dll"]