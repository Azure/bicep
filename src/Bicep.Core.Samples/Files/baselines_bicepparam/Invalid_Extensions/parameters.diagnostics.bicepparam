using 'main.bicep'
//@[06:018) [BCP424 (Error)] The following extensions are declared in the Bicep file but are missing a configuration assignment in the params files: "missingConfigAssignment1". (bicep https://aka.ms/bicep/core-diagnostics#BCP424) |'main.bicep'|

var emptyObjVar = {}

param strParam1 = 'strParam1Value'

extensionConfig validAssignment1 with {
  requiredString: 'value'
}

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
}

extensionConfig invalidSyntax5 with {
  ...validAssignment1
//@[02:021) [BCP401 (Error)] The spread operator "..." is not permitted in this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP401) |...validAssignment1|
//@[05:021) [BCP063 (Error)] The name "validAssignment1" is not a parameter, variable, resource or module. (bicep https://aka.ms/bicep/core-diagnostics#BCP063) |validAssignment1|
}

extensionConfig invalidAssignment1 with {
  requiredString: strParam1
}

extensionConfig invalidSecretAssignment1 with {
  requiredSecureString: bool(readEnvironmentVariable('xyz', 'false')) ? az.getSecret('a', 'b', 'c', 'd') : az.getSecret('w', 'x', 'y', 'z')
//@[24:139) [stacks-extensibility-compat (Info)] Secure config property values must be a key vault reference to be valid for Deployment stack deployments. (bicep core linter https://aka.ms/bicep/linter-diagnostics#stacks-extensibility-compat) |bool(readEnvironmentVariable('xyz', 'false')) ? az.getSecret('a', 'b', 'c', 'd') : az.getSecret('w', 'x', 'y', 'z')|
}

