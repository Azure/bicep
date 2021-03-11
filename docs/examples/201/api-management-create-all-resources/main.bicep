param publisherEmail string
param publisherName string

@allowed([
  'Developer'
  'Standard'
  'Premium'
])
param sku string = 'Standard'

param skuCount int = 1

@secure()
param mutualAuthenticationCertificate string

param certificatePassword string
param eventHubNamespaceConnectionString string

@secure()
param googleClientSecret string

@secure()
param openIdConnectClientSecret string

param tenantPolicy string
param apiPolicy string
param operationPolicy string
param productPolicy string
param location string = resourceGroup().location

var apiManagementServiceName = 'apiservice${uniqueString(resourceGroup().id)}'

resource apiManagementService 'Microsoft.ApiManagement/service@2020-06-01-preview' = {
  name: apiManagementServiceName
  location: location
  sku: {
    name: sku
    capacity: skuCount
  }
  properties: {
    publisherName: publisherName
    publisherEmail: publisherEmail
  }
}

resource policy 'Microsoft.ApiManagement/service/policies@2020-06-01-preview' = {
  name: '${apiManagementService.name}/policy'
  properties: {
    value: tenantPolicy
  }
}

resource petStoreApiExample 'Microsoft.ApiManagement/service/apis@2020-06-01-preview' = {
  name: '${apiManagementService.name}/PetStoreSwaggerImportExample'
  properties: {
    format: 'swagger-link-json'
    value: 'http://petstore.swagger.io/v2/swagger.json'
    path: 'examplepetstore'
  }
}

resource exampleApi 'Microsoft.ApiManagement/service/apis@2020-06-01-preview' = {
  name: '${apiManagementService.name}/exampleApi'
  properties: {
    displayName: 'Example API Name'
    description: 'Description for example API'
    serviceUrl: 'https://example.net'
    path: 'exampleapipath'
    protocols: [
      'https'
    ]
  }
}

resource exampleApiOperationDelete 'Microsoft.ApiManagement/service/apis/operations@2020-06-01-preview' = {
  name: '${exampleApi.name}/exampleOperationsDELETE'
  properties: {
    displayName: 'DELETE resource'
    method: 'DELETE'
    urlTemplate: '/resource'
    description: 'A demonstration of a DELETE call'
  }
}

resource exampleApiOperationGet 'Microsoft.ApiManagement/service/apis/operations@2020-06-01-preview' = {
  name: '${exampleApi.name}/exampleOperationsGET'
  properties: {
    displayName: 'GET resource'
    method: 'GET'
    urlTemplate: '/resource'
    description: 'A demonstration of a GET call'
  }
}

resource exampleApiOperationGetPolicy 'Microsoft.ApiManagement/service/apis/operations/policies@2020-06-01-preview' = {
  name: '${exampleApiOperationGet.name}/policy'
  properties: {
    value: operationPolicy
  }
}

resource exampleApiWithPolicy 'Microsoft.ApiManagement/service/apis@2020-06-01-preview' = {
  name: '${apiManagementService.name}/exampleApiWithPolicy'
  properties: {
    displayName: 'Example API Name with policy'
    description: 'Description for example API with policy'
    serviceUrl: 'https://examplewithpolicy.net'
    path: 'exampleapipolicypath'
    protocols: [
      'https'
    ]
  }
}

resource exampleApiWithPolicyPolicy 'Microsoft.ApiManagement/service/apis/operations/policies@2020-06-01-preview' = {
  name: '${exampleApiWithPolicy.name}/policy'
  properties: {
    value: apiPolicy
  }
}

resource exampleProduct 'Microsoft.ApiManagement/service/products@2020-06-01-preview' = {
  name: '${apiManagementService.name}/exampleProduct'
  properties: {
    displayName: 'Example Product name'
    description: 'Description for example product'
    terms: 'Terms for example product'
    subscriptionRequired: true
    approvalRequired: false
    subscriptionsLimit: 1
    state: 'published'
  }
}

