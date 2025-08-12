using 'main.bicep'
//@[06:018) [BCP424 (Error)] The following extensions are declared in the Bicep file but are missing a configuration assignment in the params files: "missingConfigAssignment1". (bicep https://aka.ms/bicep/core-diagnostics#BCP424) |'main.bicep'|

var emptyObjVar = {}
param strParam1 = 'strParam1Value'
var strVar1 = 'strVar1Value'
param secureStrParam1 = az.getSecret('a', 'b', 'c', 'param')
var secureStrVar1 = az.getSecret('a', 'b', 'c', 'var')

extensionConfig validAssignment1 with {
  requiredString: 'value'
}

extensionConfig validSecretAssignment1 with {
  requiredSecureString: az.getSecret('a', 'b', 'c', 'valid')
}

param invalidParamAssignment1 = validAssignment1.requiredString
//@[32:048) [BCP063 (Error)] The name "validAssignment1" is not a parameter, variable, resource or module. (bicep https://aka.ms/bicep/core-diagnostics#BCP063) |validAssignment1|
param invalidParamAssignment2 = validAssignment1
//@[32:048) [BCP063 (Error)] The name "validAssignment1" is not a parameter, variable, resource or module. (bicep https://aka.ms/bicep/core-diagnostics#BCP063) |validAssignment1|
param invalidParamAssignment3 = validSecretAssignment1.requiredSecureString
//@[32:054) [BCP063 (Error)] The name "validSecretAssignment1" is not a parameter, variable, resource or module. (bicep https://aka.ms/bicep/core-diagnostics#BCP063) |validSecretAssignment1|

var invalidVarAssignment1 = validAssignment1.requiredString
//@[04:025) [no-unused-vars (Warning)] Variable "invalidVarAssignment1" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |invalidVarAssignment1|
//@[28:044) [BCP063 (Error)] The name "validAssignment1" is not a parameter, variable, resource or module. (bicep https://aka.ms/bicep/core-diagnostics#BCP063) |validAssignment1|
var invalidVarAssignment2 = validAssignment1
//@[04:025) [no-unused-vars (Warning)] Variable "invalidVarAssignment2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |invalidVarAssignment2|
//@[28:044) [BCP063 (Error)] The name "validAssignment1" is not a parameter, variable, resource or module. (bicep https://aka.ms/bicep/core-diagnostics#BCP063) |validAssignment1|
var invalidVarAssignment3 = validSecretAssignment1.requiredSecureString
//@[04:025) [no-unused-vars (Warning)] Variable "invalidVarAssignment3" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |invalidVarAssignment3|
//@[28:050) [BCP063 (Error)] The name "validSecretAssignment1" is not a parameter, variable, resource or module. (bicep https://aka.ms/bicep/core-diagnostics#BCP063) |validSecretAssignment1|

extensionConfig
//@[00:015) [BCP425 (Error)] The extension configuration assignment for "<missing>" does not match an extension in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP425) |extensionConfig|
//@[15:015) [BCP202 (Error)] Expected an extension alias name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP202) ||
//@[15:015) [BCP205 (Error)] Extension "<missing>" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) ||

extensionConfig incompleteAssignment1
//@[37:037) [BCP012 (Error)] Expected the "with" keyword at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP012) ||
extensionConfig incompleteAssignment2 with
//@[42:042) [BCP018 (Error)] Expected the "{" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

extensionConfig hasNoConfig with {}
//@[16:027) [BCP205 (Error)] Extension "hasNoConfig" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |hasNoConfig|

extensionConfig invalidSyntax1 = emptyObjVar
//@[31:032) [BCP012 (Error)] Expected the "with" keyword at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP012) |=|
extensionConfig invalidSyntax2 with emptyObjVar
//@[36:047) [BCP018 (Error)] Expected the "{" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) |emptyObjVar|
extensionConfig invalidSyntax3 with {
  ...emptyObjVar
//@[02:016) [BCP401 (Error)] The spread operator "..." is not permitted in this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP401) |...emptyObjVar|
}

extensionConfig invalidSyntax4 with {
  requiredString: validAssignment1.requiredString
//@[18:034) [BCP063 (Error)] The name "validAssignment1" is not a parameter, variable, resource or module. (bicep https://aka.ms/bicep/core-diagnostics#BCP063) |validAssignment1|
//@[18:049) [BCP338 (Error)] Failed to evaluate extension config "invalidSyntax4": Cannot emit unexpected expression of type ExtensionConfigAssignmentReferenceExpression (bicep https://aka.ms/bicep/core-diagnostics#BCP338) |validAssignment1.requiredString|
}

