# Bicep Extension Host - Infrastructure

This directory contains infrastructure-as-code for deploying Bicep Extension Host to various Azure hosting options.

## Hosting Options

| Option | Folder | Status | Best For |
|--------|--------|--------|----------|
| **Azure Container Apps** | [aca/](aca/) | ✅ Ready | Serverless containers, auto-scaling |
| Azure Container Instances | aci/ | 🔲 Planned | Simple single-container deployments |
| Azure App Service | appservice/ | 🔲 Planned | PaaS with built-in features |
| Azure Kubernetes Service | aks/ | 🔲 Planned | Full orchestration control |

## Shared Files

| File | Description |
|------|-------------|
| `abbreviations.json` | Resource naming conventions (Azure CAF) |

## Prerequisites

1. **Azure CLI** installed and logged in
2. **Docker** installed (for building container images)
3. Container image pushed to ACR: `ligaracr.azurecr.io/bicep-extension-host:latest`

## Build and Push Container Image

```powershell
# From the repo root
cd c:\Users\ligar\Repos\bicep

# Build the Docker image
docker build -t ligaracr.azurecr.io/bicep-extension-host:latest -f src/Bicep.ExtensionHost/Dockerfile .

# Login to ACR
az acr login --name ligaracr

# Push the image
docker push ligaracr.azurecr.io/bicep-extension-host:latest
```

## Resource Group Naming

Each hosting option uses a separate resource group for isolation:

| Option | Resource Group |
|--------|----------------|
| Container Apps | `rg-bicep-extension-host-aca` |
| Container Instances | `rg-bicep-extension-host-aci` |
| App Service | `rg-bicep-extension-host-appservice` |
| AKS | `rg-bicep-extension-host-aks` |

## Application Endpoints

All deployments expose the same endpoints:

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/` | GET | Welcome message and status |
| `/health` | GET | Health check |
| `/extensions` | GET | List loaded extensions |
| `/extensions/{name}` | POST | Proxy to extension |

## Extensions Configuration

All hosting options accept extension configuration as JSON:

```json
[
  {
    "Name": "kubernetes",
    "RegistryUri": "https://ligaracr.azurecr.io",
    "Repository": "bicep/extensions/kubernetes",
    "Tag": "1.0.0"
  }
]
```
