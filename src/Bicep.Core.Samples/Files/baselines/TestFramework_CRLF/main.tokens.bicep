test sample 'samples/sample1.bicep' = {
//@[00:04) Identifier |test|
//@[05:11) Identifier |sample|
//@[12:35) StringComplete |'samples/sample1.bicep'|
//@[36:37) Assignment |=|
//@[38:39) LeftBrace |{|
//@[39:41) NewLine |\r\n|
  params: {
//@[02:08) Identifier |params|
//@[08:09) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
    location: 'westus'
//@[04:12) Identifier |location|
//@[12:13) Colon |:|
//@[14:22) StringComplete |'westus'|
//@[22:24) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:05) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:05) NewLine |\r\n\r\n|

// Test the main file
//@[21:23) NewLine |\r\n|
test testMain 'samples/main.bicep' = {  
//@[00:04) Identifier |test|
//@[05:13) Identifier |testMain|
//@[14:34) StringComplete |'samples/main.bicep'|
//@[35:36) Assignment |=|
//@[37:38) LeftBrace |{|
//@[40:42) NewLine |\r\n|
  params: {  
//@[02:08) Identifier |params|
//@[08:09) Colon |:|
//@[10:11) LeftBrace |{|
//@[13:15) NewLine |\r\n|
    env: 'prod'
//@[04:07) Identifier |env|
//@[07:08) Colon |:|
//@[09:15) StringComplete |'prod'|
//@[15:17) NewLine |\r\n|
    suffix: 1
//@[04:10) Identifier |suffix|
//@[10:11) Colon |:|
//@[12:13) Integer |1|
//@[13:15) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:05) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:05) NewLine |\r\n\r\n|

test testMain2 'samples/main.bicep' = {  
//@[00:04) Identifier |test|
//@[05:14) Identifier |testMain2|
//@[15:35) StringComplete |'samples/main.bicep'|
//@[36:37) Assignment |=|
//@[38:39) LeftBrace |{|
//@[41:43) NewLine |\r\n|
  params: {  
//@[02:08) Identifier |params|
//@[08:09) Colon |:|
//@[10:11) LeftBrace |{|
//@[13:15) NewLine |\r\n|
    env: 'dev'
//@[04:07) Identifier |env|
//@[07:08) Colon |:|
//@[09:14) StringComplete |'dev'|
//@[14:16) NewLine |\r\n|
    suffix: 10
//@[04:10) Identifier |suffix|
//@[10:11) Colon |:|
//@[12:14) Integer |10|
//@[14:16) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:05) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:05) NewLine |\r\n\r\n|

test testMain21 'samples/main.bicep' = {  
//@[00:04) Identifier |test|
//@[05:15) Identifier |testMain21|
//@[16:36) StringComplete |'samples/main.bicep'|
//@[37:38) Assignment |=|
//@[39:40) LeftBrace |{|
//@[42:44) NewLine |\r\n|
  params: {  
//@[02:08) Identifier |params|
//@[08:09) Colon |:|
//@[10:11) LeftBrace |{|
//@[13:15) NewLine |\r\n|
    env: 'main'
//@[04:07) Identifier |env|
//@[07:08) Colon |:|
//@[09:15) StringComplete |'main'|
//@[15:17) NewLine |\r\n|
    suffix: 10
//@[04:10) Identifier |suffix|
//@[10:11) Colon |:|
//@[12:14) Integer |10|
//@[14:16) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:05) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:05) NewLine |\r\n\r\n|

test testMain3 'samples/main.bicep' = {  
//@[00:04) Identifier |test|
//@[05:14) Identifier |testMain3|
//@[15:35) StringComplete |'samples/main.bicep'|
//@[36:37) Assignment |=|
//@[38:39) LeftBrace |{|
//@[41:43) NewLine |\r\n|
  params: {  
//@[02:08) Identifier |params|
//@[08:09) Colon |:|
//@[10:11) LeftBrace |{|
//@[13:15) NewLine |\r\n|
    env: 'NotMain'
//@[04:07) Identifier |env|
//@[07:08) Colon |:|
//@[09:18) StringComplete |'NotMain'|
//@[18:20) NewLine |\r\n|
    suffix: 10
//@[04:10) Identifier |suffix|
//@[10:11) Colon |:|
//@[12:14) Integer |10|
//@[14:16) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:05) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:05) NewLine |\r\n\r\n|

// Test the development file
//@[28:30) NewLine |\r\n|
test testDev 'samples/development.bicep' = {
//@[00:04) Identifier |test|
//@[05:12) Identifier |testDev|
//@[13:40) StringComplete |'samples/development.bicep'|
//@[41:42) Assignment |=|
//@[43:44) LeftBrace |{|
//@[44:46) NewLine |\r\n|
  params: {
//@[02:08) Identifier |params|
//@[08:09) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
    location: 'westus3'
//@[04:12) Identifier |location|
//@[12:13) Colon |:|
//@[14:23) StringComplete |'westus3'|
//@[23:25) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:05) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:05) NewLine |\r\n\r\n|

// Test the file trying to access a resource
//@[44:48) NewLine |\r\n\r\n|

test testResource2 'samples/AccessResources.bicep' = {
//@[00:04) Identifier |test|
//@[05:18) Identifier |testResource2|
//@[19:50) StringComplete |'samples/AccessResources.bicep'|
//@[51:52) Assignment |=|
//@[53:54) LeftBrace |{|
//@[54:56) NewLine |\r\n|
  params: {
//@[02:08) Identifier |params|
//@[08:09) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
    location: 'westus2'
//@[04:12) Identifier |location|
//@[12:13) Colon |:|
//@[14:23) StringComplete |'westus2'|
//@[23:25) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:05) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:07) NewLine |\r\n\r\n\r\n|


// Test the file trying to access runtime functions
//@[51:53) NewLine |\r\n|
test testRuntime 'samples/runtime.bicep' = {}
//@[00:04) Identifier |test|
//@[05:16) Identifier |testRuntime|
//@[17:40) StringComplete |'samples/runtime.bicep'|
//@[41:42) Assignment |=|
//@[43:44) LeftBrace |{|
//@[44:45) RightBrace |}|
//@[45:49) NewLine |\r\n\r\n|


//@[00:00) EndOfFile ||
