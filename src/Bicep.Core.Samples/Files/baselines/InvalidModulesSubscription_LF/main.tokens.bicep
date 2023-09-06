targetScope = 'subscription'
//@[00:11) Identifier |targetScope|
//@[12:13) Assignment |=|
//@[14:28) StringComplete |'subscription'|
//@[28:30) NewLine |\n\n|

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
  scope: subscription('ced92236-c4d9-46ab-a299-a59c387fd1ee')
//@[02:07) Identifier |scope|
//@[07:08) Colon |:|
//@[09:21) Identifier |subscription|
//@[21:22) LeftParen |(|
//@[22:60) StringComplete |'ced92236-c4d9-46ab-a299-a59c387fd1ee'|
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
  scope: subscription('ced92236-c4d9-46ab-a299-a59c387fd1ee')
//@[02:07) Identifier |scope|
//@[07:08) Colon |:|
//@[09:21) Identifier |subscription|
//@[21:22) LeftParen |(|
//@[22:60) StringComplete |'ced92236-c4d9-46ab-a299-a59c387fd1ee'|
//@[60:61) RightParen |)|
//@[61:62) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

module subscriptionModuleDuplicateName3 'modules/subscription.bicep' = {
//@[00:06) Identifier |module|
//@[07:39) Identifier |subscriptionModuleDuplicateName3|
//@[40:68) StringComplete |'modules/subscription.bicep'|
//@[69:70) Assignment |=|
//@[71:72) LeftBrace |{|
//@[72:73) NewLine |\n|
  name: 'subscriptionModuleDuplicateName'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:41) StringComplete |'subscriptionModuleDuplicateName'|
//@[41:42) NewLine |\n|
  scope: subscription()
//@[02:07) Identifier |scope|
//@[07:08) Colon |:|
//@[09:21) Identifier |subscription|
//@[21:22) LeftParen |(|
//@[22:23) RightParen |)|
//@[23:24) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:02) NewLine |\n|
module subscriptionModuleDuplicateName4 'modules/subscription.bicep' = {
//@[00:06) Identifier |module|
//@[07:39) Identifier |subscriptionModuleDuplicateName4|
//@[40:68) StringComplete |'modules/subscription.bicep'|
//@[69:70) Assignment |=|
//@[71:72) LeftBrace |{|
//@[72:73) NewLine |\n|
  name: 'subscriptionModuleDuplicateName'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:41) StringComplete |'subscriptionModuleDuplicateName'|
//@[41:42) NewLine |\n|
  scope: subscription()
//@[02:07) Identifier |scope|
//@[07:08) Colon |:|
//@[09:21) Identifier |subscription|
//@[21:22) LeftParen |(|
//@[22:23) RightParen |)|
//@[23:24) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:02) NewLine |\n|
module subscriptionModuleDuplicateName5 'modules/subscription.bicep' = {
//@[00:06) Identifier |module|
//@[07:39) Identifier |subscriptionModuleDuplicateName5|
//@[40:68) StringComplete |'modules/subscription.bicep'|
//@[69:70) Assignment |=|
//@[71:72) LeftBrace |{|
//@[72:73) NewLine |\n|
  name: 'subscriptionModuleDuplicateName'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:41) StringComplete |'subscriptionModuleDuplicateName'|
//@[41:42) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

module resourceGroupModuleDuplicateName1 'modules/resourceGroup.bicep' = {
//@[00:06) Identifier |module|
//@[07:40) Identifier |resourceGroupModuleDuplicateName1|
//@[41:70) StringComplete |'modules/resourceGroup.bicep'|
//@[71:72) Assignment |=|
//@[73:74) LeftBrace |{|
//@[74:75) NewLine |\n|
  name: 'resourceGroupModuleDuplicateName'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:42) StringComplete |'resourceGroupModuleDuplicateName'|
//@[42:43) NewLine |\n|
  scope: resourceGroup('RG')
