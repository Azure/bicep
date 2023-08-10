// Test the main file
test testMain 'main.bicep' = {  
  params: {  
    env: 'prod'
    suffix: 1
  }
}

test testMain2 'main.bicep' = {  
  params: {  
    env: 'dev'
    suffix: 10
  }
}

test testMain2 'main.bicep' = {  
  params: {  
    env: 'main'
    suffix: 10
  }
}

test testMain3 'main.bicep' = {  
  params: {  
    env: 'NotMain'
    suffix: 10
  }
}

// Skipped tests
test testNoParams 'main.bicep' ={
  params:{}
}

test testMissingParams 'main.bicep' ={
  params:{
    env: 'NotMain'
  }
}

test testWrongParamsType 'main.bicep' ={
  params:{
    env: 1
    suffix: 10
  }
}

test testWrongParamsType2 'main.bicep' ={
  params:{
    env: 'dev'
    suffix: '10'
  }
}

test testWrongParamsType3 'main.bicep' ={
  params:{
    env: 'dev'
    suffix: 10
    location: 'westus2'
  }
}

test testInexitentParam 'main.bicep' ={
  params:{
    env: 'dev'
    suffix: 10
    location: 1
  }
}

test testEmptyBody 'main.bicep' = {}

test testShouldIgnoreAdditionalProperties 'main.bicep' = {
  additionalProp: {}
}

test testShouldIgnoreAdditionalProperties2 'main.bicep' = {
  params: {
    env: 'dev'
    suffix: 10
  }
  additionalProp: {}
}

// Test the development file

test testDev 'development.bicep' = {
  params: {
    location: 'westus3'
  }
}

// Test the broken file

// test testBroken 'broken.bicep' = {
//   params: {
//     location: 'us'
//   }
// }

// Test the file trying to access a resource

test testResource2 'AccessResource.bicep' = {
  params: {
    location: 'westus2'
  }
}


// Test the file trying to access runtime functions
test testRuntime 'runtime.bicep' = {}

// Test inexistent file

test testInexistentFile 'inexistent.bicep' = {}
