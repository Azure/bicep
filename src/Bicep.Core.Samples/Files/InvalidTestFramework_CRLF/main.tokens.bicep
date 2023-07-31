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
//@[18:20) NewLine |\r\n|

//@[00:00) EndOfFile ||
