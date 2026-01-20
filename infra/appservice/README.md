# Azure App Service Deployment (Container)

Deploy Bicep Extension Host as a container on Azure App Service.

> ⚠️ **Warning**: App Service has sandbox limitations that may prevent the Extension Host from spawning child processes (gRPC extension servers). This deployment option is experimental.

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│            rg-bicep-extension-host-appservice               │
│  ┌─────────────────────────────────────────────────────┐   │
│  │              App Service Plan (Linux)               │   │
│  │  ┌─────────────────────────────────────────────┐    │   │
│  │  │              Web App (Container)             │    │   │
│  │  │  ┌─────────────────────────────────────┐    │    │   │
│  │  │  │     Bicep Extension Host            │    │    │   │
│  │  │  │     - Downloads OCI artifacts       │    │    │   │
│  │  │  │     - Starts gRPC servers (⚠️)      │    │    │   │
│  │  │  │     - Exposes HTTP endpoints        │    │    │   │
│  │  │  └─────────────────────────────────────┘    │    │   │
│  │  └─────────────────────────────────────────────┘    │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│  ┌────────────────────────────────────┐                    │
│  │    Log Analytics Workspace         │                    │
│  └────────────────────────────────────┘                    │
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
| `main.bicep` | App Service deployment template |
| `main.bicepparam` | Parameters file |

## Deploy

```powershell
# Create resource group
az group create --name rg-bicep-extension-host-appservice --location eastus

# Deploy
az deployment group create `
  --resource-group rg-bicep-extension-host-appservice `
  --template-file infra/appservice/main.bicep `
  --parameters infra/appservice/main.bicepparam
```

## Verify

```powershell
# Get the Web App URL
$APP_URL = az deployment group show `
  --resource-group rg-bicep-extension-host-appservice `
  --name main `
  --query 'properties.outputs.webAppUrl.value' -o tsv

# Check health endpoint
curl "$APP_URL/health"

# Check extensions (this may fail if process spawning is blocked)
curl "$APP_URL/extensions"
```

## View Logs

```powershell
# Stream logs
az webapp log tail `
  --resource-group rg-bicep-extension-host-appservice `
  --name (az webapp list -g rg-bicep-extension-host-appservice --query '[0].name' -o tsv)
```

## Characteristics

| Aspect | Value |
|--------|-------|
| **Scaling** | Manual or autoscale rules |
| **Cost** | Always running (B1 ~$13/month) |
| **Cold Start** | None (always-on enabled) |
| **Networking** | Public endpoint with HTTPS |
| **Complexity** | Low |
| **Process Spawning** | ⚠️ May be blocked by sandbox |

## Known Limitations

App Service runs containers in a sandboxed environment that may:
- Block execution of downloaded binaries
- Restrict certain system calls
- Limit child process creation

If the Extension Host fails to start extensions, consider using:
- **Container Apps** (recommended)
- **Azure Container Instances**
- **Azure Kubernetes Service**

## Cleanup

```powershell
az group delete --name rg-bicep-extension-host-appservice --yes --no-wait
```
