using 'main.bicep'
//@[006:018) [BCP104 (Error)] The referenced module has errors. (bicep https://aka.ms/bicep/core-diagnostics#BCP104) |'main.bicep'|

var emptyObjVar = {}

param strParam1 = 'strParam1Value'

extensionConfig validAssignment1 with {
//@[016:032) [BCP205 (Error)] Extension "validAssignment1" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |validAssignment1|
  requiredString: 'value'
}

extensionConfig
//@[000:015) [BCP425 (Error)] The extension configuration assignment for "<missing>" does not match an extension in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP425) |extensionConfig|
//@[015:015) [BCP202 (Error)] Expected an extension alias name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP202) ||
//@[015:015) [BCP205 (Error)] Extension "<missing>" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) ||

extensionConfig incompleteAssignment1
//@[016:037) [BCP205 (Error)] Extension "incompleteAssignment1" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |incompleteAssignment1|
//@[037:037) [BCP012 (Error)] Expected the "with" keyword at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP012) ||
extensionConfig incompleteAssignment2 with
//@[016:037) [BCP205 (Error)] Extension "incompleteAssignment2" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |incompleteAssignment2|
//@[042:042) [BCP018 (Error)] Expected the "{" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

extensionConfig hasNoConfig with {}
//@[016:027) [BCP205 (Error)] Extension "hasNoConfig" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |hasNoConfig|

extensionConfig invalidSyntax1 = emptyObjVar
//@[016:030) [BCP205 (Error)] Extension "invalidSyntax1" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |invalidSyntax1|
//@[031:032) [BCP012 (Error)] Expected the "with" keyword at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP012) |=|
extensionConfig invalidSyntax2 with emptyObjVar
//@[016:030) [BCP205 (Error)] Extension "invalidSyntax2" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |invalidSyntax2|
//@[036:047) [BCP018 (Error)] Expected the "{" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) |emptyObjVar|
extensionConfig invalidSyntax3 with {
//@[016:030) [BCP205 (Error)] Extension "invalidSyntax3" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |invalidSyntax3|
  ...emptyObjVar
//@[002:016) [BCP401 (Error)] The spread operator "..." is not permitted in this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP401) |...emptyObjVar|
}

extensionConfig invalidSyntax4 with {
//@[016:030) [BCP205 (Error)] Extension "invalidSyntax4" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |invalidSyntax4|
  requiredString: validAssignment1.requiredString
}

extensionConfig invalidSyntax5 with {
//@[016:030) [BCP205 (Error)] Extension "invalidSyntax5" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |invalidSyntax5|
  ...validAssignment1
//@[002:021) [BCP401 (Error)] The spread operator "..." is not permitted in this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP401) |...validAssignment1|
}

extensionConfig invalidAssignment1 with {
//@[016:034) [BCP205 (Error)] Extension "invalidAssignment1" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |invalidAssignment1|
  requiredString: strParam1
}

extensionConfig invalidSecretAssignment1 with {
//@[016:040) [BCP205 (Error)] Extension "invalidSecretAssignment1" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |invalidSecretAssignment1|
  requiredSecureString: bool(readEnvironmentVariable('xyz', 'false')) ? az.getSecret('a', 'b', 'c', 'd') : az.getSecret('w', 'x', 'y', 'z')
//@[072:104) [BCP351 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a parameter. (bicep https://aka.ms/bicep/core-diagnostics#BCP351) |az.getSecret('a', 'b', 'c', 'd')|
//@[107:139) [BCP351 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a parameter. (bicep https://aka.ms/bicep/core-diagnostics#BCP351) |az.getSecret('w', 'x', 'y', 'z')|
}

