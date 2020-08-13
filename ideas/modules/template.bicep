input namePrefix string
input location string

// modules can be deployed using 'resource module' instead of 'resource azrm'
resource module './module@vmWithDiags' vmWithDiags {
  // inputs must be fulfilled
  input.namePrefix: input.namePrefix
  input.location: input.location
  // any module property can be overriden optionally
  mainVm.properties.hardwareProfile: {
    vmSize: 'Standard_A1_v2'
  }
}

// module outputs can be accessed
output diagsUrl: vmWithDiags.outputs.blobUrl