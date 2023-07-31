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
  params: {  
    env: 'dev'
    suffix: 10
  }
}

test testMain2 'samples/main.bicep' = {  
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
}

test testMissingParams 'samples/main.bicep' ={
  params:{
    env: 'NotMain'
  }
}

test testWrongParamsType 'samples/main.bicep' ={
  params:{
    env: 1
    suffix: 10
  }
}

test testWrongParamsType2 'samples/main.bicep' ={
  params:{
    env: 'dev'
    suffix: '10'
  }
}

test testWrongParamsType3 'samples/main.bicep' ={
  params:{
    env: 'dev'
    suffix: 10
    location: 'westus2'
  }
}

test testInexitentParam 'samples/main.bicep' ={
  params:{
    env: 'dev'
    suffix: 10
    location: 1
  }
}

test testEmptyBody 'samples/main.bicep' = {}

test testShouldIgnoreAdditionalProperties 'samples/main.bicep' = {
  additionalProp: {}
}

test testShouldIgnoreAdditionalProperties2 'samples/main.bicep' = {
  params: {
    env: 'dev'
    suffix: 10
  }
  additionalProp: {}
}

// Test the development file

test testDev 'samples/development.bicep' = {
  params: {
    location: 'westus3'
  }
}

// Test the file trying to access a resource

test testResource2 'samples/AccessResource.bicep' = {
  params: {
    location: 'westus2'
  }
}


// Test the file trying to access runtime functions
test testRuntime 'samples/runtime.bicep' = {}

// Test inexistent file

test testInexistentFile 'samples/inexistent.bicep' = {}

