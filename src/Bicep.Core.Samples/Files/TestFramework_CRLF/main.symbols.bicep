test sample 'samples/sample1.bicep' = {
//@[5:11) Test sample. Type: test. Declaration start char: 0, length: 84
  params: {
    location: 'westus'
  }
}

// Test the main file
test testMain 'samples/main.bicep' = {  
//@[5:13) Test testMain. Type: test. Declaration start char: 0, length: 95
  params: {  
    env: 'prod'
    suffix: 1
  }
}

test testMain2 'samples/main.bicep' = {  
//@[5:14) Test testMain2. Type: test. Declaration start char: 0, length: 96
  params: {  
    env: 'dev'
    suffix: 10
  }
}

test testMain21 'samples/main.bicep' = {  
//@[5:15) Test testMain21. Type: test. Declaration start char: 0, length: 98
  params: {  
    env: 'main'
    suffix: 10
  }
}

test testMain3 'samples/main.bicep' = {  
//@[5:14) Test testMain3. Type: test. Declaration start char: 0, length: 100
  params: {  
    env: 'NotMain'
    suffix: 10
  }
}

// Test the development file
test testDev 'samples/development.bicep' = {
//@[5:12) Test testDev. Type: test. Declaration start char: 0, length: 90
  params: {
    location: 'westus3'
  }
}

// Test the file trying to access a resource

test testResource2 'samples/AccessResources.bicep' = {
//@[5:18) Test testResource2. Type: test. Declaration start char: 0, length: 100
  params: {
    location: 'westus2'
  }
}


// Test the file trying to access runtime functions
test testRuntime 'samples/runtime.bicep' = {}
//@[5:16) Test testRuntime. Type: test. Declaration start char: 0, length: 45


