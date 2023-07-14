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
//@[23:31) [BCP294 (Error)] Type unions must be reduceable to a single ARM type (such as 'string', 'int', or 'bool'). (CodeDescription: none) |'foo'|21|

type validStringLiteralUnion = 'foo'|'bar'|'baz'

type validUnionInvalidAddition = validStringLiteralUnion|10
//@[33:59) [BCP294 (Error)] Type unions must be reduceable to a single ARM type (such as 'string', 'int', or 'bool'). (CodeDescription: none) |validStringLiteralUnion|10|

type invalidUnionInvalidAddition = disallowedUnion|true
//@[35:55) [BCP294 (Error)] Type unions must be reduceable to a single ARM type (such as 'string', 'int', or 'bool'). (CodeDescription: none) |disallowedUnion|true|

type nullLiteral = null
//@[19:23) [BCP289 (Error)] The type definition is not valid. (CodeDescription: none) |null|

type unionOfNulls = null|null
//@[20:29) [BCP294 (Error)] Type unions must be reduceable to a single ARM type (such as 'string', 'int', or 'bool'). (CodeDescription: none) |null|null|

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
//@[25:42) [BCP293 (Error)] All members of a union type declaration must be literal values. (CodeDescription: none) |tautologicalUnion|

type tautologicalArray = tautologicalArray[]
//@[05:22) [BCP298 (Error)] This type definition includes itself as required component, which creates a constraint that cannot be fulfilled. (CodeDescription: none) |tautologicalArray|

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
  barProp: string

  @allowed(['snap', 'crackle', 'pop'])
//@[03:10) [BCP297 (Error)] Function "allowed" cannot be used as a type decorator. (CodeDescription: none) |allowed|
  krispyProp: string
}

type objectWithInvalidRecursion = {
//@[05:31) [BCP298 (Error)] This type definition includes itself as required component, which creates a constraint that cannot be fulfilled. (CodeDescription: none) |objectWithInvalidRecursion|
  requiredAndRecursiveProp: objectWithInvalidRecursion
}

type arrayWithInvalidMember = objectWithInvalidRecursion[]

@sealed()
//@[00:09) [BCP124 (Error)] The decorator "sealed" can only be attached to targets of type "object", but the target has type "string". (CodeDescription: none) |@sealed()|
param sealedStringParam string
//@[06:23) [no-unused-params (Warning)] Parameter "sealedStringParam" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |sealedStringParam|

param disallowedUnionParam 'foo'|-99
//@[06:26) [no-unused-params (Warning)] Parameter "disallowedUnionParam" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |disallowedUnionParam|
//@[27:36) [BCP294 (Error)] Type unions must be reduceable to a single ARM type (such as 'string', 'int', or 'bool'). (CodeDescription: none) |'foo'|-99|

param objectWithInvalidRecursionParam objectWithInvalidRecursion
//@[06:37) [no-unused-params (Warning)] Parameter "objectWithInvalidRecursionParam" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |objectWithInvalidRecursionParam|

type typeA = {
  type: 'a'
  value: string
}

type typeB = {
  type: 'b'
  value: int
}

@discriminator('type')
type unionAB = typeA | typeB

type typeC = {
  type: 'c'
  value: bool
  value2: string
}

type typeD = {
  type: 'd'
  value: object
}

type typeE = {
  type: 'e'
  *: string
}

type typeF = {
  type: 0
  value: string
}

type typeG = {
  type: 'g'?
  value: string
}

type objectUnion = typeA | typeB
//@[19:24) [BCP293 (Error)] All members of a union type declaration must be literal values. (CodeDescription: none) |typeA|
//@[27:32) [BCP293 (Error)] All members of a union type declaration must be literal values. (CodeDescription: none) |typeB|

@discriminator()
//@[14:16) [BCP071 (Error)] Expected 1 argument, but got 0. (CodeDescription: none) |()|
type noDiscriminatorParam = typeA | typeB

@discriminator(true)
//@[15:19) [BCP070 (Error)] Argument of type "true" is not assignable to parameter of type "string". (CodeDescription: none) |true|
type wrongDiscriminatorParamType = typeA | typeB

