resource ag 'Microsoft.DesktopVirtualization/applicationgroups@2019-12-10-preview' = {
  name: 'testAppGroup'
  location: resourceGroup().location
  properties: {
//@[2:12) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "hostPoolArmPath". |properties|
    friendlyName: 'testFriendlyName'
    applicationGroupType: 'Desktop'
  }
}

