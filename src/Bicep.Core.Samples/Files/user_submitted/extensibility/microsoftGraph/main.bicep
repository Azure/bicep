import 'microsoftGraph@1.0.0' as graph

param appRoleId string = 'bc76c90e-eb7f-4a29-943b-49e88762d09d'
param scopeId string = 'f761933c-643b-424f-a169-f9313d23a913'

resource resourceApp 'Microsoft.Graph/applications@beta' = {
  name: 'resourceApp'
  displayName: 'My Resource App'
  appRoles: [
    {
      id: appRoleId
      allowedMemberTypes: [ 'User', 'Application' ]
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
  name: 'clientApp'
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
  name: 'myGroup'
  displayName: 'My Group'
  mailEnabled: false
  mailNickName: 'myGroupMailNickname'
  securityEnabled: false
  groupTypes: [
    'Unified'
  ]
  owners: [
    resourceSp.id
  ]
}
