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
//@[00:051) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype unionAB = typeA | typeB|
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
//@[19:024) [BCP293 (Error)] All members of a union type declaration must be literal values. (CodeDescription: none) |typeA|
//@[27:032) [BCP293 (Error)] All members of a union type declaration must be literal values. (CodeDescription: none) |typeB|

@discriminator()
//@[00:058) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator()\ntype noDiscriminatorParam = typeA | typeB|
//@[14:016) [BCP071 (Error)] Expected 1 argument, but got 0. (CodeDescription: none) |()|
type noDiscriminatorParam = typeA | typeB

@discriminator(true)
//@[00:069) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator(true)\ntype wrongDiscriminatorParamType = typeA | typeB|
//@[15:019) [BCP070 (Error)] Argument of type "true" is not assignable to parameter of type "string". (CodeDescription: none) |true|
type wrongDiscriminatorParamType = typeA | typeB

@discriminator('nonexistent')
//@[00:085) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('nonexistent')\ntype discriminatorPropertyNotExistAtAll = typeA | typeB|
type discriminatorPropertyNotExistAtAll = typeA | typeB

@discriminator('nonexistent')
type discriminatorPropertyMismatch = unionAB
//@[37:044) [BCP062 (Error)] The referenced declaration with name "unionAB" is not valid. (CodeDescription: none) |unionAB|

@discriminator('type')
//@[00:095) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype discriminatorPropertyNotExistOnAtLeastOne = typeA | { value: bool }|
type discriminatorPropertyNotExistOnAtLeastOne = typeA | { value: bool }

@discriminator('type')
//@[00:022) [BCP363 (Error)] The "discriminator" decorator can only be applied to object-only union types with unique member types. (CodeDescription: none) |@discriminator('type')|
type discriminatorWithOnlyOneMember = typeA

@discriminator('type')
//@[00:090) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype discriminatorPropertyNotRequiredStringLiteral1 = typeA | typeF|
type discriminatorPropertyNotRequiredStringLiteral1 = typeA | typeF

@discriminator('type')
//@[00:090) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype discriminatorPropertyNotRequiredStringLiteral2 = typeA | typeG|
type discriminatorPropertyNotRequiredStringLiteral2 = typeA | typeG

@discriminator('type')
//@[00:074) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype discriminatorDuplicatedMember1 = typeA | typeA|
type discriminatorDuplicatedMember1 = typeA | typeA

@discriminator('type')
//@[00:098) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype discriminatorDuplicatedMember2 = typeA | { type: 'a', config: object }|
type discriminatorDuplicatedMember2 = typeA | { type: 'a', config: object }

@discriminator('type')
//@[00:077) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype discriminatorOnlyOneNonNullMember1 = typeA | null|
type discriminatorOnlyOneNonNullMember1 = typeA | null

@discriminator('type')
//@[00:022) [BCP363 (Error)] The "discriminator" decorator can only be applied to object-only union types with unique member types. (CodeDescription: none) |@discriminator('type')|
type discriminatorOnlyOneNonNullMember2 = (typeA)?

@discriminator('type')
//@[00:086) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype discriminatorMemberHasAdditionalProperties = typeA | typeE|
type discriminatorMemberHasAdditionalProperties = typeA | typeE

@discriminator('type')
//@[00:085) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype discriminatorSelfCycle1 = typeA | discriminatorSelfCycle1|
type discriminatorSelfCycle1 = typeA | discriminatorSelfCycle1
//@[05:028) [BCP298 (Error)] This type definition includes itself as required component, which creates a constraint that cannot be fulfilled. (CodeDescription: none) |discriminatorSelfCycle1|

@discriminator('type')
//@[00:088) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype discriminatorSelfCycle2 = (typeA | discriminatorSelfCycle2)?|
type discriminatorSelfCycle2 = (typeA | discriminatorSelfCycle2)?
//@[05:028) [BCP298 (Error)] This type definition includes itself as required component, which creates a constraint that cannot be fulfilled. (CodeDescription: none) |discriminatorSelfCycle2|

@discriminator('type')
//@[00:093) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype discriminatorTopLevelCycleA = typeA | discriminatorTopLevelCycleB|
type discriminatorTopLevelCycleA = typeA | discriminatorTopLevelCycleB
//@[05:032) [BCP299 (Error)] This type definition includes itself as a required component via a cycle ("discriminatorTopLevelCycleA" -> "discriminatorTopLevelCycleB"). (CodeDescription: none) |discriminatorTopLevelCycleA|
@discriminator('type')
//@[00:093) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype discriminatorTopLevelCycleB = typeB | discriminatorTopLevelCycleA|
type discriminatorTopLevelCycleB = typeB | discriminatorTopLevelCycleA
//@[05:032) [BCP299 (Error)] This type definition includes itself as a required component via a cycle ("discriminatorTopLevelCycleB" -> "discriminatorTopLevelCycleA"). (CodeDescription: none) |discriminatorTopLevelCycleB|

