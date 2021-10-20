A parameter is a "location parameter" if:
        1) it's used inside a resource's top-level 'location' property (what abou deeper such properties?)
        2) ?? it has a default value that uses resourceGroup().location or deployment().location

# Principles
1: BP: don't use hard-coded strings for locations ('location' property of a resource)
EXCEPTION: 'global'
  What it means: check value of all 'location' properties of resources, if it is a literal **or a variable that is a literal**, that's a failure.

  ISSUE: Does this apply to param? (we think No)
  I.e.:
  param location = 'westus'
    BAD:
        resource storageaccount2 'Microsoft.Storage/storageAccounts@2021-02-01' = {
        location: 'uswest'
        }

  ALSO BAD:
    var location = 'westus'
    resource storageaccount... {
        location: location

2: Don't use resourcGroup().location/deployment().location directly except in default value of a parameter
  OK: Using deployment().location for a deployments resource location?  (not a scenario to encourage in bicep, does this apply to modules?)

   BAD: var location = resourceGroup().location
         ISSUE: What if rule #3 is disabled?
   BAD: resource { location = resourceGroup().location; }
   ? BAD: resource { location = resourceGroup().location; }
   OK:
        param location string = resourceGroup().location
        param location2 string = resourceGroup().location

3: BP: param instead of var for "location parameters" (NEEDED? - just a different error message?)
  Details: for a "location" parameter (for variables that are used inside a location expression, see modules)
  TODO: How does this tie in with #1?

  BAD: var location = resourceGroup().location
    suggest: param location = resourceGroup().location

    "we would recommend that you make this a param so it has a default value and is able to be modified"

4: Modules: A "location parameter" inside a module as a location must be given an explicit value when consumed (it can't be left as default).
   module1.bicep:
     param somevalue string = resourceGroup().location   <<< counts as location parameter and has a default value
     resource ... {
       location: somevalue
     }
   main.bicep:
     module m1 "module1.bicep" {
       params: {
         // BAD: "somevalue" isn't being given an explicit value
       }
     }

10: [style] One of the location parameters should be called 'location' (off by default) (??)

ISSUE: How far data flow analysis (essentially type inference?)

E.g. 
  param location1 string = resourceGroup().location   <== counts as location parameter because references resourceGroup.location
  param location2 string  =             <== counts as location parameter becaused used in assignment of resource "location" property
  var myLocation = if(condition1, location1, location2)
  resource ... {
    location: myLocation
  }
  ISSUE: What about "condition1"?
ISSUE: What are actual usage patterns?

ALLOW:
var map = [

]




GOLD STANDARD BP:
param location = resourceGroup().location
resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2021-03-01' = {
  location: location
}



BAD:

resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2021-03-01' = {
  location: 'westus'
}

BAD?
var myparam = 'westus'
resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2021-03-01' = {
  location: myparam
}

BAD?
var myparam = resourceGroup().location
resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2021-03-01' = {
  location: myparam
}

BETTER?
param myparam = 'westus'
resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2021-03-01' = {
  location: myparam
}

FINE:
param randomvar string = 'westus'

until used in 'location' property