//@[02:07) Identifier |scope|
//@[07:08) Colon |:|
//@[09:22) Identifier |resourceGroup|
//@[22:23) LeftParen |(|
//@[23:27) StringComplete |'RG'|
//@[27:28) RightParen |)|
//@[28:29) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:02) NewLine |\n|
module resourceGroupModuleDuplicateName2 'modules/resourceGroup.bicep' = {
//@[00:06) Identifier |module|
//@[07:40) Identifier |resourceGroupModuleDuplicateName2|
//@[41:70) StringComplete |'modules/resourceGroup.bicep'|
//@[71:72) Assignment |=|
//@[73:74) LeftBrace |{|
//@[74:75) NewLine |\n|
  name: 'resourceGroupModuleDuplicateName'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:42) StringComplete |'resourceGroupModuleDuplicateName'|
//@[42:43) NewLine |\n|
  scope: resourceGroup('RG')
//@[02:07) Identifier |scope|
//@[07:08) Colon |:|
//@[09:22) Identifier |resourceGroup|
//@[22:23) LeftParen |(|
//@[23:27) StringComplete |'RG'|
//@[27:28) RightParen |)|
//@[28:29) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

module unsupportedScopeManagementGroup 'modules/managementGroup.bicep' = {
//@[00:06) Identifier |module|
//@[07:38) Identifier |unsupportedScopeManagementGroup|
//@[39:70) StringComplete |'modules/managementGroup.bicep'|
//@[71:72) Assignment |=|
//@[73:74) LeftBrace |{|
//@[74:75) NewLine |\n|
  name: 'unsupportedScopeManagementGroup'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:41) StringComplete |'unsupportedScopeManagementGroup'|
//@[41:42) NewLine |\n|
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

module singleRgModule 'modules/passthrough.bicep' = {
//@[00:06) Identifier |module|
//@[07:21) Identifier |singleRgModule|
//@[22:49) StringComplete |'modules/passthrough.bicep'|
//@[50:51) Assignment |=|
//@[52:53) LeftBrace |{|
//@[53:54) NewLine |\n|
  name: 'single-rg'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:19) StringComplete |'single-rg'|
//@[19:20) NewLine |\n|
  params: {
//@[02:08) Identifier |params|
//@[08:09) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    myInput: 'stuff'
//@[04:11) Identifier |myInput|
//@[11:12) Colon |:|
//@[13:20) StringComplete |'stuff'|
//@[20:21) NewLine |\n|
  }
//@[02:03) RightBrace |}|
//@[03:04) NewLine |\n|
  scope: resourceGroup('test')
//@[02:07) Identifier |scope|
//@[07:08) Colon |:|
//@[09:22) Identifier |resourceGroup|
//@[22:23) LeftParen |(|
//@[23:29) StringComplete |'test'|
//@[29:30) RightParen |)|
//@[30:31) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

module singleRgModule2 'modules/passthrough.bicep' = {
//@[00:06) Identifier |module|
//@[07:22) Identifier |singleRgModule2|
//@[23:50) StringComplete |'modules/passthrough.bicep'|
//@[51:52) Assignment |=|
//@[53:54) LeftBrace |{|
//@[54:55) NewLine |\n|
  name: 'single-rg2'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:20) StringComplete |'single-rg2'|
//@[20:21) NewLine |\n|
  params: {
//@[02:08) Identifier |params|
//@[08:09) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    myInput: 'stuff'
//@[04:11) Identifier |myInput|
//@[11:12) Colon |:|
//@[13:20) StringComplete |'stuff'|
//@[20:21) NewLine |\n|
  }
//@[02:03) RightBrace |}|
//@[03:04) NewLine |\n|
  scope: singleRgModule
//@[02:07) Identifier |scope|
//@[07:08) Colon |:|
//@[09:23) Identifier |singleRgModule|
//@[23:24) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:02) NewLine |\n|

//@[00:00) EndOfFile ||
