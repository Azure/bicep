using 'main.bicep'

var emptyObjVar = {}
//@[04:15) Variable emptyObjVar. Type: object. Declaration start char: 0, length: 20

param strParam1 = 'strParam1Value'
//@[06:15) ParameterAssignment strParam1. Type: 'strParam1Value'. Declaration start char: 0, length: 34

extensionConfig validAssignment1 with {
//@[16:32) ExtensionConfigAssignment validAssignment1. Type: error. Declaration start char: 0, length: 67
  requiredString: 'value'
}

extensionConfig
//@[15:15) ExtensionConfigAssignment <missing>. Type: error. Declaration start char: 0, length: 15

extensionConfig incompleteAssignment1
//@[16:37) ExtensionConfigAssignment incompleteAssignment1. Type: error. Declaration start char: 0, length: 37
extensionConfig incompleteAssignment2 with
//@[16:37) ExtensionConfigAssignment incompleteAssignment2. Type: error. Declaration start char: 0, length: 42

extensionConfig hasNoConfig with {}
//@[16:27) ExtensionConfigAssignment hasNoConfig. Type: error. Declaration start char: 0, length: 35

extensionConfig invalidSyntax1 = emptyObjVar
//@[16:30) ExtensionConfigAssignment invalidSyntax1. Type: error. Declaration start char: 0, length: 44
extensionConfig invalidSyntax2 with emptyObjVar
//@[16:30) ExtensionConfigAssignment invalidSyntax2. Type: error. Declaration start char: 0, length: 47
extensionConfig invalidSyntax3 with {
//@[16:30) ExtensionConfigAssignment invalidSyntax3. Type: error. Declaration start char: 0, length: 56
  ...emptyObjVar
}

extensionConfig invalidSyntax4 with {
//@[16:30) ExtensionConfigAssignment invalidSyntax4. Type: error. Declaration start char: 0, length: 89
  requiredString: validAssignment1.requiredString
}

extensionConfig invalidSyntax5 with {
//@[16:30) ExtensionConfigAssignment invalidSyntax5. Type: error. Declaration start char: 0, length: 61
  ...validAssignment1
}

extensionConfig invalidAssignment1 with {
//@[16:34) ExtensionConfigAssignment invalidAssignment1. Type: error. Declaration start char: 0, length: 71
  requiredString: strParam1
}

extensionConfig invalidSecretAssignment1 with {
//@[16:40) ExtensionConfigAssignment invalidSecretAssignment1. Type: error. Declaration start char: 0, length: 189
  requiredSecureString: bool(readEnvironmentVariable('xyz', 'false')) ? az.getSecret('a', 'b', 'c', 'd') : az.getSecret('w', 'x', 'y', 'z')
}

