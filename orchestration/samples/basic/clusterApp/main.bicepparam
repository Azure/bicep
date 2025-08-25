using 'main.bicep' with {
  mode: 'stack'
  scope: '/subscriptions/${externalInput('stack.setting', 'subscriptionId')}/resourceGroups/${externalInput('stack.setting', 'resourceGroup')}'
  name: string(externalInput('stack.setting', 'name'))
}
