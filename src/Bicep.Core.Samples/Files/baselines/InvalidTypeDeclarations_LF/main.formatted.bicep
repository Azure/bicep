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

type disallowedUnion = 'foo' | 21

type validStringLiteralUnion = 'foo' | 'bar' | 'baz'

type validUnionInvalidAddition = validStringLiteralUnion | 10

type invalidUnionInvalidAddition = disallowedUnion | true

type nullLiteral = null

type unionOfNulls = null | null

@minLength(3)
type lengthConstrainedInt = int

@minValue(3)
type valueConstrainedString = string

type tautology = tautology

type tautologicalUnion = tautologicalUnion | 'foo'

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

param disallowedUnionParam 'foo' | -99

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

type primitiveUnion = bool | bool

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
type discriminatorInnerSelfCycle1 =
  | typeA
  | {
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

type strings = string[]

type invalidTupleAccess = strings[0]

type stringTuple = [string, string]

type invalidItemTypeAccess = stringTuple[*]

type anObject = {
  property: string
}

type invalidAdditionalPropertiesAccess = anObject.*

type stringDict = {
  *: string
}

type invalidPropertyAccess = stringDict.property
