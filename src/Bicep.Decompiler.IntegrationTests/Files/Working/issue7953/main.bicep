@description('Deployment Location')
param location string = resourceGroup().location

@description('The base URI where artifacts required by this template are located including a trailing \'/\'')
param _artifactsLocation string = deployment().properties.templateLink.uri

@description('The sasToken required to access _artifactsLocation.')
@secure()
param _artifactsLocationSasToken string = ''

output location string = location
output _artifactsLocation string = _artifactsLocation
output _artifactsLocationSasToken string = _artifactsLocationSasToken
