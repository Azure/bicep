// mandatory params
//@[019:020) NewLine |\n|
param dnsPrefix string
//@[000:005) Identifier |param|
//@[006:015) Identifier |dnsPrefix|
//@[016:022) Identifier |string|
//@[022:023) NewLine |\n|
param linuxAdminUsername string
//@[000:005) Identifier |param|
//@[006:024) Identifier |linuxAdminUsername|
//@[025:031) Identifier |string|
//@[031:032) NewLine |\n|
param sshRSAPublicKey string
//@[000:005) Identifier |param|
//@[006:021) Identifier |sshRSAPublicKey|
//@[022:028) Identifier |string|
//@[028:030) NewLine |\n\n|

@secure()
//@[000:001) At |@|
//@[001:007) Identifier |secure|
//@[007:008) LeftParen |(|
//@[008:009) RightParen |)|
//@[009:010) NewLine |\n|
param servcePrincipalClientId string
//@[000:005) Identifier |param|
//@[006:029) Identifier |servcePrincipalClientId|
//@[030:036) Identifier |string|
//@[036:038) NewLine |\n\n|

@secure()
//@[000:001) At |@|
//@[001:007) Identifier |secure|
//@[007:008) LeftParen |(|
//@[008:009) RightParen |)|
//@[009:010) NewLine |\n|
param servicePrincipalClientSecret string
//@[000:005) Identifier |param|
//@[006:034) Identifier |servicePrincipalClientSecret|
//@[035:041) Identifier |string|
//@[041:043) NewLine |\n\n|

// optional params
//@[018:019) NewLine |\n|
param clusterName string = 'aks101cluster'
//@[000:005) Identifier |param|
//@[006:017) Identifier |clusterName|
//@[018:024) Identifier |string|
//@[025:026) Assignment |=|
//@[027:042) StringComplete |'aks101cluster'|
//@[042:043) NewLine |\n|
param location string = resourceGroup().location
//@[000:005) Identifier |param|
//@[006:014) Identifier |location|
//@[015:021) Identifier |string|
//@[022:023) Assignment |=|
//@[024:037) Identifier |resourceGroup|
//@[037:038) LeftParen |(|
//@[038:039) RightParen |)|
//@[039:040) Dot |.|
//@[040:048) Identifier |location|
//@[048:050) NewLine |\n\n|

@minValue(0)
//@[000:001) At |@|
//@[001:009) Identifier |minValue|
//@[009:010) LeftParen |(|
//@[010:011) Integer |0|
//@[011:012) RightParen |)|
//@[012:013) NewLine |\n|
@maxValue(1023)
//@[000:001) At |@|
//@[001:009) Identifier |maxValue|
//@[009:010) LeftParen |(|
//@[010:014) Integer |1023|
//@[014:015) RightParen |)|
//@[015:016) NewLine |\n|
param osDiskSizeGB int = 0
//@[000:005) Identifier |param|
//@[006:018) Identifier |osDiskSizeGB|
//@[019:022) Identifier |int|
//@[023:024) Assignment |=|
//@[025:026) Integer |0|
//@[026:028) NewLine |\n\n|

@minValue(1)
//@[000:001) At |@|
//@[001:009) Identifier |minValue|
//@[009:010) LeftParen |(|
//@[010:011) Integer |1|
//@[011:012) RightParen |)|
//@[012:013) NewLine |\n|
@maxValue(50)
//@[000:001) At |@|
//@[001:009) Identifier |maxValue|
//@[009:010) LeftParen |(|
//@[010:012) Integer |50|
//@[012:013) RightParen |)|
//@[013:014) NewLine |\n|
param agentCount int = 3
//@[000:005) Identifier |param|
//@[006:016) Identifier |agentCount|
//@[017:020) Identifier |int|
//@[021:022) Assignment |=|
//@[023:024) Integer |3|
//@[024:026) NewLine |\n\n|

param agentVMSize string = 'Standard_DS2_v2'
//@[000:005) Identifier |param|
//@[006:017) Identifier |agentVMSize|
//@[018:024) Identifier |string|
//@[025:026) Assignment |=|
//@[027:044) StringComplete |'Standard_DS2_v2'|
//@[044:045) NewLine |\n|
// osType was a defaultValue with only one allowedValue, which seems strange?, could be a good TTK test
//@[103:105) NewLine |\n\n|

resource aks 'Microsoft.ContainerService/managedClusters@2020-03-01' = {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |aks|
//@[013:068) StringComplete |'Microsoft.ContainerService/managedClusters@2020-03-01'|
//@[069:070) Assignment |=|
//@[071:072) LeftBrace |{|
//@[072:073) NewLine |\n|
    name: clusterName
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:021) Identifier |clusterName|
//@[021:022) NewLine |\n|
    location: location
