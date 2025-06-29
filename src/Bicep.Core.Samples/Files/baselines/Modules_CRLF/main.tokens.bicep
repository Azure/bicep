
//@[000:002) NewLine |\r\n|
@sys.description('this is deployTimeSuffix param')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:049) StringComplete |'this is deployTimeSuffix param'|
//@[049:050) RightParen |)|
//@[050:052) NewLine |\r\n|
param deployTimeSuffix string = newGuid()
//@[000:005) Identifier |param|
//@[006:022) Identifier |deployTimeSuffix|
//@[023:029) Identifier |string|
//@[030:031) Assignment |=|
//@[032:039) Identifier |newGuid|
//@[039:040) LeftParen |(|
//@[040:041) RightParen |)|
//@[041:045) NewLine |\r\n\r\n|

@sys.description('this module a')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:032) StringComplete |'this module a'|
//@[032:033) RightParen |)|
//@[033:035) NewLine |\r\n|
module modATest './modulea.bicep' = {
//@[000:006) Identifier |module|
//@[007:015) Identifier |modATest|
//@[016:033) StringComplete |'./modulea.bicep'|
//@[034:035) Assignment |=|
//@[036:037) LeftBrace |{|
//@[037:039) NewLine |\r\n|
  name: 'modATest'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:018) StringComplete |'modATest'|
//@[018:020) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    stringParamB: 'hello!'
//@[004:016) Identifier |stringParamB|
//@[016:017) Colon |:|
//@[018:026) StringComplete |'hello!'|
//@[026:028) NewLine |\r\n|
    objParam: {
//@[004:012) Identifier |objParam|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
      a: 'b'
//@[006:007) Identifier |a|
//@[007:008) Colon |:|
//@[009:012) StringComplete |'b'|
//@[012:014) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
    arrayParam: [
//@[004:014) Identifier |arrayParam|
//@[014:015) Colon |:|
//@[016:017) LeftSquare |[|
//@[017:019) NewLine |\r\n|
      {
//@[006:007) LeftBrace |{|
//@[007:009) NewLine |\r\n|
        a: 'b'
//@[008:009) Identifier |a|
//@[009:010) Colon |:|
//@[011:014) StringComplete |'b'|
//@[014:016) NewLine |\r\n|
      }
//@[006:007) RightBrace |}|
//@[007:009) NewLine |\r\n|
      'abc'
//@[006:011) StringComplete |'abc'|
//@[011:013) NewLine |\r\n|
    ]
//@[004:005) RightSquare |]|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:007) NewLine |\r\n\r\n\r\n|


@sys.description('this module b')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:032) StringComplete |'this module b'|
//@[032:033) RightParen |)|
//@[033:035) NewLine |\r\n|
module modB './child/moduleb.bicep' = {
//@[000:006) Identifier |module|
//@[007:011) Identifier |modB|
//@[012:035) StringComplete |'./child/moduleb.bicep'|
//@[036:037) Assignment |=|
//@[038:039) LeftBrace |{|
//@[039:041) NewLine |\r\n|
  name: 'modB'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'modB'|
//@[014:016) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    location: 'West US'
//@[004:012) Identifier |location|
//@[012:013) Colon |:|
//@[014:023) StringComplete |'West US'|
//@[023:025) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

@sys.description('this is just module b with a condition')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:057) StringComplete |'this is just module b with a condition'|
//@[057:058) RightParen |)|
//@[058:060) NewLine |\r\n|
module modBWithCondition './child/moduleb.bicep' = if (1 + 1 == 2) {
//@[000:006) Identifier |module|
//@[007:024) Identifier |modBWithCondition|
//@[025:048) StringComplete |'./child/moduleb.bicep'|
//@[049:050) Assignment |=|
//@[051:053) Identifier |if|
//@[054:055) LeftParen |(|
//@[055:056) Integer |1|
//@[057:058) Plus |+|
//@[059:060) Integer |1|
//@[061:063) Equals |==|
//@[064:065) Integer |2|
//@[065:066) RightParen |)|
//@[067:068) LeftBrace |{|
//@[068:070) NewLine |\r\n|
  name: 'modBWithCondition'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:027) StringComplete |'modBWithCondition'|
//@[027:029) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    location: 'East US'
//@[004:012) Identifier |location|
//@[012:013) Colon |:|
//@[014:023) StringComplete |'East US'|
//@[023:025) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module modBWithCondition2 './child/moduleb.bicep' =
//@[000:006) Identifier |module|
//@[007:025) Identifier |modBWithCondition2|
//@[026:049) StringComplete |'./child/moduleb.bicep'|
//@[050:051) Assignment |=|
//@[051:053) NewLine |\r\n|
// awkward comment
//@[018:020) NewLine |\r\n|
if (1 + 1 == 2) {
//@[000:002) Identifier |if|
//@[003:004) LeftParen |(|
//@[004:005) Integer |1|
//@[006:007) Plus |+|
//@[008:009) Integer |1|
//@[010:012) Equals |==|
//@[013:014) Integer |2|
//@[014:015) RightParen |)|
//@[016:017) LeftBrace |{|
//@[017:019) NewLine |\r\n|
  name: 'modBWithCondition2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:028) StringComplete |'modBWithCondition2'|
//@[028:030) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    location: 'East US'
//@[004:012) Identifier |location|
//@[012:013) Colon |:|
//@[014:023) StringComplete |'East US'|
//@[023:025) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module modC './child/modulec.json' = {
//@[000:006) Identifier |module|
//@[007:011) Identifier |modC|
//@[012:034) StringComplete |'./child/modulec.json'|
//@[035:036) Assignment |=|
//@[037:038) LeftBrace |{|
//@[038:040) NewLine |\r\n|
  name: 'modC'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'modC'|
//@[014:016) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    location: 'West US'
//@[004:012) Identifier |location|
//@[012:013) Colon |:|
//@[014:023) StringComplete |'West US'|
//@[023:025) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module modCWithCondition './child/modulec.json' = if (2 - 1 == 1) {
//@[000:006) Identifier |module|
//@[007:024) Identifier |modCWithCondition|
//@[025:047) StringComplete |'./child/modulec.json'|
//@[048:049) Assignment |=|
//@[050:052) Identifier |if|
//@[053:054) LeftParen |(|
//@[054:055) Integer |2|
//@[056:057) Minus |-|
//@[058:059) Integer |1|
//@[060:062) Equals |==|
//@[063:064) Integer |1|
//@[064:065) RightParen |)|
//@[066:067) LeftBrace |{|
//@[067:069) NewLine |\r\n|
  name: 'modCWithCondition'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:027) StringComplete |'modCWithCondition'|
//@[027:029) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    location: 'East US'
//@[004:012) Identifier |location|
//@[012:013) Colon |:|
//@[014:023) StringComplete |'East US'|
//@[023:025) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module optionalWithNoParams1 './child/optionalParams.bicep'= {
//@[000:006) Identifier |module|
//@[007:028) Identifier |optionalWithNoParams1|
//@[029:059) StringComplete |'./child/optionalParams.bicep'|
//@[059:060) Assignment |=|
//@[061:062) LeftBrace |{|
//@[062:064) NewLine |\r\n|
  name: 'optionalWithNoParams1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:031) StringComplete |'optionalWithNoParams1'|
//@[031:033) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module optionalWithNoParams2 './child/optionalParams.bicep'= {
//@[000:006) Identifier |module|
//@[007:028) Identifier |optionalWithNoParams2|
//@[029:059) StringComplete |'./child/optionalParams.bicep'|
//@[059:060) Assignment |=|
//@[061:062) LeftBrace |{|
//@[062:064) NewLine |\r\n|
  name: 'optionalWithNoParams2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:031) StringComplete |'optionalWithNoParams2'|
//@[031:033) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module optionalWithAllParams './child/optionalParams.bicep'= {
//@[000:006) Identifier |module|
//@[007:028) Identifier |optionalWithAllParams|
//@[029:059) StringComplete |'./child/optionalParams.bicep'|
//@[059:060) Assignment |=|
//@[061:062) LeftBrace |{|
//@[062:064) NewLine |\r\n|
  name: 'optionalWithNoParams3'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:031) StringComplete |'optionalWithNoParams3'|
//@[031:033) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    optionalString: 'abc'
//@[004:018) Identifier |optionalString|
//@[018:019) Colon |:|
//@[020:025) StringComplete |'abc'|
//@[025:027) NewLine |\r\n|
    optionalInt: 42
//@[004:015) Identifier |optionalInt|
//@[015:016) Colon |:|
//@[017:019) Integer |42|
//@[019:021) NewLine |\r\n|
    optionalObj: { }
//@[004:015) Identifier |optionalObj|
//@[015:016) Colon |:|
//@[017:018) LeftBrace |{|
//@[019:020) RightBrace |}|
//@[020:022) NewLine |\r\n|
    optionalArray: [ ]
