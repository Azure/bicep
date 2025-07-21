using 'main.bicep'

var emptyObjVar = {}
//@[04:15) Variable emptyObjVar. Type: object. Declaration start char: 0, length: 20
param strParam1 = 'strParam1Value'
//@[06:15) ParameterAssignment strParam1. Type: 'strParam1Value'. Declaration start char: 0, length: 34
var strVar1 = 'strVar1Value'
//@[04:11) Variable strVar1. Type: 'strVar1Value'. Declaration start char: 0, length: 28
param secureStrParam1 = az.getSecret('a', 'b', 'c', 'param')
//@[06:21) ParameterAssignment secureStrParam1. Type: string. Declaration start char: 0, length: 60
var secureStrVar1 = az.getSecret('a', 'b', 'c', 'var')
//@[04:17) Variable secureStrVar1. Type: string. Declaration start char: 0, length: 54

extensionConfig validAssignment1 with {
//@[16:32) ExtensionConfigAssignment validAssignment1. Type: config. Declaration start char: 0, length: 67
  requiredString: 'value'
}

extensionConfig
//@[15:15) ExtensionConfigAssignment <missing>. Type: error. Declaration start char: 0, length: 15

extensionConfig incompleteAssignment1
//@[16:37) ExtensionConfigAssignment incompleteAssignment1. Type: config. Declaration start char: 0, length: 37
extensionConfig incompleteAssignment2 with
//@[16:37) ExtensionConfigAssignment incompleteAssignment2. Type: config. Declaration start char: 0, length: 42

extensionConfig hasNoConfig with {}
//@[16:27) ExtensionConfigAssignment hasNoConfig. Type: error. Declaration start char: 0, length: 35

extensionConfig invalidSyntax1 = emptyObjVar
//@[16:30) ExtensionConfigAssignment invalidSyntax1. Type: config. Declaration start char: 0, length: 44
extensionConfig invalidSyntax2 with emptyObjVar
//@[16:30) ExtensionConfigAssignment invalidSyntax2. Type: config. Declaration start char: 0, length: 47
extensionConfig invalidSyntax3 with {
//@[16:30) ExtensionConfigAssignment invalidSyntax3. Type: config. Declaration start char: 0, length: 56
  ...emptyObjVar
}

extensionConfig invalidSyntax4 with {
//@[16:30) ExtensionConfigAssignment invalidSyntax4. Type: config. Declaration start char: 0, length: 89
  requiredString: validAssignment1.requiredString
}

extensionConfig invalidSyntax5 with {
//@[16:30) ExtensionConfigAssignment invalidSyntax5. Type: config. Declaration start char: 0, length: 61
  ...validAssignment1
}

extensionConfig invalidAssignment1 with {
//@[16:34) ExtensionConfigAssignment invalidAssignment1. Type: config. Declaration start char: 0, length: 71
  requiredString: strParam1
}

extensionConfig invalidAssignment2 with {
//@[16:34) ExtensionConfigAssignment invalidAssignment2. Type: config. Declaration start char: 0, length: 69
  requiredString: strVar1
}

extensionConfig invalidSecretAssignment1 with {
//@[16:40) ExtensionConfigAssignment invalidSecretAssignment1. Type: config. Declaration start char: 0, length: 189
  requiredSecureString: bool(readEnvironmentVariable('xyz', 'false')) ? az.getSecret('a', 'b', 'c', 'd') : az.getSecret('w', 'x', 'y', 'z')
}

extensionConfig invalidSecretAssignment2 with {
//@[16:40) ExtensionConfigAssignment invalidSecretAssignment2. Type: config. Declaration start char: 0, length: 123
  requiredSecureString: secureStrParam1
  optionalString: secureStrParam1
}

extensionConfig invalidSecretAssignment3 with {
//@[16:40) ExtensionConfigAssignment invalidSecretAssignment3. Type: config. Declaration start char: 0, length: 119
  requiredSecureString: secureStrVar1
  optionalString: secureStrVar1
}

extensionConfig invalidDiscrimAssignment1 with {
//@[16:41) ExtensionConfigAssignment invalidDiscrimAssignment1. Type: b. Declaration start char: 0, length: 103
  discrim: 'a' // this property cannot be reassigned
}