//@[004:012) Identifier |location|
//@[012:013) Colon |:|
//@[014:022) Identifier |location|
//@[022:023) NewLine |\n|
    properties: {
//@[004:014) Identifier |properties|
//@[014:015) Colon |:|
//@[016:017) LeftBrace |{|
//@[017:018) NewLine |\n|
        dnsPrefix: dnsPrefix
//@[008:017) Identifier |dnsPrefix|
//@[017:018) Colon |:|
//@[019:028) Identifier |dnsPrefix|
//@[028:029) NewLine |\n|
        agentPoolProfiles: [
//@[008:025) Identifier |agentPoolProfiles|
//@[025:026) Colon |:|
//@[027:028) LeftSquare |[|
//@[028:029) NewLine |\n|
            {
//@[012:013) LeftBrace |{|
//@[013:014) NewLine |\n|
                name: 'agentpool'
//@[016:020) Identifier |name|
//@[020:021) Colon |:|
//@[022:033) StringComplete |'agentpool'|
//@[033:034) NewLine |\n|
                osDiskSizeGB: osDiskSizeGB
//@[016:028) Identifier |osDiskSizeGB|
//@[028:029) Colon |:|
//@[030:042) Identifier |osDiskSizeGB|
//@[042:043) NewLine |\n|
                vmSize: agentVMSize
//@[016:022) Identifier |vmSize|
//@[022:023) Colon |:|
//@[024:035) Identifier |agentVMSize|
//@[035:036) NewLine |\n|
                osType: 'Linux'
//@[016:022) Identifier |osType|
//@[022:023) Colon |:|
//@[024:031) StringComplete |'Linux'|
//@[031:032) NewLine |\n|
                storageProfile: 'ManagedDisks'
//@[016:030) Identifier |storageProfile|
//@[030:031) Colon |:|
//@[032:046) StringComplete |'ManagedDisks'|
//@[046:047) NewLine |\n|
            }
//@[012:013) RightBrace |}|
//@[013:014) NewLine |\n|
        ]
//@[008:009) RightSquare |]|
//@[009:010) NewLine |\n|
        linuxProfile: {
//@[008:020) Identifier |linuxProfile|
//@[020:021) Colon |:|
//@[022:023) LeftBrace |{|
//@[023:024) NewLine |\n|
            adminUsername: linuxAdminUsername
//@[012:025) Identifier |adminUsername|
//@[025:026) Colon |:|
//@[027:045) Identifier |linuxAdminUsername|
//@[045:046) NewLine |\n|
            ssh: {
//@[012:015) Identifier |ssh|
//@[015:016) Colon |:|
//@[017:018) LeftBrace |{|
//@[018:019) NewLine |\n|
                publicKeys: [
//@[016:026) Identifier |publicKeys|
//@[026:027) Colon |:|
//@[028:029) LeftSquare |[|
//@[029:030) NewLine |\n|
                    {
//@[020:021) LeftBrace |{|
//@[021:022) NewLine |\n|
                        keyData: sshRSAPublicKey
//@[024:031) Identifier |keyData|
//@[031:032) Colon |:|
//@[033:048) Identifier |sshRSAPublicKey|
//@[048:049) NewLine |\n|
                    }
//@[020:021) RightBrace |}|
//@[021:022) NewLine |\n|
                ]
//@[016:017) RightSquare |]|
//@[017:018) NewLine |\n|
            }
//@[012:013) RightBrace |}|
//@[013:014) NewLine |\n|
        }
//@[008:009) RightBrace |}|
//@[009:010) NewLine |\n|
        servicePrincipalProfile: {
//@[008:031) Identifier |servicePrincipalProfile|
//@[031:032) Colon |:|
//@[033:034) LeftBrace |{|
//@[034:035) NewLine |\n|
            clientId: servcePrincipalClientId
//@[012:020) Identifier |clientId|
//@[020:021) Colon |:|
//@[022:045) Identifier |servcePrincipalClientId|
//@[045:046) NewLine |\n|
            secret: servicePrincipalClientSecret
//@[012:018) Identifier |secret|
//@[018:019) Colon |:|
//@[020:048) Identifier |servicePrincipalClientSecret|
//@[048:049) NewLine |\n|
        }
//@[008:009) RightBrace |}|
//@[009:010) NewLine |\n|
    }
//@[004:005) RightBrace |}|
//@[005:006) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

// fyi - dot property access (aks.fqdn) has not been spec'd
//@[059:060) NewLine |\n|
//output controlPlaneFQDN string = aks.properties.fqdn 
//@[055:055) EndOfFile ||
