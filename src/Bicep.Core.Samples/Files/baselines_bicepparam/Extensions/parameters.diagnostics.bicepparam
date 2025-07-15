using 'main.bicep'
//@[06:018) [BCP104 (Error)] The referenced module has errors. (bicep https://aka.ms/bicep/core-diagnostics#BCP104) |'main.bicep'|

extensionConfig hasObjConfig1 with {
//@[00:074) [BCP425 (Error)] The extension configuration assignment for "hasObjConfig1" does not match an extension in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP425) |extensionConfig hasObjConfig1 with {\n  requiredString: 'valueFromParams'\n}|
//@[16:029) [BCP205 (Error)] Extension "hasObjConfig1" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |hasObjConfig1|
  requiredString: 'valueFromParams'
}

extensionConfig hasObjConfig2 with {
//@[00:067) [BCP425 (Error)] The extension configuration assignment for "hasObjConfig2" does not match an extension in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP425) |extensionConfig hasObjConfig2 with {\n  optionalString: 'optional'\n}|
//@[16:029) [BCP205 (Error)] Extension "hasObjConfig2" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |hasObjConfig2|
  optionalString: 'optional'
}

extensionConfig hasObjConfig3 with {}
//@[00:037) [BCP425 (Error)] The extension configuration assignment for "hasObjConfig3" does not match an extension in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP425) |extensionConfig hasObjConfig3 with {}|
//@[16:029) [BCP205 (Error)] Extension "hasObjConfig3" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |hasObjConfig3|

// hasObjConfig4 not here to test assignment is not required because required field is defaulted

extensionConfig hasSecureConfig1 with {
//@[00:147) [BCP425 (Error)] The extension configuration assignment for "hasSecureConfig1" does not match an extension in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP425) |extensionConfig hasSecureConfig1 with {\n  requiredSecureString: az.getSecret('00000000-0000-0000-0000-000000000001', 'mock-rg', 'kv', 'mySecret')\n}|
//@[16:032) [BCP205 (Error)] Extension "hasSecureConfig1" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |hasSecureConfig1|
  requiredSecureString: az.getSecret('00000000-0000-0000-0000-000000000001', 'mock-rg', 'kv', 'mySecret')
//@[24:105) [BCP351 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a parameter. (bicep https://aka.ms/bicep/core-diagnostics#BCP351) |az.getSecret('00000000-0000-0000-0000-000000000001', 'mock-rg', 'kv', 'mySecret')|
}

extensionConfig hasSecureConfig2 with {
//@[00:111) [BCP425 (Error)] The extension configuration assignment for "hasSecureConfig2" does not match an extension in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP425) |extensionConfig hasSecureConfig2 with {\n  requiredSecureString: 'Inlined'\n  optionalString: 'valueFromParams'\n}|
//@[16:032) [BCP205 (Error)] Extension "hasSecureConfig2" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |hasSecureConfig2|
  requiredSecureString: 'Inlined'
  optionalString: 'valueFromParams'
}

extensionConfig hasDiscrimConfig1 with {
//@[00:081) [BCP425 (Error)] The extension configuration assignment for "hasDiscrimConfig1" does not match an extension in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP425) |extensionConfig hasDiscrimConfig1 with {\n  discrim: 'a'\n  z1: 'z1v'\n  a1: 'a1v'\n}|
//@[16:033) [BCP205 (Error)] Extension "hasDiscrimConfig1" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |hasDiscrimConfig1|
  discrim: 'a'
  z1: 'z1v'
  a1: 'a1v'
}

extensionConfig hasDiscrimConfig2 with {
//@[00:054) [BCP425 (Error)] The extension configuration assignment for "hasDiscrimConfig2" does not match an extension in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP425) |extensionConfig hasDiscrimConfig2 with {\n  a1: 'a1v'\n}|
//@[16:033) [BCP205 (Error)] Extension "hasDiscrimConfig2" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |hasDiscrimConfig2|
  a1: 'a1v'
}

extensionConfig hasDiscrimConfig3 with {
//@[00:069) [BCP425 (Error)] The extension configuration assignment for "hasDiscrimConfig3" does not match an extension in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP425) |extensionConfig hasDiscrimConfig3 with {\n  discrim: 'b'\n  b1: 'b1v'\n}|
//@[16:033) [BCP205 (Error)] Extension "hasDiscrimConfig3" does not support configuration. (bicep https://aka.ms/bicep/core-diagnostics#BCP205) |hasDiscrimConfig3|
  discrim: 'b'
  b1: 'b1v'
}

