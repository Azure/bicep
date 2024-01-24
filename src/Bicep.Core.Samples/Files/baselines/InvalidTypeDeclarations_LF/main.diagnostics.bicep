type 44
//@[05:007) [BCP325 (Error)] Expected a type identifier at this location. (CodeDescription: none) |44|
//@[07:007) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) ||
//@[07:007) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

type noAssignment
//@[17:017) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) ||
//@[17:017) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

type incompleteAssignment =
//@[27:027) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) ||

type resource = bool
//@[05:013) [BCP301 (Error)] The type name "resource" is reserved and may not be attached to a user-defined type. (CodeDescription: none) |resource|

@sealed()
//@[00:009) [BCP124 (Error)] The decorator "sealed" can only be attached to targets of type "object", but the target has type "string". (CodeDescription: none) |@sealed()|
type sealedString = string

@sealed()
//@[00:009) [BCP316 (Error)] The "sealed" decorator may not be used on object types with an explicit additional properties type declaration. (CodeDescription: none) |@sealed()|
type sealedDictionary = {
	*: string
}

type disallowedUnion = 'foo'|21
//@[23:031) [BCP294 (Error)] Type unions must be reduceable to a single ARM type (such as 'string', 'int', or 'bool'). (CodeDescription: none) |'foo'|21|

type validStringLiteralUnion = 'foo'|'bar'|'baz'

type validUnionInvalidAddition = validStringLiteralUnion|10
//@[33:059) [BCP294 (Error)] Type unions must be reduceable to a single ARM type (such as 'string', 'int', or 'bool'). (CodeDescription: none) |validStringLiteralUnion|10|

type invalidUnionInvalidAddition = disallowedUnion|true
//@[35:055) [BCP294 (Error)] Type unions must be reduceable to a single ARM type (such as 'string', 'int', or 'bool'). (CodeDescription: none) |disallowedUnion|true|

type nullLiteral = null
//@[19:023) [BCP289 (Error)] The type definition is not valid. (CodeDescription: none) |null|

type unionOfNulls = null|null
//@[20:029) [BCP294 (Error)] Type unions must be reduceable to a single ARM type (such as 'string', 'int', or 'bool'). (CodeDescription: none) |null|null|

@minLength(3)
//@[00:013) [BCP124 (Error)] The decorator "minLength" can only be attached to targets of type "array | string", but the target has type "int". (CodeDescription: none) |@minLength(3)|
type lengthConstrainedInt = int

@minValue(3)
//@[00:012) [BCP124 (Error)] The decorator "minValue" can only be attached to targets of type "int", but the target has type "string". (CodeDescription: none) |@minValue(3)|
type valueConstrainedString = string

type tautology = tautology
//@[05:014) [BCP298 (Error)] This type definition includes itself as required component, which creates a constraint that cannot be fulfilled. (CodeDescription: none) |tautology|

type tautologicalUnion = tautologicalUnion|'foo'
//@[05:022) [BCP298 (Error)] This type definition includes itself as required component, which creates a constraint that cannot be fulfilled. (CodeDescription: none) |tautologicalUnion|
//@[25:042) [BCP293 (Error)] All members of a union type declaration must be literal values. (CodeDescription: none) |tautologicalUnion|

type tautologicalArray = tautologicalArray[]
//@[05:022) [BCP298 (Error)] This type definition includes itself as required component, which creates a constraint that cannot be fulfilled. (CodeDescription: none) |tautologicalArray|

type directCycleStart = directCycleReturn
//@[05:021) [BCP299 (Error)] This type definition includes itself as a required component via a cycle ("directCycleStart" -> "directCycleReturn"). (CodeDescription: none) |directCycleStart|

type directCycleReturn = directCycleStart
//@[05:022) [BCP299 (Error)] This type definition includes itself as a required component via a cycle ("directCycleReturn" -> "directCycleStart"). (CodeDescription: none) |directCycleReturn|

type cycleRoot = connector
//@[05:014) [BCP299 (Error)] This type definition includes itself as a required component via a cycle ("cycleRoot" -> "connector" -> "cycleBack"). (CodeDescription: none) |cycleRoot|

type connector = cycleBack
//@[05:014) [BCP299 (Error)] This type definition includes itself as a required component via a cycle ("connector" -> "cycleBack" -> "cycleRoot"). (CodeDescription: none) |connector|

type cycleBack = cycleRoot
//@[05:014) [BCP299 (Error)] This type definition includes itself as a required component via a cycle ("cycleBack" -> "cycleRoot" -> "connector"). (CodeDescription: none) |cycleBack|

