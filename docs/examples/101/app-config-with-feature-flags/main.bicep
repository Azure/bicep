param location string = resourceGroup().location
param appConfigSku string = 'free'
var uniqueStr = uniqueString(resourceGroup().id)
var appConfigName = 'myappconfig${uniqueStr}'

var flag01Name = 'flag01'
var flag02Name = 'flag02'
var ffContentType = 'application/vnd.microsoft.appconfig.ff+json;charset=utf-8'

//the feature flag resources are embedded because of the bicep naming rules 
//for names that don't match the segment length, it appears the feature flag resource 
//name requires an additional '/' which has to be represented with the '~2F'
resource appConfig 'Microsoft.AppConfiguration/configurationStores@2021-03-01-preview'={
  location: location
  name: appConfigName
  sku: {
    name: appConfigSku
  }
  resource flag01 'keyValues@2021-03-01-preview' = {
    name: '.appconfig.featureflag~2F${flag01Name}'
    properties:{
      value: '{	"id": "${flag01Name}",	"description": "",	"enabled": false,	"conditions": {		"client_filters": []	}}'
      contentType: ffContentType
    }
  }
  resource flag02 'keyValues@2021-03-01-preview' = {
    name: '.appconfig.featureflag~2F${flag02Name}'
    properties:{
      value: '{	"id": "${flag02Name}",	"description": "",	"enabled": false,	"conditions": {		"client_filters": []	}}'
      contentType: ffContentType
    }
  }
}

output connstr string = appConfig.listKeys().value[0].ConnectionString