@discriminator('type')
//@[00:120) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype discriminatorInnerSelfCycle1 = typeA | {\n  type: 'b'\n  value: discriminatorInnerSelfCycle1\n}|
type discriminatorInnerSelfCycle1 = typeA | {
  type: 'b'
  value: discriminatorInnerSelfCycle1
//@[09:037) [BCP062 (Error)] The referenced declaration with name "discriminatorInnerSelfCycle1" is not valid. (CodeDescription: none) |discriminatorInnerSelfCycle1|
}

type discriminatorInnerSelfCycle2Helper = {
  type: 'b'
  value: discriminatorInnerSelfCycle2
//@[09:037) [BCP062 (Error)] The referenced declaration with name "discriminatorInnerSelfCycle2" is not valid. (CodeDescription: none) |discriminatorInnerSelfCycle2|
}
@discriminator('type')
//@[00:101) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype discriminatorInnerSelfCycle2 = typeA | discriminatorInnerSelfCycle2Helper|
type discriminatorInnerSelfCycle2 = typeA | discriminatorInnerSelfCycle2Helper

@discriminator('type')
//@[00:022) [BCP363 (Error)] The "discriminator" decorator can only be applied to object-only union types with unique member types. (CodeDescription: none) |@discriminator('type')|
type discriminatorTupleBadType1 = [typeA, typeB]

@discriminator('type')
//@[00:022) [BCP363 (Error)] The "discriminator" decorator can only be applied to object-only union types with unique member types. (CodeDescription: none) |@discriminator('type')|
type discriminatorTupleBadType2 = [typeA | typeB]
//@[35:040) [BCP293 (Error)] All members of a union type declaration must be literal values. (CodeDescription: none) |typeA|
//@[43:048) [BCP293 (Error)] All members of a union type declaration must be literal values. (CodeDescription: none) |typeB|

@discriminator('type')
//@[00:022) [BCP363 (Error)] The "discriminator" decorator can only be applied to object-only union types with unique member types. (CodeDescription: none) |@discriminator('type')|
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
//@[02:043) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\n  *: typeA | typeA|
  *: typeA | typeA
}

type discriminatorInlineAdditionalPropsBadType3 = {
  @discriminator('type')
//@[02:024) [BCP363 (Error)] The "discriminator" decorator can only be applied to object-only union types with unique member types. (CodeDescription: none) |@discriminator('type')|
  *: string
}

type discriminatorInlineAdditionalPropsCycle1 = {
//@[05:045) [BCP298 (Error)] This type definition includes itself as required component, which creates a constraint that cannot be fulfilled. (CodeDescription: none) |discriminatorInlineAdditionalPropsCycle1|
  type: 'b'
  @discriminator('type')
//@[02:078) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\n  *: typeA | discriminatorInlineAdditionalPropsCycle1|
  *: typeA | discriminatorInlineAdditionalPropsCycle1
}

@discriminator('type')
//@[00:131) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype discriminatedUnionDuplicateMemberInsensitive = { type: 'a', value: string } | { type: 'A', value: int }|
type discriminatedUnionDuplicateMemberInsensitive = { type: 'a', value: string } | { type: 'A', value: int }

@discriminator('TYPE')
//@[00:131) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('TYPE')\ntype discriminatedUnionCaseSensitiveDiscriminator = { type: 'a', value: string } | { type: 'b', value: int }|
type discriminatedUnionCaseSensitiveDiscriminator = { type: 'a', value: string } | { type: 'b', value: int }

@discriminator('type')
//@[00:022) [BCP363 (Error)] The "discriminator" decorator can only be applied to object-only union types with unique member types. (CodeDescription: none) |@discriminator('type')|
param discriminatorParamBadType1 typeA
//@[06:032) [no-unused-params (Warning)] Parameter "discriminatorParamBadType1" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |discriminatorParamBadType1|

@discriminator('type')
//@[00:065) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\nparam discriminatorParamBadType2 'a' | 'b'|
param discriminatorParamBadType2 'a' | 'b'
//@[06:032) [no-unused-params (Warning)] Parameter "discriminatorParamBadType2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |discriminatorParamBadType2|

@discriminator('type')
//@[00:022) [BCP363 (Error)] The "discriminator" decorator can only be applied to object-only union types with unique member types. (CodeDescription: none) |@discriminator('type')|
output discriminatorOutputBadType1 typeA = { type: 'a', value: 'a' }

