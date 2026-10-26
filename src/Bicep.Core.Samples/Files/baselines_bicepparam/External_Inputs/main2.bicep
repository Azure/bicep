@export()
var vmConfig VmConfig = {
  name: 'beefyVM'
  size: 'Standard_D2s_v7'
}

@export()
var storageConfig StorageConfig = {
  name: 'beefyStorage'
  sku: 'Standard_ZRS'
}


type StorageConfig = {
  name: string
  sku: string
}

type VmConfig = {
  name: string
  size: string
}

@export()
type InfraConfig = {
  storage: StorageConfig
  vm: VmConfig
  tag: string
}
