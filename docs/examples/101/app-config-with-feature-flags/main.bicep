param location string = resourceGroup().location
param appConfigSku string = 'free'
var uniquestr = uniqueString(resourceGroup().id)
var appconfigName = 'myappconfig${uniquestr}'

var flag01name = 'flag01'
var flag02name = 'flag02'
var ffcontenttype = 'application/vnd.microsoft.appconfig.ff+json;charset=utf-8'

//the feature flag resources are embedded because of the bicep naming rules 
//for names that don't match the segment length, it appears the feature flag resource 
//name requires an additional '/' which has to be represented with the '~2F'
resource appconfig 'Microsoft.AppConfiguration/configurationStores@2021-03-01-preview'={
  location: location
  name: appconfigName
  sku: {
    name: appConfigSku
  }
  resource flag01 'keyValues@2021-03-01-preview' = {
    name: '.appconfig.featureflag~2F${flag01name}'
    properties:{
      value: '{	"id": "${flag01name}",	"description": "",	"enabled": false,	"conditions": {		"client_filters": []	}}'
      contentType: ffcontenttype
    }
  }
  resource flag02 'keyValues@2021-03-01-preview' = {
    name: '.appconfig.featureflag~2F${flag02name}'
    properties:{
      value: '{	"id": "${flag02name}",	"description": "",	"enabled": false,	"conditions": {		"client_filters": []	}}'
      contentType: ffcontenttype
    }
  }
}

output connstr string = appconfig.listKeys().value[0].ConnectionString