//@[004:017) Identifier |optionalArray|
//@[017:018) Colon |:|
//@[019:020) LeftSquare |[|
//@[021:022) RightSquare |]|
//@[022:024) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource resWithDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:028) Identifier |resWithDependencies|
//@[029:062) StringComplete |'Mock.Rp/mockResource@2020-01-01'|
//@[063:064) Assignment |=|
//@[065:066) LeftBrace |{|
//@[066:068) NewLine |\r\n|
  name: 'harry'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:015) StringComplete |'harry'|
//@[015:017) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    modADep: modATest.outputs.stringOutputA
//@[004:011) Identifier |modADep|
//@[011:012) Colon |:|
//@[013:021) Identifier |modATest|
//@[021:022) Dot |.|
//@[022:029) Identifier |outputs|
//@[029:030) Dot |.|
//@[030:043) Identifier |stringOutputA|
//@[043:045) NewLine |\r\n|
    modBDep: modB.outputs.myResourceId
//@[004:011) Identifier |modBDep|
//@[011:012) Colon |:|
//@[013:017) Identifier |modB|
//@[017:018) Dot |.|
//@[018:025) Identifier |outputs|
//@[025:026) Dot |.|
//@[026:038) Identifier |myResourceId|
//@[038:040) NewLine |\r\n|
    modCDep: modC.outputs.myResourceId
//@[004:011) Identifier |modCDep|
//@[011:012) Colon |:|
//@[013:017) Identifier |modC|
//@[017:018) Dot |.|
//@[018:025) Identifier |outputs|
//@[025:026) Dot |.|
//@[026:038) Identifier |myResourceId|
//@[038:040) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module optionalWithAllParamsAndManualDependency './child/optionalParams.bicep'= {
//@[000:006) Identifier |module|
//@[007:047) Identifier |optionalWithAllParamsAndManualDependency|
//@[048:078) StringComplete |'./child/optionalParams.bicep'|
//@[078:079) Assignment |=|
//@[080:081) LeftBrace |{|
//@[081:083) NewLine |\r\n|
  name: 'optionalWithAllParamsAndManualDependency'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:050) StringComplete |'optionalWithAllParamsAndManualDependency'|
//@[050:052) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    optionalString: 'abc'
//@[004:018) Identifier |optionalString|
//@[018:019) Colon |:|
//@[020:025) StringComplete |'abc'|
//@[025:027) NewLine |\r\n|
    optionalInt: 42
//@[004:015) Identifier |optionalInt|
//@[015:016) Colon |:|
//@[017:019) Integer |42|
//@[019:021) NewLine |\r\n|
    optionalObj: { }
//@[004:015) Identifier |optionalObj|
//@[015:016) Colon |:|
//@[017:018) LeftBrace |{|
//@[019:020) RightBrace |}|
//@[020:022) NewLine |\r\n|
    optionalArray: [ ]
//@[004:017) Identifier |optionalArray|
//@[017:018) Colon |:|
//@[019:020) LeftSquare |[|
//@[021:022) RightSquare |]|
//@[022:024) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:016) NewLine |\r\n|
    resWithDependencies
//@[004:023) Identifier |resWithDependencies|
//@[023:025) NewLine |\r\n|
    optionalWithAllParams
//@[004:025) Identifier |optionalWithAllParams|
//@[025:027) NewLine |\r\n|
  ]
//@[002:003) RightSquare |]|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module optionalWithImplicitDependency './child/optionalParams.bicep'= {
//@[000:006) Identifier |module|
//@[007:037) Identifier |optionalWithImplicitDependency|
//@[038:068) StringComplete |'./child/optionalParams.bicep'|
//@[068:069) Assignment |=|
//@[070:071) LeftBrace |{|
//@[071:073) NewLine |\r\n|
  name: 'optionalWithImplicitDependency'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:040) StringComplete |'optionalWithImplicitDependency'|
//@[040:042) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
//@[004:018) Identifier |optionalString|
//@[018:019) Colon |:|
//@[020:026) Identifier |concat|
//@[026:027) LeftParen |(|
//@[027:046) Identifier |resWithDependencies|
//@[046:047) Dot |.|
//@[047:049) Identifier |id|
//@[049:050) Comma |,|
//@[051:091) Identifier |optionalWithAllParamsAndManualDependency|
//@[091:092) Dot |.|
//@[092:096) Identifier |name|
//@[096:097) RightParen |)|
//@[097:099) NewLine |\r\n|
    optionalInt: 42
//@[004:015) Identifier |optionalInt|
//@[015:016) Colon |:|
//@[017:019) Integer |42|
//@[019:021) NewLine |\r\n|
    optionalObj: { }
//@[004:015) Identifier |optionalObj|
//@[015:016) Colon |:|
//@[017:018) LeftBrace |{|
//@[019:020) RightBrace |}|
//@[020:022) NewLine |\r\n|
    optionalArray: [ ]
//@[004:017) Identifier |optionalArray|
//@[017:018) Colon |:|
//@[019:020) LeftSquare |[|
//@[021:022) RightSquare |]|
//@[022:024) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module moduleWithCalculatedName './child/optionalParams.bicep'= {
//@[000:006) Identifier |module|
//@[007:031) Identifier |moduleWithCalculatedName|
//@[032:062) StringComplete |'./child/optionalParams.bicep'|
//@[062:063) Assignment |=|
//@[064:065) LeftBrace |{|
//@[065:067) NewLine |\r\n|
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringLeftPiece |'${|
//@[011:051) Identifier |optionalWithAllParamsAndManualDependency|
//@[051:052) Dot |.|
//@[052:056) Identifier |name|
//@[056:059) StringMiddlePiece |}${|
//@[059:075) Identifier |deployTimeSuffix|
//@[075:077) StringRightPiece |}'|
//@[077:079) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
//@[004:018) Identifier |optionalString|
//@[018:019) Colon |:|
//@[020:026) Identifier |concat|
//@[026:027) LeftParen |(|
//@[027:046) Identifier |resWithDependencies|
//@[046:047) Dot |.|
//@[047:049) Identifier |id|
//@[049:050) Comma |,|
//@[051:091) Identifier |optionalWithAllParamsAndManualDependency|
//@[091:092) Dot |.|
//@[092:096) Identifier |name|
//@[096:097) RightParen |)|
//@[097:099) NewLine |\r\n|
    optionalInt: 42
//@[004:015) Identifier |optionalInt|
//@[015:016) Colon |:|
//@[017:019) Integer |42|
//@[019:021) NewLine |\r\n|
    optionalObj: { }
//@[004:015) Identifier |optionalObj|
//@[015:016) Colon |:|
//@[017:018) LeftBrace |{|
//@[019:020) RightBrace |}|
//@[020:022) NewLine |\r\n|
    optionalArray: [ ]
//@[004:017) Identifier |optionalArray|
//@[017:018) Colon |:|
//@[019:020) LeftSquare |[|
//@[021:022) RightSquare |]|
//@[022:024) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource resWithCalculatedNameDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:042) Identifier |resWithCalculatedNameDependencies|
//@[043:076) StringComplete |'Mock.Rp/mockResource@2020-01-01'|
//@[077:078) Assignment |=|
//@[079:080) LeftBrace |{|
//@[080:082) NewLine |\r\n|
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringLeftPiece |'${|
//@[011:051) Identifier |optionalWithAllParamsAndManualDependency|
//@[051:052) Dot |.|
//@[052:056) Identifier |name|
//@[056:059) StringMiddlePiece |}${|
//@[059:075) Identifier |deployTimeSuffix|
//@[075:077) StringRightPiece |}'|
//@[077:079) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    modADep: moduleWithCalculatedName.outputs.outputObj
//@[004:011) Identifier |modADep|
//@[011:012) Colon |:|
//@[013:037) Identifier |moduleWithCalculatedName|
//@[037:038) Dot |.|
//@[038:045) Identifier |outputs|
//@[045:046) Dot |.|
//@[046:055) Identifier |outputObj|
//@[055:057) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

output stringOutputA string = modATest.outputs.stringOutputA
//@[000:006) Identifier |output|
//@[007:020) Identifier |stringOutputA|
//@[021:027) Identifier |string|
//@[028:029) Assignment |=|
//@[030:038) Identifier |modATest|
//@[038:039) Dot |.|
//@[039:046) Identifier |outputs|
//@[046:047) Dot |.|
//@[047:060) Identifier |stringOutputA|
//@[060:062) NewLine |\r\n|
output stringOutputB string = modATest.outputs.stringOutputB
//@[000:006) Identifier |output|
//@[007:020) Identifier |stringOutputB|
//@[021:027) Identifier |string|
//@[028:029) Assignment |=|
//@[030:038) Identifier |modATest|
//@[038:039) Dot |.|
//@[039:046) Identifier |outputs|
//@[046:047) Dot |.|
//@[047:060) Identifier |stringOutputB|
//@[060:062) NewLine |\r\n|
output objOutput object = modATest.outputs.objOutput
//@[000:006) Identifier |output|
//@[007:016) Identifier |objOutput|
//@[017:023) Identifier |object|
//@[024:025) Assignment |=|
//@[026:034) Identifier |modATest|
//@[034:035) Dot |.|
//@[035:042) Identifier |outputs|
//@[042:043) Dot |.|
//@[043:052) Identifier |objOutput|
//@[052:054) NewLine |\r\n|
output arrayOutput array = modATest.outputs.arrayOutput
//@[000:006) Identifier |output|
//@[007:018) Identifier |arrayOutput|
//@[019:024) Identifier |array|
//@[025:026) Assignment |=|
//@[027:035) Identifier |modATest|
//@[035:036) Dot |.|
//@[036:043) Identifier |outputs|
//@[043:044) Dot |.|
//@[044:055) Identifier |arrayOutput|
//@[055:057) NewLine |\r\n|
output modCalculatedNameOutput object = moduleWithCalculatedName.outputs.outputObj
//@[000:006) Identifier |output|
//@[007:030) Identifier |modCalculatedNameOutput|
//@[031:037) Identifier |object|
//@[038:039) Assignment |=|
//@[040:064) Identifier |moduleWithCalculatedName|
//@[064:065) Dot |.|
//@[065:072) Identifier |outputs|
//@[072:073) Dot |.|
//@[073:082) Identifier |outputObj|
//@[082:086) NewLine |\r\n\r\n|

