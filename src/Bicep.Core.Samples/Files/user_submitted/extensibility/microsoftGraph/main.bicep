extension microsoftGraphV1_0
extension microsoftGraphBeta

param appRoleId string = 'bc76c90e-eb7f-4a29-943b-49e88762d09d'
param scopeId string = 'f761933c-643b-424f-a169-f9313d23a913'

resource resourceApp 'Microsoft.Graph/applications@beta' = {
  uniqueName: 'resourceApp'
  displayName: 'My Resource App'
  appRoles: [
    {
      id: appRoleId
      allowedMemberTypes: ['User', 'Application']
      description: 'Resource app role'
      displayName: 'ResourceApp.Read.All'
      value: 'ResourceApp.Read.All'
      isEnabled: true
    }
  ]
  api: {
    oauth2PermissionScopes: [
      {
        id: scopeId
        type: 'Admin'
        adminConsentDescription: 'Description of the resource scope'
        adminConsentDisplayName: 'ResourceScope.Read.All'
        value: 'ResourceScope.Read.All'
        isEnabled: true
      }
    ]
  }
}

resource resourceSp 'Microsoft.Graph/servicePrincipals@beta' = {
  appId: resourceApp.appId
}

resource clientApp 'Microsoft.Graph/applications@beta' = {
  uniqueName: 'clientApp'
  displayName: 'My Client App'
}

resource clientSp 'Microsoft.Graph/servicePrincipals@beta' = {
  appId: clientApp.appId
}

resource permissionGrant 'Microsoft.Graph/oauth2PermissionGrants@beta' = {
  clientId: clientSp.id
  consentType: 'AllPrincipals'
  resourceId: resourceSp.id
  scope: 'ResourceScope.Read.All'
}

resource appRoleAssignedTo 'Microsoft.Graph/appRoleAssignedTo@beta' = {
  appRoleId: appRoleId
  principalId: clientSp.id
  resourceId: resourceSp.id
}

resource group 'Microsoft.Graph/groups@beta' = {
  uniqueName: 'myGroup'
  displayName: 'My Group'
  mailEnabled: false
  mailNickname: 'myGroupMailNickname'
  securityEnabled: false
  groupTypes: ['Unified']
  owners: [resourceSp.id]
}

resource appV1 'Microsoft.Graph/applications@v1.0' = {
  displayName: 'TestAppV1'
  uniqueName: 'testAppV1'

  resource myTestFIC 'federatedIdentityCredentials' = {
    name: '${appV1.uniqueName}/mytestfic'
    audiences: ['audience']
    description: 'My test fic'
    issuer: 'issuer'
    subject: 'subject'
  }
}
