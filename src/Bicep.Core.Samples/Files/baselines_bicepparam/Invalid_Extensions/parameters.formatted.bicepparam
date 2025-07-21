using 'main.bicep'

var emptyObjVar = {}
param strParam1 = 'strParam1Value'
var strVar1 = 'strVar1Value'
param secureStrParam1 = az.getSecret('a', 'b', 'c', 'param')
var secureStrVar1 = az.getSecret('a', 'b', 'c', 'var')

extensionConfig validAssignment1 with {
  requiredString: 'value'
}

extensionConfig

extensionConfig incompleteAssignment1
extensionConfig incompleteAssignment2 with

extensionConfig hasNoConfig with {}

extensionConfig invalidSyntax1 = emptyObjVar
extensionConfig invalidSyntax2 with emptyObjVar
extensionConfig invalidSyntax3 with { ...emptyObjVar }

extensionConfig invalidSyntax4 with {
  requiredString: validAssignment1.requiredString
}

extensionConfig invalidSyntax5 with { ...validAssignment1 }

extensionConfig invalidAssignment1 with {
  requiredString: strParam1
}

extensionConfig invalidAssignment2 with {
  requiredString: strVar1
}

extensionConfig invalidSecretAssignment1 with {
  requiredSecureString: bool(readEnvironmentVariable('xyz', 'false'))
    ? az.getSecret('a', 'b', 'c', 'd')
    : az.getSecret('w', 'x', 'y', 'z')
}

extensionConfig invalidSecretAssignment2 with {
  requiredSecureString: secureStrParam1
  optionalString: secureStrParam1
}

extensionConfig invalidSecretAssignment3 with {
  requiredSecureString: secureStrVar1
  optionalString: secureStrVar1
}

extensionConfig invalidDiscrimAssignment1 with {
  discrim: 'a' // this property cannot be reassigned
}