/*
  valid loop cases
*/
//@[002:006) NewLine |\r\n\r\n|

@sys.description('this is myModules')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:036) StringComplete |'this is myModules'|
//@[036:037) RightParen |)|
//@[037:039) NewLine |\r\n|
var myModules = [
//@[000:003) Identifier |var|
//@[004:013) Identifier |myModules|
//@[014:015) Assignment |=|
//@[016:017) LeftSquare |[|
//@[017:019) NewLine |\r\n|
  {
//@[002:003) LeftBrace |{|
//@[003:005) NewLine |\r\n|
    name: 'one'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:015) StringComplete |'one'|
//@[015:017) NewLine |\r\n|
    location: 'eastus2'
//@[004:012) Identifier |location|
//@[012:013) Colon |:|
//@[014:023) StringComplete |'eastus2'|
//@[023:025) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  {
//@[002:003) LeftBrace |{|
//@[003:005) NewLine |\r\n|
    name: 'two'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:015) StringComplete |'two'|
//@[015:017) NewLine |\r\n|
    location: 'westus'
//@[004:012) Identifier |location|
//@[012:013) Colon |:|
//@[014:022) StringComplete |'westus'|
//@[022:024) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
]
//@[000:001) RightSquare |]|
//@[001:005) NewLine |\r\n\r\n|

var emptyArray = []
//@[000:003) Identifier |var|
//@[004:014) Identifier |emptyArray|
//@[015:016) Assignment |=|
//@[017:018) LeftSquare |[|
//@[018:019) RightSquare |]|
//@[019:023) NewLine |\r\n\r\n|

