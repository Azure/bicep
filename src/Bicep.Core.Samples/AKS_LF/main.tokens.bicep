// mandatory params
//@[19:20) NewLine |\n|
param dnsPrefix string
//@[0:5) Identifier |param|
//@[6:15) Identifier |dnsPrefix|
//@[16:22) Identifier |string|
//@[22:23) NewLine |\n|
param linuxAdminUsername string
//@[0:5) Identifier |param|
//@[6:24) Identifier |linuxAdminUsername|
//@[25:31) Identifier |string|
//@[31:32) NewLine |\n|
param sshRSAPublicKey string
//@[0:5) Identifier |param|
//@[6:21) Identifier |sshRSAPublicKey|
//@[22:28) Identifier |string|
//@[28:29) NewLine |\n|
param servcePrincipalClientId string {
//@[0:5) Identifier |param|
//@[6:29) Identifier |servcePrincipalClientId|
//@[30:36) Identifier |string|
//@[37:38) LeftBrace |{|
//@[38:39) NewLine |\n|
    secure: true
//@[4:10) Identifier |secure|
//@[10:11) Colon |:|
//@[12:16) TrueKeyword |true|
//@[16:17) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:2) NewLine |\n|
param servicePrincipalClientSecret string {
//@[0:5) Identifier |param|
//@[6:34) Identifier |servicePrincipalClientSecret|
//@[35:41) Identifier |string|
//@[42:43) LeftBrace |{|
//@[43:44) NewLine |\n|
    secure: true
//@[4:10) Identifier |secure|
//@[10:11) Colon |:|
//@[12:16) TrueKeyword |true|
//@[16:17) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// optional params
//@[18:19) NewLine |\n|
param clusterName string = 'aks101cluster'
//@[0:5) Identifier |param|
//@[6:17) Identifier |clusterName|
//@[18:24) Identifier |string|
//@[25:26) Assignment |=|
//@[27:42) StringComplete |'aks101cluster'|
//@[42:43) NewLine |\n|
param location string = resourceGroup().location
//@[0:5) Identifier |param|
//@[6:14) Identifier |location|
//@[15:21) Identifier |string|
//@[22:23) Assignment |=|
//@[24:37) Identifier |resourceGroup|
//@[37:38) LeftParen |(|
//@[38:39) RightParen |)|
//@[39:40) Dot |.|
//@[40:48) Identifier |location|
//@[48:49) NewLine |\n|
param osDiskSizeGB int {
//@[0:5) Identifier |param|
//@[6:18) Identifier |osDiskSizeGB|
//@[19:22) Identifier |int|
//@[23:24) LeftBrace |{|
//@[24:25) NewLine |\n|
    default: 0
//@[4:11) Identifier |default|
//@[11:12) Colon |:|
//@[13:14) Number |0|
//@[14:15) NewLine |\n|
    minValue: 0
//@[4:12) Identifier |minValue|
//@[12:13) Colon |:|
//@[14:15) Number |0|
//@[15:16) NewLine |\n|
    maxValue: 1023
//@[4:12) Identifier |maxValue|
//@[12:13) Colon |:|
//@[14:18) Number |1023|
//@[18:19) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:2) NewLine |\n|
param agentCount int {
//@[0:5) Identifier |param|
//@[6:16) Identifier |agentCount|
//@[17:20) Identifier |int|
//@[21:22) LeftBrace |{|
//@[22:23) NewLine |\n|
    default: 3
//@[4:11) Identifier |default|
//@[11:12) Colon |:|
//@[13:14) Number |3|
//@[14:15) NewLine |\n|
    minValue: 1
//@[4:12) Identifier |minValue|
//@[12:13) Colon |:|
//@[14:15) Number |1|
//@[15:16) NewLine |\n|
    maxValue: 50
//@[4:12) Identifier |maxValue|
//@[12:13) Colon |:|
//@[14:16) Number |50|
//@[16:17) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:2) NewLine |\n|
param agentVMSize string = 'Standard_DS2_v2'
//@[0:5) Identifier |param|
//@[6:17) Identifier |agentVMSize|
//@[18:24) Identifier |string|
//@[25:26) Assignment |=|
//@[27:44) StringComplete |'Standard_DS2_v2'|
//@[44:45) NewLine |\n|
// osType was a defaultValue with only one allowedValue, which seems strange?, could be a good TTK test
//@[103:105) NewLine |\n\n|

resource aks 'Microsoft.ContainerService/managedClusters@2020-03-01' = {
//@[0:8) Identifier |resource|
//@[9:12) Identifier |aks|
//@[13:68) StringComplete |'Microsoft.ContainerService/managedClusters@2020-03-01'|
//@[69:70) Assignment |=|
//@[71:72) LeftBrace |{|
//@[72:73) NewLine |\n|
    name: clusterName
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:21) Identifier |clusterName|
//@[21:22) NewLine |\n|
    location: location
//@[4:12) Identifier |location|
//@[12:13) Colon |:|
//@[14:22) Identifier |location|
//@[22:23) NewLine |\n|
    properties: {
//@[4:14) Identifier |properties|
//@[14:15) Colon |:|
//@[16:17) LeftBrace |{|
//@[17:18) NewLine |\n|
        dnsPrefix: dnsPrefix
//@[8:17) Identifier |dnsPrefix|
//@[17:18) Colon |:|
//@[19:28) Identifier |dnsPrefix|
//@[28:29) NewLine |\n|
        agentPoolProfiles: [
//@[8:25) Identifier |agentPoolProfiles|
//@[25:26) Colon |:|
//@[27:28) LeftSquare |[|
//@[28:29) NewLine |\n|
            {
//@[12:13) LeftBrace |{|
//@[13:14) NewLine |\n|
                name: 'agentpool'
//@[16:20) Identifier |name|
//@[20:21) Colon |:|
//@[22:33) StringComplete |'agentpool'|
//@[33:34) NewLine |\n|
                osDiskSizeGB: osDiskSizeGB
//@[16:28) Identifier |osDiskSizeGB|
//@[28:29) Colon |:|
//@[30:42) Identifier |osDiskSizeGB|
//@[42:43) NewLine |\n|
                vmSize: agentVMSize
//@[16:22) Identifier |vmSize|
//@[22:23) Colon |:|
//@[24:35) Identifier |agentVMSize|
//@[35:36) NewLine |\n|
                osType: 'Linux'
//@[16:22) Identifier |osType|
//@[22:23) Colon |:|
//@[24:31) StringComplete |'Linux'|
//@[31:32) NewLine |\n|
                storageProfile: 'ManagedDisks'
//@[16:30) Identifier |storageProfile|
//@[30:31) Colon |:|
//@[32:46) StringComplete |'ManagedDisks'|
//@[46:47) NewLine |\n|
            }
//@[12:13) RightBrace |}|
//@[13:14) NewLine |\n|
        ]
//@[8:9) RightSquare |]|
//@[9:10) NewLine |\n|
        linuxProfile: {
//@[8:20) Identifier |linuxProfile|
//@[20:21) Colon |:|
//@[22:23) LeftBrace |{|
//@[23:24) NewLine |\n|
            adminUsername: linuxAdminUsername
//@[12:25) Identifier |adminUsername|
//@[25:26) Colon |:|
//@[27:45) Identifier |linuxAdminUsername|
//@[45:46) NewLine |\n|
            ssh: {
//@[12:15) Identifier |ssh|
//@[15:16) Colon |:|
//@[17:18) LeftBrace |{|
//@[18:19) NewLine |\n|
                publicKeys: [
//@[16:26) Identifier |publicKeys|
//@[26:27) Colon |:|
//@[28:29) LeftSquare |[|
//@[29:30) NewLine |\n|
                    {
//@[20:21) LeftBrace |{|
//@[21:22) NewLine |\n|
                        keyData: sshRSAPublicKey
//@[24:31) Identifier |keyData|
//@[31:32) Colon |:|
//@[33:48) Identifier |sshRSAPublicKey|
//@[48:49) NewLine |\n|
                    }
//@[20:21) RightBrace |}|
//@[21:22) NewLine |\n|
                ]
//@[16:17) RightSquare |]|
//@[17:18) NewLine |\n|
            }
//@[12:13) RightBrace |}|
//@[13:14) NewLine |\n|
        }
//@[8:9) RightBrace |}|
//@[9:10) NewLine |\n|
        servicePrincipalProfile: {
//@[8:31) Identifier |servicePrincipalProfile|
//@[31:32) Colon |:|
//@[33:34) LeftBrace |{|
//@[34:35) NewLine |\n|
            clientId: servcePrincipalClientId
//@[12:20) Identifier |clientId|
//@[20:21) Colon |:|
//@[22:45) Identifier |servcePrincipalClientId|
//@[45:46) NewLine |\n|
            secret: servicePrincipalClientSecret
//@[12:18) Identifier |secret|
//@[18:19) Colon |:|
//@[20:48) Identifier |servicePrincipalClientSecret|
//@[48:49) NewLine |\n|
        }
//@[8:9) RightBrace |}|
//@[9:10) NewLine |\n|
    }
//@[4:5) RightBrace |}|
//@[5:6) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// fyi - dot property access (aks.fqdn) has not been spec'd
//@[59:60) NewLine |\n|
//output controlPlaneFQDN string = aks.properties.fqdn 
//@[55:55) EndOfFile ||
