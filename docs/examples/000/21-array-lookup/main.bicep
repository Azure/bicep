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

//Note that this could be extented to all regions
var regionToTimezone = {
  eastus: 'Eastern Standard Time'
  eastus2: 'Eastern Standard Time'
  centralus: 'Central Standard Time'
  northcentralus: 'Central Standard Time'
  southcentralus: 'Central Standard Time'
  westcentralus: 'Mountain Standard Time'
  westus: 'Pacific Standard Time'
  westus2: 'Pacific Standard Time'
}
//Lookup the timezone in the object hash
var timezone = regionToTimezone[azRegion]

output AzRegion string = azRegion
output Timezone string = timezone
