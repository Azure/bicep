type 44

type noAssignment

type incompleteAssignment =

type resource = bool

@sealed()
type sealedString = string

@sealed()
type sealedDictionary = {
	*: string
}

type disallowedUnion = 'foo'|21

type validStringLiteralUnion = 'foo'|'bar'|'baz'

type validUnionInvalidAddition = validStringLiteralUnion|10

type invalidUnionInvalidAddition = disallowedUnion|true

type nullLiteral = null

type unionOfNulls = null|null

@minLength(3)
type lengthConstrainedInt = int

@minValue(3)
type valueConstrainedString = string

type tautology = tautology

type tautologicalUnion = tautologicalUnion|'foo'

type tautologicalArray = tautologicalArray[]

type directCycleStart = directCycleReturn

type directCycleReturn = directCycleStart

type cycleRoot = connector

type connector = cycleBack

type cycleBack = cycleRoot

type objectWithInvalidPropertyDecorators = {
  @sealed()
  fooProp: string

  @secure()
  barProp: string

  @allowed(['snap', 'crackle', 'pop'])
  krispyProp: string
}

type objectWithInvalidRecursion = {
  requiredAndRecursiveProp: objectWithInvalidRecursion
}

type arrayWithInvalidMember = objectWithInvalidRecursion[]

@sealed()
param sealedStringParam string

param disallowedUnionParam 'foo'|-99

param objectWithInvalidRecursionParam objectWithInvalidRecursion

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

@discriminator()
type noDiscriminatorParam = typeA | typeB

@discriminator(true)
type wrongDiscriminatorParamType = typeA | typeB

@discriminator('nonexistent')
type discriminatorPropertyNotExistAtAll = typeA | typeB

@discriminator('nonexistent')
type discriminatorPropertyMismatch = unionAB

@discriminator('type')
type discriminatorPropertyNotExistOnAtLeastOne = typeA | { value: bool }

@discriminator('type')
type discriminatorWithOnlyOneMember = typeA

@discriminator('type')
type discriminatorPropertyNotRequiredStringLiteral1 = typeA | typeF

@discriminator('type')
type discriminatorPropertyNotRequiredStringLiteral2 = typeA | typeG

@discriminator('type')
type discriminatorDuplicatedMember1 = typeA | typeA

@discriminator('type')
type discriminatorDuplicatedMember2 = typeA | { type: 'a', config: object }

@discriminator('type')
type discriminatorOnlyOneNonNullMember1 = typeA | null

@discriminator('type')
type discriminatorOnlyOneNonNullMember2 = (typeA)?

@discriminator('type')
type discriminatorMemberHasAdditionalProperties = typeA | typeE

@discriminator('type')
type discriminatorSelfCycle1 = typeA | discriminatorSelfCycle1

@discriminator('type')
type discriminatorSelfCycle2 = (typeA | discriminatorSelfCycle2)?

@discriminator('type')
type discriminatorTopLevelCycleA = typeA | discriminatorTopLevelCycleB
@discriminator('type')
type discriminatorTopLevelCycleB = typeB | discriminatorTopLevelCycleA

@discriminator('type')
type discriminatorInnerSelfCycle1 = typeA | {
  type: 'b'
  value: discriminatorInnerSelfCycle1
}

type discriminatorInnerSelfCycle2Helper = {
  type: 'b'
  value: discriminatorInnerSelfCycle2
}
@discriminator('type')
type discriminatorInnerSelfCycle2 = typeA | discriminatorInnerSelfCycle2Helper

@discriminator('type')
type discriminatorTupleBadType1 = [typeA, typeB]

@discriminator('type')
type discriminatorTupleBadType2 = [typeA | typeB]

@discriminator('type')
type discriminatorTupleBadType3 = [typeA | typeB, typeC | typeD]

type discriminatorInlineAdditionalPropsBadType1 = {
  @discriminator('type')
  *: typeA
}

type discriminatorInlineAdditionalPropsBadType2 = {
  @discriminator('type')
  *: typeA | typeA
}

type discriminatorInlineAdditionalPropsBadType3 = {
  @discriminator('type')
  *: string
}

type discriminatorInlineAdditionalPropsCycle1 = {
  type: 'b'
  @discriminator('type')
  *: typeA | discriminatorInlineAdditionalPropsCycle1
}

