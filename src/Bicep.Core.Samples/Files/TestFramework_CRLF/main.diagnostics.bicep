test sample 'samples/sample1.bicep' = {
  params: {
    location: 'westus'
  }
}


// Test the main file
test testMain 'samples/main.bicep' = {  
  params: {  
    env: 'prod'
    suffix: 1
  }
}

test testMain2 'samples/main.bicep' = {  
//@[05:14) [BCP028 (Error)] Identifier "testMain2" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |testMain2|
  params: {  
    env: 'dev'
    suffix: 10
  }
}

test testMain2 'samples/main.bicep' = {  
//@[05:14) [BCP028 (Error)] Identifier "testMain2" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |testMain2|
  params: {  
    env: 'main'
    suffix: 10
  }
}

test testMain3 'samples/main.bicep' = {  
  params: {  
    env: 'NotMain'
    suffix: 10
  }
}

// Skipped tests
test testNoParams 'samples/main.bicep' ={
  params:{}
//@[02:08) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "env", "suffix". (CodeDescription: none) |params|
}

test testMissingParams 'samples/main.bicep' ={
  params:{
//@[02:08) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "suffix". (CodeDescription: none) |params|
    env: 'NotMain'
  }
}

test testWrongParamsType 'samples/main.bicep' ={
  params:{
    env: 1
//@[09:10) [BCP036 (Error)] The property "env" expected a value of type "string" but the provided value is of type "1". (CodeDescription: none) |1|
    suffix: 10
  }
}

test testWrongParamsType2 'samples/main.bicep' ={
  params:{
    env: 'dev'
    suffix: '10'
//@[12:16) [BCP036 (Error)] The property "suffix" expected a value of type "int" but the provided value is of type "'10'". (CodeDescription: none) |'10'|
  }
}

test testWrongParamsType3 'samples/main.bicep' ={
  params:{
    env: 'dev'
    suffix: 10
    location: 'westus2'
//@[04:12) [BCP037 (Error)] The property "location" is not allowed on objects of type "params". No other properties are allowed. (CodeDescription: none) |location|
  }
}

test testInexitentParam 'samples/main.bicep' ={
  params:{
    env: 'dev'
    suffix: 10
    location: 1
//@[04:12) [BCP037 (Error)] The property "location" is not allowed on objects of type "params". No other properties are allowed. (CodeDescription: none) |location|
  }
}

test testEmptyBody 'samples/main.bicep' = {}
//@[05:18) [BCP035 (Error)] The specified "test" declaration is missing the following required properties: "params". (CodeDescription: none) |testEmptyBody|

test testShouldIgnoreAdditionalProperties 'samples/main.bicep' = {
//@[05:41) [BCP035 (Error)] The specified "test" declaration is missing the following required properties: "params". (CodeDescription: none) |testShouldIgnoreAdditionalProperties|
  additionalProp: {}
//@[02:16) [BCP037 (Error)] The property "additionalProp" is not allowed on objects of type "module". Permissible properties include "params". (CodeDescription: none) |additionalProp|
}

test testShouldIgnoreAdditionalProperties2 'samples/main.bicep' = {
  params: {
    env: 'dev'
    suffix: 10
  }
  additionalProp: {}
//@[02:16) [BCP037 (Error)] The property "additionalProp" is not allowed on objects of type "module". No other properties are allowed. (CodeDescription: none) |additionalProp|
}

// Test the development file

test testDev 'samples/development.bicep' = {
  params: {
    location: 'westus3'
  }
}

// Test the file trying to access a resource

test testResource2 'samples/AccessResource.bicep' = {
//@[19:49) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/samples/AccessResource.bicep'. (CodeDescription: none) |'samples/AccessResource.bicep'|
  params: {
    location: 'westus2'
  }
}


// Test the file trying to access runtime functions
test testRuntime 'samples/runtime.bicep' = {}

// Test inexistent file

test testInexistentFile 'samples/inexistent.bicep' = {}
//@[24:50) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/samples/inexistent.bicep'. (CodeDescription: none) |'samples/inexistent.bicep'|

