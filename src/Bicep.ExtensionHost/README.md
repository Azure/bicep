# Bicep Extension Host

A web application that hosts multiple Bicep extension gRPC servers. It downloads OCI artifacts from Azure Container Registry at startup and exposes HTTP endpoints to interact with the extensions.

## Features

- Downloads OCI artifacts from ACR (supports anonymous pull)
- Automatically selects the appropriate binary for the current platform (Windows, Linux, macOS)
- Starts multiple extension gRPC servers concurrently
- Provides health check endpoints
- Proxies HTTP requests to the appropriate gRPC extension

## Configuration

The application is configured via `appsettings.json` or environment variables.

### appsettings.json Example

```json
{
  "ExtensionHost": {
    "ExtensionStoragePath": "/tmp/bicep-extensions",
    "Extensions": [
      {
        "Name": "http",
        "RegistryUri": "https://myacr.azurecr.io",
        "Repository": "extensions/http",
        "Tag": "0.1.1"
      },
      {
        "Name": "keyvault",
        "RegistryUri": "https://myacr.azurecr.io",
        "Repository": "extensions/keyvault",
        "Tag": "0.1.3"
      }
    ]
  }
}
```

### Environment Variables

For Azure Container Apps or other cloud deployments, you can configure extensions using environment variables:

```bash
# Extension 1
ExtensionHost__Extensions__0__Name=http
ExtensionHost__Extensions__0__RegistryUri=https://myacr.azurecr.io
ExtensionHost__Extensions__0__Repository=extensions/http
ExtensionHost__Extensions__0__Tag=0.1.1

# Extension 2
ExtensionHost__Extensions__1__Name=keyvault
ExtensionHost__Extensions__1__RegistryUri=https://myacr.azurecr.io
ExtensionHost__Extensions__1__Repository=extensions/keyvault
ExtensionHost__Extensions__1__Tag=0.1.3

# Optional: Storage path for downloaded binaries
ExtensionHost__ExtensionStoragePath=/tmp/bicep-extensions
```

## HTTP Endpoints

### Root
```
GET /
```
Returns basic service information and available endpoints.

### Health Check
```
GET /health
```
Returns health status of all extensions. Returns:
- `200 OK` with status "Healthy" if all extensions are running and responding to pings
- `503 Service Unavailable` with status "Unhealthy" if any extension is not healthy

Response example:
```json
{
  "status": "Healthy",
  "extensions": {
    "http": {
      "isRunning": true,
      "isPingSuccessful": true,
      "status": "Healthy"
    },
    "keyvault": {
      "isRunning": true,
      "isPingSuccessful": true,
      "status": "Healthy"
    }
  }
}
```

### List Extensions
```
GET /extensions
```
Returns a list of available extension names.

### Extension Operations
```
POST /extensions/{extensionName}
```

Proxies requests to the specified extension's gRPC server.

#### Request Body

For **CreateOrUpdate** or **Preview** operations:
```json
{
  "operation": "CreateOrUpdate",
  "specification": {
    "type": "MyResource/Type",
    "apiVersion": "2024-01-01",
    "config": "{\"key\": \"value\"}",
    "properties": {
      "name": "my-resource",
      "location": "westus2"
    }
  }
}
```

For **Get** or **Delete** operations:
```json
{
  "operation": "Get",
  "reference": {
    "type": "MyResource/Type",
    "apiVersion": "2024-01-01",
    "config": "{\"key\": \"value\"}",
    "identifiers": {
      "id": "resource-id"
    }
  }
}
```

#### Supported Operations
- `CreateOrUpdate` - Create or update a resource
- `Preview` - Preview a resource change
- `Get` - Get a resource
- `Delete` - Delete a resource

## Running Locally

```bash
cd src/Bicep.ExtensionHost
dotnet run
```

The application will start on `http://localhost:5180` (or `https://localhost:7180`).

## Deploying to Azure Container Apps

1. Build and push the Docker image (current working directory is the repo root):
```bash
docker build -t ligaracr.azurecr.io/bicep-extension-host:latest -f src/Bicep.ExtensionHost/Dockerfile .
az acr login -g ligar-test --name ligaracr
docker push ligaracr.azurecr.io/bicep-extension-host:latest
```

2. Create an Azure Container App with the required environment variables for your extensions.

3. Configure the health probe to use `/health` endpoint.

## Platform Support

The application automatically selects the correct binary based on the current platform:
- `win-x64` / `win-arm64` - Windows
- `linux-x64` / `linux-arm64` - Linux
- `osx-x64` / `osx-arm64` - macOS

## OCI Artifact Structure

The application expects OCI artifacts with the following layer media types:
- `application/vnd.ms.bicep.provider.layer.v1.win-x64.binary` - Windows x64
- `application/vnd.ms.bicep.provider.layer.v1.win-arm64.binary` - Windows ARM64
- `application/vnd.ms.bicep.provider.layer.v1.linux-x64.binary` - Linux x64
- `application/vnd.ms.bicep.provider.layer.v1.linux-arm64.binary` - Linux ARM64
- `application/vnd.ms.bicep.provider.layer.v1.osx-x64.binary` - macOS x64
- `application/vnd.ms.bicep.provider.layer.v1.osx-arm64.binary` - macOS ARM64
