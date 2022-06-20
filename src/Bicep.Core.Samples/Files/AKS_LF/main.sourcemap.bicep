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
//@[56:56]       "location": "[parameters('location')]",
    properties: {
//@[57:82]       "properties": {
        dnsPrefix: dnsPrefix
//@[58:58]         "dnsPrefix": "[parameters('dnsPrefix')]",
        agentPoolProfiles: [
//@[59:67]         "agentPoolProfiles": [
            {
                name: 'agentpool'
//@[61:61]             "name": "agentpool",
                osDiskSizeGB: osDiskSizeGB
//@[62:62]             "osDiskSizeGB": "[parameters('osDiskSizeGB')]",
                vmSize: agentVMSize
//@[63:63]             "vmSize": "[parameters('agentVMSize')]",
                osType: 'Linux'
//@[64:64]             "osType": "Linux",
                storageProfile: 'ManagedDisks'
//@[65:65]             "storageProfile": "ManagedDisks"
            }
        ]
        linuxProfile: {
//@[68:77]         "linuxProfile": {
            adminUsername: linuxAdminUsername
//@[69:69]           "adminUsername": "[parameters('linuxAdminUsername')]",
            ssh: {
//@[70:76]           "ssh": {
                publicKeys: [
//@[71:75]             "publicKeys": [
                    {
                        keyData: sshRSAPublicKey
//@[73:73]                 "keyData": "[parameters('sshRSAPublicKey')]"
                    }
                ]
            }
        }
        servicePrincipalProfile: {
//@[78:81]         "servicePrincipalProfile": {
            clientId: servcePrincipalClientId
//@[79:79]           "clientId": "[parameters('servcePrincipalClientId')]",
            secret: servicePrincipalClientSecret
//@[80:80]           "secret": "[parameters('servicePrincipalClientSecret')]"
        }
    }
}

// fyi - dot property access (aks.fqdn) has not been spec'd
//output controlPlaneFQDN string = aks.properties.fqdn 