resource exampleProductApi 'Microsoft.ApiManagement/service/apis@2020-06-01-preview' = {
  name: '${exampleProduct.name}/exampleApi'
}

resource exampleProductPolicy 'Microsoft.ApiManagement/service/products/policies@2020-06-01-preview' = {
  name: '${exampleProduct.name}/policy'
  properties: {
    value: productPolicy
  }
}

resource exampleUser1 'Microsoft.ApiManagement/service/users@2020-06-01-preview' = {
  name: '${apiManagementService.name}/exampleUser1'
  properties: {
    firstName: 'ExampleFirstName1'
    lastName: 'ExampleLastName1'
    email: 'examplefirst1@example.com'
    state: 'active'
    note: 'note for example user 1'
  }
}

resource exampleUser2 'Microsoft.ApiManagement/service/users@2020-06-01-preview' = {
  name: '${apiManagementService.name}/exampleUser2'
  properties: {
    firstName: 'ExampleFirstName2'
    lastName: 'ExampleLastName2'
    email: 'examplefirst2@example.com'
    state: 'active'
    note: 'note for example user 2'
  }
}

resource exampleUser3 'Microsoft.ApiManagement/service/users@2020-06-01-preview' = {
  name: '${apiManagementService.name}/exampleUser1'
  properties: {
    firstName: 'ExampleFirstName3'
    lastName: 'ExampleLastName3'
    email: 'examplefirst3@example.com'
    state: 'active'
    note: 'note for example user 3'
  }
}

resource exampleProperty 'Microsoft.ApiManagement/service/properties@2019-01-01' = {
  name: '${apiManagementService.name}/exampleproperties'
  properties: {
    displayName: 'propertyExampleName'
    value: 'propertyExampleValue'
    tags: [
      'exampleTag'
    ]
  }
}

resource subscription1 'Microsoft.ApiManagement/service/subscriptions@2017-03-01' = {
  name: '${apiManagementService.name}/examplesubscription1'
  properties: {
    displayName: 'examplesubscription1'
    productId: exampleProduct.id
    userId: exampleUser1.id
  }
}

resource subscription2 'Microsoft.ApiManagement/service/subscriptions@2017-03-01' = {
  name: '${apiManagementService.name}/examplesubscription2'
  properties: {
    displayName: 'examplesubscription2'
    productId: exampleProduct.id
    userId: exampleUser3.id
  }
}

resource certificate 'Microsoft.ApiManagement/service/certificates@2020-06-01-preview' = {
  name: '${apiManagementService.name}/exampleCertificate'
  properties: {
    data: mutualAuthenticationCertificate
    password: certificatePassword
  }
}

resource exampleGroup 'Microsoft.ApiManagement/service/groups@2020-06-01-preview' = {
  name: '${apiManagementService.name}/exampleGroup'
  properties: {
    displayName: 'Example Group Name'
    description: 'Example group description'
  }
}

resource openIdConnectProvider 'Microsoft.ApiManagement/service/openidConnectProviders@2020-06-01-preview' = {
  name: '${apiManagementService.name}/exampleOpenIdConnectProvider'
  properties: {
    displayName: 'exampleOpenIdConnectProviderName'
    description: 'Description for example OpenId Connect provider'
    metadataEndpoint: 'https://example-openIdConnect-url.net'
    clientId: 'exampleClientId'
    clientSecret: openIdConnectClientSecret
  }
}

resource exampleLogger 'Microsoft.ApiManagement/service/loggers@2020-06-01-preview' = {
  name: '${apiManagementService.name}/exampleLogger'
  properties: {
    loggerType: 'azureEventHub'
    description: 'Description for example logger'
    credentials: {
      name: 'exampleEventHubName'
      eventHubNamespaceConnectionString: eventHubNamespaceConnectionString
    }
  }
}

resource identityProvider 'Microsoft.ApiManagement/service/identityProviders@2020-06-01-preview' = {
  name: any('${apiManagementService.name}/google')
  properties: {
    clientId: 'googleClientId'
    clientSecret: googleClientSecret
  }
  dependsOn: [
    apiManagementService
  ]
}
