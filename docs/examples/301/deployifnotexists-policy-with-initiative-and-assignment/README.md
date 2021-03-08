# DeployIfNotExists Policy with Initiative and Assignment

Resources Deployed:
* 1x Resource Group
* 1x Action Group
* 1x Policy Definition with DeployIfNotExists effect for a Metric Alert v2 (Load Balancer - DipAvailability)
* 1x Policy Initiative (policyset)
* 1x Policy Assignment

Authored & Tested with:
* azure-cli version 2.20.0
* bicep cli version 0.3.1 (d0f5c9b164)
* bicep 0.3.1 vscode extension

Example Deployment steps
```
az login
az bicep build -f ./main.bicep
az deployment sub create -f ./main.bicep -l australiaeast
```