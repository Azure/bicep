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

// Skipped tests
test testNoParams 'samples/main.bicep' = {
  params: {}
}

test testMissingParams 'samples/main.bicep' = {
  params: {
    env: 'NotMain'
  }
}

test testWrongParamsType 'samples/main.bicep' = {
  params: {
    env: 1
    suffix: 10
  }
}

test testWrongParamsType2 'samples/main.bicep' = {
  params: {
    env: 'dev'
    suffix: '10'
  }
}

test testWrongParamsType3 'samples/main.bicep' = {
  params: {
    env: 'dev'
    suffix: 10
    location: 'westus2'
  }
}

test testInexitentParam 'samples/main.bicep' = {
  params: {
    env: 'dev'
    suffix: 10
    location: 1
  }
}

var tryToAssign = testInexitentParam

test testEmptyBody 'samples/main.bicep' = {}

// Test inexistent file

test testInexistentFile 'samples/inexistent.bicep' = {}

test sample 'samples/sample1.bicep' = {
  params: {
    location: 'westus'
  }

test sample 'samples/sample1.bicep'{
    params: {
      location: 'westus'
    }
  }

test sample ={
params: {
location: 'westus'
}
}

test sample 'samples/sample1.bicep'{
    params: {
      location: 'westus',
    }
  }

test sample{
params: {
location: 'westus'
}
}

test sample{
params: {
location: 'westus',
},
}

test sample{
params: {
location: 'westus',
env:'prod'
},
}

test 'samples/sample1.bicep'{
    params: {
      location: 'westus',
      env:'prod'
    },
  }

test

test sample

test sample 'samples/sample1.bicep'

test sample 'samples/sample1.bicep' =

test sample 'samples/sample1.bicep' = {

test sample '' = {
