// $1 = logicAppConnector
// $2 = 'name'
// $3 = 'logicAppConnectorApi'

resource logicAppConnector 'Microsoft.Web/connections@2016-06-01' = {
  name: 'name'
  location: 'logicAppConnectorApi'
//@[12:34) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'logicAppConnectorApi' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'logicAppConnectorApi'|
  properties: {
    api: any({
      id: subscriptionResourceId('Microsoft.Web/locations/managedApis', resourceGroup().location, 'logicAppConnectorApi')
//@[72:96) [no-loc-expr-outside-params (Warning)] Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-loc-expr-outside-params)) |resourceGroup().location|
    })
  }
}
// Insert snippet here

