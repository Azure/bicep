testsample'samples/sample1.bicep'={
  params: {
    location: 'westus'
  }
}

// Test the main file
testtestMain'samples/main.bicep'={
  params: {
    env: 'prod'
    suffix: 1
  }
}

testtestMain2'samples/main.bicep'={
  params: {
    env: 'dev'
    suffix: 10
  }
}

testtestMain2'samples/main.bicep'={
  params: {
    env: 'main'
    suffix: 10
  }
}

testtestMain3'samples/main.bicep'={
  params: {
    env: 'NotMain'
    suffix: 10
  }
}

// Skipped tests
testtestNoParams'samples/main.bicep'={
  params: {}
}

testtestMissingParams'samples/main.bicep'={
  params: {
    env: 'NotMain'
  }
}

testtestWrongParamsType'samples/main.bicep'={
  params: {
    env: 1
    suffix: 10
  }
}

testtestWrongParamsType2'samples/main.bicep'={
  params: {
    env: 'dev'
    suffix: '10'
  }
}

testtestWrongParamsType3'samples/main.bicep'={
  params: {
    env: 'dev'
    suffix: 10
    location: 'westus2'
  }
}

testtestInexitentParam'samples/main.bicep'={
  params: {
    env: 'dev'
    suffix: 10
    location: 1
  }
}

testtestEmptyBody'samples/main.bicep'={}

testtestShouldIgnoreAdditionalProperties'samples/main.bicep'={
  additionalProp: {}
}

testtestShouldIgnoreAdditionalProperties2'samples/main.bicep'={
  params: {
    env: 'dev'
    suffix: 10
  }
  additionalProp: {}
}

// Test the development file

testtestDev'samples/development.bicep'={
  params: {
    location: 'westus3'
  }
}

// Test the broken file

testtestBroken'samples/broken.bicep'={
  params: {
    location: 'us'
  }
}

// Test the file trying to access a resource

testtestResource2'samples/AccessResource.bicep'={
  params: {
    location: 'westus2'
  }
}

// Test the file trying to access runtime functions
testtestRuntime'samples/runtime.bicep'={}

// Test inexistent file

testtestInexistentFile'samples/inexistent.bicep'={}
