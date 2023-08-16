test testShouldIgnoreAdditionalProperties 'samples/main.bicep' = {
//@[42:62) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/samples/main.bicep'. (CodeDescription: none) |'samples/main.bicep'|
  additionalProp: {}
}

test testShouldIgnoreAdditionalProperties2 'samples/main.bicep' = {
//@[43:63) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/samples/main.bicep'. (CodeDescription: none) |'samples/main.bicep'|
  params: {
    env: 'dev'
    suffix: 10
  }
  additionalProp: {}
}

// Skipped tests
test testNoParams 'samples/main.bicep' ={
//@[18:38) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/samples/main.bicep'. (CodeDescription: none) |'samples/main.bicep'|
  params:{}
}

test testMissingParams 'samples/main.bicep' ={
//@[23:43) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/samples/main.bicep'. (CodeDescription: none) |'samples/main.bicep'|
  params:{
    env: 'NotMain'
  }
}

test testWrongParamsType 'samples/main.bicep' ={
//@[25:45) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/samples/main.bicep'. (CodeDescription: none) |'samples/main.bicep'|
  params:{
    env: 1
    suffix: 10
  }
}

test testWrongParamsType2 'samples/main.bicep' ={
//@[26:46) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/samples/main.bicep'. (CodeDescription: none) |'samples/main.bicep'|
  params:{
    env: 'dev'
    suffix: '10'
  }
}

test testWrongParamsType3 'samples/main.bicep' ={
//@[26:46) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/samples/main.bicep'. (CodeDescription: none) |'samples/main.bicep'|
  params:{
    env: 'dev'
    suffix: 10
    location: 'westus2'
  }
}

test testInexitentParam 'samples/main.bicep' ={
//@[24:44) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/samples/main.bicep'. (CodeDescription: none) |'samples/main.bicep'|
  params:{
    env: 'dev'
    suffix: 10
    location: 1
  }
}

test testEmptyBody 'samples/main.bicep' = {}
//@[19:39) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/samples/main.bicep'. (CodeDescription: none) |'samples/main.bicep'|

// Test inexistent file

test testInexistentFile 'samples/inexistent.bicep' = {}
//@[24:50) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/samples/inexistent.bicep'. (CodeDescription: none) |'samples/inexistent.bicep'|


test sample 'samples/sample1.bicep' = {
//@[05:11) [BCP028 (Error)] Identifier "sample" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |sample|
  params: {
//@[02:08) [BCP025 (Error)] The property "params" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |params|
    location: 'westus'
  }

test sample 'samples/sample1.bicep'{
//@[05:11) [BCP018 (Error)] Expected the ":" character at this location. (CodeDescription: none) |sample|
//@[36:36) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
    params: {
//@[04:10) [BCP025 (Error)] The property "params" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |params|
      location: 'westus'
    }
  }

test sample ={
//@[05:11) [BCP028 (Error)] Identifier "sample" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |sample|
//@[12:13) [BCP0347 (Error)] Expected a test path string at this location. (CodeDescription: none) |=|
//@[12:14) [BCP358 (Error)] This declaration is missing a template file path reference. (CodeDescription: none) |={|
//@[14:14) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||
    params: {
//@[04:10) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |params|
      location: 'westus'
//@[06:14) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |location|
    }
//@[04:05) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
  }
//@[02:03) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |}|

test sample 'samples/sample1.bicep'{
//@[05:11) [BCP028 (Error)] Identifier "sample" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |sample|
//@[35:36) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) |{|
    params: {
      location: 'westus',
//@[25:25) [BCP238 (Error)] Unexpected new line character after a comma. (CodeDescription: none) ||
    }
  }

test sample{
//@[05:11) [BCP028 (Error)] Identifier "sample" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |sample|
//@[11:12) [BCP0347 (Error)] Expected a test path string at this location. (CodeDescription: none) |{|
//@[11:12) [BCP358 (Error)] This declaration is missing a template file path reference. (CodeDescription: none) |{|
//@[12:12) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||
    params: {
//@[04:10) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |params|
      location: 'westus'
//@[06:14) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |location|
    }
//@[04:05) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
  }
//@[02:03) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |}|

test sample{
//@[05:11) [BCP028 (Error)] Identifier "sample" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |sample|
//@[11:12) [BCP0347 (Error)] Expected a test path string at this location. (CodeDescription: none) |{|
//@[11:12) [BCP358 (Error)] This declaration is missing a template file path reference. (CodeDescription: none) |{|
//@[12:12) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||
    params: {
//@[04:10) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |params|
      location: 'westus',
//@[06:14) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |location|
    },
//@[04:05) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
  }
//@[02:03) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |}|

test sample{
//@[05:11) [BCP028 (Error)] Identifier "sample" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |sample|
//@[11:12) [BCP0347 (Error)] Expected a test path string at this location. (CodeDescription: none) |{|
//@[11:12) [BCP358 (Error)] This declaration is missing a template file path reference. (CodeDescription: none) |{|
//@[12:12) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||
    params: {
//@[04:10) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |params|
      location: 'westus',
//@[06:14) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |location|
      env:'prod'
//@[06:09) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |env|
    },
//@[04:05) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
  }
//@[02:03) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |}|

test 'samples/sample1.bicep'{
//@[05:28) [BCP0346 (Error)] Expected a test identifier at this location. (CodeDescription: none) |'samples/sample1.bicep'|
//@[28:29) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) |{|
    params: {
      location: 'westus',
//@[25:25) [BCP238 (Error)] Unexpected new line character after a comma. (CodeDescription: none) ||
      env:'prod'
    },
//@[06:06) [BCP238 (Error)] Unexpected new line character after a comma. (CodeDescription: none) ||
  }

test
//@[04:04) [BCP0346 (Error)] Expected a test identifier at this location. (CodeDescription: none) ||
//@[04:04) [BCP358 (Error)] This declaration is missing a template file path reference. (CodeDescription: none) ||

test sample
//@[05:11) [BCP028 (Error)] Identifier "sample" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |sample|
//@[11:11) [BCP0347 (Error)] Expected a test path string at this location. (CodeDescription: none) ||
//@[11:11) [BCP358 (Error)] This declaration is missing a template file path reference. (CodeDescription: none) ||

test sample 'samples/sample1.bicep'
//@[05:11) [BCP028 (Error)] Identifier "sample" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |sample|
//@[35:35) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

test sample 'samples/sample1.bicep' = 
//@[05:11) [BCP028 (Error)] Identifier "sample" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |sample|
//@[38:38) [BCP018 (Error)] Expected the "{" character at this location. (CodeDescription: none) ||

test sample 'samples/sample1.bicep' = {
//@[05:11) [BCP028 (Error)] Identifier "sample" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |sample|

test sample '' = {



//@[00:00) [BCP018 (Error)] Expected the "}" character at this location. (CodeDescription: none) ||
