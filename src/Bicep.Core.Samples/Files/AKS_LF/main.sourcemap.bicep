// mandatory params
param dnsPrefix string
//@[12:14]     "dnsPrefix": {
param linuxAdminUsername string
//@[15:17]     "linuxAdminUsername": {
param sshRSAPublicKey string
//@[18:20]     "sshRSAPublicKey": {

@secure()
param servcePrincipalClientId string
//@[21:23]     "servcePrincipalClientId": {

@secure()
param servicePrincipalClientSecret string
//@[24:26]     "servicePrincipalClientSecret": {

// optional params
param clusterName string = 'aks101cluster'
//@[27:30]     "clusterName": {
param location string = resourceGroup().location
//@[31:34]     "location": {

@minValue(0)
@maxValue(1023)
param osDiskSizeGB int = 0
//@[35:40]     "osDiskSizeGB": {

@minValue(1)
@maxValue(50)
param agentCount int = 3
//@[41:46]     "agentCount": {

param agentVMSize string = 'Standard_DS2_v2'
//@[47:50]     "agentVMSize": {
// osType was a defaultValue with only one allowedValue, which seems strange?, could be a good TTK test

resource aks 'Microsoft.ContainerService/managedClusters@2020-03-01' = {
//@[53:84]       "type": "Microsoft.ContainerService/managedClusters",
    name: clusterName
    location: location
//@[57:57]       "location": "[parameters('location')]",
    properties: {
//@[58:83]       "properties": {
        dnsPrefix: dnsPrefix
//@[59:59]         "dnsPrefix": "[parameters('dnsPrefix')]",
        agentPoolProfiles: [
//@[60:68]         "agentPoolProfiles": [
            {
                name: 'agentpool'
//@[62:62]             "name": "agentpool",
                osDiskSizeGB: osDiskSizeGB
//@[63:63]             "osDiskSizeGB": "[parameters('osDiskSizeGB')]",
                vmSize: agentVMSize
//@[64:64]             "vmSize": "[parameters('agentVMSize')]",
                osType: 'Linux'
//@[65:65]             "osType": "Linux",
                storageProfile: 'ManagedDisks'
//@[66:66]             "storageProfile": "ManagedDisks"
            }
        ]
        linuxProfile: {
//@[69:78]         "linuxProfile": {
            adminUsername: linuxAdminUsername
//@[70:70]           "adminUsername": "[parameters('linuxAdminUsername')]",
            ssh: {
//@[71:77]           "ssh": {
                publicKeys: [
//@[72:76]             "publicKeys": [
                    {
                        keyData: sshRSAPublicKey
//@[74:74]                 "keyData": "[parameters('sshRSAPublicKey')]"
                    }
                ]
            }
        }
        servicePrincipalProfile: {
//@[79:82]         "servicePrincipalProfile": {
            clientId: servcePrincipalClientId
//@[80:80]           "clientId": "[parameters('servcePrincipalClientId')]",
            secret: servicePrincipalClientSecret
//@[81:81]           "secret": "[parameters('servicePrincipalClientSecret')]"
        }
    }
}

// fyi - dot property access (aks.fqdn) has not been spec'd
//output controlPlaneFQDN string = aks.properties.fqdn 
