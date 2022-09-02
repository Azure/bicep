// mandatory params
//@[000:1520) ProgramSyntax
//@[019:0020) ├─Token(NewLine) |\n|
param dnsPrefix string
//@[000:0022) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |dnsPrefix|
//@[016:0022) | └─SimpleTypeSyntax
//@[016:0022) |   └─Token(Identifier) |string|
//@[022:0023) ├─Token(NewLine) |\n|
param linuxAdminUsername string
//@[000:0031) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0024) | ├─IdentifierSyntax
//@[006:0024) | | └─Token(Identifier) |linuxAdminUsername|
//@[025:0031) | └─SimpleTypeSyntax
//@[025:0031) |   └─Token(Identifier) |string|
//@[031:0032) ├─Token(NewLine) |\n|
param sshRSAPublicKey string
//@[000:0028) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0021) | ├─IdentifierSyntax
//@[006:0021) | | └─Token(Identifier) |sshRSAPublicKey|
//@[022:0028) | └─SimpleTypeSyntax
//@[022:0028) |   └─Token(Identifier) |string|
//@[028:0030) ├─Token(NewLine) |\n\n|

@secure()
//@[000:0046) ├─ParameterDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |secure|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
param servcePrincipalClientId string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0029) | ├─IdentifierSyntax
//@[006:0029) | | └─Token(Identifier) |servcePrincipalClientId|
//@[030:0036) | └─SimpleTypeSyntax
//@[030:0036) |   └─Token(Identifier) |string|
//@[036:0038) ├─Token(NewLine) |\n\n|

@secure()
//@[000:0051) ├─ParameterDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |secure|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
param servicePrincipalClientSecret string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0034) | ├─IdentifierSyntax
//@[006:0034) | | └─Token(Identifier) |servicePrincipalClientSecret|
//@[035:0041) | └─SimpleTypeSyntax
//@[035:0041) |   └─Token(Identifier) |string|
//@[041:0043) ├─Token(NewLine) |\n\n|

// optional params
//@[018:0019) ├─Token(NewLine) |\n|
param clusterName string = 'aks101cluster'
//@[000:0042) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0017) | ├─IdentifierSyntax
//@[006:0017) | | └─Token(Identifier) |clusterName|
//@[018:0024) | ├─SimpleTypeSyntax
//@[018:0024) | | └─Token(Identifier) |string|
//@[025:0042) | └─ParameterDefaultValueSyntax
//@[025:0026) |   ├─Token(Assignment) |=|
//@[027:0042) |   └─StringSyntax
//@[027:0042) |     └─Token(StringComplete) |'aks101cluster'|
//@[042:0043) ├─Token(NewLine) |\n|
param location string = resourceGroup().location
//@[000:0048) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0014) | ├─IdentifierSyntax
//@[006:0014) | | └─Token(Identifier) |location|
//@[015:0021) | ├─SimpleTypeSyntax
//@[015:0021) | | └─Token(Identifier) |string|
//@[022:0048) | └─ParameterDefaultValueSyntax
//@[022:0023) |   ├─Token(Assignment) |=|
//@[024:0048) |   └─PropertyAccessSyntax
//@[024:0039) |     ├─FunctionCallSyntax
//@[024:0037) |     | ├─IdentifierSyntax
//@[024:0037) |     | | └─Token(Identifier) |resourceGroup|
//@[037:0038) |     | ├─Token(LeftParen) |(|
//@[038:0039) |     | └─Token(RightParen) |)|
//@[039:0040) |     ├─Token(Dot) |.|
//@[040:0048) |     └─IdentifierSyntax
//@[040:0048) |       └─Token(Identifier) |location|
//@[048:0050) ├─Token(NewLine) |\n\n|

