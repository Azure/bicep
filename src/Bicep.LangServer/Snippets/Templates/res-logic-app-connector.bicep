﻿// Logic App Connector
resource ${1:logicAppConnector} 'Microsoft.Web/connections@2015-08-01-preview' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    name: ${3:'name'}
    api: {
      id: subscriptionResourceId('Microsoft.Web/locations/managedApis', resourceGroup().location, ${4:'logicAppConnectorApi'})
    }
  }
}