@discriminator('type')
//@[00:022) [BCP363 (Error)] The "discriminator" decorator can only be applied to object-only union types with unique member types. (CodeDescription: none) |@discriminator('type')|
output discriminatorOutputBadType2 object = { prop: 'value' }

// BEGIN: valid tagged unions baselines; move this back to TypeDeclarations_LF when backend updates are released and uncomment typesA-D
//type typeA = {
//  type: 'a'
//  value: string
//}
//
//type typeB = {
//  type: 'b'
//  value: int
//}
//
//type typeC = {
//  type: 'c'
//  value: bool
//  value2: string
//}
//
//type typeD = {
//  type: 'd'
//  value: object
//}

type typeH = {
  type: 'h'
  value: 'a' | 'b'
}

type typeI = {
  type: 'i'
  *: string
}

@discriminator('type')
//@[00:063) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype discriminatedUnion1 = typeA | typeB|
type discriminatedUnion1 = typeA | typeB

@discriminator('type')
//@[00:107) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype discriminatedUnion2 = { type: 'c', value: string } | { type: 'd', value: bool }|
type discriminatedUnion2 = { type: 'c', value: string } | { type: 'd', value: bool }

@discriminator('type')
//@[00:122) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype discriminatedUnion3 = discriminatedUnion1 | discriminatedUnion2 | { type: 'e', value: string }|
type discriminatedUnion3 = discriminatedUnion1 | discriminatedUnion2 | { type: 'e', value: string }

@discriminator('type')
//@[00:101) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype discriminatedUnion4 = discriminatedUnion1 | (discriminatedUnion2 | typeH)|
//@[00:101) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype discriminatedUnion4 = discriminatedUnion1 | (discriminatedUnion2 | typeH)|
type discriminatedUnion4 = discriminatedUnion1 | (discriminatedUnion2 | typeH)

@discriminator('type')
//@[00:066) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype discriminatedUnion5 = (typeA | typeB)?|
type discriminatedUnion5 = (typeA | typeB)?

type inlineDiscriminatedUnion1 = {
  @discriminator('type')
//@[02:046) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\n  prop: typeA | typeC|
  prop: typeA | typeC
}

type inlineDiscriminatedUnion2 = {
  @discriminator('type')
//@[02:067) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\n  prop: { type: 'a', value: bool } | typeB|
  prop: { type: 'a', value: bool } | typeB
}

@discriminator('type')
//@[00:232) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype inlineDiscriminatedUnion3 = {\n  type: 'a'\n  @discriminator('type')\n  prop: { type: 'a', value: bool } | typeB\n} | {\n  type: 'b'\n  @discriminator('type')\n  prop: discriminatedUnion1 | discriminatedUnion2\n}|
type inlineDiscriminatedUnion3 = {
  type: 'a'
  @discriminator('type')
//@[02:067) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\n  prop: { type: 'a', value: bool } | typeB|
  prop: { type: 'a', value: bool } | typeB
} | {
  type: 'b'
  @discriminator('type')
//@[02:074) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\n  prop: discriminatedUnion1 | discriminatedUnion2|
  prop: discriminatedUnion1 | discriminatedUnion2
}

type inlineDiscriminatedUnion4 = {
  @discriminator('type')
//@[02:049) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\n  prop: (typeA | typeC)?|
  prop: (typeA | typeC)?
}

type discriminatorUnionAsPropertyType = {
  prop1: discriminatedUnion1
//@[09:028) [BCP062 (Error)] The referenced declaration with name "discriminatedUnion1" is not valid. (CodeDescription: none) |discriminatedUnion1|
  prop2: discriminatedUnion3
//@[09:028) [BCP062 (Error)] The referenced declaration with name "discriminatedUnion3" is not valid. (CodeDescription: none) |discriminatedUnion3|
}

type discriminatedUnionInlineAdditionalProps1 = {
  @discriminator('type')
//@[02:043) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\n  *: typeA | typeB|
  *: typeA | typeB
}

type discriminatedUnionInlineAdditionalProps2 = {
  @discriminator('type')
//@[02:046) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\n  *: (typeA | typeB)?|
  *: (typeA | typeB)?
}

@discriminator('type')
//@[00:111) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype discriminatorMemberHasAdditionalProperties1 = typeA | typeI | { type: 'g', *: int }|
type discriminatorMemberHasAdditionalProperties1 = typeA | typeI | { type: 'g', *: int }

@discriminator('type')
//@[00:137) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\ntype discriminatorInnerSelfOptionalCycle1 = typeA | {\n  type: 'b'\n  value: discriminatorInnerSelfOptionalCycle1?\n}|
type discriminatorInnerSelfOptionalCycle1 = typeA | {
  type: 'b'
  value: discriminatorInnerSelfOptionalCycle1?
//@[09:045) [BCP062 (Error)] The referenced declaration with name "discriminatorInnerSelfOptionalCycle1" is not valid. (CodeDescription: none) |discriminatorInnerSelfOptionalCycle1|
}

