// Resource Group
resource /*${1:resourceGroup}*/resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3|'eastasia','southeastasia','centralus','eastus','eastus2','westus','northcentralus','southcentralus','northeurope','westeurope','japanwest','japaneast','brazilsouth','australiaeast','australiasoutheast','southindia','centralindia','westindia','canadacentral','canadaeast','uksouth','ukwest','westcentralus','westus2','koreacentral','koreasouth','francecentral','francesouth','australiacentral','australiacentral2','uaecentral','uaenorth','southafricanorth','southafricawest','switzerlandnorth','switzerlandwest','germanynorth','germanywestcentral','norwaywest','norwayeast','brazilsoutheast','westus3','swedencentral'|}*/'location'
  tags:{
    'tag': 'tagValue'   
  }
}