@minValue(0)
//@[000:0055) ├─ParameterDeclarationSyntax
//@[000:0012) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0012) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |minValue|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0011) | |   ├─FunctionArgumentSyntax
//@[010:0011) | |   | └─IntegerLiteralSyntax
//@[010:0011) | |   |   └─Token(Integer) |0|
//@[011:0012) | |   └─Token(RightParen) |)|
//@[012:0013) | ├─Token(NewLine) |\n|
@maxValue(1023)
//@[000:0015) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0015) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |maxValue|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0014) | |   ├─FunctionArgumentSyntax
//@[010:0014) | |   | └─IntegerLiteralSyntax
//@[010:0014) | |   |   └─Token(Integer) |1023|
//@[014:0015) | |   └─Token(RightParen) |)|
//@[015:0016) | ├─Token(NewLine) |\n|
param osDiskSizeGB int = 0
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0018) | ├─IdentifierSyntax
//@[006:0018) | | └─Token(Identifier) |osDiskSizeGB|
//@[019:0022) | ├─SimpleTypeSyntax
//@[019:0022) | | └─Token(Identifier) |int|
//@[023:0026) | └─ParameterDefaultValueSyntax
//@[023:0024) |   ├─Token(Assignment) |=|
//@[025:0026) |   └─IntegerLiteralSyntax
//@[025:0026) |     └─Token(Integer) |0|
//@[026:0028) ├─Token(NewLine) |\n\n|

@minValue(1)
//@[000:0051) ├─ParameterDeclarationSyntax
//@[000:0012) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0012) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |minValue|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0011) | |   ├─FunctionArgumentSyntax
//@[010:0011) | |   | └─IntegerLiteralSyntax
//@[010:0011) | |   |   └─Token(Integer) |1|
//@[011:0012) | |   └─Token(RightParen) |)|
//@[012:0013) | ├─Token(NewLine) |\n|
@maxValue(50)
//@[000:0013) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0013) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |maxValue|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0012) | |   ├─FunctionArgumentSyntax
//@[010:0012) | |   | └─IntegerLiteralSyntax
//@[010:0012) | |   |   └─Token(Integer) |50|
//@[012:0013) | |   └─Token(RightParen) |)|
//@[013:0014) | ├─Token(NewLine) |\n|
param agentCount int = 3
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0016) | ├─IdentifierSyntax
//@[006:0016) | | └─Token(Identifier) |agentCount|
//@[017:0020) | ├─SimpleTypeSyntax
//@[017:0020) | | └─Token(Identifier) |int|
//@[021:0024) | └─ParameterDefaultValueSyntax
//@[021:0022) |   ├─Token(Assignment) |=|
//@[023:0024) |   └─IntegerLiteralSyntax
//@[023:0024) |     └─Token(Integer) |3|
//@[024:0026) ├─Token(NewLine) |\n\n|

param agentVMSize string = 'Standard_DS2_v2'
//@[000:0044) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0017) | ├─IdentifierSyntax
//@[006:0017) | | └─Token(Identifier) |agentVMSize|
//@[018:0024) | ├─SimpleTypeSyntax
//@[018:0024) | | └─Token(Identifier) |string|
//@[025:0044) | └─ParameterDefaultValueSyntax
//@[025:0026) |   ├─Token(Assignment) |=|
//@[027:0044) |   └─StringSyntax
//@[027:0044) |     └─Token(StringComplete) |'Standard_DS2_v2'|
//@[044:0045) ├─Token(NewLine) |\n|
// osType was a defaultValue with only one allowedValue, which seems strange?, could be a good TTK test
//@[103:0105) ├─Token(NewLine) |\n\n|

