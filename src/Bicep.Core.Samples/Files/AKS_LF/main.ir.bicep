// mandatory params
//@[00:1520) ProgramExpression
param dnsPrefix string
//@[00:0022) ├─DeclaredParameterExpression { Name = dnsPrefix }
param linuxAdminUsername string
//@[00:0031) ├─DeclaredParameterExpression { Name = linuxAdminUsername }
param sshRSAPublicKey string
//@[00:0028) ├─DeclaredParameterExpression { Name = sshRSAPublicKey }

@secure()
//@[00:0046) ├─DeclaredParameterExpression { Name = servcePrincipalClientId }
param servcePrincipalClientId string

@secure()
//@[00:0051) ├─DeclaredParameterExpression { Name = servicePrincipalClientSecret }
param servicePrincipalClientSecret string

// optional params
param clusterName string = 'aks101cluster'
//@[00:0042) ├─DeclaredParameterExpression { Name = clusterName }
//@[27:0042) | └─StringLiteralExpression { Value = aks101cluster }
param location string = resourceGroup().location
//@[00:0048) ├─DeclaredParameterExpression { Name = location }
//@[24:0048) | └─PropertyAccessExpression { PropertyName = location }
//@[24:0039) |   └─FunctionCallExpression { Name = resourceGroup }

@minValue(0)
//@[00:0055) ├─DeclaredParameterExpression { Name = osDiskSizeGB }
@maxValue(1023)
param osDiskSizeGB int = 0
//@[25:0026) | └─IntegerLiteralExpression { Value = 0 }

@minValue(1)
//@[00:0051) ├─DeclaredParameterExpression { Name = agentCount }
@maxValue(50)
param agentCount int = 3
//@[23:0024) | └─IntegerLiteralExpression { Value = 3 }

param agentVMSize string = 'Standard_DS2_v2'
//@[00:0044) ├─DeclaredParameterExpression { Name = agentVMSize }
//@[27:0044) | └─StringLiteralExpression { Value = Standard_DS2_v2 }
// osType was a defaultValue with only one allowedValue, which seems strange?, could be a good TTK test

resource aks 'Microsoft.ContainerService/managedClusters@2020-03-01' = {
//@[00:0825) └─DeclaredResourceExpression
//@[71:0825)   └─ObjectExpression
    name: clusterName
    location: location
//@[04:0022)     ├─ObjectPropertyExpression
//@[04:0012)     | ├─StringLiteralExpression { Value = location }
//@[14:0022)     | └─ParametersReferenceExpression { Parameter = location }
    properties: {
//@[04:0705)     └─ObjectPropertyExpression
//@[04:0014)       ├─StringLiteralExpression { Value = properties }
//@[16:0705)       └─ObjectExpression
        dnsPrefix: dnsPrefix
//@[08:0028)         ├─ObjectPropertyExpression
//@[08:0017)         | ├─StringLiteralExpression { Value = dnsPrefix }
//@[19:0028)         | └─ParametersReferenceExpression { Parameter = dnsPrefix }
        agentPoolProfiles: [
//@[08:0258)         ├─ObjectPropertyExpression
//@[08:0025)         | ├─StringLiteralExpression { Value = agentPoolProfiles }
//@[27:0258)         | └─ArrayExpression
            {
//@[12:0219)         |   └─ObjectExpression
                name: 'agentpool'
//@[16:0033)         |     ├─ObjectPropertyExpression
//@[16:0020)         |     | ├─StringLiteralExpression { Value = name }
//@[22:0033)         |     | └─StringLiteralExpression { Value = agentpool }
                osDiskSizeGB: osDiskSizeGB
//@[16:0042)         |     ├─ObjectPropertyExpression
//@[16:0028)         |     | ├─StringLiteralExpression { Value = osDiskSizeGB }
//@[30:0042)         |     | └─ParametersReferenceExpression { Parameter = osDiskSizeGB }
                vmSize: agentVMSize
//@[16:0035)         |     ├─ObjectPropertyExpression
//@[16:0022)         |     | ├─StringLiteralExpression { Value = vmSize }
//@[24:0035)         |     | └─ParametersReferenceExpression { Parameter = agentVMSize }
                osType: 'Linux'
//@[16:0031)         |     ├─ObjectPropertyExpression
//@[16:0022)         |     | ├─StringLiteralExpression { Value = osType }
//@[24:0031)         |     | └─StringLiteralExpression { Value = Linux }
                storageProfile: 'ManagedDisks'
//@[16:0046)         |     └─ObjectPropertyExpression
//@[16:0030)         |       ├─StringLiteralExpression { Value = storageProfile }
//@[32:0046)         |       └─StringLiteralExpression { Value = ManagedDisks }
            }
        ]
        linuxProfile: {
//@[08:0253)         ├─ObjectPropertyExpression
//@[08:0020)         | ├─StringLiteralExpression { Value = linuxProfile }
//@[22:0253)         | └─ObjectExpression
            adminUsername: linuxAdminUsername
//@[12:0045)         |   ├─ObjectPropertyExpression
//@[12:0025)         |   | ├─StringLiteralExpression { Value = adminUsername }
//@[27:0045)         |   | └─ParametersReferenceExpression { Parameter = linuxAdminUsername }
            ssh: {
//@[12:0173)         |   └─ObjectPropertyExpression
//@[12:0015)         |     ├─StringLiteralExpression { Value = ssh }
//@[17:0173)         |     └─ObjectExpression
                publicKeys: [
//@[16:0140)         |       └─ObjectPropertyExpression
//@[16:0026)         |         ├─StringLiteralExpression { Value = publicKeys }
//@[28:0140)         |         └─ArrayExpression
                    {
//@[20:0092)         |           └─ObjectExpression
                        keyData: sshRSAPublicKey
//@[24:0048)         |             └─ObjectPropertyExpression
//@[24:0031)         |               ├─StringLiteralExpression { Value = keyData }
//@[33:0048)         |               └─ParametersReferenceExpression { Parameter = sshRSAPublicKey }
                    }
                ]
            }
        }
        servicePrincipalProfile: {
//@[08:0139)         └─ObjectPropertyExpression
//@[08:0031)           ├─StringLiteralExpression { Value = servicePrincipalProfile }
//@[33:0139)           └─ObjectExpression
            clientId: servcePrincipalClientId
//@[12:0045)             ├─ObjectPropertyExpression
//@[12:0020)             | ├─StringLiteralExpression { Value = clientId }
//@[22:0045)             | └─ParametersReferenceExpression { Parameter = servcePrincipalClientId }
            secret: servicePrincipalClientSecret
//@[12:0048)             └─ObjectPropertyExpression
//@[12:0018)               ├─StringLiteralExpression { Value = secret }
//@[20:0048)               └─ParametersReferenceExpression { Parameter = servicePrincipalClientSecret }
        }
    }
}

// fyi - dot property access (aks.fqdn) has not been spec'd
//output controlPlaneFQDN string = aks.properties.fqdn 