type discriminatedUnionMemberOptionalCycle1 = {
  type: 'b'
  @discriminator('type')
//@[02:082) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\n  prop: (typeA | discriminatedUnionMemberOptionalCycle1)?|
  prop: (typeA | discriminatedUnionMemberOptionalCycle1)?
}

type discriminatedUnionTuple1 = [
  discriminatedUnion1
//@[02:021) [BCP062 (Error)] The referenced declaration with name "discriminatedUnion1" is not valid. (CodeDescription: none) |discriminatedUnion1|
  string
]

type discriminatedUnionInlineTuple1 = [
  @discriminator('type')
//@[02:071) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\n  typeA | typeB | { type: 'c', value: object }|
  typeA | typeB | { type: 'c', value: object }
  string
]

param paramDiscriminatedUnionTypeAlias1 discriminatedUnion1
//@[06:039) [no-unused-params (Warning)] Parameter "paramDiscriminatedUnionTypeAlias1" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |paramDiscriminatedUnionTypeAlias1|
//@[40:059) [BCP062 (Error)] The referenced declaration with name "discriminatedUnion1" is not valid. (CodeDescription: none) |discriminatedUnion1|
param paramDiscriminatedUnionTypeAlias2 discriminatedUnion5
//@[06:039) [no-unused-params (Warning)] Parameter "paramDiscriminatedUnionTypeAlias2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |paramDiscriminatedUnionTypeAlias2|
//@[40:059) [BCP062 (Error)] The referenced declaration with name "discriminatedUnion5" is not valid. (CodeDescription: none) |discriminatedUnion5|

@discriminator('type')
//@[00:073) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\nparam paramInlineDiscriminatedUnion1 typeA | typeB|
param paramInlineDiscriminatedUnion1 typeA | typeB
//@[06:036) [no-unused-params (Warning)] Parameter "paramInlineDiscriminatedUnion1" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |paramInlineDiscriminatedUnion1|

@discriminator('type')
//@[00:101) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\nparam paramInlineDiscriminatedUnion2 (typeA | typeB) = { type: 'b', value: 0 }|
param paramInlineDiscriminatedUnion2 (typeA | typeB) = { type: 'b', value: 0 }
//@[06:036) [no-unused-params (Warning)] Parameter "paramInlineDiscriminatedUnion2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |paramInlineDiscriminatedUnion2|

@discriminator('type')
//@[00:076) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\nparam paramInlineDiscriminatedUnion3 (typeA | typeB)?|
param paramInlineDiscriminatedUnion3 (typeA | typeB)?
//@[06:036) [no-unused-params (Warning)] Parameter "paramInlineDiscriminatedUnion3" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |paramInlineDiscriminatedUnion3|

output outputDiscriminatedUnionTypeAlias1 discriminatedUnion1 = { type: 'a', value: 'str' }
//@[42:061) [BCP062 (Error)] The referenced declaration with name "discriminatedUnion1" is not valid. (CodeDescription: none) |discriminatedUnion1|
@discriminator('type')
output outputDiscriminatedUnionTypeAlias2 discriminatedUnion1 = { type: 'a', value: 'str' }
//@[42:061) [BCP062 (Error)] The referenced declaration with name "discriminatedUnion1" is not valid. (CodeDescription: none) |discriminatedUnion1|
output outputDiscriminatedUnionTypeAlias3 discriminatedUnion5 = null
//@[42:061) [BCP062 (Error)] The referenced declaration with name "discriminatedUnion5" is not valid. (CodeDescription: none) |discriminatedUnion5|

@discriminator('type')
//@[00:131) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\noutput outputInlineDiscriminatedUnion1 typeA | typeB | { type: 'c', value: int } = { type: 'a', value: 'a' }|
output outputInlineDiscriminatedUnion1 typeA | typeB | { type: 'c', value: int } = { type: 'a', value: 'a' }

@discriminator('type')
//@[00:131) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\noutput outputInlineDiscriminatedUnion2 typeA | typeB | ({ type: 'c', value: int }) = { type: 'c', value: 1 }|
output outputInlineDiscriminatedUnion2 typeA | typeB | ({ type: 'c', value: int }) = { type: 'c', value: 1 }

@discriminator('type')
//@[00:085) [BCP367 (Error)] The "tagged unions" feature is temporarily disabled. (CodeDescription: none) |@discriminator('type')\noutput outputInlineDiscriminatedUnion3 (typeA | typeB)? = null|
output outputInlineDiscriminatedUnion3 (typeA | typeB)? = null
// END: valid tagged unions baselines; move this back to TypeDeclarations_LF when backend updates are released and uncomment typesA-D

