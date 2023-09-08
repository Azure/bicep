param env string
param suffix int

var name = '${env}-solution-app'
var nameWithSuffix = '${name}-${suffix}'
var location = (env == 'prod' || env == 'main') ? 'eastus' : 'westus'



// output supported array = pickZones('Microsoft.Compute', 'virtualMachines', 'westus2')

// assert supportedV = supported == ['1', '2', '3']

// Name asserts
assert nameIsCorrect = name == 'dev-solution-app' || name == 'prod-solution-app'
assert nameHasValidEnv = contains(name, 'prod') || contains(name, 'dev') || contains(name, 'main')
assert nameContainsSuffix = contains(nameWithSuffix, '1') || contains(nameWithSuffix, '2') || contains(nameWithSuffix, '3')


// // Location asserts
// assert locationIsInUS = contains(location, 'us')
// assert locationIsInEastForProd = env == 'prod' && contains(location, 'east')


