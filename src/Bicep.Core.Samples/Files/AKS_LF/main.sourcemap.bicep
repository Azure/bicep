// mandatory params
param dnsPrefix string
//@[11:13]     "dnsPrefix": {\r
param linuxAdminUsername string
//@[14:16]     "linuxAdminUsername": {\r
param sshRSAPublicKey string
//@[17:19]     "sshRSAPublicKey": {\r

@secure()
param servcePrincipalClientId string
//@[20:22]     "servcePrincipalClientId": {\r

@secure()
param servicePrincipalClientSecret string
//@[23:25]     "servicePrincipalClientSecret": {\r

// optional params
param clusterName string = 'aks101cluster'
//@[26:29]     "clusterName": {\r
param location string = resourceGroup().location
//@[30:33]     "location": {\r

@minValue(0)
@maxValue(1023)
param osDiskSizeGB int = 0
//@[34:39]     "osDiskSizeGB": {\r

@minValue(1)
@maxValue(50)
param agentCount int = 3
//@[40:45]     "agentCount": {\r

param agentVMSize string = 'Standard_DS2_v2'
//@[46:49]     "agentVMSize": {\r
// osType was a defaultValue with only one allowedValue, which seems strange?, could be a good TTK test

resource aks 'Microsoft.ContainerService/managedClusters@2020-03-01' = {
//@[52:83]       "type": "Microsoft.ContainerService/managedClusters",\r
    name: clusterName
    location: location
//@[56:56]       "location": "[parameters('location')]",\r
    properties: {
//@[57:82]       "properties": {\r
        dnsPrefix: dnsPrefix
//@[58:58]         "dnsPrefix": "[parameters('dnsPrefix')]",\r
        agentPoolProfiles: [
//@[59:67]         "agentPoolProfiles": [\r
            {
                name: 'agentpool'
//@[61:61]             "name": "agentpool",\r
                osDiskSizeGB: osDiskSizeGB
//@[62:62]             "osDiskSizeGB": "[parameters('osDiskSizeGB')]",\r
                vmSize: agentVMSize
//@[63:63]             "vmSize": "[parameters('agentVMSize')]",\r
                osType: 'Linux'
//@[64:64]             "osType": "Linux",\r
                storageProfile: 'ManagedDisks'
//@[65:65]             "storageProfile": "ManagedDisks"\r
            }
        ]
        linuxProfile: {
//@[68:77]         "linuxProfile": {\r
            adminUsername: linuxAdminUsername
//@[69:69]           "adminUsername": "[parameters('linuxAdminUsername')]",\r
            ssh: {
//@[70:76]           "ssh": {\r
                publicKeys: [
//@[71:75]             "publicKeys": [\r
                    {
                        keyData: sshRSAPublicKey
//@[73:73]                 "keyData": "[parameters('sshRSAPublicKey')]"\r
                    }
                ]
            }
        }
        servicePrincipalProfile: {
//@[78:81]         "servicePrincipalProfile": {\r
            clientId: servcePrincipalClientId
//@[79:79]           "clientId": "[parameters('servcePrincipalClientId')]",\r
            secret: servicePrincipalClientSecret
//@[80:80]           "secret": "[parameters('servicePrincipalClientSecret')]"\r
        }
    }
}

// fyi - dot property access (aks.fqdn) has not been spec'd
//output controlPlaneFQDN string = aks.properties.fqdn 
