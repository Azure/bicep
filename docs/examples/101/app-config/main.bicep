param configStoreName string = 'myConfigStore'
param location string = resourceGroup().location

// Specifies the names of the key-value resources. 
param keyValueNames array = [
  'myKey'
  'myKey2$myLabel' // The name is a combination of key and label with $ as delimiter. The label is optional.
]

// Specifies the values of the key-value resources. It's optional
param keyValueValues array = [
  'key-value without label'
  'key-value with label'
]

param contentType string = 'the-content-type' // suprised this isn't an enum value?

resource config 'Microsoft.AppConfiguration/configurationStores@2020-07-01-preview' = {
  name: configStoreName
  location: location
  sku: {
    name: 'Standard'
  }
}

resource key1 'Microsoft.AppConfiguration/configurationStores/keyValues@2020-07-01-preview' = {
  name: '${config.name}/${keyValueNames[0]}'
  properties: {
    value: keyValueValues[0]
    contentType: contentType
  }
}

resource key2 'Microsoft.AppConfiguration/configurationStores/keyValues@2020-07-01-preview' = {
  name: '${config.name}/${keyValueNames[1]}'
  properties: {
    value: keyValueValues[1]
    contentType: contentType
  }
}

output keys object = key1 // keys are *NOT* secret
