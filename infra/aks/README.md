# Azure Kubernetes Service (AKS) Deployment

Deploy Bicep Extension Host to Azure Kubernetes Service.

> ✅ **Recommended**: AKS provides full control over container execution with no sandbox limitations. Process spawning works as expected.

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                 rg-bicep-extension-host-aks                 │
│  ┌─────────────────────────────────────────────────────┐   │
│  │                   AKS Cluster                        │   │
│  │  ┌─────────────────────────────────────────────┐    │   │
│  │  │              Node Pool (Linux)               │    │   │
│  │  │  ┌─────────────────────────────────────┐    │    │   │
│  │  │  │     Pod: Bicep Extension Host       │    │    │   │
│  │  │  │     - Downloads OCI artifacts       │    │    │   │
│  │  │  │     - Starts gRPC servers ✅        │    │    │   │
│  │  │  │     - Exposes HTTP endpoints        │    │    │   │
│  │  │  └─────────────────────────────────────┘    │    │   │
│  │  └─────────────────────────────────────────────┘    │   │
│  │                        │                             │   │
│  │              LoadBalancer Service                    │   │
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
| `main.bicep` | AKS cluster deployment template |
| `main.bicepparam` | Parameters file |
| `k8s-deployment.yaml` | Kubernetes deployment & service manifest |

## Deploy

### Step 1: Create AKS Cluster

```powershell
# Create resource group
az group create --name rg-bicep-extension-host-aks --location eastus

# Deploy AKS cluster
az deployment group create `
  --resource-group rg-bicep-extension-host-aks `
  --template-file infra/aks/main.bicep `
  --parameters infra/aks/main.bicepparam
```

### Step 2: Get AKS Credentials

```powershell
az aks get-credentials `
  --resource-group rg-bicep-extension-host-aks `
  --name (az aks list -g rg-bicep-extension-host-aks --query '[0].name' -o tsv)
```

### Step 3: Deploy Application

```powershell
kubectl apply -f infra/aks/k8s-deployment.yaml
```

### Step 4: Get External IP

```powershell
# Wait for external IP (may take a few minutes)
kubectl get service bicep-extension-host --watch
```

## Verify

```powershell
# Get the external IP
$EXTERNAL_IP = kubectl get service bicep-extension-host -o jsonpath='{.status.loadBalancer.ingress[0].ip}'

# Check health endpoint
curl "http://$EXTERNAL_IP/health"

# Check extensions
curl "http://$EXTERNAL_IP/extensions"
```

## View Logs

```powershell
# Get pod name
$POD = kubectl get pods -l app=bicep-extension-host -o jsonpath='{.items[0].metadata.name}'

# View logs
kubectl logs $POD

# Follow logs
kubectl logs $POD -f
```

## Characteristics

| Aspect | Value |
|--------|-------|
| **Scaling** | Manual or HPA (Horizontal Pod Autoscaler) |
| **Cost** | VM cost (~$30-50/month for B2s) + AKS free tier |
| **Cold Start** | None (always running) |
| **Networking** | LoadBalancer with public IP |
| **Complexity** | Medium (requires kubectl) |
| **Process Spawning** | ✅ Full support |

## Configure Extensions

Edit `k8s-deployment.yaml` to add extension configuration:

```yaml
env:
  - name: ExtensionHost__Extensions
    value: '[{"Name":"kubernetes","RegistryUri":"https://ligaracr.azurecr.io","Repository":"bicep/extensions/kubernetes","Tag":"1.0.0"}]'
```

Then apply:
```powershell
kubectl apply -f infra/aks/k8s-deployment.yaml
```

## Cleanup

```powershell
# Delete Kubernetes resources
kubectl delete -f infra/aks/k8s-deployment.yaml

# Delete Azure resources
az group delete --name rg-bicep-extension-host-aks --yes --no-wait
```