resource aks 'Microsoft.ContainerService/managedClusters@2020-03-01' = {
//@[000:0825) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0012) | ├─IdentifierSyntax
//@[009:0012) | | └─Token(Identifier) |aks|
//@[013:0068) | ├─StringSyntax
//@[013:0068) | | └─Token(StringComplete) |'Microsoft.ContainerService/managedClusters@2020-03-01'|
//@[069:0070) | ├─Token(Assignment) |=|
//@[071:0825) | └─ObjectSyntax
//@[071:0072) |   ├─Token(LeftBrace) |{|
//@[072:0073) |   ├─Token(NewLine) |\n|
    name: clusterName
//@[004:0021) |   ├─ObjectPropertySyntax
//@[004:0008) |   | ├─IdentifierSyntax
//@[004:0008) |   | | └─Token(Identifier) |name|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0021) |   | └─VariableAccessSyntax
//@[010:0021) |   |   └─IdentifierSyntax
//@[010:0021) |   |     └─Token(Identifier) |clusterName|
//@[021:0022) |   ├─Token(NewLine) |\n|
    location: location
//@[004:0022) |   ├─ObjectPropertySyntax
//@[004:0012) |   | ├─IdentifierSyntax
//@[004:0012) |   | | └─Token(Identifier) |location|
//@[012:0013) |   | ├─Token(Colon) |:|
//@[014:0022) |   | └─VariableAccessSyntax
//@[014:0022) |   |   └─IdentifierSyntax
//@[014:0022) |   |     └─Token(Identifier) |location|
//@[022:0023) |   ├─Token(NewLine) |\n|
    properties: {
//@[004:0705) |   ├─ObjectPropertySyntax
//@[004:0014) |   | ├─IdentifierSyntax
//@[004:0014) |   | | └─Token(Identifier) |properties|
//@[014:0015) |   | ├─Token(Colon) |:|
//@[016:0705) |   | └─ObjectSyntax
//@[016:0017) |   |   ├─Token(LeftBrace) |{|
//@[017:0018) |   |   ├─Token(NewLine) |\n|
        dnsPrefix: dnsPrefix
//@[008:0028) |   |   ├─ObjectPropertySyntax
//@[008:0017) |   |   | ├─IdentifierSyntax
//@[008:0017) |   |   | | └─Token(Identifier) |dnsPrefix|
//@[017:0018) |   |   | ├─Token(Colon) |:|
//@[019:0028) |   |   | └─VariableAccessSyntax
//@[019:0028) |   |   |   └─IdentifierSyntax
//@[019:0028) |   |   |     └─Token(Identifier) |dnsPrefix|
//@[028:0029) |   |   ├─Token(NewLine) |\n|
        agentPoolProfiles: [
//@[008:0258) |   |   ├─ObjectPropertySyntax
//@[008:0025) |   |   | ├─IdentifierSyntax
//@[008:0025) |   |   | | └─Token(Identifier) |agentPoolProfiles|
//@[025:0026) |   |   | ├─Token(Colon) |:|
//@[027:0258) |   |   | └─ArraySyntax
//@[027:0028) |   |   |   ├─Token(LeftSquare) |[|
//@[028:0029) |   |   |   ├─Token(NewLine) |\n|
            {
//@[012:0219) |   |   |   ├─ArrayItemSyntax
//@[012:0219) |   |   |   | └─ObjectSyntax
//@[012:0013) |   |   |   |   ├─Token(LeftBrace) |{|
//@[013:0014) |   |   |   |   ├─Token(NewLine) |\n|
                name: 'agentpool'
//@[016:0033) |   |   |   |   ├─ObjectPropertySyntax
//@[016:0020) |   |   |   |   | ├─IdentifierSyntax
//@[016:0020) |   |   |   |   | | └─Token(Identifier) |name|
//@[020:0021) |   |   |   |   | ├─Token(Colon) |:|
//@[022:0033) |   |   |   |   | └─StringSyntax
//@[022:0033) |   |   |   |   |   └─Token(StringComplete) |'agentpool'|
//@[033:0034) |   |   |   |   ├─Token(NewLine) |\n|
                osDiskSizeGB: osDiskSizeGB
//@[016:0042) |   |   |   |   ├─ObjectPropertySyntax
//@[016:0028) |   |   |   |   | ├─IdentifierSyntax
//@[016:0028) |   |   |   |   | | └─Token(Identifier) |osDiskSizeGB|
//@[028:0029) |   |   |   |   | ├─Token(Colon) |:|
//@[030:0042) |   |   |   |   | └─VariableAccessSyntax
//@[030:0042) |   |   |   |   |   └─IdentifierSyntax
//@[030:0042) |   |   |   |   |     └─Token(Identifier) |osDiskSizeGB|
//@[042:0043) |   |   |   |   ├─Token(NewLine) |\n|
                vmSize: agentVMSize
//@[016:0035) |   |   |   |   ├─ObjectPropertySyntax
//@[016:0022) |   |   |   |   | ├─IdentifierSyntax
//@[016:0022) |   |   |   |   | | └─Token(Identifier) |vmSize|
//@[022:0023) |   |   |   |   | ├─Token(Colon) |:|
//@[024:0035) |   |   |   |   | └─VariableAccessSyntax
//@[024:0035) |   |   |   |   |   └─IdentifierSyntax
//@[024:0035) |   |   |   |   |     └─Token(Identifier) |agentVMSize|
//@[035:0036) |   |   |   |   ├─Token(NewLine) |\n|
                osType: 'Linux'
//@[016:0031) |   |   |   |   ├─ObjectPropertySyntax
//@[016:0022) |   |   |   |   | ├─IdentifierSyntax
//@[016:0022) |   |   |   |   | | └─Token(Identifier) |osType|
//@[022:0023) |   |   |   |   | ├─Token(Colon) |:|
//@[024:0031) |   |   |   |   | └─StringSyntax
//@[024:0031) |   |   |   |   |   └─Token(StringComplete) |'Linux'|
//@[031:0032) |   |   |   |   ├─Token(NewLine) |\n|
                storageProfile: 'ManagedDisks'
//@[016:0046) |   |   |   |   ├─ObjectPropertySyntax
//@[016:0030) |   |   |   |   | ├─IdentifierSyntax
//@[016:0030) |   |   |   |   | | └─Token(Identifier) |storageProfile|
//@[030:0031) |   |   |   |   | ├─Token(Colon) |:|
//@[032:0046) |   |   |   |   | └─StringSyntax
//@[032:0046) |   |   |   |   |   └─Token(StringComplete) |'ManagedDisks'|
//@[046:0047) |   |   |   |   ├─Token(NewLine) |\n|
            }
//@[012:0013) |   |   |   |   └─Token(RightBrace) |}|
//@[013:0014) |   |   |   ├─Token(NewLine) |\n|
        ]
//@[008:0009) |   |   |   └─Token(RightSquare) |]|
//@[009:0010) |   |   ├─Token(NewLine) |\n|
        linuxProfile: {
//@[008:0253) |   |   ├─ObjectPropertySyntax
//@[008:0020) |   |   | ├─IdentifierSyntax
//@[008:0020) |   |   | | └─Token(Identifier) |linuxProfile|
//@[020:0021) |   |   | ├─Token(Colon) |:|
//@[022:0253) |   |   | └─ObjectSyntax
//@[022:0023) |   |   |   ├─Token(LeftBrace) |{|
//@[023:0024) |   |   |   ├─Token(NewLine) |\n|
            adminUsername: linuxAdminUsername
//@[012:0045) |   |   |   ├─ObjectPropertySyntax
//@[012:0025) |   |   |   | ├─IdentifierSyntax
//@[012:0025) |   |   |   | | └─Token(Identifier) |adminUsername|
//@[025:0026) |   |   |   | ├─Token(Colon) |:|
//@[027:0045) |   |   |   | └─VariableAccessSyntax
//@[027:0045) |   |   |   |   └─IdentifierSyntax
//@[027:0045) |   |   |   |     └─Token(Identifier) |linuxAdminUsername|
//@[045:0046) |   |   |   ├─Token(NewLine) |\n|
            ssh: {
//@[012:0173) |   |   |   ├─ObjectPropertySyntax
//@[012:0015) |   |   |   | ├─IdentifierSyntax
//@[012:0015) |   |   |   | | └─Token(Identifier) |ssh|
//@[015:0016) |   |   |   | ├─Token(Colon) |:|
//@[017:0173) |   |   |   | └─ObjectSyntax
//@[017:0018) |   |   |   |   ├─Token(LeftBrace) |{|
//@[018:0019) |   |   |   |   ├─Token(NewLine) |\n|
                publicKeys: [
//@[016:0140) |   |   |   |   ├─ObjectPropertySyntax
//@[016:0026) |   |   |   |   | ├─IdentifierSyntax
//@[016:0026) |   |   |   |   | | └─Token(Identifier) |publicKeys|
//@[026:0027) |   |   |   |   | ├─Token(Colon) |:|
//@[028:0140) |   |   |   |   | └─ArraySyntax
//@[028:0029) |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[029:0030) |   |   |   |   |   ├─Token(NewLine) |\n|
                    {
//@[020:0092) |   |   |   |   |   ├─ArrayItemSyntax
//@[020:0092) |   |   |   |   |   | └─ObjectSyntax
//@[020:0021) |   |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[021:0022) |   |   |   |   |   |   ├─Token(NewLine) |\n|
                        keyData: sshRSAPublicKey
//@[024:0048) |   |   |   |   |   |   ├─ObjectPropertySyntax
//@[024:0031) |   |   |   |   |   |   | ├─IdentifierSyntax
//@[024:0031) |   |   |   |   |   |   | | └─Token(Identifier) |keyData|
//@[031:0032) |   |   |   |   |   |   | ├─Token(Colon) |:|
//@[033:0048) |   |   |   |   |   |   | └─VariableAccessSyntax
//@[033:0048) |   |   |   |   |   |   |   └─IdentifierSyntax
//@[033:0048) |   |   |   |   |   |   |     └─Token(Identifier) |sshRSAPublicKey|
//@[048:0049) |   |   |   |   |   |   ├─Token(NewLine) |\n|
                    }
//@[020:0021) |   |   |   |   |   |   └─Token(RightBrace) |}|
//@[021:0022) |   |   |   |   |   ├─Token(NewLine) |\n|
                ]
//@[016:0017) |   |   |   |   |   └─Token(RightSquare) |]|
//@[017:0018) |   |   |   |   ├─Token(NewLine) |\n|
            }
//@[012:0013) |   |   |   |   └─Token(RightBrace) |}|
//@[013:0014) |   |   |   ├─Token(NewLine) |\n|
        }
//@[008:0009) |   |   |   └─Token(RightBrace) |}|
//@[009:0010) |   |   ├─Token(NewLine) |\n|
        servicePrincipalProfile: {
//@[008:0139) |   |   ├─ObjectPropertySyntax
//@[008:0031) |   |   | ├─IdentifierSyntax
//@[008:0031) |   |   | | └─Token(Identifier) |servicePrincipalProfile|
//@[031:0032) |   |   | ├─Token(Colon) |:|
//@[033:0139) |   |   | └─ObjectSyntax
//@[033:0034) |   |   |   ├─Token(LeftBrace) |{|
//@[034:0035) |   |   |   ├─Token(NewLine) |\n|
            clientId: servcePrincipalClientId
//@[012:0045) |   |   |   ├─ObjectPropertySyntax
//@[012:0020) |   |   |   | ├─IdentifierSyntax
//@[012:0020) |   |   |   | | └─Token(Identifier) |clientId|
//@[020:0021) |   |   |   | ├─Token(Colon) |:|
//@[022:0045) |   |   |   | └─VariableAccessSyntax
//@[022:0045) |   |   |   |   └─IdentifierSyntax
//@[022:0045) |   |   |   |     └─Token(Identifier) |servcePrincipalClientId|
//@[045:0046) |   |   |   ├─Token(NewLine) |\n|
            secret: servicePrincipalClientSecret
//@[012:0048) |   |   |   ├─ObjectPropertySyntax
//@[012:0018) |   |   |   | ├─IdentifierSyntax
//@[012:0018) |   |   |   | | └─Token(Identifier) |secret|
//@[018:0019) |   |   |   | ├─Token(Colon) |:|
//@[020:0048) |   |   |   | └─VariableAccessSyntax
//@[020:0048) |   |   |   |   └─IdentifierSyntax
//@[020:0048) |   |   |   |     └─Token(Identifier) |servicePrincipalClientSecret|
//@[048:0049) |   |   |   ├─Token(NewLine) |\n|
        }
//@[008:0009) |   |   |   └─Token(RightBrace) |}|
//@[009:0010) |   |   ├─Token(NewLine) |\n|
    }
//@[004:0005) |   |   └─Token(RightBrace) |}|
//@[005:0006) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

// fyi - dot property access (aks.fqdn) has not been spec'd
//@[059:0060) ├─Token(NewLine) |\n|
//output controlPlaneFQDN string = aks.properties.fqdn 
//@[055:0055) └─Token(EndOfFile) ||
