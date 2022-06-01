targetScope = 'tenant'
//@[00:11) Identifier |targetScope|
//@[12:13) Assignment |=|
//@[14:22) StringComplete |'tenant'|
//@[22:24) NewLine |\n\n|

module tenantModuleDuplicateName1 'modules/tenant.bicep' = {
//@[00:06) Identifier |module|
//@[07:33) Identifier |tenantModuleDuplicateName1|
//@[34:56) StringComplete |'modules/tenant.bicep'|
//@[57:58) Assignment |=|
//@[59:60) LeftBrace |{|
//@[60:61) NewLine |\n|
  name: 'tenantModuleDuplicateName'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:35) StringComplete |'tenantModuleDuplicateName'|
//@[35:36) NewLine |\n|
  scope: tenant()
//@[02:07) Identifier |scope|
//@[07:08) Colon |:|
//@[09:15) Identifier |tenant|
//@[15:16) LeftParen |(|
//@[16:17) RightParen |)|
//@[17:18) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

module tenantModuleDuplicateName2 'modules/tenant.bicep' = {
//@[00:06) Identifier |module|
//@[07:33) Identifier |tenantModuleDuplicateName2|
//@[34:56) StringComplete |'modules/tenant.bicep'|
//@[57:58) Assignment |=|
//@[59:60) LeftBrace |{|
//@[60:61) NewLine |\n|
  name: 'tenantModuleDuplicateName'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:35) StringComplete |'tenantModuleDuplicateName'|
//@[35:36) NewLine |\n|
  scope: tenant()
//@[02:07) Identifier |scope|
//@[07:08) Colon |:|
//@[09:15) Identifier |tenant|
//@[15:16) LeftParen |(|
//@[16:17) RightParen |)|
//@[17:18) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

module tenantModuleDuplicateName3 'modules/tenant.bicep' = {
//@[00:06) Identifier |module|
//@[07:33) Identifier |tenantModuleDuplicateName3|
//@[34:56) StringComplete |'modules/tenant.bicep'|
//@[57:58) Assignment |=|
//@[59:60) LeftBrace |{|
//@[60:61) NewLine |\n|
  name: 'tenantModuleDuplicateName'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:35) StringComplete |'tenantModuleDuplicateName'|
//@[35:36) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

module managementGroupModuleDuplicateName1 'modules/managementGroup.bicep' = {
//@[00:06) Identifier |module|
//@[07:42) Identifier |managementGroupModuleDuplicateName1|
//@[43:74) StringComplete |'modules/managementGroup.bicep'|
//@[75:76) Assignment |=|
//@[77:78) LeftBrace |{|
//@[78:79) NewLine |\n|
  name: 'managementGroupModuleDuplicateName'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:44) StringComplete |'managementGroupModuleDuplicateName'|
//@[44:45) NewLine |\n|
  scope: managementGroup('MG')
//@[02:07) Identifier |scope|
//@[07:08) Colon |:|
//@[09:24) Identifier |managementGroup|
//@[24:25) LeftParen |(|
//@[25:29) StringComplete |'MG'|
//@[29:30) RightParen |)|
//@[30:31) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

module managementGroupModuleDuplicateName2 'modules/managementGroup.bicep' = {
//@[00:06) Identifier |module|
//@[07:42) Identifier |managementGroupModuleDuplicateName2|
//@[43:74) StringComplete |'modules/managementGroup.bicep'|
//@[75:76) Assignment |=|
//@[77:78) LeftBrace |{|
//@[78:79) NewLine |\n|
  name: 'managementGroupModuleDuplicateName'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:44) StringComplete |'managementGroupModuleDuplicateName'|
//@[44:45) NewLine |\n|
  scope: managementGroup('MG')
//@[02:07) Identifier |scope|
//@[07:08) Colon |:|
//@[09:24) Identifier |managementGroup|
//@[24:25) LeftParen |(|
//@[25:29) StringComplete |'MG'|
//@[29:30) RightParen |)|
//@[30:31) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

module subscriptionModuleDuplicateName1 'modules/subscription.bicep' = {
//@[00:06) Identifier |module|
//@[07:39) Identifier |subscriptionModuleDuplicateName1|
//@[40:68) StringComplete |'modules/subscription.bicep'|
//@[69:70) Assignment |=|
//@[71:72) LeftBrace |{|
//@[72:73) NewLine |\n|
  name: 'subscriptionModuleDuplicateName'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:41) StringComplete |'subscriptionModuleDuplicateName'|
//@[41:42) NewLine |\n|
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
//@[02:07) Identifier |scope|
//@[07:08) Colon |:|
//@[09:21) Identifier |subscription|
//@[21:22) LeftParen |(|
//@[22:60) StringComplete |'1ad827ac-2669-4c2f-9970-282b93c3c550'|
//@[60:61) RightParen |)|
//@[61:62) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

module subscriptionModuleDuplicateName2 'modules/subscription.bicep' = {
//@[00:06) Identifier |module|
//@[07:39) Identifier |subscriptionModuleDuplicateName2|
//@[40:68) StringComplete |'modules/subscription.bicep'|
//@[69:70) Assignment |=|
//@[71:72) LeftBrace |{|
//@[72:73) NewLine |\n|
  name: 'subscriptionModuleDuplicateName'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:41) StringComplete |'subscriptionModuleDuplicateName'|
