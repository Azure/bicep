targetScope = 'tenant'
//@[000:011) Identifier |targetScope|
//@[012:013) Assignment |=|
//@[014:022) StringComplete |'tenant'|
//@[022:024) NewLine |\n\n|

module myManagementGroupMod 'modules/managementgroup.bicep' = {
//@[000:006) Identifier |module|
//@[007:027) Identifier |myManagementGroupMod|
//@[028:059) StringComplete |'modules/managementgroup.bicep'|
//@[060:061) Assignment |=|
//@[062:063) LeftBrace |{|
//@[063:064) NewLine |\n|
  name: 'myManagementGroupMod'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:030) StringComplete |'myManagementGroupMod'|
//@[030:031) NewLine |\n|
  scope: managementGroup('myManagementGroup')
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:024) Identifier |managementGroup|
//@[024:025) LeftParen |(|
//@[025:044) StringComplete |'myManagementGroup'|
//@[044:045) RightParen |)|
//@[045:046) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:002) NewLine |\n|
module myManagementGroupModWithDuplicatedNameButDifferentScope 'modules/managementgroup_empty.bicep' = {
//@[000:006) Identifier |module|
//@[007:062) Identifier |myManagementGroupModWithDuplicatedNameButDifferentScope|
//@[063:100) StringComplete |'modules/managementgroup_empty.bicep'|
//@[101:102) Assignment |=|
//@[103:104) LeftBrace |{|
//@[104:105) NewLine |\n|
  name: 'myManagementGroupMod'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:030) StringComplete |'myManagementGroupMod'|
//@[030:031) NewLine |\n|
  scope: managementGroup('myManagementGroup2')
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:024) Identifier |managementGroup|
//@[024:025) LeftParen |(|
//@[025:045) StringComplete |'myManagementGroup2'|
//@[045:046) RightParen |)|
//@[046:047) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:002) NewLine |\n|
module mySubscriptionMod 'modules/subscription.bicep' = {
//@[000:006) Identifier |module|
//@[007:024) Identifier |mySubscriptionMod|
//@[025:053) StringComplete |'modules/subscription.bicep'|
//@[054:055) Assignment |=|
//@[056:057) LeftBrace |{|
//@[057:058) NewLine |\n|
  name: 'mySubscriptionMod'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:027) StringComplete |'mySubscriptionMod'|
//@[027:028) NewLine |\n|
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:021) Identifier |subscription|
//@[021:022) LeftParen |(|
//@[022:060) StringComplete |'ee44cd78-68c6-43d9-874e-e684ec8d1191'|
//@[060:061) RightParen |)|
//@[061:062) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module mySubscriptionModWithCondition 'modules/subscription.bicep' = if (length('foo') == 3) {
//@[000:006) Identifier |module|
//@[007:037) Identifier |mySubscriptionModWithCondition|
//@[038:066) StringComplete |'modules/subscription.bicep'|
//@[067:068) Assignment |=|
//@[069:071) Identifier |if|
//@[072:073) LeftParen |(|
//@[073:079) Identifier |length|
//@[079:080) LeftParen |(|
//@[080:085) StringComplete |'foo'|
//@[085:086) RightParen |)|
//@[087:089) Equals |==|
//@[090:091) Integer |3|
//@[091:092) RightParen |)|
//@[093:094) LeftBrace |{|
//@[094:095) NewLine |\n|
  name: 'mySubscriptionModWithCondition'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:040) StringComplete |'mySubscriptionModWithCondition'|
//@[040:041) NewLine |\n|
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:021) Identifier |subscription|
//@[021:022) LeftParen |(|
//@[022:060) StringComplete |'ee44cd78-68c6-43d9-874e-e684ec8d1191'|
//@[060:061) RightParen |)|
//@[061:062) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module mySubscriptionModWithDuplicatedNameButDifferentScope 'modules/subscription_empty.bicep' = {
//@[000:006) Identifier |module|
//@[007:059) Identifier |mySubscriptionModWithDuplicatedNameButDifferentScope|
//@[060:094) StringComplete |'modules/subscription_empty.bicep'|
//@[095:096) Assignment |=|
//@[097:098) LeftBrace |{|
//@[098:099) NewLine |\n|
  name: 'mySubscriptionMod'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:027) StringComplete |'mySubscriptionMod'|
//@[027:028) NewLine |\n|
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:021) Identifier |subscription|
//@[021:022) LeftParen |(|
//@[022:060) StringComplete |'1ad827ac-2669-4c2f-9970-282b93c3c550'|
//@[060:061) RightParen |)|
//@[061:062) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:004) NewLine |\n\n\n|


output myManagementGroupOutput string = myManagementGroupMod.outputs.myOutput
//@[000:006) Identifier |output|
//@[007:030) Identifier |myManagementGroupOutput|
//@[031:037) Identifier |string|
//@[038:039) Assignment |=|
//@[040:060) Identifier |myManagementGroupMod|
//@[060:061) Dot |.|
//@[061:068) Identifier |outputs|
//@[068:069) Dot |.|
//@[069:077) Identifier |myOutput|
//@[077:078) NewLine |\n|
output mySubscriptionOutput string = mySubscriptionMod.outputs.myOutput
//@[000:006) Identifier |output|
//@[007:027) Identifier |mySubscriptionOutput|
//@[028:034) Identifier |string|
//@[035:036) Assignment |=|
//@[037:054) Identifier |mySubscriptionMod|
//@[054:055) Dot |.|
//@[055:062) Identifier |outputs|
//@[062:063) Dot |.|
//@[063:071) Identifier |myOutput|
//@[071:072) NewLine |\n|

//@[000:000) EndOfFile ||
