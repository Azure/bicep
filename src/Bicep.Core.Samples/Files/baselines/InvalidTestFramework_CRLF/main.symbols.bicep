test testShouldIgnoreAdditionalProperties 'samples/main.bicep' = {
//@[5:41) Test testShouldIgnoreAdditionalProperties. Type: error. Declaration start char: 0, length: 91
  additionalProp: {}
}

test testShouldIgnoreAdditionalProperties2 'samples/main.bicep' = {
//@[5:42) Test testShouldIgnoreAdditionalProperties2. Type: error. Declaration start char: 0, length: 142
  params: {
    env: 'dev'
    suffix: 10
  }
  additionalProp: {}
}

// Skipped tests
test testNoParams 'samples/main.bicep' ={
//@[5:17) Test testNoParams. Type: error. Declaration start char: 0, length: 57
  params:{}
}

test testMissingParams 'samples/main.bicep' ={
//@[5:22) Test testMissingParams. Type: error. Declaration start char: 0, length: 86
  params:{
    env: 'NotMain'
  }
}

test testWrongParamsType 'samples/main.bicep' ={
//@[5:24) Test testWrongParamsType. Type: error. Declaration start char: 0, length: 96
  params:{
    env: 1
    suffix: 10
  }
}

test testWrongParamsType2 'samples/main.bicep' ={
//@[5:25) Test testWrongParamsType2. Type: error. Declaration start char: 0, length: 103
  params:{
    env: 'dev'
    suffix: '10'
  }
}

test testWrongParamsType3 'samples/main.bicep' ={
//@[5:25) Test testWrongParamsType3. Type: error. Declaration start char: 0, length: 126
  params:{
    env: 'dev'
    suffix: 10
    location: 'westus2'
  }
}

test testInexitentParam 'samples/main.bicep' ={
//@[5:23) Test testInexitentParam. Type: error. Declaration start char: 0, length: 116
  params:{
    env: 'dev'
    suffix: 10
    location: 1
  }
}

var tryToAssign = testInexitentParam
//@[4:15) Variable tryToAssign. Type: error. Declaration start char: 0, length: 36

test testEmptyBody 'samples/main.bicep' = {}
//@[5:18) Test testEmptyBody. Type: error. Declaration start char: 0, length: 44

// Test inexistent file

test testInexistentFile 'samples/inexistent.bicep' = {}
//@[5:23) Test testInexistentFile. Type: error. Declaration start char: 0, length: 55


test sample 'samples/sample1.bicep' = {
//@[5:11) Test sample. Type: test. Declaration start char: 0, length: 174
  params: {
    location: 'westus'
  }

test sample 'samples/sample1.bicep'{
    params: {
      location: 'westus'
    }
  }

test sample ={
//@[5:11) Test sample. Type: error. Declaration start char: 0, length: 14
    params: {
      location: 'westus'
    }
  }

test sample 'samples/sample1.bicep'{
//@[5:11) Test sample. Type: test. Declaration start char: 0, length: 90
    params: {
      location: 'westus',
    }
  }

test sample{
//@[5:11) Test sample. Type: error. Declaration start char: 0, length: 12
    params: {
      location: 'westus'
    }
  }

test sample{
//@[5:11) Test sample. Type: error. Declaration start char: 0, length: 12
    params: {
      location: 'westus',
    },
  }

test sample{
//@[5:11) Test sample. Type: error. Declaration start char: 0, length: 12
    params: {
      location: 'westus',
      env:'prod'
    },
  }

test 'samples/sample1.bicep'{
//@[5:05) Test <missing>. Type: test. Declaration start char: 0, length: 102
    params: {
      location: 'westus',
      env:'prod'
    },
  }

test
//@[4:04) Test <missing>. Type: error. Declaration start char: 0, length: 4

test sample
//@[5:11) Test sample. Type: error. Declaration start char: 0, length: 11

test sample 'samples/sample1.bicep'
//@[5:11) Test sample. Type: test. Declaration start char: 0, length: 35

test sample 'samples/sample1.bicep' = 
//@[5:11) Test sample. Type: test. Declaration start char: 0, length: 38

test sample 'samples/sample1.bicep' = {
//@[5:11) Test sample. Type: test. Declaration start char: 0, length: 67

test sample '' = {



