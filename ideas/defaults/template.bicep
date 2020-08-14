input namePrefix string
input location string
input subnetResourceId string

resource module './defaults@basicStorage' diagsAccount {
  input.name: concat(input.namePrefix, '-sa')
  input.location: input.location
}

resource module './defaults@basicNic' myNic {
  input: {
    name: concat(input.namePrefix, '-nic')
    location: input.location
    subnet: reference(subnetResourceId)
  }
}

resource module './defaults@basicVm' myVm {
  // inputs must be fulfilled
  input: {
    name: concat(input.namePrefix, '-vm')
    location: input.location
    adminUsername: 'myUsername'
    adminPassword: 'myPassword'
    storageAccount: diagsAccount
    subnet: reference(subnetResourceId)
  }
  
  // any module property can be overriden optionally
  mainVm.properties.hardwareProfile: {
    vmSize: 'Standard_A1_v2'
  }
}

output diagsUrl: diagsAccount.blobUrl