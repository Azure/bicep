// mandatory params
param dnsPrefix string
//@[11:13]     "dnsPrefix": {
param linuxAdminUsername string
//@[14:16]     "linuxAdminUsername": {
param sshRSAPublicKey string
//@[17:19]     "sshRSAPublicKey": {

@secure()
param servcePrincipalClientId string
//@[20:22]     "servcePrincipalClientId": {

@secure()
param servicePrincipalClientSecret string
//@[23:25]     "servicePrincipalClientSecret": {

// optional params
param clusterName string = 'aks101cluster'
//@[26:29]     "clusterName": {
param location string = resourceGroup().location
//@[30:33]     "location": {

@minValue(0)
@maxValue(1023)
param osDiskSizeGB int = 0
//@[34:39]     "osDiskSizeGB": {

@minValue(1)
@maxValue(50)
param agentCount int = 3
//@[40:45]     "agentCount": {

param agentVMSize string = 'Standard_DS2_v2'
//@[46:49]     "agentVMSize": {
// osType was a defaultValue with only one allowedValue, which seems strange?, could be a good TTK test

resource aks 'Microsoft.ContainerService/managedClusters@2020-03-01' = {
//@[52:83]       "type": "Microsoft.ContainerService/managedClusters",
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
