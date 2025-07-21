using 'main.bicep'

var strVar1 = 'strVar1Value'
//@[04:11) Variable strVar1. Type: 'strVar1Value'. Declaration start char: 0, length: 28
param strParam1 = 'strParam1Value'
//@[06:15) ParameterAssignment strParam1. Type: 'strParam1Value'. Declaration start char: 0, length: 34
param secureStrParam1 = az.getSecret('p', 'r', 'a', 'm')
//@[06:21) ParameterAssignment secureStrParam1. Type: string. Declaration start char: 0, length: 56

extensionConfig hasObjConfig1 with {
//@[16:29) ExtensionConfigAssignment hasObjConfig1. Type: config. Declaration start char: 0, length: 74
  requiredString: 'valueFromParams'
}

extensionConfig hasObjConfig2 with {
//@[16:29) ExtensionConfigAssignment hasObjConfig2. Type: config. Declaration start char: 0, length: 67
  optionalString: 'optional'
}

extensionConfig hasObjConfig3 with {}
//@[16:29) ExtensionConfigAssignment hasObjConfig3. Type: config. Declaration start char: 0, length: 37

// hasObjConfig4 not here to test assignment is not required because required field is defaulted

extensionConfig hasObjConfig5 with {
//@[16:29) ExtensionConfigAssignment hasObjConfig5. Type: config. Declaration start char: 0, length: 152
  requiredString: strVar1
  optionalString: bool(readEnvironmentVariable('xyz', 'false')) ? 'inlineVal' : strVar1
}


extensionConfig hasSecureConfig1 with {
//@[16:32) ExtensionConfigAssignment hasSecureConfig1. Type: config. Declaration start char: 0, length: 147
  requiredSecureString: az.getSecret('00000000-0000-0000-0000-000000000001', 'mock-rg', 'kv', 'mySecret')
}

extensionConfig hasSecureConfig2 with {
//@[16:32) ExtensionConfigAssignment hasSecureConfig2. Type: config. Declaration start char: 0, length: 111
  requiredSecureString: 'Inlined'
  optionalString: 'valueFromParams'
}

extensionConfig hasSecureConfig3 with {
//@[16:32) ExtensionConfigAssignment hasSecureConfig3. Type: config. Declaration start char: 0, length: 73
  requiredSecureString: strVar1
}

extensionConfig hasSecureConfig4 with {
//@[16:32) ExtensionConfigAssignment hasSecureConfig4. Type: config. Declaration start char: 0, length: 103
  requiredSecureString: strParam1
  optionalString: strParam1
}

extensionConfig hasDiscrimConfig1 with {
//@[16:33) ExtensionConfigAssignment hasDiscrimConfig1. Type: config. Declaration start char: 0, length: 81
  discrim: 'a'
  z1: 'z1v'
  a1: 'a1v'
}

extensionConfig hasDiscrimConfig2 with {
//@[16:33) ExtensionConfigAssignment hasDiscrimConfig2. Type: a. Declaration start char: 0, length: 54
  a1: 'a1v'
}

extensionConfig hasDiscrimConfig3 with {
//@[16:33) ExtensionConfigAssignment hasDiscrimConfig3. Type: config. Declaration start char: 0, length: 132
  discrim: 'b'
  b1: bool(readEnvironmentVariable('xyz', 'false')) ? 'b1True' : 'b1False'
}

