test sample 'samples/sample1.bicep' = {
//@[5:11) Test sample. Type: module. Declaration start char: 0, length: 84
  params: {
    location: 'westus'
  }
}


// Test the main file
test testMain 'samples/main.bicep' = {  
//@[5:13) Test testMain. Type: module. Declaration start char: 0, length: 95
  params: {  
    env: 'prod'
    suffix: 1
  }
}

test testMain2 'samples/main.bicep' = {  
//@[5:14) Test testMain2. Type: module. Declaration start char: 0, length: 96
  params: {  
    env: 'dev'
    suffix: 10
  }
}

test testMain2 'samples/main.bicep' = {  
//@[5:14) Test testMain2. Type: module. Declaration start char: 0, length: 97
  params: {  
    env: 'main'
    suffix: 10
  }
}

test testMain3 'samples/main.bicep' = {  
//@[5:14) Test testMain3. Type: module. Declaration start char: 0, length: 100
  params: {  
    env: 'NotMain'
    suffix: 10
  }
}

// Skipped tests
test testNoParams 'samples/main.bicep' ={
//@[5:17) Test testNoParams. Type: module. Declaration start char: 0, length: 57
  params:{}
}

test testMissingParams 'samples/main.bicep' ={
//@[5:22) Test testMissingParams. Type: module. Declaration start char: 0, length: 86
  params:{
    env: 'NotMain'
  }
}

test testWrongParamsType 'samples/main.bicep' ={
//@[5:24) Test testWrongParamsType. Type: module. Declaration start char: 0, length: 96
  params:{
    env: 1
    suffix: 10
  }
}

test testWrongParamsType2 'samples/main.bicep' ={
//@[5:25) Test testWrongParamsType2. Type: module. Declaration start char: 0, length: 103
  params:{
    env: 'dev'
    suffix: '10'
  }
}

test testWrongParamsType3 'samples/main.bicep' ={
//@[5:25) Test testWrongParamsType3. Type: module. Declaration start char: 0, length: 126
  params:{
    env: 'dev'
    suffix: 10
    location: 'westus2'
  }
}

test testInexitentParam 'samples/main.bicep' ={
//@[5:23) Test testInexitentParam. Type: module. Declaration start char: 0, length: 116
  params:{
    env: 'dev'
    suffix: 10
    location: 1
  }
}

test testEmptyBody 'samples/main.bicep' = {}
//@[5:18) Test testEmptyBody. Type: module. Declaration start char: 0, length: 44

test testShouldIgnoreAdditionalProperties 'samples/main.bicep' = {
//@[5:41) Test testShouldIgnoreAdditionalProperties. Type: module. Declaration start char: 0, length: 91
  additionalProp: {}
}

test testShouldIgnoreAdditionalProperties2 'samples/main.bicep' = {
//@[5:42) Test testShouldIgnoreAdditionalProperties2. Type: module. Declaration start char: 0, length: 142
  params: {
    env: 'dev'
    suffix: 10
  }
  additionalProp: {}
}

// Test the development file

test testDev 'samples/development.bicep' = {
//@[5:12) Test testDev. Type: module. Declaration start char: 0, length: 90
  params: {
    location: 'westus3'
  }
}

// Test the file trying to access a resource

test testResource2 'samples/AccessResource.bicep' = {
//@[5:18) Test testResource2. Type: error. Declaration start char: 0, length: 99
  params: {
    location: 'westus2'
  }
}


// Test the file trying to access runtime functions
test testRuntime 'samples/runtime.bicep' = {}
//@[5:16) Test testRuntime. Type: module. Declaration start char: 0, length: 45

// Test inexistent file

test testInexistentFile 'samples/inexistent.bicep' = {}
//@[5:23) Test testInexistentFile. Type: error. Declaration start char: 0, length: 55

