//Define Azure Files parmeters
param storageaccountlocation string = 'westeurope'
param storageaccountName string
param storageaccountkind string
param storgeaccountglobalRedundancy string = 'Premium_LRS'
param fileshareFolderName string = 'profilecontainers'
//Concat FileShare
var filesharelocation = '${sa.name}/default/${fileshareFolderName}'

//Create Storage account
resource sa 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
  name : storageaccountName
  location : storageaccountlocation
  kind : storageaccountkind
  sku: {
    name: storgeaccountglobalRedundancy
  }
}

//Create FileShare
resource fs 'Microsoft.Storage/storageAccounts/fileServices/shares@2020-08-01-preview' = {
  name :  filesharelocation
}