//@[41:42) NewLine |\n|
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
//@[02:07) Identifier |scope|
//@[07:08) Colon |:|
//@[09:21) Identifier |subscription|
//@[21:22) LeftParen |(|
//@[22:60) StringComplete |'1ad827ac-2669-4c2f-9970-282b93c3c550'|
//@[60:61) RightParen |)|
//@[61:62) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

module managementGroupModules 'modules/managementGroup.bicep' = [for (mg, i) in []: {
//@[00:06) Identifier |module|
//@[07:29) Identifier |managementGroupModules|
//@[30:61) StringComplete |'modules/managementGroup.bicep'|
//@[62:63) Assignment |=|
//@[64:65) LeftSquare |[|
//@[65:68) Identifier |for|
//@[69:70) LeftParen |(|
//@[70:72) Identifier |mg|
//@[72:73) Comma |,|
//@[74:75) Identifier |i|
//@[75:76) RightParen |)|
//@[77:79) Identifier |in|
//@[80:81) LeftSquare |[|
//@[81:82) RightSquare |]|
//@[82:83) Colon |:|
//@[84:85) LeftBrace |{|
//@[85:86) NewLine |\n|
  name: 'dep-${mg}'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:15) StringLeftPiece |'dep-${|
//@[15:17) Identifier |mg|
//@[17:19) StringRightPiece |}'|
//@[19:20) NewLine |\n|
  scope: managementGroup(mg)
//@[02:07) Identifier |scope|
//@[07:08) Colon |:|
//@[09:24) Identifier |managementGroup|
//@[24:25) LeftParen |(|
//@[25:27) Identifier |mg|
//@[27:28) RightParen |)|
//@[28:29) NewLine |\n|
}]
//@[00:01) RightBrace |}|
//@[01:02) RightSquare |]|
//@[02:04) NewLine |\n\n|

module cannotUseModuleCollectionAsScope 'modules/managementGroup.bicep' = [for (mg, i) in []: {
//@[00:06) Identifier |module|
//@[07:39) Identifier |cannotUseModuleCollectionAsScope|
//@[40:71) StringComplete |'modules/managementGroup.bicep'|
//@[72:73) Assignment |=|
//@[74:75) LeftSquare |[|
//@[75:78) Identifier |for|
//@[79:80) LeftParen |(|
//@[80:82) Identifier |mg|
//@[82:83) Comma |,|
//@[84:85) Identifier |i|
//@[85:86) RightParen |)|
//@[87:89) Identifier |in|
//@[90:91) LeftSquare |[|
//@[91:92) RightSquare |]|
//@[92:93) Colon |:|
//@[94:95) LeftBrace |{|
//@[95:96) NewLine |\n|
  name: 'dep-${mg}'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:15) StringLeftPiece |'dep-${|
//@[15:17) Identifier |mg|
//@[17:19) StringRightPiece |}'|
//@[19:20) NewLine |\n|
  scope: managementGroupModules
//@[02:07) Identifier |scope|
//@[07:08) Colon |:|
//@[09:31) Identifier |managementGroupModules|
//@[31:32) NewLine |\n|
}]
//@[00:01) RightBrace |}|
//@[01:02) RightSquare |]|
//@[02:04) NewLine |\n\n|

module cannotUseSingleModuleAsScope 'modules/managementGroup.bicep' = [for (mg, i) in []: {
//@[00:06) Identifier |module|
//@[07:35) Identifier |cannotUseSingleModuleAsScope|
//@[36:67) StringComplete |'modules/managementGroup.bicep'|
//@[68:69) Assignment |=|
//@[70:71) LeftSquare |[|
//@[71:74) Identifier |for|
//@[75:76) LeftParen |(|
//@[76:78) Identifier |mg|
//@[78:79) Comma |,|
//@[80:81) Identifier |i|
//@[81:82) RightParen |)|
//@[83:85) Identifier |in|
//@[86:87) LeftSquare |[|
//@[87:88) RightSquare |]|
//@[88:89) Colon |:|
//@[90:91) LeftBrace |{|
//@[91:92) NewLine |\n|
  name: 'dep-${mg}'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:15) StringLeftPiece |'dep-${|
//@[15:17) Identifier |mg|
//@[17:19) StringRightPiece |}'|
//@[19:20) NewLine |\n|
  scope: managementGroupModules[i]
//@[02:07) Identifier |scope|
//@[07:08) Colon |:|
//@[09:31) Identifier |managementGroupModules|
//@[31:32) LeftSquare |[|
//@[32:33) Identifier |i|
//@[33:34) RightSquare |]|
//@[34:35) NewLine |\n|
}]
//@[00:01) RightBrace |}|
//@[01:02) RightSquare |]|
//@[02:04) NewLine |\n\n|

module cannotUseSingleModuleAsScope2 'modules/managementGroup.bicep' = {
//@[00:06) Identifier |module|
//@[07:36) Identifier |cannotUseSingleModuleAsScope2|
//@[37:68) StringComplete |'modules/managementGroup.bicep'|
//@[69:70) Assignment |=|
//@[71:72) LeftBrace |{|
//@[72:73) NewLine |\n|
  name: 'test'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:14) StringComplete |'test'|
//@[14:15) NewLine |\n|
  scope: managementGroupModuleDuplicateName1
//@[02:07) Identifier |scope|
//@[07:08) Colon |:|
//@[09:44) Identifier |managementGroupModuleDuplicateName1|
//@[44:45) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:02) NewLine |\n|

//@[00:00) EndOfFile ||
