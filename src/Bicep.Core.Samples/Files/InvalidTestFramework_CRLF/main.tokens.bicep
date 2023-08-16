test testShouldIgnoreAdditionalProperties 'samples/main.bicep' = {
//@[00:04) Identifier |test|
//@[05:41) Identifier |testShouldIgnoreAdditionalProperties|
//@[42:62) StringComplete |'samples/main.bicep'|
//@[63:64) Assignment |=|
//@[65:66) LeftBrace |{|
//@[66:68) NewLine |\r\n|
  additionalProp: {}
//@[02:16) Identifier |additionalProp|
//@[16:17) Colon |:|
//@[18:19) LeftBrace |{|
//@[19:20) RightBrace |}|
//@[20:22) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:05) NewLine |\r\n\r\n|

test testShouldIgnoreAdditionalProperties2 'samples/main.bicep' = {
//@[00:04) Identifier |test|
//@[05:42) Identifier |testShouldIgnoreAdditionalProperties2|
//@[43:63) StringComplete |'samples/main.bicep'|
//@[64:65) Assignment |=|
//@[66:67) LeftBrace |{|
//@[67:69) NewLine |\r\n|
  params: {
//@[02:08) Identifier |params|
//@[08:09) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
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
  additionalProp: {}
//@[02:16) Identifier |additionalProp|
//@[16:17) Colon |:|
//@[18:19) LeftBrace |{|
//@[19:20) RightBrace |}|
//@[20:22) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:05) NewLine |\r\n\r\n|

// Skipped tests
//@[16:18) NewLine |\r\n|
test testNoParams 'samples/main.bicep' ={
//@[00:04) Identifier |test|
//@[05:17) Identifier |testNoParams|
//@[18:38) StringComplete |'samples/main.bicep'|
//@[39:40) Assignment |=|
//@[40:41) LeftBrace |{|
//@[41:43) NewLine |\r\n|
  params:{}
//@[02:08) Identifier |params|
//@[08:09) Colon |:|
//@[09:10) LeftBrace |{|
//@[10:11) RightBrace |}|
//@[11:13) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:05) NewLine |\r\n\r\n|

test testMissingParams 'samples/main.bicep' ={
//@[00:04) Identifier |test|
//@[05:22) Identifier |testMissingParams|
//@[23:43) StringComplete |'samples/main.bicep'|
//@[44:45) Assignment |=|
//@[45:46) LeftBrace |{|
//@[46:48) NewLine |\r\n|
  params:{
//@[02:08) Identifier |params|
//@[08:09) Colon |:|
//@[09:10) LeftBrace |{|
//@[10:12) NewLine |\r\n|
    env: 'NotMain'
//@[04:07) Identifier |env|
//@[07:08) Colon |:|
//@[09:18) StringComplete |'NotMain'|
//@[18:20) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:05) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:05) NewLine |\r\n\r\n|

test testWrongParamsType 'samples/main.bicep' ={
//@[00:04) Identifier |test|
//@[05:24) Identifier |testWrongParamsType|
//@[25:45) StringComplete |'samples/main.bicep'|
//@[46:47) Assignment |=|
//@[47:48) LeftBrace |{|
//@[48:50) NewLine |\r\n|
  params:{
//@[02:08) Identifier |params|
//@[08:09) Colon |:|
//@[09:10) LeftBrace |{|
//@[10:12) NewLine |\r\n|
    env: 1
//@[04:07) Identifier |env|
//@[07:08) Colon |:|
//@[09:10) Integer |1|
//@[10:12) NewLine |\r\n|
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

test testWrongParamsType2 'samples/main.bicep' ={
//@[00:04) Identifier |test|
//@[05:25) Identifier |testWrongParamsType2|
//@[26:46) StringComplete |'samples/main.bicep'|
//@[47:48) Assignment |=|
//@[48:49) LeftBrace |{|
//@[49:51) NewLine |\r\n|
  params:{
//@[02:08) Identifier |params|
//@[08:09) Colon |:|
//@[09:10) LeftBrace |{|
//@[10:12) NewLine |\r\n|
    env: 'dev'
//@[04:07) Identifier |env|
//@[07:08) Colon |:|
//@[09:14) StringComplete |'dev'|
//@[14:16) NewLine |\r\n|
    suffix: '10'
//@[04:10) Identifier |suffix|
//@[10:11) Colon |:|
//@[12:16) StringComplete |'10'|
//@[16:18) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:05) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:05) NewLine |\r\n\r\n|

test testWrongParamsType3 'samples/main.bicep' ={
//@[00:04) Identifier |test|
//@[05:25) Identifier |testWrongParamsType3|
//@[26:46) StringComplete |'samples/main.bicep'|
//@[47:48) Assignment |=|
//@[48:49) LeftBrace |{|
//@[49:51) NewLine |\r\n|
  params:{
//@[02:08) Identifier |params|
//@[08:09) Colon |:|
//@[09:10) LeftBrace |{|
//@[10:12) NewLine |\r\n|
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
//@[01:05) NewLine |\r\n\r\n|

test testInexitentParam 'samples/main.bicep' ={
//@[00:04) Identifier |test|
//@[05:23) Identifier |testInexitentParam|
//@[24:44) StringComplete |'samples/main.bicep'|
//@[45:46) Assignment |=|
//@[46:47) LeftBrace |{|
//@[47:49) NewLine |\r\n|
  params:{
//@[02:08) Identifier |params|
//@[08:09) Colon |:|
//@[09:10) LeftBrace |{|
//@[10:12) NewLine |\r\n|
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
    location: 1
//@[04:12) Identifier |location|
//@[12:13) Colon |:|
//@[14:15) Integer |1|
//@[15:17) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:05) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:05) NewLine |\r\n\r\n|

test testEmptyBody 'samples/main.bicep' = {}
//@[00:04) Identifier |test|
//@[05:18) Identifier |testEmptyBody|
//@[19:39) StringComplete |'samples/main.bicep'|
//@[40:41) Assignment |=|
//@[42:43) LeftBrace |{|
//@[43:44) RightBrace |}|
//@[44:48) NewLine |\r\n\r\n|

// Test inexistent file
//@[23:27) NewLine |\r\n\r\n|

test testInexistentFile 'samples/inexistent.bicep' = {}
//@[00:04) Identifier |test|
//@[05:23) Identifier |testInexistentFile|
//@[24:50) StringComplete |'samples/inexistent.bicep'|
//@[51:52) Assignment |=|
//@[53:54) LeftBrace |{|
//@[54:55) RightBrace |}|
//@[55:61) NewLine |\r\n\r\n\r\n|


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
//@[03:07) NewLine |\r\n\r\n|

test sample 'samples/sample1.bicep'{
//@[00:04) Identifier |test|
//@[05:11) Identifier |sample|
//@[12:35) StringComplete |'samples/sample1.bicep'|
//@[35:36) LeftBrace |{|
//@[36:38) NewLine |\r\n|
    params: {
//@[04:10) Identifier |params|
//@[10:11) Colon |:|
//@[12:13) LeftBrace |{|
//@[13:15) NewLine |\r\n|
      location: 'westus'
//@[06:14) Identifier |location|
//@[14:15) Colon |:|
//@[16:24) StringComplete |'westus'|
//@[24:26) NewLine |\r\n|
    }
//@[04:05) RightBrace |}|
//@[05:07) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:07) NewLine |\r\n\r\n|

test sample ={
//@[00:04) Identifier |test|
//@[05:11) Identifier |sample|
//@[12:13) Assignment |=|
//@[13:14) LeftBrace |{|
//@[14:16) NewLine |\r\n|
    params: {
//@[04:10) Identifier |params|
//@[10:11) Colon |:|
//@[12:13) LeftBrace |{|
//@[13:15) NewLine |\r\n|
      location: 'westus'
//@[06:14) Identifier |location|
//@[14:15) Colon |:|
//@[16:24) StringComplete |'westus'|
//@[24:26) NewLine |\r\n|
    }
//@[04:05) RightBrace |}|
//@[05:07) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:07) NewLine |\r\n\r\n|

test sample 'samples/sample1.bicep'{
//@[00:04) Identifier |test|
//@[05:11) Identifier |sample|
//@[12:35) StringComplete |'samples/sample1.bicep'|
//@[35:36) LeftBrace |{|
//@[36:38) NewLine |\r\n|
    params: {
//@[04:10) Identifier |params|
//@[10:11) Colon |:|
//@[12:13) LeftBrace |{|
//@[13:15) NewLine |\r\n|
      location: 'westus',
//@[06:14) Identifier |location|
//@[14:15) Colon |:|
//@[16:24) StringComplete |'westus'|
//@[24:25) Comma |,|
//@[25:27) NewLine |\r\n|
    }
//@[04:05) RightBrace |}|
//@[05:07) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:07) NewLine |\r\n\r\n|

test sample{
//@[00:04) Identifier |test|
//@[05:11) Identifier |sample|
//@[11:12) LeftBrace |{|
//@[12:14) NewLine |\r\n|
    params: {
//@[04:10) Identifier |params|
//@[10:11) Colon |:|
//@[12:13) LeftBrace |{|
//@[13:15) NewLine |\r\n|
      location: 'westus'
//@[06:14) Identifier |location|
//@[14:15) Colon |:|
//@[16:24) StringComplete |'westus'|
//@[24:26) NewLine |\r\n|
    }
//@[04:05) RightBrace |}|
//@[05:07) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:07) NewLine |\r\n\r\n|

test sample{
//@[00:04) Identifier |test|
//@[05:11) Identifier |sample|
//@[11:12) LeftBrace |{|
//@[12:14) NewLine |\r\n|
    params: {
//@[04:10) Identifier |params|
//@[10:11) Colon |:|
//@[12:13) LeftBrace |{|
//@[13:15) NewLine |\r\n|
      location: 'westus',
//@[06:14) Identifier |location|
//@[14:15) Colon |:|
//@[16:24) StringComplete |'westus'|
//@[24:25) Comma |,|
//@[25:27) NewLine |\r\n|
    },
//@[04:05) RightBrace |}|
//@[05:06) Comma |,|
//@[06:08) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:07) NewLine |\r\n\r\n|

test sample{
//@[00:04) Identifier |test|
//@[05:11) Identifier |sample|
//@[11:12) LeftBrace |{|
//@[12:14) NewLine |\r\n|
    params: {
//@[04:10) Identifier |params|
//@[10:11) Colon |:|
//@[12:13) LeftBrace |{|
//@[13:15) NewLine |\r\n|
      location: 'westus',
//@[06:14) Identifier |location|
//@[14:15) Colon |:|
//@[16:24) StringComplete |'westus'|
//@[24:25) Comma |,|
//@[25:27) NewLine |\r\n|
      env:'prod'
//@[06:09) Identifier |env|
//@[09:10) Colon |:|
//@[10:16) StringComplete |'prod'|
//@[16:18) NewLine |\r\n|
    },
//@[04:05) RightBrace |}|
//@[05:06) Comma |,|
//@[06:08) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:07) NewLine |\r\n\r\n|

test 'samples/sample1.bicep'{
//@[00:04) Identifier |test|
//@[05:28) StringComplete |'samples/sample1.bicep'|
//@[28:29) LeftBrace |{|
//@[29:31) NewLine |\r\n|
    params: {
//@[04:10) Identifier |params|
//@[10:11) Colon |:|
//@[12:13) LeftBrace |{|
//@[13:15) NewLine |\r\n|
      location: 'westus',
//@[06:14) Identifier |location|
//@[14:15) Colon |:|
//@[16:24) StringComplete |'westus'|
//@[24:25) Comma |,|
//@[25:27) NewLine |\r\n|
      env:'prod'
//@[06:09) Identifier |env|
//@[09:10) Colon |:|
//@[10:16) StringComplete |'prod'|
//@[16:18) NewLine |\r\n|
    },
//@[04:05) RightBrace |}|
//@[05:06) Comma |,|
//@[06:08) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:07) NewLine |\r\n\r\n|

test
//@[00:04) Identifier |test|
//@[04:08) NewLine |\r\n\r\n|

test sample
//@[00:04) Identifier |test|
//@[05:11) Identifier |sample|
//@[11:15) NewLine |\r\n\r\n|

test sample 'samples/sample1.bicep'
//@[00:04) Identifier |test|
//@[05:11) Identifier |sample|
//@[12:35) StringComplete |'samples/sample1.bicep'|
//@[35:39) NewLine |\r\n\r\n|

test sample 'samples/sample1.bicep' = 
//@[00:04) Identifier |test|
//@[05:11) Identifier |sample|
//@[12:35) StringComplete |'samples/sample1.bicep'|
//@[36:37) Assignment |=|
//@[38:42) NewLine |\r\n\r\n|

test sample 'samples/sample1.bicep' = {
//@[00:04) Identifier |test|
//@[05:11) Identifier |sample|
//@[12:35) StringComplete |'samples/sample1.bicep'|
//@[36:37) Assignment |=|
//@[38:39) LeftBrace |{|
//@[39:43) NewLine |\r\n\r\n|

test sample '' = {
//@[00:04) Identifier |test|
//@[05:11) Identifier |sample|
//@[12:14) StringComplete |''|
//@[15:16) Assignment |=|
//@[17:18) LeftBrace |{|
//@[18:24) NewLine |\r\n\r\n\r\n|



//@[00:00) EndOfFile ||