@discriminator('type')
type discriminatedUnionDuplicateMemberInsensitive = { type: 'a', value: string } | { type: 'A', value: int }

@discriminator('TYPE')
type discriminatedUnionCaseSensitiveDiscriminator = { type: 'a', value: string } | { type: 'b', value: int }

@discriminator('type')
param discriminatorParamBadType1 typeA

@discriminator('type')
param discriminatorParamBadType2 'a' | 'b'

@discriminator('type')
output discriminatorOutputBadType1 typeA = { type: 'a', value: 'a' }

@discriminator('type')
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
type discriminatedUnion1 = typeA | typeB

@discriminator('type')
type discriminatedUnion2 = { type: 'c', value: string } | { type: 'd', value: bool }

@discriminator('type')
type discriminatedUnion3 = discriminatedUnion1 | discriminatedUnion2 | { type: 'e', value: string }

@discriminator('type')
type discriminatedUnion4 = discriminatedUnion1 | (discriminatedUnion2 | typeH)

@discriminator('type')
type discriminatedUnion5 = (typeA | typeB)?

type inlineDiscriminatedUnion1 = {
  @discriminator('type')
  prop: typeA | typeC
}

type inlineDiscriminatedUnion2 = {
  @discriminator('type')
  prop: { type: 'a', value: bool } | typeB
}

@discriminator('type')
type inlineDiscriminatedUnion3 = {
  type: 'a'
  @discriminator('type')
  prop: { type: 'a', value: bool } | typeB
} | {
  type: 'b'
  @discriminator('type')
  prop: discriminatedUnion1 | discriminatedUnion2
}

type inlineDiscriminatedUnion4 = {
  @discriminator('type')
  prop: (typeA | typeC)?
}

type discriminatorUnionAsPropertyType = {
  prop1: discriminatedUnion1
  prop2: discriminatedUnion3
}

type discriminatedUnionInlineAdditionalProps1 = {
  @discriminator('type')
  *: typeA | typeB
}

type discriminatedUnionInlineAdditionalProps2 = {
  @discriminator('type')
  *: (typeA | typeB)?
}

@discriminator('type')
type discriminatorMemberHasAdditionalProperties1 = typeA | typeI | { type: 'g', *: int }

@discriminator('type')
type discriminatorInnerSelfOptionalCycle1 = typeA | {
  type: 'b'
  value: discriminatorInnerSelfOptionalCycle1?
}

type discriminatedUnionMemberOptionalCycle1 = {
  type: 'b'
  @discriminator('type')
  prop: (typeA | discriminatedUnionMemberOptionalCycle1)?
}

type discriminatedUnionTuple1 = [
  discriminatedUnion1
  string
]

type discriminatedUnionInlineTuple1 = [
  @discriminator('type')
  typeA | typeB | { type: 'c', value: object }
  string
]

param paramDiscriminatedUnionTypeAlias1 discriminatedUnion1
param paramDiscriminatedUnionTypeAlias2 discriminatedUnion5

@discriminator('type')
param paramInlineDiscriminatedUnion1 typeA | typeB

@discriminator('type')
param paramInlineDiscriminatedUnion2 (typeA | typeB) = { type: 'b', value: 0 }

@discriminator('type')
param paramInlineDiscriminatedUnion3 (typeA | typeB)?

output outputDiscriminatedUnionTypeAlias1 discriminatedUnion1 = { type: 'a', value: 'str' }
@discriminator('type')
output outputDiscriminatedUnionTypeAlias2 discriminatedUnion1 = { type: 'a', value: 'str' }
output outputDiscriminatedUnionTypeAlias3 discriminatedUnion5 = null

@discriminator('type')
output outputInlineDiscriminatedUnion1 typeA | typeB | { type: 'c', value: int } = { type: 'a', value: 'a' }

@discriminator('type')
output outputInlineDiscriminatedUnion2 typeA | typeB | ({ type: 'c', value: int }) = { type: 'c', value: 1 }

@discriminator('type')
output outputInlineDiscriminatedUnion3 (typeA | typeB)? = null
// END: valid tagged unions baselines; move this back to TypeDeclarations_LF when backend updates are released and uncomment typesA-D
