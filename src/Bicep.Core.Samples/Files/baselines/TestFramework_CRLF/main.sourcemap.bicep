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

test testMain21 'samples/main.bicep' = {  
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

// Test the development file
test testDev 'samples/development.bicep' = {
  params: {
    location: 'westus3'
  }
}

// Test the file trying to access a resource

test testResource2 'samples/AccessResources.bicep' = {
  params: {
    location: 'westus2'
  }
}


// Test the file trying to access runtime functions
test testRuntime 'samples/runtime.bicep' = {}