// simple module loop
//@[021:023) NewLine |\r\n|
module storageResources 'modulea.bicep' = [for module in myModules: {
//@[000:006) Identifier |module|
//@[007:023) Identifier |storageResources|
//@[024:039) StringComplete |'modulea.bicep'|
//@[040:041) Assignment |=|
//@[042:043) LeftSquare |[|
//@[043:046) Identifier |for|
//@[047:053) Identifier |module|
//@[054:056) Identifier |in|
//@[057:066) Identifier |myModules|
//@[066:067) Colon |:|
//@[068:069) LeftBrace |{|
//@[069:071) NewLine |\r\n|
  name: module.name
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) Identifier |module|
//@[014:015) Dot |.|
//@[015:019) Identifier |name|
//@[019:021) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    arrayParam: []
//@[004:014) Identifier |arrayParam|
//@[014:015) Colon |:|
//@[016:017) LeftSquare |[|
//@[017:018) RightSquare |]|
//@[018:020) NewLine |\r\n|
    objParam: module
//@[004:012) Identifier |objParam|
//@[012:013) Colon |:|
//@[014:020) Identifier |module|
//@[020:022) NewLine |\r\n|
    stringParamB: module.location
//@[004:016) Identifier |stringParamB|
//@[016:017) Colon |:|
//@[018:024) Identifier |module|
//@[024:025) Dot |.|
//@[025:033) Identifier |location|
//@[033:035) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

// simple indexed module loop
//@[029:031) NewLine |\r\n|
module storageResourcesWithIndex 'modulea.bicep' = [for (module, i) in myModules: {
//@[000:006) Identifier |module|
//@[007:032) Identifier |storageResourcesWithIndex|
//@[033:048) StringComplete |'modulea.bicep'|
//@[049:050) Assignment |=|
//@[051:052) LeftSquare |[|
//@[052:055) Identifier |for|
//@[056:057) LeftParen |(|
//@[057:063) Identifier |module|
//@[063:064) Comma |,|
//@[065:066) Identifier |i|
//@[066:067) RightParen |)|
//@[068:070) Identifier |in|
//@[071:080) Identifier |myModules|
//@[080:081) Colon |:|
//@[082:083) LeftBrace |{|
//@[083:085) NewLine |\r\n|
  name: module.name
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) Identifier |module|
//@[014:015) Dot |.|
//@[015:019) Identifier |name|
//@[019:021) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    arrayParam: [
//@[004:014) Identifier |arrayParam|
//@[014:015) Colon |:|
//@[016:017) LeftSquare |[|
//@[017:019) NewLine |\r\n|
      i + 1
//@[006:007) Identifier |i|
//@[008:009) Plus |+|
//@[010:011) Integer |1|
//@[011:013) NewLine |\r\n|
    ]
//@[004:005) RightSquare |]|
//@[005:007) NewLine |\r\n|
    objParam: module
//@[004:012) Identifier |objParam|
//@[012:013) Colon |:|
//@[014:020) Identifier |module|
//@[020:022) NewLine |\r\n|
    stringParamB: module.location
//@[004:016) Identifier |stringParamB|
//@[016:017) Colon |:|
//@[018:024) Identifier |module|
//@[024:025) Dot |.|
//@[025:033) Identifier |location|
//@[033:035) NewLine |\r\n|
    stringParamA: concat('a', i)
//@[004:016) Identifier |stringParamA|
//@[016:017) Colon |:|
//@[018:024) Identifier |concat|
//@[024:025) LeftParen |(|
//@[025:028) StringComplete |'a'|
//@[028:029) Comma |,|
//@[030:031) Identifier |i|
//@[031:032) RightParen |)|
//@[032:034) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

// nested module loop
//@[021:023) NewLine |\r\n|
module nestedModuleLoop 'modulea.bicep' = [for module in myModules: {
//@[000:006) Identifier |module|
//@[007:023) Identifier |nestedModuleLoop|
//@[024:039) StringComplete |'modulea.bicep'|
//@[040:041) Assignment |=|
//@[042:043) LeftSquare |[|
//@[043:046) Identifier |for|
//@[047:053) Identifier |module|
//@[054:056) Identifier |in|
//@[057:066) Identifier |myModules|
//@[066:067) Colon |:|
//@[068:069) LeftBrace |{|
//@[069:071) NewLine |\r\n|
  name: module.name
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) Identifier |module|
//@[014:015) Dot |.|
//@[015:019) Identifier |name|
//@[019:021) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    arrayParam: [for i in range(0,3): concat('test-', i, '-', module.name)]
//@[004:014) Identifier |arrayParam|
//@[014:015) Colon |:|
//@[016:017) LeftSquare |[|
//@[017:020) Identifier |for|
//@[021:022) Identifier |i|
//@[023:025) Identifier |in|
//@[026:031) Identifier |range|
//@[031:032) LeftParen |(|
//@[032:033) Integer |0|
//@[033:034) Comma |,|
//@[034:035) Integer |3|
//@[035:036) RightParen |)|
//@[036:037) Colon |:|
//@[038:044) Identifier |concat|
//@[044:045) LeftParen |(|
//@[045:052) StringComplete |'test-'|
//@[052:053) Comma |,|
//@[054:055) Identifier |i|
//@[055:056) Comma |,|
//@[057:060) StringComplete |'-'|
//@[060:061) Comma |,|
//@[062:068) Identifier |module|
//@[068:069) Dot |.|
//@[069:073) Identifier |name|
//@[073:074) RightParen |)|
//@[074:075) RightSquare |]|
//@[075:077) NewLine |\r\n|
    objParam: module
//@[004:012) Identifier |objParam|
//@[012:013) Colon |:|
//@[014:020) Identifier |module|
//@[020:022) NewLine |\r\n|
    stringParamB: module.location
//@[004:016) Identifier |stringParamB|
//@[016:017) Colon |:|
//@[018:024) Identifier |module|
//@[024:025) Dot |.|
//@[025:033) Identifier |location|
//@[033:035) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

// duplicate identifiers across scopes are allowed (inner hides the outer)
//@[074:076) NewLine |\r\n|
module duplicateIdentifiersWithinLoop 'modulea.bicep' = [for x in emptyArray:{
//@[000:006) Identifier |module|
//@[007:037) Identifier |duplicateIdentifiersWithinLoop|
//@[038:053) StringComplete |'modulea.bicep'|
//@[054:055) Assignment |=|
//@[056:057) LeftSquare |[|
//@[057:060) Identifier |for|
//@[061:062) Identifier |x|
//@[063:065) Identifier |in|
//@[066:076) Identifier |emptyArray|
//@[076:077) Colon |:|
//@[077:078) LeftBrace |{|
//@[078:080) NewLine |\r\n|
  name: 'hello-${x}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:017) StringLeftPiece |'hello-${|
//@[017:018) Identifier |x|
//@[018:020) StringRightPiece |}'|
//@[020:022) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    objParam: {}
//@[004:012) Identifier |objParam|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) RightBrace |}|
//@[016:018) NewLine |\r\n|
    stringParamA: 'test'
//@[004:016) Identifier |stringParamA|
//@[016:017) Colon |:|
//@[018:024) StringComplete |'test'|
//@[024:026) NewLine |\r\n|
    stringParamB: 'test'
//@[004:016) Identifier |stringParamB|
//@[016:017) Colon |:|
//@[018:024) StringComplete |'test'|
//@[024:026) NewLine |\r\n|
    arrayParam: [for x in emptyArray: x]
//@[004:014) Identifier |arrayParam|
//@[014:015) Colon |:|
//@[016:017) LeftSquare |[|
//@[017:020) Identifier |for|
//@[021:022) Identifier |x|
//@[023:025) Identifier |in|
//@[026:036) Identifier |emptyArray|
//@[036:037) Colon |:|
//@[038:039) Identifier |x|
//@[039:040) RightSquare |]|
//@[040:042) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

// duplicate identifiers across scopes are allowed (inner hides the outer)
//@[074:076) NewLine |\r\n|
var duplicateAcrossScopes = 'hello'
//@[000:003) Identifier |var|
//@[004:025) Identifier |duplicateAcrossScopes|
//@[026:027) Assignment |=|
//@[028:035) StringComplete |'hello'|
//@[035:037) NewLine |\r\n|
module duplicateInGlobalAndOneLoop 'modulea.bicep' = [for duplicateAcrossScopes in []: {
//@[000:006) Identifier |module|
//@[007:034) Identifier |duplicateInGlobalAndOneLoop|
//@[035:050) StringComplete |'modulea.bicep'|
//@[051:052) Assignment |=|
//@[053:054) LeftSquare |[|
//@[054:057) Identifier |for|
//@[058:079) Identifier |duplicateAcrossScopes|
//@[080:082) Identifier |in|
//@[083:084) LeftSquare |[|
//@[084:085) RightSquare |]|
//@[085:086) Colon |:|
//@[087:088) LeftBrace |{|
//@[088:090) NewLine |\r\n|
  name: 'hello-${duplicateAcrossScopes}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:017) StringLeftPiece |'hello-${|
//@[017:038) Identifier |duplicateAcrossScopes|
//@[038:040) StringRightPiece |}'|
//@[040:042) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    objParam: {}
//@[004:012) Identifier |objParam|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) RightBrace |}|
//@[016:018) NewLine |\r\n|
    stringParamA: 'test'
//@[004:016) Identifier |stringParamA|
//@[016:017) Colon |:|
//@[018:024) StringComplete |'test'|
//@[024:026) NewLine |\r\n|
    stringParamB: 'test'
//@[004:016) Identifier |stringParamB|
//@[016:017) Colon |:|
//@[018:024) StringComplete |'test'|
//@[024:026) NewLine |\r\n|
    arrayParam: [for x in emptyArray: x]
//@[004:014) Identifier |arrayParam|
//@[014:015) Colon |:|
//@[016:017) LeftSquare |[|
//@[017:020) Identifier |for|
//@[021:022) Identifier |x|
//@[023:025) Identifier |in|
//@[026:036) Identifier |emptyArray|
//@[036:037) Colon |:|
//@[038:039) Identifier |x|
//@[039:040) RightSquare |]|
//@[040:042) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

var someDuplicate = true
//@[000:003) Identifier |var|
//@[004:017) Identifier |someDuplicate|
//@[018:019) Assignment |=|
//@[020:024) TrueKeyword |true|
//@[024:026) NewLine |\r\n|
var otherDuplicate = false
//@[000:003) Identifier |var|
//@[004:018) Identifier |otherDuplicate|
//@[019:020) Assignment |=|
//@[021:026) FalseKeyword |false|
//@[026:028) NewLine |\r\n|
module duplicatesEverywhere 'modulea.bicep' = [for someDuplicate in []: {
//@[000:006) Identifier |module|
//@[007:027) Identifier |duplicatesEverywhere|
//@[028:043) StringComplete |'modulea.bicep'|
//@[044:045) Assignment |=|
//@[046:047) LeftSquare |[|
//@[047:050) Identifier |for|
//@[051:064) Identifier |someDuplicate|
//@[065:067) Identifier |in|
//@[068:069) LeftSquare |[|
//@[069:070) RightSquare |]|
//@[070:071) Colon |:|
//@[072:073) LeftBrace |{|
//@[073:075) NewLine |\r\n|
  name: 'hello-${someDuplicate}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:017) StringLeftPiece |'hello-${|
//@[017:030) Identifier |someDuplicate|
//@[030:032) StringRightPiece |}'|
//@[032:034) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    objParam: {}
//@[004:012) Identifier |objParam|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) RightBrace |}|
//@[016:018) NewLine |\r\n|
    stringParamB: 'test'
//@[004:016) Identifier |stringParamB|
//@[016:017) Colon |:|
//@[018:024) StringComplete |'test'|
//@[024:026) NewLine |\r\n|
    arrayParam: [for otherDuplicate in emptyArray: '${someDuplicate}-${otherDuplicate}']
//@[004:014) Identifier |arrayParam|
//@[014:015) Colon |:|
//@[016:017) LeftSquare |[|
//@[017:020) Identifier |for|
//@[021:035) Identifier |otherDuplicate|
//@[036:038) Identifier |in|
//@[039:049) Identifier |emptyArray|
//@[049:050) Colon |:|
//@[051:054) StringLeftPiece |'${|
//@[054:067) Identifier |someDuplicate|
//@[067:071) StringMiddlePiece |}-${|
//@[071:085) Identifier |otherDuplicate|
//@[085:087) StringRightPiece |}'|
//@[087:088) RightSquare |]|
//@[088:090) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

module propertyLoopInsideParameterValue 'modulea.bicep' = {
//@[000:006) Identifier |module|
//@[007:039) Identifier |propertyLoopInsideParameterValue|
//@[040:055) StringComplete |'modulea.bicep'|
//@[056:057) Assignment |=|
//@[058:059) LeftBrace |{|
//@[059:061) NewLine |\r\n|
  name: 'propertyLoopInsideParameterValue'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:042) StringComplete |'propertyLoopInsideParameterValue'|
//@[042:044) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    objParam: {
//@[004:012) Identifier |objParam|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
      a: [for i in range(0,10): i]
//@[006:007) Identifier |a|
//@[007:008) Colon |:|
//@[009:010) LeftSquare |[|
//@[010:013) Identifier |for|
//@[014:015) Identifier |i|
//@[016:018) Identifier |in|
//@[019:024) Identifier |range|
//@[024:025) LeftParen |(|
//@[025:026) Integer |0|
//@[026:027) Comma |,|
//@[027:029) Integer |10|
//@[029:030) RightParen |)|
//@[030:031) Colon |:|
//@[032:033) Identifier |i|
//@[033:034) RightSquare |]|
//@[034:036) NewLine |\r\n|
      b: [for i in range(1,2): i]
//@[006:007) Identifier |b|
//@[007:008) Colon |:|
//@[009:010) LeftSquare |[|
//@[010:013) Identifier |for|
//@[014:015) Identifier |i|
//@[016:018) Identifier |in|
//@[019:024) Identifier |range|
//@[024:025) LeftParen |(|
//@[025:026) Integer |1|
//@[026:027) Comma |,|
//@[027:028) Integer |2|
//@[028:029) RightParen |)|
//@[029:030) Colon |:|
//@[031:032) Identifier |i|
//@[032:033) RightSquare |]|
//@[033:035) NewLine |\r\n|
      c: {
//@[006:007) Identifier |c|
//@[007:008) Colon |:|
//@[009:010) LeftBrace |{|
//@[010:012) NewLine |\r\n|
        d: [for j in range(2,3): j]
//@[008:009) Identifier |d|
//@[009:010) Colon |:|
//@[011:012) LeftSquare |[|
//@[012:015) Identifier |for|
//@[016:017) Identifier |j|
//@[018:020) Identifier |in|
//@[021:026) Identifier |range|
//@[026:027) LeftParen |(|
//@[027:028) Integer |2|
//@[028:029) Comma |,|
//@[029:030) Integer |3|
//@[030:031) RightParen |)|
//@[031:032) Colon |:|
//@[033:034) Identifier |j|
//@[034:035) RightSquare |]|
//@[035:037) NewLine |\r\n|
      }
//@[006:007) RightBrace |}|
//@[007:009) NewLine |\r\n|
      e: [for k in range(4,4): {
//@[006:007) Identifier |e|
//@[007:008) Colon |:|
//@[009:010) LeftSquare |[|
//@[010:013) Identifier |for|
//@[014:015) Identifier |k|
//@[016:018) Identifier |in|
//@[019:024) Identifier |range|
//@[024:025) LeftParen |(|
//@[025:026) Integer |4|
//@[026:027) Comma |,|
//@[027:028) Integer |4|
//@[028:029) RightParen |)|
//@[029:030) Colon |:|
//@[031:032) LeftBrace |{|
//@[032:034) NewLine |\r\n|
        f: k
//@[008:009) Identifier |f|
//@[009:010) Colon |:|
//@[011:012) Identifier |k|
//@[012:014) NewLine |\r\n|
      }]
//@[006:007) RightBrace |}|
//@[007:008) RightSquare |]|
//@[008:010) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
    stringParamB: ''
//@[004:016) Identifier |stringParamB|
//@[016:017) Colon |:|
//@[018:020) StringComplete |''|
//@[020:022) NewLine |\r\n|
    arrayParam: [
//@[004:014) Identifier |arrayParam|
//@[014:015) Colon |:|
//@[016:017) LeftSquare |[|
//@[017:019) NewLine |\r\n|
      {
//@[006:007) LeftBrace |{|
//@[007:009) NewLine |\r\n|
        e: [for j in range(7,7): j]
//@[008:009) Identifier |e|
//@[009:010) Colon |:|
//@[011:012) LeftSquare |[|
//@[012:015) Identifier |for|
//@[016:017) Identifier |j|
//@[018:020) Identifier |in|
//@[021:026) Identifier |range|
//@[026:027) LeftParen |(|
//@[027:028) Integer |7|
//@[028:029) Comma |,|
//@[029:030) Integer |7|
//@[030:031) RightParen |)|
//@[031:032) Colon |:|
//@[033:034) Identifier |j|
//@[034:035) RightSquare |]|
//@[035:037) NewLine |\r\n|
      }
//@[006:007) RightBrace |}|
//@[007:009) NewLine |\r\n|
    ]
//@[004:005) RightSquare |]|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module propertyLoopInsideParameterValueWithIndexes 'modulea.bicep' = {
//@[000:006) Identifier |module|
//@[007:050) Identifier |propertyLoopInsideParameterValueWithIndexes|
//@[051:066) StringComplete |'modulea.bicep'|
//@[067:068) Assignment |=|
//@[069:070) LeftBrace |{|
//@[070:072) NewLine |\r\n|
  name: 'propertyLoopInsideParameterValueWithIndexes'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:053) StringComplete |'propertyLoopInsideParameterValueWithIndexes'|
//@[053:055) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    objParam: {
//@[004:012) Identifier |objParam|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
      a: [for (i, i2) in range(0,10): i + i2]
//@[006:007) Identifier |a|
//@[007:008) Colon |:|
//@[009:010) LeftSquare |[|
//@[010:013) Identifier |for|
//@[014:015) LeftParen |(|
//@[015:016) Identifier |i|
//@[016:017) Comma |,|
//@[018:020) Identifier |i2|
//@[020:021) RightParen |)|
//@[022:024) Identifier |in|
//@[025:030) Identifier |range|
//@[030:031) LeftParen |(|
//@[031:032) Integer |0|
//@[032:033) Comma |,|
//@[033:035) Integer |10|
//@[035:036) RightParen |)|
//@[036:037) Colon |:|
//@[038:039) Identifier |i|
//@[040:041) Plus |+|
//@[042:044) Identifier |i2|
//@[044:045) RightSquare |]|
//@[045:047) NewLine |\r\n|
      b: [for (i, i2) in range(1,2): i / i2]
//@[006:007) Identifier |b|
//@[007:008) Colon |:|
//@[009:010) LeftSquare |[|
//@[010:013) Identifier |for|
//@[014:015) LeftParen |(|
//@[015:016) Identifier |i|
//@[016:017) Comma |,|
//@[018:020) Identifier |i2|
//@[020:021) RightParen |)|
//@[022:024) Identifier |in|
//@[025:030) Identifier |range|
//@[030:031) LeftParen |(|
//@[031:032) Integer |1|
//@[032:033) Comma |,|
//@[033:034) Integer |2|
//@[034:035) RightParen |)|
//@[035:036) Colon |:|
//@[037:038) Identifier |i|
//@[039:040) Slash |/|
//@[041:043) Identifier |i2|
//@[043:044) RightSquare |]|
//@[044:046) NewLine |\r\n|
      c: {
//@[006:007) Identifier |c|
//@[007:008) Colon |:|
//@[009:010) LeftBrace |{|
//@[010:012) NewLine |\r\n|
        d: [for (j, j2) in range(2,3): j * j2]
//@[008:009) Identifier |d|
//@[009:010) Colon |:|
//@[011:012) LeftSquare |[|
//@[012:015) Identifier |for|
//@[016:017) LeftParen |(|
//@[017:018) Identifier |j|
//@[018:019) Comma |,|
//@[020:022) Identifier |j2|
//@[022:023) RightParen |)|
//@[024:026) Identifier |in|
//@[027:032) Identifier |range|
//@[032:033) LeftParen |(|
//@[033:034) Integer |2|
//@[034:035) Comma |,|
//@[035:036) Integer |3|
//@[036:037) RightParen |)|
//@[037:038) Colon |:|
//@[039:040) Identifier |j|
//@[041:042) Asterisk |*|
//@[043:045) Identifier |j2|
//@[045:046) RightSquare |]|
//@[046:048) NewLine |\r\n|
      }
//@[006:007) RightBrace |}|
//@[007:009) NewLine |\r\n|
      e: [for (k, k2) in range(4,4): {
//@[006:007) Identifier |e|
//@[007:008) Colon |:|
//@[009:010) LeftSquare |[|
//@[010:013) Identifier |for|
//@[014:015) LeftParen |(|
//@[015:016) Identifier |k|
//@[016:017) Comma |,|
//@[018:020) Identifier |k2|
//@[020:021) RightParen |)|
//@[022:024) Identifier |in|
//@[025:030) Identifier |range|
//@[030:031) LeftParen |(|
//@[031:032) Integer |4|
//@[032:033) Comma |,|
//@[033:034) Integer |4|
//@[034:035) RightParen |)|
//@[035:036) Colon |:|
//@[037:038) LeftBrace |{|
//@[038:040) NewLine |\r\n|
        f: k
//@[008:009) Identifier |f|
//@[009:010) Colon |:|
//@[011:012) Identifier |k|
//@[012:014) NewLine |\r\n|
        g: k2
//@[008:009) Identifier |g|
//@[009:010) Colon |:|
//@[011:013) Identifier |k2|
//@[013:015) NewLine |\r\n|
      }]
//@[006:007) RightBrace |}|
//@[007:008) RightSquare |]|
//@[008:010) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
    stringParamB: ''
//@[004:016) Identifier |stringParamB|
//@[016:017) Colon |:|
//@[018:020) StringComplete |''|
//@[020:022) NewLine |\r\n|
    arrayParam: [
//@[004:014) Identifier |arrayParam|
//@[014:015) Colon |:|
//@[016:017) LeftSquare |[|
//@[017:019) NewLine |\r\n|
      {
//@[006:007) LeftBrace |{|
//@[007:009) NewLine |\r\n|
        e: [for j in range(7,7): j]
//@[008:009) Identifier |e|
//@[009:010) Colon |:|
//@[011:012) LeftSquare |[|
//@[012:015) Identifier |for|
//@[016:017) Identifier |j|
//@[018:020) Identifier |in|
//@[021:026) Identifier |range|
//@[026:027) LeftParen |(|
//@[027:028) Integer |7|
//@[028:029) Comma |,|
//@[029:030) Integer |7|
//@[030:031) RightParen |)|
//@[031:032) Colon |:|
//@[033:034) Identifier |j|
//@[034:035) RightSquare |]|
//@[035:037) NewLine |\r\n|
      }
//@[006:007) RightBrace |}|
//@[007:009) NewLine |\r\n|
    ]
//@[004:005) RightSquare |]|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module propertyLoopInsideParameterValueInsideModuleLoop 'modulea.bicep' = [for thing in range(0,1): {
//@[000:006) Identifier |module|
//@[007:055) Identifier |propertyLoopInsideParameterValueInsideModuleLoop|
//@[056:071) StringComplete |'modulea.bicep'|
//@[072:073) Assignment |=|
//@[074:075) LeftSquare |[|
//@[075:078) Identifier |for|
//@[079:084) Identifier |thing|
//@[085:087) Identifier |in|
//@[088:093) Identifier |range|
//@[093:094) LeftParen |(|
//@[094:095) Integer |0|
//@[095:096) Comma |,|
//@[096:097) Integer |1|
//@[097:098) RightParen |)|
//@[098:099) Colon |:|
//@[100:101) LeftBrace |{|
//@[101:103) NewLine |\r\n|
  name: 'propertyLoopInsideParameterValueInsideModuleLoop'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:058) StringComplete |'propertyLoopInsideParameterValueInsideModuleLoop'|
//@[058:060) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    objParam: {
//@[004:012) Identifier |objParam|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
      a: [for i in range(0,10): i + thing]
//@[006:007) Identifier |a|
//@[007:008) Colon |:|
//@[009:010) LeftSquare |[|
//@[010:013) Identifier |for|
//@[014:015) Identifier |i|
//@[016:018) Identifier |in|
//@[019:024) Identifier |range|
//@[024:025) LeftParen |(|
//@[025:026) Integer |0|
//@[026:027) Comma |,|
//@[027:029) Integer |10|
//@[029:030) RightParen |)|
//@[030:031) Colon |:|
//@[032:033) Identifier |i|
//@[034:035) Plus |+|
//@[036:041) Identifier |thing|
//@[041:042) RightSquare |]|
//@[042:044) NewLine |\r\n|
      b: [for i in range(1,2): i * thing]
//@[006:007) Identifier |b|
//@[007:008) Colon |:|
//@[009:010) LeftSquare |[|
//@[010:013) Identifier |for|
//@[014:015) Identifier |i|
//@[016:018) Identifier |in|
//@[019:024) Identifier |range|
//@[024:025) LeftParen |(|
//@[025:026) Integer |1|
//@[026:027) Comma |,|
//@[027:028) Integer |2|
//@[028:029) RightParen |)|
//@[029:030) Colon |:|
//@[031:032) Identifier |i|
//@[033:034) Asterisk |*|
//@[035:040) Identifier |thing|
//@[040:041) RightSquare |]|
//@[041:043) NewLine |\r\n|
      c: {
//@[006:007) Identifier |c|
//@[007:008) Colon |:|
//@[009:010) LeftBrace |{|
//@[010:012) NewLine |\r\n|
        d: [for j in range(2,3): j]
//@[008:009) Identifier |d|
//@[009:010) Colon |:|
//@[011:012) LeftSquare |[|
//@[012:015) Identifier |for|
//@[016:017) Identifier |j|
//@[018:020) Identifier |in|
//@[021:026) Identifier |range|
//@[026:027) LeftParen |(|
//@[027:028) Integer |2|
//@[028:029) Comma |,|
//@[029:030) Integer |3|
//@[030:031) RightParen |)|
//@[031:032) Colon |:|
//@[033:034) Identifier |j|
//@[034:035) RightSquare |]|
//@[035:037) NewLine |\r\n|
      }
//@[006:007) RightBrace |}|
//@[007:009) NewLine |\r\n|
      e: [for k in range(4,4): {
//@[006:007) Identifier |e|
//@[007:008) Colon |:|
//@[009:010) LeftSquare |[|
//@[010:013) Identifier |for|
//@[014:015) Identifier |k|
//@[016:018) Identifier |in|
//@[019:024) Identifier |range|
//@[024:025) LeftParen |(|
//@[025:026) Integer |4|
//@[026:027) Comma |,|
//@[027:028) Integer |4|
//@[028:029) RightParen |)|
//@[029:030) Colon |:|
//@[031:032) LeftBrace |{|
//@[032:034) NewLine |\r\n|
        f: k - thing
//@[008:009) Identifier |f|
//@[009:010) Colon |:|
//@[011:012) Identifier |k|
//@[013:014) Minus |-|
//@[015:020) Identifier |thing|
//@[020:022) NewLine |\r\n|
      }]
//@[006:007) RightBrace |}|
//@[007:008) RightSquare |]|
//@[008:010) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
    stringParamB: ''
//@[004:016) Identifier |stringParamB|
//@[016:017) Colon |:|
//@[018:020) StringComplete |''|
//@[020:022) NewLine |\r\n|
    arrayParam: [
//@[004:014) Identifier |arrayParam|
//@[014:015) Colon |:|
//@[016:017) LeftSquare |[|
//@[017:019) NewLine |\r\n|
      {
//@[006:007) LeftBrace |{|
//@[007:009) NewLine |\r\n|
        e: [for j in range(7,7): j % (thing + 1)]
//@[008:009) Identifier |e|
//@[009:010) Colon |:|
//@[011:012) LeftSquare |[|
//@[012:015) Identifier |for|
//@[016:017) Identifier |j|
//@[018:020) Identifier |in|
//@[021:026) Identifier |range|
//@[026:027) LeftParen |(|
//@[027:028) Integer |7|
//@[028:029) Comma |,|
//@[029:030) Integer |7|
//@[030:031) RightParen |)|
//@[031:032) Colon |:|
//@[033:034) Identifier |j|
//@[035:036) Modulo |%|
//@[037:038) LeftParen |(|
//@[038:043) Identifier |thing|
//@[044:045) Plus |+|
//@[046:047) Integer |1|
//@[047:048) RightParen |)|
//@[048:049) RightSquare |]|
//@[049:051) NewLine |\r\n|
      }
//@[006:007) RightBrace |}|
//@[007:009) NewLine |\r\n|
    ]
//@[004:005) RightSquare |]|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:008) NewLine |\r\n\r\n\r\n|


// BEGIN: Key Vault Secret Reference
//@[036:040) NewLine |\r\n\r\n|

resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[000:008) Identifier |resource|
//@[009:011) Identifier |kv|
//@[012:050) StringComplete |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[051:059) Identifier |existing|
//@[060:061) Assignment |=|
//@[062:063) LeftBrace |{|
//@[063:065) NewLine |\r\n|
  name: 'testkeyvault'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:022) StringComplete |'testkeyvault'|
//@[022:024) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module secureModule1 'child/secureParams.bicep' = {
//@[000:006) Identifier |module|
//@[007:020) Identifier |secureModule1|
//@[021:047) StringComplete |'child/secureParams.bicep'|
//@[048:049) Assignment |=|
//@[050:051) LeftBrace |{|
//@[051:053) NewLine |\r\n|
  name: 'secureModule1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:023) StringComplete |'secureModule1'|
//@[023:025) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    secureStringParam1: kv.getSecret('mySecret')
//@[004:022) Identifier |secureStringParam1|
//@[022:023) Colon |:|
//@[024:026) Identifier |kv|
//@[026:027) Dot |.|
//@[027:036) Identifier |getSecret|
//@[036:037) LeftParen |(|
//@[037:047) StringComplete |'mySecret'|
//@[047:048) RightParen |)|
//@[048:050) NewLine |\r\n|
    secureStringParam2: kv.getSecret('mySecret','secretVersion')
//@[004:022) Identifier |secureStringParam2|
//@[022:023) Colon |:|
//@[024:026) Identifier |kv|
//@[026:027) Dot |.|
//@[027:036) Identifier |getSecret|
//@[036:037) LeftParen |(|
//@[037:047) StringComplete |'mySecret'|
//@[047:048) Comma |,|
//@[048:063) StringComplete |'secretVersion'|
//@[063:064) RightParen |)|
//@[064:066) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource scopedKv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[000:008) Identifier |resource|
//@[009:017) Identifier |scopedKv|
//@[018:056) StringComplete |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[057:065) Identifier |existing|
//@[066:067) Assignment |=|
//@[068:069) LeftBrace |{|
//@[069:071) NewLine |\r\n|
  name: 'testkeyvault'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:022) StringComplete |'testkeyvault'|
//@[022:024) NewLine |\r\n|
  scope: resourceGroup('otherGroup')
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:022) Identifier |resourceGroup|
//@[022:023) LeftParen |(|
//@[023:035) StringComplete |'otherGroup'|
//@[035:036) RightParen |)|
//@[036:038) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module secureModule2 'child/secureParams.bicep' = {
//@[000:006) Identifier |module|
//@[007:020) Identifier |secureModule2|
//@[021:047) StringComplete |'child/secureParams.bicep'|
//@[048:049) Assignment |=|
//@[050:051) LeftBrace |{|
//@[051:053) NewLine |\r\n|
  name: 'secureModule2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:023) StringComplete |'secureModule2'|
//@[023:025) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    secureStringParam1: scopedKv.getSecret('mySecret')
//@[004:022) Identifier |secureStringParam1|
//@[022:023) Colon |:|
//@[024:032) Identifier |scopedKv|
//@[032:033) Dot |.|
//@[033:042) Identifier |getSecret|
//@[042:043) LeftParen |(|
//@[043:053) StringComplete |'mySecret'|
//@[053:054) RightParen |)|
//@[054:056) NewLine |\r\n|
    secureStringParam2: scopedKv.getSecret('mySecret','secretVersion')
//@[004:022) Identifier |secureStringParam2|
//@[022:023) Colon |:|
//@[024:032) Identifier |scopedKv|
//@[032:033) Dot |.|
//@[033:042) Identifier |getSecret|
//@[042:043) LeftParen |(|
//@[043:053) StringComplete |'mySecret'|
//@[053:054) Comma |,|
//@[054:069) StringComplete |'secretVersion'|
//@[069:070) RightParen |)|
//@[070:072) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

//looped module with looped existing resource (Issue #2862)
//@[059:061) NewLine |\r\n|
var vaults = [
//@[000:003) Identifier |var|
//@[004:010) Identifier |vaults|
//@[011:012) Assignment |=|
//@[013:014) LeftSquare |[|
//@[014:016) NewLine |\r\n|
  {
//@[002:003) LeftBrace |{|
//@[003:005) NewLine |\r\n|
    vaultName: 'test-1-kv'
//@[004:013) Identifier |vaultName|
//@[013:014) Colon |:|
//@[015:026) StringComplete |'test-1-kv'|
//@[026:028) NewLine |\r\n|
    vaultRG: 'test-1-rg'
//@[004:011) Identifier |vaultRG|
//@[011:012) Colon |:|
//@[013:024) StringComplete |'test-1-rg'|
//@[024:026) NewLine |\r\n|
    vaultSub: 'abcd-efgh'
//@[004:012) Identifier |vaultSub|
//@[012:013) Colon |:|
//@[014:025) StringComplete |'abcd-efgh'|
//@[025:027) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  {
//@[002:003) LeftBrace |{|
//@[003:005) NewLine |\r\n|
    vaultName: 'test-2-kv'
//@[004:013) Identifier |vaultName|
//@[013:014) Colon |:|
//@[015:026) StringComplete |'test-2-kv'|
//@[026:028) NewLine |\r\n|
    vaultRG: 'test-2-rg'
//@[004:011) Identifier |vaultRG|
//@[011:012) Colon |:|
//@[013:024) StringComplete |'test-2-rg'|
//@[024:026) NewLine |\r\n|
    vaultSub: 'ijkl-1adg1'
//@[004:012) Identifier |vaultSub|
//@[012:013) Colon |:|
//@[014:026) StringComplete |'ijkl-1adg1'|
//@[026:028) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\r\n|
var secrets = [
//@[000:003) Identifier |var|
//@[004:011) Identifier |secrets|
//@[012:013) Assignment |=|
//@[014:015) LeftSquare |[|
//@[015:017) NewLine |\r\n|
  {
//@[002:003) LeftBrace |{|
//@[003:005) NewLine |\r\n|
    name: 'secret01'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:020) StringComplete |'secret01'|
//@[020:022) NewLine |\r\n|
    version: 'versionA'
//@[004:011) Identifier |version|
//@[011:012) Colon |:|
//@[013:023) StringComplete |'versionA'|
//@[023:025) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  {
//@[002:003) LeftBrace |{|
//@[003:005) NewLine |\r\n|
    name: 'secret02'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:020) StringComplete |'secret02'|
//@[020:022) NewLine |\r\n|
    version: 'versionB'
//@[004:011) Identifier |version|
//@[011:012) Colon |:|
//@[013:023) StringComplete |'versionB'|
//@[023:025) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
]
//@[000:001) RightSquare |]|
//@[001:005) NewLine |\r\n\r\n|

resource loopedKv 'Microsoft.KeyVault/vaults@2019-09-01' existing = [for vault in vaults: {
//@[000:008) Identifier |resource|
//@[009:017) Identifier |loopedKv|
//@[018:056) StringComplete |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[057:065) Identifier |existing|
//@[066:067) Assignment |=|
//@[068:069) LeftSquare |[|
//@[069:072) Identifier |for|
//@[073:078) Identifier |vault|
//@[079:081) Identifier |in|
//@[082:088) Identifier |vaults|
//@[088:089) Colon |:|
//@[090:091) LeftBrace |{|
//@[091:093) NewLine |\r\n|
  name: vault.vaultName
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:013) Identifier |vault|
//@[013:014) Dot |.|
//@[014:023) Identifier |vaultName|
//@[023:025) NewLine |\r\n|
  scope: resourceGroup(vault.vaultSub, vault.vaultRG)
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:022) Identifier |resourceGroup|
//@[022:023) LeftParen |(|
//@[023:028) Identifier |vault|
//@[028:029) Dot |.|
//@[029:037) Identifier |vaultSub|
//@[037:038) Comma |,|
//@[039:044) Identifier |vault|
//@[044:045) Dot |.|
//@[045:052) Identifier |vaultRG|
//@[052:053) RightParen |)|
//@[053:055) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

module secureModuleLooped 'child/secureParams.bicep' = [for (secret, i) in secrets: {
//@[000:006) Identifier |module|
//@[007:025) Identifier |secureModuleLooped|
//@[026:052) StringComplete |'child/secureParams.bicep'|
//@[053:054) Assignment |=|
//@[055:056) LeftSquare |[|
//@[056:059) Identifier |for|
//@[060:061) LeftParen |(|
//@[061:067) Identifier |secret|
//@[067:068) Comma |,|
//@[069:070) Identifier |i|
//@[070:071) RightParen |)|
//@[072:074) Identifier |in|
//@[075:082) Identifier |secrets|
//@[082:083) Colon |:|
//@[084:085) LeftBrace |{|
//@[085:087) NewLine |\r\n|
  name: 'secureModuleLooped-${i}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:030) StringLeftPiece |'secureModuleLooped-${|
//@[030:031) Identifier |i|
//@[031:033) StringRightPiece |}'|
//@[033:035) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    secureStringParam1: loopedKv[i].getSecret(secret.name)
//@[004:022) Identifier |secureStringParam1|
//@[022:023) Colon |:|
//@[024:032) Identifier |loopedKv|
//@[032:033) LeftSquare |[|
//@[033:034) Identifier |i|
//@[034:035) RightSquare |]|
//@[035:036) Dot |.|
//@[036:045) Identifier |getSecret|
//@[045:046) LeftParen |(|
//@[046:052) Identifier |secret|
//@[052:053) Dot |.|
//@[053:057) Identifier |name|
//@[057:058) RightParen |)|
//@[058:060) NewLine |\r\n|
    secureStringParam2: loopedKv[i].getSecret(secret.name, secret.version)
//@[004:022) Identifier |secureStringParam2|
//@[022:023) Colon |:|
//@[024:032) Identifier |loopedKv|
//@[032:033) LeftSquare |[|
//@[033:034) Identifier |i|
//@[034:035) RightSquare |]|
//@[035:036) Dot |.|
//@[036:045) Identifier |getSecret|
//@[045:046) LeftParen |(|
//@[046:052) Identifier |secret|
//@[052:053) Dot |.|
//@[053:057) Identifier |name|
//@[057:058) Comma |,|
//@[059:065) Identifier |secret|
//@[065:066) Dot |.|
//@[066:073) Identifier |version|
//@[073:074) RightParen |)|
//@[074:076) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

module secureModuleCondition 'child/secureParams.bicep' = {
//@[000:006) Identifier |module|
//@[007:028) Identifier |secureModuleCondition|
//@[029:055) StringComplete |'child/secureParams.bicep'|
//@[056:057) Assignment |=|
//@[058:059) LeftBrace |{|
//@[059:061) NewLine |\r\n|
  name: 'secureModuleCondition'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:031) StringComplete |'secureModuleCondition'|
//@[031:033) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    secureStringParam1: true ? kv.getSecret('mySecret') : 'notTrue'
//@[004:022) Identifier |secureStringParam1|
//@[022:023) Colon |:|
//@[024:028) TrueKeyword |true|
//@[029:030) Question |?|
//@[031:033) Identifier |kv|
//@[033:034) Dot |.|
//@[034:043) Identifier |getSecret|
//@[043:044) LeftParen |(|
//@[044:054) StringComplete |'mySecret'|
//@[054:055) RightParen |)|
//@[056:057) Colon |:|
//@[058:067) StringComplete |'notTrue'|
//@[067:069) NewLine |\r\n|
    secureStringParam2: true ? false ? 'false' : kv.getSecret('mySecret','secretVersion') : 'notTrue'
//@[004:022) Identifier |secureStringParam2|
//@[022:023) Colon |:|
//@[024:028) TrueKeyword |true|
//@[029:030) Question |?|
//@[031:036) FalseKeyword |false|
//@[037:038) Question |?|
//@[039:046) StringComplete |'false'|
//@[047:048) Colon |:|
//@[049:051) Identifier |kv|
//@[051:052) Dot |.|
//@[052:061) Identifier |getSecret|
//@[061:062) LeftParen |(|
//@[062:072) StringComplete |'mySecret'|
//@[072:073) Comma |,|
//@[073:088) StringComplete |'secretVersion'|
//@[088:089) RightParen |)|
//@[090:091) Colon |:|
//@[092:101) StringComplete |'notTrue'|
//@[101:103) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// END: Key Vault Secret Reference
//@[034:038) NewLine |\r\n\r\n|

module withSpace 'module with space.bicep' = {
//@[000:006) Identifier |module|
//@[007:016) Identifier |withSpace|
//@[017:042) StringComplete |'module with space.bicep'|
//@[043:044) Assignment |=|
//@[045:046) LeftBrace |{|
//@[046:048) NewLine |\r\n|
  name: 'withSpace'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'withSpace'|
//@[019:021) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module folderWithSpace 'child/folder with space/child with space.bicep' = {
//@[000:006) Identifier |module|
//@[007:022) Identifier |folderWithSpace|
//@[023:071) StringComplete |'child/folder with space/child with space.bicep'|
//@[072:073) Assignment |=|
//@[074:075) LeftBrace |{|
//@[075:077) NewLine |\r\n|
  name: 'childWithSpace'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:024) StringComplete |'childWithSpace'|
//@[024:026) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// nameof
//@[009:013) NewLine |\r\n\r\n|

var nameofModule = nameof(folderWithSpace)
//@[000:003) Identifier |var|
//@[004:016) Identifier |nameofModule|
//@[017:018) Assignment |=|
//@[019:025) Identifier |nameof|
//@[025:026) LeftParen |(|
//@[026:041) Identifier |folderWithSpace|
//@[041:042) RightParen |)|
//@[042:044) NewLine |\r\n|
var nameofModuleParam = nameof(secureModuleCondition.outputs.exposedSecureString)
//@[000:003) Identifier |var|
//@[004:021) Identifier |nameofModuleParam|
//@[022:023) Assignment |=|
//@[024:030) Identifier |nameof|
//@[030:031) LeftParen |(|
//@[031:052) Identifier |secureModuleCondition|
//@[052:053) Dot |.|
//@[053:060) Identifier |outputs|
//@[060:061) Dot |.|
//@[061:080) Identifier |exposedSecureString|
//@[080:081) RightParen |)|
//@[081:085) NewLine |\r\n\r\n|

module moduleWithNameof 'modulea.bicep' = {
//@[000:006) Identifier |module|
//@[007:023) Identifier |moduleWithNameof|
//@[024:039) StringComplete |'modulea.bicep'|
//@[040:041) Assignment |=|
//@[042:043) LeftBrace |{|
//@[043:045) NewLine |\r\n|
  name: 'nameofModule'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:022) StringComplete |'nameofModule'|
//@[022:024) NewLine |\r\n|
  scope: resourceGroup(nameof(nameofModuleParam))
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:022) Identifier |resourceGroup|
//@[022:023) LeftParen |(|
//@[023:029) Identifier |nameof|
//@[029:030) LeftParen |(|
//@[030:047) Identifier |nameofModuleParam|
//@[047:048) RightParen |)|
//@[048:049) RightParen |)|
//@[049:051) NewLine |\r\n|
  params:{
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[009:010) LeftBrace |{|
//@[010:012) NewLine |\r\n|
    stringParamA: nameof(withSpace)
//@[004:016) Identifier |stringParamA|
//@[016:017) Colon |:|
//@[018:024) Identifier |nameof|
//@[024:025) LeftParen |(|
//@[025:034) Identifier |withSpace|
//@[034:035) RightParen |)|
//@[035:037) NewLine |\r\n|
    stringParamB: nameof(folderWithSpace)
//@[004:016) Identifier |stringParamB|
//@[016:017) Colon |:|
//@[018:024) Identifier |nameof|
//@[024:025) LeftParen |(|
//@[025:040) Identifier |folderWithSpace|
//@[040:041) RightParen |)|
//@[041:043) NewLine |\r\n|
    objParam: {
//@[004:012) Identifier |objParam|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
      a: nameof(secureModuleCondition.outputs.exposedSecureString)
//@[006:007) Identifier |a|
//@[007:008) Colon |:|
//@[009:015) Identifier |nameof|
//@[015:016) LeftParen |(|
//@[016:037) Identifier |secureModuleCondition|
//@[037:038) Dot |.|
//@[038:045) Identifier |outputs|
//@[045:046) Dot |.|
//@[046:065) Identifier |exposedSecureString|
//@[065:066) RightParen |)|
//@[066:068) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
    arrayParam: [
//@[004:014) Identifier |arrayParam|
//@[014:015) Colon |:|
//@[016:017) LeftSquare |[|
//@[017:019) NewLine |\r\n|
      nameof(vaults)
//@[006:012) Identifier |nameof|
//@[012:013) LeftParen |(|
//@[013:019) Identifier |vaults|
//@[019:020) RightParen |)|
//@[020:022) NewLine |\r\n|
    ]
//@[004:005) RightSquare |]|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module moduleWithNullableOutputs 'child/nullableOutputs.bicep' = {
//@[000:006) Identifier |module|
//@[007:032) Identifier |moduleWithNullableOutputs|
//@[033:062) StringComplete |'child/nullableOutputs.bicep'|
//@[063:064) Assignment |=|
//@[065:066) LeftBrace |{|
//@[066:068) NewLine |\r\n|
  name: 'nullableOutputs'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:025) StringComplete |'nullableOutputs'|
//@[025:027) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

output nullableString string? = moduleWithNullableOutputs.outputs.?nullableString
//@[000:006) Identifier |output|
//@[007:021) Identifier |nullableString|
//@[022:028) Identifier |string|
//@[028:029) Question |?|
//@[030:031) Assignment |=|
//@[032:057) Identifier |moduleWithNullableOutputs|
//@[057:058) Dot |.|
//@[058:065) Identifier |outputs|
//@[065:066) Dot |.|
//@[066:067) Question |?|
//@[067:081) Identifier |nullableString|
//@[081:083) NewLine |\r\n|
output deeplyNestedProperty string? = moduleWithNullableOutputs.outputs.?nullableObj.deeply.nested.property
//@[000:006) Identifier |output|
//@[007:027) Identifier |deeplyNestedProperty|
//@[028:034) Identifier |string|
//@[034:035) Question |?|
//@[036:037) Assignment |=|
//@[038:063) Identifier |moduleWithNullableOutputs|
//@[063:064) Dot |.|
//@[064:071) Identifier |outputs|
//@[071:072) Dot |.|
//@[072:073) Question |?|
//@[073:084) Identifier |nullableObj|
//@[084:085) Dot |.|
//@[085:091) Identifier |deeply|
//@[091:092) Dot |.|
//@[092:098) Identifier |nested|
//@[098:099) Dot |.|
//@[099:107) Identifier |property|
//@[107:109) NewLine |\r\n|
output deeplyNestedArrayItem string? = moduleWithNullableOutputs.outputs.?nullableObj.deeply.nested.array[0]
//@[000:006) Identifier |output|
//@[007:028) Identifier |deeplyNestedArrayItem|
//@[029:035) Identifier |string|
//@[035:036) Question |?|
//@[037:038) Assignment |=|
//@[039:064) Identifier |moduleWithNullableOutputs|
//@[064:065) Dot |.|
//@[065:072) Identifier |outputs|
//@[072:073) Dot |.|
//@[073:074) Question |?|
//@[074:085) Identifier |nullableObj|
//@[085:086) Dot |.|
//@[086:092) Identifier |deeply|
//@[092:093) Dot |.|
//@[093:099) Identifier |nested|
//@[099:100) Dot |.|
//@[100:105) Identifier |array|
//@[105:106) LeftSquare |[|
//@[106:107) Integer |0|
//@[107:108) RightSquare |]|
//@[108:110) NewLine |\r\n|
output deeplyNestedArrayItemFromEnd string? = moduleWithNullableOutputs.outputs.?nullableObj.deeply.nested.array[^1]
//@[000:006) Identifier |output|
//@[007:035) Identifier |deeplyNestedArrayItemFromEnd|
//@[036:042) Identifier |string|
//@[042:043) Question |?|
//@[044:045) Assignment |=|
//@[046:071) Identifier |moduleWithNullableOutputs|
//@[071:072) Dot |.|
//@[072:079) Identifier |outputs|
//@[079:080) Dot |.|
//@[080:081) Question |?|
//@[081:092) Identifier |nullableObj|
//@[092:093) Dot |.|
//@[093:099) Identifier |deeply|
//@[099:100) Dot |.|
//@[100:106) Identifier |nested|
//@[106:107) Dot |.|
//@[107:112) Identifier |array|
//@[112:113) LeftSquare |[|
//@[113:114) Hat |^|
//@[114:115) Integer |1|
//@[115:116) RightSquare |]|
//@[116:118) NewLine |\r\n|
output deeplyNestedArrayItemFromEndAttempt string? = moduleWithNullableOutputs.outputs.?nullableObj.deeply.nested.array[?^1]
//@[000:006) Identifier |output|
//@[007:042) Identifier |deeplyNestedArrayItemFromEndAttempt|
//@[043:049) Identifier |string|
//@[049:050) Question |?|
//@[051:052) Assignment |=|
//@[053:078) Identifier |moduleWithNullableOutputs|
//@[078:079) Dot |.|
//@[079:086) Identifier |outputs|
//@[086:087) Dot |.|
//@[087:088) Question |?|
//@[088:099) Identifier |nullableObj|
//@[099:100) Dot |.|
//@[100:106) Identifier |deeply|
//@[106:107) Dot |.|
//@[107:113) Identifier |nested|
//@[113:114) Dot |.|
//@[114:119) Identifier |array|
//@[119:120) LeftSquare |[|
//@[120:121) Question |?|
//@[121:122) Hat |^|
//@[122:123) Integer |1|
//@[123:124) RightSquare |]|
//@[124:126) NewLine |\r\n|

//@[000:000) EndOfFile ||