type objectWithInvalidPropertyDecorators = {
  @sealed()
//@[02:011) [BCP124 (Error)] The decorator "sealed" can only be attached to targets of type "object", but the target has type "string". (CodeDescription: none) |@sealed()|
  fooProp: string

  @secure()
  barProp: string

  @allowed(['snap', 'crackle', 'pop'])
//@[03:010) [BCP297 (Error)] Function "allowed" cannot be used as a type decorator. (CodeDescription: none) |allowed|
  krispyProp: string
}

type objectWithInvalidRecursion = {
//@[05:031) [BCP298 (Error)] This type definition includes itself as required component, which creates a constraint that cannot be fulfilled. (CodeDescription: none) |objectWithInvalidRecursion|
  requiredAndRecursiveProp: objectWithInvalidRecursion
}

type arrayWithInvalidMember = objectWithInvalidRecursion[]

@sealed()
//@[00:009) [BCP124 (Error)] The decorator "sealed" can only be attached to targets of type "object", but the target has type "string". (CodeDescription: none) |@sealed()|
param sealedStringParam string
//@[06:023) [no-unused-params (Warning)] Parameter "sealedStringParam" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |sealedStringParam|

param disallowedUnionParam 'foo'|-99
//@[06:026) [no-unused-params (Warning)] Parameter "disallowedUnionParam" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |disallowedUnionParam|
//@[27:036) [BCP294 (Error)] Type unions must be reduceable to a single ARM type (such as 'string', 'int', or 'bool'). (CodeDescription: none) |'foo'|-99|

param objectWithInvalidRecursionParam objectWithInvalidRecursion
//@[06:037) [no-unused-params (Warning)] Parameter "objectWithInvalidRecursionParam" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |objectWithInvalidRecursionParam|

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

type primitiveUnion = | bool | bool
//@[24:028) [BCP293 (Error)] All members of a union type declaration must be literal values. (CodeDescription: none) |bool|
//@[31:035) [BCP293 (Error)] All members of a union type declaration must be literal values. (CodeDescription: none) |bool|

type objectUnion = typeA | typeB
//@[19:024) [BCP293 (Error)] All members of a union type declaration must be literal values. (CodeDescription: none) |typeA|
//@[27:032) [BCP293 (Error)] All members of a union type declaration must be literal values. (CodeDescription: none) |typeB|

@discriminator()
//@[14:016) [BCP071 (Error)] Expected 1 argument, but got 0. (CodeDescription: none) |()|
type noDiscriminatorParam = typeA | typeB

@discriminator(true)
//@[15:019) [BCP070 (Error)] Argument of type "true" is not assignable to parameter of type "string". (CodeDescription: none) |true|
type wrongDiscriminatorParamType = typeA | typeB

@discriminator('nonexistent')
type discriminatorPropertyNotExistAtAll = typeA | typeB
//@[42:047) [BCP364 (Error)] The property "nonexistent" must be a required string literal on all union member types. (CodeDescription: none) |typeA|
//@[50:055) [BCP364 (Error)] The property "nonexistent" must be a required string literal on all union member types. (CodeDescription: none) |typeB|

@discriminator('nonexistent')
//@[00:029) [BCP366 (Error)] The discriminator property name must be "nonexistent" on all union member types. (CodeDescription: none) |@discriminator('nonexistent')|
type discriminatorPropertyMismatch = unionAB

@discriminator('type')
type discriminatorPropertyNotExistOnAtLeastOne = typeA | { value: bool }
//@[57:072) [BCP364 (Error)] The property "type" must be a required string literal on all union member types. (CodeDescription: none) |{ value: bool }|

@discriminator('type')
//@[00:022) [BCP363 (Error)] The "discriminator" decorator can only be applied to object-only union types with unique member types. (CodeDescription: none) |@discriminator('type')|
type discriminatorWithOnlyOneMember = typeA

@discriminator('type')
type discriminatorPropertyNotRequiredStringLiteral1 = typeA | typeF
//@[62:067) [BCP364 (Error)] The property "type" must be a required string literal on all union member types. (CodeDescription: none) |typeF|

@discriminator('type')
type discriminatorPropertyNotRequiredStringLiteral2 = typeA | typeG
//@[62:067) [BCP364 (Error)] The property "type" must be a required string literal on all union member types. (CodeDescription: none) |typeG|

@discriminator('type')
type discriminatorDuplicatedMember1 = typeA | typeA

@discriminator('type')
type discriminatorDuplicatedMember2 = typeA | { type: 'a', config: object }
//@[46:075) [BCP365 (Error)] The value "'a'" for discriminator property "type" is duplicated across multiple union member types. The value must be unique across all union member types. (CodeDescription: none) |{ type: 'a', config: object }|

@discriminator('type')
type discriminatorOnlyOneNonNullMember1 = typeA | null
//@[50:054) [BCP286 (Error)] This union member is invalid because it cannot be assigned to the 'object' type. (CodeDescription: none) |null|