extensionConfig invalidSyntax5 with {
  ...validAssignment1
//@[02:021) [BCP401 (Error)] The spread operator "..." is not permitted in this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP401) |...validAssignment1|
//@[05:021) [BCP063 (Error)] The name "validAssignment1" is not a parameter, variable, resource or module. (bicep https://aka.ms/bicep/core-diagnostics#BCP063) |validAssignment1|
}

extensionConfig invalidAssignment1 with {
  requiredString: strParam1
}

extensionConfig invalidAssignment2 with {
  requiredString: strVar1
}

extensionConfig invalidSecretAssignment1 with {
  requiredSecureString: bool(readEnvironmentVariable('xyz', 'false')) ? az.getSecret('a', 'b', 'c', 'd') : az.getSecret('w', 'x', 'y', 'z')
//@[24:139) [BCP338 (Error)] Failed to evaluate extension config "invalidSecretAssignment1": Cannot emit unexpected expression of type ParameterKeyVaultReferenceExpression (bicep https://aka.ms/bicep/core-diagnostics#BCP338) |bool(readEnvironmentVariable('xyz', 'false')) ? az.getSecret('a', 'b', 'c', 'd') : az.getSecret('w', 'x', 'y', 'z')|
//@[24:139) [stacks-extensibility-compat (Info)] Secure config property values must be a key vault reference to be valid for Deployment stack deployments. (bicep core linter https://aka.ms/bicep/linter-diagnostics#stacks-extensibility-compat) |bool(readEnvironmentVariable('xyz', 'false')) ? az.getSecret('a', 'b', 'c', 'd') : az.getSecret('w', 'x', 'y', 'z')|
}

extensionConfig invalidSecretAssignment2 with {
  requiredSecureString: secureStrParam1
//@[24:039) [BCP338 (Error)] Failed to evaluate extension config "invalidSecretAssignment2": Unhandled exception during evaluating template language function 'parameters' is not handled. (bicep https://aka.ms/bicep/core-diagnostics#BCP338) |secureStrParam1|
//@[24:039) [stacks-extensibility-compat (Info)] Secure config property values must be a key vault reference to be valid for Deployment stack deployments. (bicep core linter https://aka.ms/bicep/linter-diagnostics#stacks-extensibility-compat) |secureStrParam1|
  optionalString: secureStrParam1
//@[18:033) [BCP338 (Error)] Failed to evaluate extension config "invalidSecretAssignment2": Unhandled exception during evaluating template language function 'parameters' is not handled. (bicep https://aka.ms/bicep/core-diagnostics#BCP338) |secureStrParam1|
}

extensionConfig invalidSecretAssignment3 with {
  requiredSecureString: secureStrVar1
//@[24:037) [BCP338 (Error)] Failed to evaluate extension config "invalidSecretAssignment3": Unhandled exception during evaluating template language function 'variables' is not handled. (bicep https://aka.ms/bicep/core-diagnostics#BCP338) |secureStrVar1|
//@[24:037) [stacks-extensibility-compat (Info)] Secure config property values must be a key vault reference to be valid for Deployment stack deployments. (bicep core linter https://aka.ms/bicep/linter-diagnostics#stacks-extensibility-compat) |secureStrVar1|
  optionalString: secureStrVar1
//@[18:031) [BCP338 (Error)] Failed to evaluate extension config "invalidSecretAssignment3": Unhandled exception during evaluating template language function 'variables' is not handled. (bicep https://aka.ms/bicep/core-diagnostics#BCP338) |secureStrVar1|
}

extensionConfig invalidDiscrimAssignment1 with {
//@[47:103) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "b1". (bicep https://aka.ms/bicep/core-diagnostics#BCP035) |{\n  discrim: 'a' // this property cannot be reassigned\n}|
  discrim: 'a' // this property cannot be reassigned
//@[02:009) [BCP037 (Error)] The property "discrim" is not allowed on objects of type "b". Permissible properties include "b1", "z1". (bicep https://aka.ms/bicep/core-diagnostics#BCP037) |discrim|
}

