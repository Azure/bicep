param storageCount int = 2

var baseName_var = 'storage${uniqueString(resourceGroup().id)}'

resource baseName 'Microsoft.Storage/storageAccounts@2019-04-01' = [for i in range(0, storageCount): {
  name: concat(i, baseName_var)
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'Storage'
  properties: {}
}]

output storageEndpoints array = [for i in range(0, storageCount): baseName.properties.primaryEndpoints.blob]
//@[66:74) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. |baseName|
//@[75:85) [BCP055 (Error)] Cannot access properties of type "Microsoft.Storage/storageAccounts@2019-04-01[]". An "object" type is required. |properties|
output copyIndex array = [for i in range(0, storageCount): copyIndex()]
//@[59:70) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. |copyIndex()|
output copyIndexWithInt array = [for i in range(0, storageCount): copyIndex(123)]
//@[66:80) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. |copyIndex(123)|