@discriminator('type')
//@[00:022) [BCP363 (Error)] The "discriminator" decorator can only be applied to object-only union types with unique member types. (CodeDescription: none) |@discriminator('type')|
type discriminatorOnlyOneNonNullMember2 = (typeA)?

@discriminator('type')
type discriminatorMemberHasAdditionalProperties = typeA | typeE

@discriminator('type')
type discriminatorSelfCycle1 = typeA | discriminatorSelfCycle1
//@[05:028) [BCP298 (Error)] This type definition includes itself as required component, which creates a constraint that cannot be fulfilled. (CodeDescription: none) |discriminatorSelfCycle1|
//@[39:062) [BCP286 (Error)] This union member is invalid because it cannot be assigned to the 'object' type. (CodeDescription: none) |discriminatorSelfCycle1|

@discriminator('type')
type discriminatorSelfCycle2 = (typeA | discriminatorSelfCycle2)?
//@[05:028) [BCP298 (Error)] This type definition includes itself as required component, which creates a constraint that cannot be fulfilled. (CodeDescription: none) |discriminatorSelfCycle2|
//@[40:063) [BCP286 (Error)] This union member is invalid because it cannot be assigned to the 'object' type. (CodeDescription: none) |discriminatorSelfCycle2|

@discriminator('type')
type discriminatorTopLevelCycleA = typeA | discriminatorTopLevelCycleB
//@[05:032) [BCP299 (Error)] This type definition includes itself as a required component via a cycle ("discriminatorTopLevelCycleA" -> "discriminatorTopLevelCycleB"). (CodeDescription: none) |discriminatorTopLevelCycleA|
//@[43:070) [BCP286 (Error)] This union member is invalid because it cannot be assigned to the 'object' type. (CodeDescription: none) |discriminatorTopLevelCycleB|
@discriminator('type')
type discriminatorTopLevelCycleB = typeB | discriminatorTopLevelCycleA
//@[05:032) [BCP299 (Error)] This type definition includes itself as a required component via a cycle ("discriminatorTopLevelCycleB" -> "discriminatorTopLevelCycleA"). (CodeDescription: none) |discriminatorTopLevelCycleB|
//@[43:070) [BCP286 (Error)] This union member is invalid because it cannot be assigned to the 'object' type. (CodeDescription: none) |discriminatorTopLevelCycleA|

@discriminator('type')
type discriminatorInnerSelfCycle1 = typeA | {
//@[05:033) [BCP298 (Error)] This type definition includes itself as required component, which creates a constraint that cannot be fulfilled. (CodeDescription: none) |discriminatorInnerSelfCycle1|
  type: 'b'
  value: discriminatorInnerSelfCycle1
}

type discriminatorInnerSelfCycle2Helper = {
//@[05:039) [BCP299 (Error)] This type definition includes itself as a required component via a cycle ("discriminatorInnerSelfCycle2Helper" -> "discriminatorInnerSelfCycle2"). (CodeDescription: none) |discriminatorInnerSelfCycle2Helper|
  type: 'b'
  value: discriminatorInnerSelfCycle2
}
@discriminator('type')
type discriminatorInnerSelfCycle2 = typeA | discriminatorInnerSelfCycle2Helper
//@[05:033) [BCP299 (Error)] This type definition includes itself as a required component via a cycle ("discriminatorInnerSelfCycle2" -> "discriminatorInnerSelfCycle2Helper"). (CodeDescription: none) |discriminatorInnerSelfCycle2|

@discriminator('type')
//@[00:022) [BCP363 (Error)] The "discriminator" decorator can only be applied to object-only union types with unique member types. (CodeDescription: none) |@discriminator('type')|
//@[00:022) [BCP124 (Error)] The decorator "discriminator" can only be attached to targets of type "object", but the target has type "[typeA, typeB]". (CodeDescription: none) |@discriminator('type')|
type discriminatorTupleBadType1 = [typeA, typeB]

@discriminator('type')
//@[00:022) [BCP363 (Error)] The "discriminator" decorator can only be applied to object-only union types with unique member types. (CodeDescription: none) |@discriminator('type')|
//@[00:022) [BCP124 (Error)] The decorator "discriminator" can only be attached to targets of type "object", but the target has type "[typeA | typeB]". (CodeDescription: none) |@discriminator('type')|
type discriminatorTupleBadType2 = [typeA | typeB]
//@[35:040) [BCP293 (Error)] All members of a union type declaration must be literal values. (CodeDescription: none) |typeA|
//@[43:048) [BCP293 (Error)] All members of a union type declaration must be literal values. (CodeDescription: none) |typeB|

