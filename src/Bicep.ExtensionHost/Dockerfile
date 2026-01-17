# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy files needed for restore
COPY ["Bicep.sln", "."]
COPY ["version.json", "."]
COPY ["global.json", "."]
COPY ["src/BannedSymbols.txt", "src/"]
COPY ["src/Directory.Build.props", "src/"]
COPY ["src/Directory.Build.targets", "src/"]
COPY ["src/Directory.Packages.props", "src/"]
COPY ["src/build/", "src/build/"]
COPY ["src/Bicep.ExtensionHost/Bicep.ExtensionHost.csproj", "src/Bicep.ExtensionHost/"]
COPY ["src/Bicep.Local.Rpc/Bicep.Local.Rpc.csproj", "src/Bicep.Local.Rpc/"]
COPY ["src/Bicep.Local.Rpc/extension.proto", "src/Bicep.Local.Rpc/"]

# Restore dependencies
RUN dotnet restore "src/Bicep.ExtensionHost/Bicep.ExtensionHost.csproj"

# Copy source code
COPY ["src/Bicep.ExtensionHost/", "src/Bicep.ExtensionHost/"]
COPY ["src/Bicep.Local.Rpc/", "src/Bicep.Local.Rpc/"]

# Build
WORKDIR "/app/src/Bicep.ExtensionHost"
RUN dotnet build "Bicep.ExtensionHost.csproj" -c Release -o /app/build -p:GenerateDocumentationFile=false

# Publish stage
FROM build AS publish
RUN dotnet publish "Bicep.ExtensionHost.csproj" -c Release -o /app/publish /p:UseAppHost=false /p:GenerateDocumentationFile=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN groupadd -r appgroup && useradd -r -g appgroup appuser

# Create extensions directory
RUN mkdir -p /app/extensions && chown -R appuser:appgroup /app/extensions

# Copy published app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ExtensionHost__ExtensionStoragePath=/app/extensions

# Expose port
EXPOSE 8080

# Switch to non-root user
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "Azure.Bicep.ExtensionHost.dll"]
