// mandatory params
param dnsPrefix string
param linuxAdminUsername string
param sshRSAPublicKey string

@secure()
param servcePrincipalClientId string

@secure()
param servicePrincipalClientSecret string

// optional params
param clusterName string = 'aks101cluster'
param location string = resourceGroup().location

@minValue(0)
@maxValue(1023)
param osDiskSizeGB int = 0

@minValue(1)
@maxValue(50)
param agentCount int = 3
//@[06:16) [no-unused-params (Warning)] Parameter "agentCount" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |agentCount|

param agentVMSize string = 'Standard_DS2_v2'
// osType was a defaultValue with only one allowedValue, which seems strange?, could be a good TTK test

resource aks 'Microsoft.ContainerService/managedClusters@2020-03-01' = {
//@[57:67) [use-recent-api-version (Warning)] Use more recent API version for 'Microsoft.ContainerService/managedClusters'. '2020-03-01' is 869 days old, should be no more than 730 days old. Acceptable versions: 2022-05-02-preview, 2022-04-02-preview, 2022-04-01, 2022-03-01, 2022-02-01, 2022-01-01, 2021-10-01, 2021-09-01, 2021-08-01, 2021-07-01, 2021-05-01, 2021-03-01, 2021-02-01, 2020-12-01, 2020-11-01, 2020-09-01 (CodeDescription: bicep core(https://aka.ms/bicep/linter/use-recent-api-version)) |2020-03-01|
    name: clusterName
    location: location
    properties: {
        dnsPrefix: dnsPrefix
        agentPoolProfiles: [
            {
                name: 'agentpool'
                osDiskSizeGB: osDiskSizeGB
                vmSize: agentVMSize
                osType: 'Linux'
                storageProfile: 'ManagedDisks'
//@[16:30) [BCP037 (Warning)] The property "storageProfile" is not allowed on objects of type "ManagedClusterAgentPoolProfile". Permissible properties include "availabilityZones", "count", "enableAutoScaling", "enableNodePublicIP", "maxCount", "maxPods", "minCount", "mode", "nodeLabels", "nodeTaints", "orchestratorVersion", "scaleSetEvictionPolicy", "scaleSetPriority", "spotMaxPrice", "tags", "type", "vnetSubnetID". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |storageProfile|
            }
        ]
        linuxProfile: {
            adminUsername: linuxAdminUsername
            ssh: {
                publicKeys: [
                    {
                        keyData: sshRSAPublicKey
                    }
                ]
            }
        }
        servicePrincipalProfile: {
            clientId: servcePrincipalClientId
            secret: servicePrincipalClientSecret
        }
    }
}

// fyi - dot property access (aks.fqdn) has not been spec'd
//output controlPlaneFQDN string = aks.properties.fqdn 