@discriminator('nonexistent')
type discriminatorPropertyNotExistAtAll = typeA | typeB
//@[42:47) [BCP352 (Error)] The property "nonexistent" must be a required string literal on all union member types. (CodeDescription: none) |typeA|
//@[50:55) [BCP352 (Error)] The property "nonexistent" must be a required string literal on all union member types. (CodeDescription: none) |typeB|

@discriminator('type')
type discriminatorPropertyNotExistOnAtLeastOne = typeA | { value: bool }
//@[57:72) [BCP352 (Error)] The property "type" must be a required string literal on all union member types. (CodeDescription: none) |{ value: bool }|

@discriminator('type')
//@[00:22) [BCP351 (Error)] The "discriminator" decorator can only be applied to object-only union types with unique member types. (CodeDescription: none) |@discriminator('type')|
type discriminatorWithOnlyOneMember = typeA

@discriminator('type')
type discriminatorPropertyNotRequiredStringLiteral1 = typeA | typeF
//@[62:67) [BCP352 (Error)] The property "type" must be a required string literal on all union member types. (CodeDescription: none) |typeF|

@discriminator('type')
type discriminatorPropertyNotRequiredStringLiteral2 = typeA | typeG
//@[62:67) [BCP352 (Error)] The property "type" must be a required string literal on all union member types. (CodeDescription: none) |typeG|

@discriminator('type')
type discriminatorDuplicatedMember1 = typeA | typeA
//@[46:51) [BCP353 (Error)] The value "a" for discriminator property "type" is duplicated across multiple union member types. The value must be unique across all union member types. (CodeDescription: none) |typeA|

@discriminator('type')
type discriminatorDuplicatedMember2 = typeA | { type: 'a', config: object }
//@[46:75) [BCP353 (Error)] The value "a" for discriminator property "type" is duplicated across multiple union member types. The value must be unique across all union member types. (CodeDescription: none) |{ type: 'a', config: object }|

@discriminator('type')
type discriminatorSelfCycle = typeA | discriminatorSelfCycle
//@[05:27) [BCP298 (Error)] This type definition includes itself as required component, which creates a constraint that cannot be fulfilled. (CodeDescription: none) |discriminatorSelfCycle|

@discriminator('type')
type discriminatorTopLevelCycleA = typeA | discriminatorTopLevelCycleB
//@[05:32) [BCP299 (Error)] This type definition includes itself as a required component via a cycle ("discriminatorTopLevelCycleA" -> "discriminatorTopLevelCycleB"). (CodeDescription: none) |discriminatorTopLevelCycleA|
@discriminator('type')
type discriminatorTopLevelCycleB = typeB | discriminatorTopLevelCycleA
//@[05:32) [BCP299 (Error)] This type definition includes itself as a required component via a cycle ("discriminatorTopLevelCycleB" -> "discriminatorTopLevelCycleA"). (CodeDescription: none) |discriminatorTopLevelCycleB|

@discriminator('type')
type discriminatorInnerSelfCycle1 = typeA | {
//@[05:33) [BCP298 (Error)] This type definition includes itself as required component, which creates a constraint that cannot be fulfilled. (CodeDescription: none) |discriminatorInnerSelfCycle1|
  type: 'b'
  value: discriminatorInnerSelfCycle1
}

type discriminatorInnerSelfCycle2Helper = {
//@[05:39) [BCP299 (Error)] This type definition includes itself as a required component via a cycle ("discriminatorInnerSelfCycle2Helper" -> "discriminatorInnerSelfCycle2"). (CodeDescription: none) |discriminatorInnerSelfCycle2Helper|
  type: 'b'
  value: discriminatorInnerSelfCycle2
}
@discriminator('type')
type discriminatorInnerSelfCycle2 = typeA | discriminatorInnerSelfCycle2Helper
//@[05:33) [BCP299 (Error)] This type definition includes itself as a required component via a cycle ("discriminatorInnerSelfCycle2" -> "discriminatorInnerSelfCycle2Helper"). (CodeDescription: none) |discriminatorInnerSelfCycle2|

