# Azure Container Apps Deployment

Deploy Bicep Extension Host to Azure Container Apps.

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│               rg-bicep-extension-host-aca                   │
│  ┌─────────────────────────────────────────────────────┐   │
│  │           Container Apps Environment                 │   │
│  │  ┌─────────────────────────────────────────────┐    │   │
│  │  │           Container App                      │    │   │
│  │  │  ┌─────────────────────────────────────┐    │    │   │
│  │  │  │     Bicep Extension Host            │    │    │   │
│  │  │  │     - Downloads OCI artifacts       │    │    │   │
│  │  │  │     - Starts gRPC servers           │    │    │   │
│  │  │  │     - Exposes HTTP endpoints        │    │    │   │
│  │  │  └─────────────────────────────────────┘    │    │   │
│  │  └─────────────────────────────────────────────┘    │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│  ┌──────────────────┐  ┌────────────────────────────────┐  │
│  │ Managed Identity │  │    Log Analytics Workspace     │  │
│  └──────────────────┘  └────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
         │
         ▼
┌──────────────────┐
│   ligaracr       │  (Existing ACR - anonymous pull)
└──────────────────┘
```

## Files

| File | Description |
|------|-------------|
| `main.bicep` | Container Apps deployment template |
| `main.bicepparam` | Parameters file |

## Deploy

```powershell
# Create resource group
az group create --name rg-bicep-extension-host-aca --location eastus

# Deploy
az deployment group create `
  --resource-group rg-bicep-extension-host-aca `
  --template-file infra/aca/main.bicep `
  --parameters infra/aca/main.bicepparam
```

## Characteristics

| Aspect | Value |
|--------|-------|
| **Scaling** | 1-3 replicas (HTTP-based autoscaling) |
| **Cost** | Pay per use (consumption plan) |
| **Cold Start** | Possible if scaled to 0 |
| **Networking** | Public endpoint with HTTPS |
| **Complexity** | Low |

## Cleanup

```powershell
az group delete --name rg-bicep-extension-host-aca --yes --no-wait
```
