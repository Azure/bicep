var boolVal = true
//@[000:003) Identifier |var|
//@[004:011) Identifier |boolVal|
//@[012:013) Assignment |=|
//@[014:018) TrueKeyword |true|
//@[018:020) NewLine |\n\n|

var vmProperties = {
//@[000:003) Identifier |var|
//@[004:016) Identifier |vmProperties|
//@[017:018) Assignment |=|
//@[019:020) LeftBrace |{|
//@[020:021) NewLine |\n|
  diagnosticsProfile: {
//@[002:020) Identifier |diagnosticsProfile|
//@[020:021) Colon |:|
//@[022:023) LeftBrace |{|
//@[023:024) NewLine |\n|
    bootDiagnostics: {
//@[004:019) Identifier |bootDiagnostics|
//@[019:020) Colon |:|
//@[021:022) LeftBrace |{|
//@[022:023) NewLine |\n|
      enabled: 123
//@[006:013) Identifier |enabled|
//@[013:014) Colon |:|
//@[015:018) Integer |123|
//@[018:019) NewLine |\n|
      storageUri: true
//@[006:016) Identifier |storageUri|
//@[016:017) Colon |:|
//@[018:022) TrueKeyword |true|
//@[022:023) NewLine |\n|
      unknownProp: 'asdf'
//@[006:017) Identifier |unknownProp|
//@[017:018) Colon |:|
//@[019:025) StringComplete |'asdf'|
//@[025:026) NewLine |\n|
    }
//@[004:005) RightBrace |}|
//@[005:006) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  evictionPolicy: boolVal
//@[002:016) Identifier |evictionPolicy|
//@[016:017) Colon |:|
//@[018:025) Identifier |boolVal|
//@[025:026) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
//@[000:008) Identifier |resource|
//@[009:011) Identifier |vm|
//@[012:058) StringComplete |'Microsoft.Compute/virtualMachines@2020-12-01'|
//@[059:060) Assignment |=|
//@[061:062) LeftBrace |{|
//@[062:063) NewLine |\n|
  name: 'vm'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:012) StringComplete |'vm'|
//@[012:013) NewLine |\n|
  location: 'West US'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:021) StringComplete |'West US'|
//@[021:022) NewLine |\n|
  properties: vmProperties
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:026) Identifier |vmProperties|
//@[026:027) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

var ipConfigurations = [for i in range(0, 2): {
//@[000:003) Identifier |var|
//@[004:020) Identifier |ipConfigurations|
//@[021:022) Assignment |=|
//@[023:024) LeftSquare |[|
//@[024:027) Identifier |for|
//@[028:029) Identifier |i|
//@[030:032) Identifier |in|
//@[033:038) Identifier |range|
//@[038:039) LeftParen |(|
//@[039:040) Integer |0|
//@[040:041) Comma |,|
//@[042:043) Integer |2|
//@[043:044) RightParen |)|
//@[044:045) Colon |:|
//@[046:047) LeftBrace |{|
//@[047:048) NewLine |\n|
  id: true
//@[002:004) Identifier |id|
//@[004:005) Colon |:|
//@[006:010) TrueKeyword |true|
//@[010:011) NewLine |\n|
  name: 'asdf${i}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:015) StringLeftPiece |'asdf${|
//@[015:016) Identifier |i|
//@[016:018) StringRightPiece |}'|
//@[018:019) NewLine |\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) NewLine |\n|
    madeUpProperty: boolVal
//@[004:018) Identifier |madeUpProperty|
//@[018:019) Colon |:|
//@[020:027) Identifier |boolVal|
//@[027:028) NewLine |\n|
    subnet: 'hello'
//@[004:010) Identifier |subnet|
//@[010:011) Colon |:|
//@[012:019) StringComplete |'hello'|
//@[019:020) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

resource nic 'Microsoft.Network/networkInterfaces@2020-11-01' = {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |nic|
//@[013:061) StringComplete |'Microsoft.Network/networkInterfaces@2020-11-01'|
//@[062:063) Assignment |=|
//@[064:065) LeftBrace |{|
//@[065:066) NewLine |\n|
  name: 'abc'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:013) StringComplete |'abc'|
//@[013:014) NewLine |\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) NewLine |\n|
    ipConfigurations: ipConfigurations
//@[004:020) Identifier |ipConfigurations|
//@[020:021) Colon |:|
//@[022:038) Identifier |ipConfigurations|
//@[038:039) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

resource nicLoop 'Microsoft.Network/networkInterfaces@2020-11-01' = [for i in range(0, 2): {
//@[000:008) Identifier |resource|
//@[009:016) Identifier |nicLoop|
//@[017:065) StringComplete |'Microsoft.Network/networkInterfaces@2020-11-01'|
//@[066:067) Assignment |=|
//@[068:069) LeftSquare |[|
//@[069:072) Identifier |for|
//@[073:074) Identifier |i|
//@[075:077) Identifier |in|
//@[078:083) Identifier |range|
//@[083:084) LeftParen |(|
//@[084:085) Integer |0|
//@[085:086) Comma |,|
//@[087:088) Integer |2|
//@[088:089) RightParen |)|
//@[089:090) Colon |:|
//@[091:092) LeftBrace |{|
//@[092:093) NewLine |\n|
  name: 'abc${i}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringLeftPiece |'abc${|
//@[014:015) Identifier |i|
//@[015:017) StringRightPiece |}'|
//@[017:018) NewLine |\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) NewLine |\n|
    ipConfigurations: [
//@[004:020) Identifier |ipConfigurations|
//@[020:021) Colon |:|
//@[022:023) LeftSquare |[|
//@[023:024) NewLine |\n|
      // TODO: fix this
//@[023:024) NewLine |\n|
      ipConfigurations[i]
//@[006:022) Identifier |ipConfigurations|
//@[022:023) LeftSquare |[|
//@[023:024) Identifier |i|
//@[024:025) RightSquare |]|
//@[025:026) NewLine |\n|
    ]
//@[004:005) RightSquare |]|
//@[005:006) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

resource nicLoop2 'Microsoft.Network/networkInterfaces@2020-11-01' = [for ipConfig in ipConfigurations: {
//@[000:008) Identifier |resource|
//@[009:017) Identifier |nicLoop2|
//@[018:066) StringComplete |'Microsoft.Network/networkInterfaces@2020-11-01'|
//@[067:068) Assignment |=|
//@[069:070) LeftSquare |[|
//@[070:073) Identifier |for|
//@[074:082) Identifier |ipConfig|
//@[083:085) Identifier |in|
//@[086:102) Identifier |ipConfigurations|
//@[102:103) Colon |:|
//@[104:105) LeftBrace |{|
//@[105:106) NewLine |\n|
  name: 'abc${ipConfig.name}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringLeftPiece |'abc${|
//@[014:022) Identifier |ipConfig|
//@[022:023) Dot |.|
//@[023:027) Identifier |name|
//@[027:029) StringRightPiece |}'|
//@[029:030) NewLine |\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) NewLine |\n|
    ipConfigurations: [
//@[004:020) Identifier |ipConfigurations|
//@[020:021) Colon |:|
//@[022:023) LeftSquare |[|
//@[023:024) NewLine |\n|
      // TODO: fix this
//@[023:024) NewLine |\n|
      ipConfig
//@[006:014) Identifier |ipConfig|
//@[014:015) NewLine |\n|
    ]
//@[004:005) RightSquare |]|
//@[005:006) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:003) NewLine |\n|

//@[000:000) EndOfFile ||
