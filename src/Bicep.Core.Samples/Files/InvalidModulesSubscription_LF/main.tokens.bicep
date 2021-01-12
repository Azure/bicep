targetScope = 'subscription'
//@[0:11) Identifier |targetScope|
//@[12:13) Assignment |=|
//@[14:28) StringComplete |'subscription'|
//@[28:30) NewLine |\n\n|

module subscriptionModuleDuplicateName1 'modules/subscription.bicep' = {
//@[0:6) Identifier |module|
//@[7:39) Identifier |subscriptionModuleDuplicateName1|
//@[40:68) StringComplete |'modules/subscription.bicep'|
//@[69:70) Assignment |=|
//@[71:72) LeftBrace |{|
//@[72:73) NewLine |\n|
  name: 'subscriptionModuleDuplicateName'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:41) StringComplete |'subscriptionModuleDuplicateName'|
//@[41:42) NewLine |\n|
  scope: subscription('ced92236-c4d9-46ab-a299-a59c387fd1ee')
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:21) Identifier |subscription|
//@[21:22) LeftParen |(|
//@[22:60) StringComplete |'ced92236-c4d9-46ab-a299-a59c387fd1ee'|
//@[60:61) RightParen |)|
//@[61:62) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module subscriptionModuleDuplicateName2 'modules/subscription.bicep' = {
//@[0:6) Identifier |module|
//@[7:39) Identifier |subscriptionModuleDuplicateName2|
//@[40:68) StringComplete |'modules/subscription.bicep'|
//@[69:70) Assignment |=|
//@[71:72) LeftBrace |{|
//@[72:73) NewLine |\n|
  name: 'subscriptionModuleDuplicateName'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:41) StringComplete |'subscriptionModuleDuplicateName'|
//@[41:42) NewLine |\n|
  scope: subscription('ced92236-c4d9-46ab-a299-a59c387fd1ee')
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:21) Identifier |subscription|
//@[21:22) LeftParen |(|
//@[22:60) StringComplete |'ced92236-c4d9-46ab-a299-a59c387fd1ee'|
//@[60:61) RightParen |)|
//@[61:62) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module subscriptionModuleDuplicateName3 'modules/subscription.bicep' = {
//@[0:6) Identifier |module|
//@[7:39) Identifier |subscriptionModuleDuplicateName3|
//@[40:68) StringComplete |'modules/subscription.bicep'|
//@[69:70) Assignment |=|
//@[71:72) LeftBrace |{|
//@[72:73) NewLine |\n|
  name: 'subscriptionModuleDuplicateName'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:41) StringComplete |'subscriptionModuleDuplicateName'|
//@[41:42) NewLine |\n|
  scope: subscription()
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:21) Identifier |subscription|
//@[21:22) LeftParen |(|
//@[22:23) RightParen |)|
//@[23:24) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:2) NewLine |\n|
module subscriptionModuleDuplicateName4 'modules/subscription.bicep' = {
//@[0:6) Identifier |module|
//@[7:39) Identifier |subscriptionModuleDuplicateName4|
//@[40:68) StringComplete |'modules/subscription.bicep'|
//@[69:70) Assignment |=|
//@[71:72) LeftBrace |{|
//@[72:73) NewLine |\n|
  name: 'subscriptionModuleDuplicateName'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:41) StringComplete |'subscriptionModuleDuplicateName'|
//@[41:42) NewLine |\n|
  scope: subscription()
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:21) Identifier |subscription|
//@[21:22) LeftParen |(|
//@[22:23) RightParen |)|
//@[23:24) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:2) NewLine |\n|
module subscriptionModuleDuplicateName5 'modules/subscription.bicep' = {
//@[0:6) Identifier |module|
//@[7:39) Identifier |subscriptionModuleDuplicateName5|
//@[40:68) StringComplete |'modules/subscription.bicep'|
//@[69:70) Assignment |=|
//@[71:72) LeftBrace |{|
//@[72:73) NewLine |\n|
  name: 'subscriptionModuleDuplicateName'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:41) StringComplete |'subscriptionModuleDuplicateName'|
//@[41:42) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module resourceGroupModuleDuplicateName1 'modules/resourceGroup.bicep' = {
//@[0:6) Identifier |module|
//@[7:40) Identifier |resourceGroupModuleDuplicateName1|
//@[41:70) StringComplete |'modules/resourceGroup.bicep'|
//@[71:72) Assignment |=|
//@[73:74) LeftBrace |{|
//@[74:75) NewLine |\n|
  name: 'resourceGroupModuleDuplicateName'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:42) StringComplete |'resourceGroupModuleDuplicateName'|
//@[42:43) NewLine |\n|
  scope: resourceGroup('RG')
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:22) Identifier |resourceGroup|
//@[22:23) LeftParen |(|
//@[23:27) StringComplete |'RG'|
//@[27:28) RightParen |)|
//@[28:29) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:2) NewLine |\n|
module resourceGroupModuleDuplicateName2 'modules/resourceGroup.bicep' = {
//@[0:6) Identifier |module|
//@[7:40) Identifier |resourceGroupModuleDuplicateName2|
//@[41:70) StringComplete |'modules/resourceGroup.bicep'|
//@[71:72) Assignment |=|
//@[73:74) LeftBrace |{|
//@[74:75) NewLine |\n|
  name: 'resourceGroupModuleDuplicateName'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:42) StringComplete |'resourceGroupModuleDuplicateName'|
//@[42:43) NewLine |\n|
  scope: resourceGroup('RG')
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:22) Identifier |resourceGroup|
//@[22:23) LeftParen |(|
//@[23:27) StringComplete |'RG'|
//@[27:28) RightParen |)|
//@[28:29) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:2) NewLine |\n|

//@[0:0) EndOfFile ||
