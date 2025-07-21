using 'main.bicep'
//@[006:018) [BCP104 (Error)] The referenced module has errors. (bicep https://aka.ms/bicep/core-diagnostics#BCP104) |'main.bicep'|

var emptyObjVar = {}

param strParam1 = 'strParam1Value'

extensionConfig validAssignment1 with {
//@[000:067) [BCP425 (Error)] The extension configuration assignment for "validAssignment1" does not match an extension in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP425) |extensionConfig validAssignment1 with {\n  requiredString: 'value'\n}|
//@[016:032) [BCP205 (Error)] Extension "validAssignment1" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |validAssignment1|
  requiredString: 'value'
}

extensionConfig
//@[000:015) [BCP425 (Error)] The extension configuration assignment for "<missing>" does not match an extension in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP425) |extensionConfig|
//@[015:015) [BCP202 (Error)] Expected an extension alias name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP202) ||
//@[015:015) [BCP205 (Error)] Extension "<missing>" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) ||

extensionConfig incompleteAssignment1
//@[000:037) [BCP425 (Error)] The extension configuration assignment for "incompleteAssignment1" does not match an extension in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP425) |extensionConfig incompleteAssignment1|
//@[016:037) [BCP205 (Error)] Extension "incompleteAssignment1" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |incompleteAssignment1|
//@[037:037) [BCP012 (Error)] Expected the "with" keyword at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP012) ||
extensionConfig incompleteAssignment2 with
//@[000:042) [BCP425 (Error)] The extension configuration assignment for "incompleteAssignment2" does not match an extension in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP425) |extensionConfig incompleteAssignment2 with|
//@[016:037) [BCP205 (Error)] Extension "incompleteAssignment2" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |incompleteAssignment2|
//@[042:042) [BCP018 (Error)] Expected the "{" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

extensionConfig hasNoConfig with {}
//@[000:035) [BCP425 (Error)] The extension configuration assignment for "hasNoConfig" does not match an extension in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP425) |extensionConfig hasNoConfig with {}|
//@[016:027) [BCP205 (Error)] Extension "hasNoConfig" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |hasNoConfig|

extensionConfig invalidSyntax1 = emptyObjVar
//@[000:044) [BCP425 (Error)] The extension configuration assignment for "invalidSyntax1" does not match an extension in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP425) |extensionConfig invalidSyntax1 = emptyObjVar|
//@[016:030) [BCP205 (Error)] Extension "invalidSyntax1" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |invalidSyntax1|
//@[031:032) [BCP012 (Error)] Expected the "with" keyword at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP012) |=|
extensionConfig invalidSyntax2 with emptyObjVar
//@[000:047) [BCP425 (Error)] The extension configuration assignment for "invalidSyntax2" does not match an extension in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP425) |extensionConfig invalidSyntax2 with emptyObjVar|
//@[016:030) [BCP205 (Error)] Extension "invalidSyntax2" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |invalidSyntax2|
//@[036:047) [BCP018 (Error)] Expected the "{" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) |emptyObjVar|
extensionConfig invalidSyntax3 with {
//@[000:056) [BCP425 (Error)] The extension configuration assignment for "invalidSyntax3" does not match an extension in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP425) |extensionConfig invalidSyntax3 with {\n  ...emptyObjVar\n}|
//@[016:030) [BCP205 (Error)] Extension "invalidSyntax3" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |invalidSyntax3|
  ...emptyObjVar
//@[002:016) [BCP401 (Error)] The spread operator "..." is not permitted in this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP401) |...emptyObjVar|
}

extensionConfig invalidSyntax4 with {
//@[000:089) [BCP425 (Error)] The extension configuration assignment for "invalidSyntax4" does not match an extension in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP425) |extensionConfig invalidSyntax4 with {\n  requiredString: validAssignment1.requiredString\n}|
//@[016:030) [BCP205 (Error)] Extension "invalidSyntax4" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |invalidSyntax4|
  requiredString: validAssignment1.requiredString
}

extensionConfig invalidSyntax5 with {
//@[000:061) [BCP425 (Error)] The extension configuration assignment for "invalidSyntax5" does not match an extension in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP425) |extensionConfig invalidSyntax5 with {\n  ...validAssignment1\n}|
//@[016:030) [BCP205 (Error)] Extension "invalidSyntax5" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |invalidSyntax5|
  ...validAssignment1
//@[002:021) [BCP401 (Error)] The spread operator "..." is not permitted in this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP401) |...validAssignment1|
}

extensionConfig invalidAssignment1 with {
//@[000:071) [BCP425 (Error)] The extension configuration assignment for "invalidAssignment1" does not match an extension in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP425) |extensionConfig invalidAssignment1 with {\n  requiredString: strParam1\n}|
//@[016:034) [BCP205 (Error)] Extension "invalidAssignment1" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |invalidAssignment1|
  requiredString: strParam1
}

extensionConfig invalidSecretAssignment1 with {
//@[000:189) [BCP425 (Error)] The extension configuration assignment for "invalidSecretAssignment1" does not match an extension in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP425) |extensionConfig invalidSecretAssignment1 with {\n  requiredSecureString: bool(readEnvironmentVariable('xyz', 'false')) ? az.getSecret('a', 'b', 'c', 'd') : az.getSecret('w', 'x', 'y', 'z')\n}|
//@[016:040) [BCP205 (Error)] Extension "invalidSecretAssignment1" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |invalidSecretAssignment1|
  requiredSecureString: bool(readEnvironmentVariable('xyz', 'false')) ? az.getSecret('a', 'b', 'c', 'd') : az.getSecret('w', 'x', 'y', 'z')
//@[072:104) [BCP351 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a parameter. (bicep https://aka.ms/bicep/core-diagnostics#BCP351) |az.getSecret('a', 'b', 'c', 'd')|
//@[107:139) [BCP351 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a parameter. (bicep https://aka.ms/bicep/core-diagnostics#BCP351) |az.getSecret('w', 'x', 'y', 'z')|
}

