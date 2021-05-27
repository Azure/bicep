//Array Lookup Sample
@allowed([
  'eastus'
  'eastus2'
  'centralus'
  'northcentralus'
  'southcentralus'
  'westcentralus'
  'westus'
  'westus2'
])
param azRegion string

//Note that this should be extented to all regions
var RegionToTimezone = {
  eastus: 'Eastern Standard Time'
  eastus2: 'Eastern Standard Time'
  centralus: 'Central Standard Time'
  northcentralus: 'Central Standard Time'
  southcentralus: 'Central Standard Time'
  westcentralus: 'Mountain Standard Time'
  westus: 'Pacific Standard Time'
  westus2: 'Pacific Standard Time'
}
var timezone = RegionToTimezone['${azRegion}']

output AzRegion string = azRegion
output Timezone string = timezone
