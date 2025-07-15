using 'main.bicep'

extensionConfig hasObjConfig1 with {
//@[16:29) ExtensionConfigAssignment hasObjConfig1. Type: error. Declaration start char: 0, length: 74
  requiredString: 'valueFromParams'
}

extensionConfig hasObjConfig2 with {
//@[16:29) ExtensionConfigAssignment hasObjConfig2. Type: error. Declaration start char: 0, length: 67
  optionalString: 'optional'
}

extensionConfig hasObjConfig3 with {}
//@[16:29) ExtensionConfigAssignment hasObjConfig3. Type: error. Declaration start char: 0, length: 37

// hasObjConfig4 not here to test assignment is not required because required field is defaulted

extensionConfig hasSecureConfig1 with {
//@[16:32) ExtensionConfigAssignment hasSecureConfig1. Type: error. Declaration start char: 0, length: 147
  requiredSecureString: az.getSecret('00000000-0000-0000-0000-000000000001', 'mock-rg', 'kv', 'mySecret')
}

extensionConfig hasSecureConfig2 with {
//@[16:32) ExtensionConfigAssignment hasSecureConfig2. Type: error. Declaration start char: 0, length: 111
  requiredSecureString: 'Inlined'
  optionalString: 'valueFromParams'
}

extensionConfig hasDiscrimConfig1 with {
//@[16:33) ExtensionConfigAssignment hasDiscrimConfig1. Type: error. Declaration start char: 0, length: 81
  discrim: 'a'
  z1: 'z1v'
  a1: 'a1v'
}

extensionConfig hasDiscrimConfig2 with {
//@[16:33) ExtensionConfigAssignment hasDiscrimConfig2. Type: error. Declaration start char: 0, length: 54
  a1: 'a1v'
}

extensionConfig hasDiscrimConfig3 with {
//@[16:33) ExtensionConfigAssignment hasDiscrimConfig3. Type: error. Declaration start char: 0, length: 69
  discrim: 'b'
  b1: 'b1v'
}

