type 44
//@[05:07) [BCP015 (Error)] Expected a variable identifier at this location. (CodeDescription: none) |44|
//@[07:07) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) ||
//@[07:07) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

type noAssignment
//@[17:17) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) ||
//@[17:17) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

type incompleteAssignment =
//@[27:27) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) ||

type resource = bool
//@[05:13) [BCP301 (Error)] The type name "resource" is reserved and may not be attached to a user-defined type. (CodeDescription: none) |resource|

@sealed()
//@[00:09) [BCP124 (Error)] The decorator "sealed" can only be attached to targets of type "object", but the target has type "string". (CodeDescription: none) |@sealed()|
type sealedString = string

@sealed()
//@[00:09) [BCP316 (Error)] The "sealed" decorator may not be used on object types with an explicit additional properties type declaration. (CodeDescription: none) |@sealed()|
type sealedDictionary = {
	*: string
}

type disallowedUnion = 'foo'|21
//@[29:31) [BCP286 (Error)] This union member is invalid because it cannot be assigned to the 'string' type. (CodeDescription: none) |21|

type validStringLiteralUnion = 'foo'|'bar'|'baz'

type validUnionInvalidAddition = validStringLiteralUnion|10
//@[57:59) [BCP286 (Error)] This union member is invalid because it cannot be assigned to the 'string' type. (CodeDescription: none) |10|

type invalidUnionInvalidAddition = disallowedUnion|true
//@[35:50) [BCP062 (Error)] The referenced declaration with name "disallowedUnion" is not valid. (CodeDescription: none) |disallowedUnion|

type nullLiteral = null
//@[19:23) [BCP289 (Error)] The type definition is not valid. (CodeDescription: none) |null|

type unionOfNulls = null|null
//@[20:24) [BCP289 (Error)] The type definition is not valid. (CodeDescription: none) |null|
//@[25:29) [BCP289 (Error)] The type definition is not valid. (CodeDescription: none) |null|

@minLength(3)
//@[00:13) [BCP124 (Error)] The decorator "minLength" can only be attached to targets of type "array | string", but the target has type "int". (CodeDescription: none) |@minLength(3)|
type lengthConstrainedInt = int

@minValue(3)
//@[00:12) [BCP124 (Error)] The decorator "minValue" can only be attached to targets of type "int", but the target has type "string". (CodeDescription: none) |@minValue(3)|
type valueConstrainedString = string

type tautology = tautology
//@[05:14) [BCP298 (Error)] This type definition includes itself as required component, which creates a constraint that cannot be fulfilled. (CodeDescription: none) |tautology|

type tautologicalUnion = tautologicalUnion|'foo'
//@[05:22) [BCP298 (Error)] This type definition includes itself as required component, which creates a constraint that cannot be fulfilled. (CodeDescription: none) |tautologicalUnion|

type tautologicalArray = tautologicalArray[]
//@[05:22) [BCP298 (Error)] This type definition includes itself as required component, which creates a constraint that cannot be fulfilled. (CodeDescription: none) |tautologicalArray|
//@[25:42) [BCP062 (Error)] The referenced declaration with name "tautologicalArray" is not valid. (CodeDescription: none) |tautologicalArray|

type directCycleStart = directCycleReturn
//@[05:21) [BCP299 (Error)] This type definition includes itself as a required component via a cycle ("directCycleStart" -> "directCycleReturn"). (CodeDescription: none) |directCycleStart|

type directCycleReturn = directCycleStart
//@[05:22) [BCP299 (Error)] This type definition includes itself as a required component via a cycle ("directCycleReturn" -> "directCycleStart"). (CodeDescription: none) |directCycleReturn|

type cycleRoot = connector
//@[05:14) [BCP299 (Error)] This type definition includes itself as a required component via a cycle ("cycleRoot" -> "connector" -> "cycleBack"). (CodeDescription: none) |cycleRoot|

type connector = cycleBack
//@[05:14) [BCP299 (Error)] This type definition includes itself as a required component via a cycle ("connector" -> "cycleBack" -> "cycleRoot"). (CodeDescription: none) |connector|

type cycleBack = cycleRoot
//@[05:14) [BCP299 (Error)] This type definition includes itself as a required component via a cycle ("cycleBack" -> "cycleRoot" -> "connector"). (CodeDescription: none) |cycleBack|

type objectWithInvalidPropertyDecorators = {
  @sealed()
//@[02:11) [BCP124 (Error)] The decorator "sealed" can only be attached to targets of type "object", but the target has type "string". (CodeDescription: none) |@sealed()|
  fooProp: string

  @secure()
//@[03:09) [BCP297 (Error)] Function "secure" cannot be used as a type decorator. (CodeDescription: none) |secure|
  barProp: string

  @allowed(['snap', 'crackle', 'pop'])
//@[03:10) [BCP297 (Error)] Function "allowed" cannot be used as a type decorator. (CodeDescription: none) |allowed|
  krispyProp: string
}

type objectWithInvalidRecursion = {
//@[05:31) [BCP298 (Error)] This type definition includes itself as required component, which creates a constraint that cannot be fulfilled. (CodeDescription: none) |objectWithInvalidRecursion|
  requiredAndRecursiveProp: objectWithInvalidRecursion
//@[28:54) [BCP062 (Error)] The referenced declaration with name "objectWithInvalidRecursion" is not valid. (CodeDescription: none) |objectWithInvalidRecursion|
}

type arrayWithInvalidMember = objectWithInvalidRecursion[]
//@[30:56) [BCP062 (Error)] The referenced declaration with name "objectWithInvalidRecursion" is not valid. (CodeDescription: none) |objectWithInvalidRecursion|

@sealed()
//@[00:09) [BCP124 (Error)] The decorator "sealed" can only be attached to targets of type "object", but the target has type "string". (CodeDescription: none) |@sealed()|
param sealedStringParam string
//@[06:23) [no-unused-params (Warning)] Parameter "sealedStringParam" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |sealedStringParam|

param disallowedUnionParam 'foo'|-99
//@[06:26) [no-unused-params (Warning)] Parameter "disallowedUnionParam" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |disallowedUnionParam|
//@[33:36) [BCP286 (Error)] This union member is invalid because it cannot be assigned to the 'string' type. (CodeDescription: none) |-99|

param objectWithInvalidRecursionParam objectWithInvalidRecursion
//@[06:37) [no-unused-params (Warning)] Parameter "objectWithInvalidRecursionParam" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |objectWithInvalidRecursionParam|
//@[38:64) [BCP062 (Error)] The referenced declaration with name "objectWithInvalidRecursion" is not valid. (CodeDescription: none) |objectWithInvalidRecursion|
