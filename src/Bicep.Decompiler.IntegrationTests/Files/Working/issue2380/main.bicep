targetScope = 'managementGroup'

@description('EnrollmentAccount used for subscription billing')
param enrollmentAccount string

@description('BillingAccount used for subscription billing')
param billingAccount string

@description('Alias to assign to the subscription')
param subscriptionAlias string

@description('Display name for the subscription')
param subscriptionDisplayName string

@description('Workload type for the subscription')
@allowed([
  'Production'
  'DevTest'
])
param subscriptionWorkload string = 'Production'

resource subscriptionAlias_resource 'Microsoft.Subscription/aliases@2020-09-01' = {
//@[82:333) [BCP135 (Error)] Scope "managementGroup" is not valid for this resource type. Permitted scopes: "tenant". (CodeDescription: none) |{\n  name: subscriptionAlias\n  properties: {\n    workload: subscriptionWorkload\n    displayName: subscriptionDisplayName\n    billingScope: tenantResourceId('Microsoft.Billing/billingAccounts/enrollmentAccounts', billingAccount, enrollmentAccount)\n  }\n}|
  name: subscriptionAlias
  properties: {
    workload: subscriptionWorkload
    displayName: subscriptionDisplayName
    billingScope: tenantResourceId('Microsoft.Billing/billingAccounts/enrollmentAccounts', billingAccount, enrollmentAccount)
  }
}

output subscriptionId string = subscriptionAlias_resource.properties.subscriptionId
