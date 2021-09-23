
# Azure Kubernetes Service

## Deployment steps ##

### 0. Optional: Install Bicep ###
* [Install the Bicep CLI](https://github.com/Azure/bicep/blob/main/docs/installing.md) by following the instruction.

### 1. Create Service Principal ###
```
az ad sp create-for-rbac
```
### 2. Create Resource Group ####
```
az group create -n <resource-group-name> -l westus
```

### 3. Optional: Generete SSH Keys (Windows) ###
[How to Generate SSH Key in Windows 10](https://phoenixnap.com/kb/generate-ssh-key-windows-10)

### 4. Deploy ####
```
az deployment group create -g <resource-group-name> -f .\main.bicep --parameters dnsPrefix=<dns-prefix> linuxAdminUsername=<username> servicePrincipalClientId=<sp-appid> servicePrincipalClientSecret=<sp-password> sshRSAPublicKey=<ssh-public-key>
```