@discriminator('type')
//@[00:022) [BCP363 (Error)] The "discriminator" decorator can only be applied to object-only union types with unique member types. (CodeDescription: none) |@discriminator('type')|
//@[00:022) [BCP124 (Error)] The decorator "discriminator" can only be attached to targets of type "object", but the target has type "[typeA | typeB, typeC | typeD]". (CodeDescription: none) |@discriminator('type')|
type discriminatorTupleBadType3 = [typeA | typeB, typeC | typeD]
//@[35:040) [BCP293 (Error)] All members of a union type declaration must be literal values. (CodeDescription: none) |typeA|
//@[43:048) [BCP293 (Error)] All members of a union type declaration must be literal values. (CodeDescription: none) |typeB|
//@[50:055) [BCP293 (Error)] All members of a union type declaration must be literal values. (CodeDescription: none) |typeC|
//@[58:063) [BCP293 (Error)] All members of a union type declaration must be literal values. (CodeDescription: none) |typeD|

type discriminatorInlineAdditionalPropsBadType1 = {
  @discriminator('type')
//@[02:024) [BCP363 (Error)] The "discriminator" decorator can only be applied to object-only union types with unique member types. (CodeDescription: none) |@discriminator('type')|
  *: typeA
}

type discriminatorInlineAdditionalPropsBadType2 = {
  @discriminator('type')
  *: typeA | typeA
}

type discriminatorInlineAdditionalPropsBadType3 = {
  @discriminator('type')
//@[02:024) [BCP363 (Error)] The "discriminator" decorator can only be applied to object-only union types with unique member types. (CodeDescription: none) |@discriminator('type')|
//@[02:024) [BCP124 (Error)] The decorator "discriminator" can only be attached to targets of type "object", but the target has type "string". (CodeDescription: none) |@discriminator('type')|
  *: string
}

@discriminator('type')
type discriminatedUnionDuplicateMemberInsensitive = { type: 'a', value: string } | { type: 'A', value: int }
//@[83:108) [BCP365 (Error)] The value "'A'" for discriminator property "type" is duplicated across multiple union member types. The value must be unique across all union member types. (CodeDescription: none) |{ type: 'A', value: int }|

@discriminator('TYPE')
type discriminatedUnionCaseSensitiveDiscriminator = { type: 'a', value: string } | { type: 'b', value: int }
//@[52:080) [BCP364 (Error)] The property "TYPE" must be a required string literal on all union member types. (CodeDescription: none) |{ type: 'a', value: string }|
//@[83:108) [BCP364 (Error)] The property "TYPE" must be a required string literal on all union member types. (CodeDescription: none) |{ type: 'b', value: int }|

@discriminator('type')
//@[00:022) [BCP363 (Error)] The "discriminator" decorator can only be applied to object-only union types with unique member types. (CodeDescription: none) |@discriminator('type')|
param discriminatorParamBadType1 typeA
//@[06:032) [no-unused-params (Warning)] Parameter "discriminatorParamBadType1" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |discriminatorParamBadType1|

@discriminator('type')
param discriminatorParamBadType2 'a' | 'b'
//@[06:032) [no-unused-params (Warning)] Parameter "discriminatorParamBadType2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |discriminatorParamBadType2|
//@[33:036) [BCP286 (Error)] This union member is invalid because it cannot be assigned to the 'object' type. (CodeDescription: none) |'a'|
//@[39:042) [BCP286 (Error)] This union member is invalid because it cannot be assigned to the 'object' type. (CodeDescription: none) |'b'|

@discriminator('type')
//@[00:022) [BCP363 (Error)] The "discriminator" decorator can only be applied to object-only union types with unique member types. (CodeDescription: none) |@discriminator('type')|
output discriminatorOutputBadType1 typeA = { type: 'a', value: 'a' }

@discriminator('type')
//@[00:022) [BCP363 (Error)] The "discriminator" decorator can only be applied to object-only union types with unique member types. (CodeDescription: none) |@discriminator('type')|
output discriminatorOutputBadType2 object = { prop: 'value' }

type strings = string[]

type invalidTupleAccess = strings[0]
//@[34:035) [BCP388 (Error)] Cannot access elements of type "string[]" by index. An tuple type is required. (CodeDescription: none) |0|

type stringTuple = [string, string]

type invalidItemTypeAccess = stringTuple[*]
//@[40:043) [BCP390 (Error)] The array item type access operator ('[*]') can only be used with typed arrays. (CodeDescription: none) |[*]|

type anObject = {
  property: string
}

type invalidAdditionalPropertiesAccess = anObject.*
//@[50:051) [BCP389 (Error)] The type "{ property: string }" does not declare an additional properties type. (CodeDescription: none) |*|

type stringDict = {
  *: string
}

type invalidPropertyAccess = stringDict.property
//@[40:048) [BCP052 (Error)] The type "{ *: string }" does not contain property "property". (CodeDescription: none) |property|

