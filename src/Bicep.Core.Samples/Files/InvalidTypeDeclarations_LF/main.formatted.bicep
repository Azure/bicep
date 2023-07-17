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

  @allowed([ 'snap', 'crackle', 'pop' ])
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

type objectUnion = typeA | typeB

@discriminator()
type noDiscriminatorParam = typeA | typeB

@discriminator(true)
type wrongDiscriminatorParamType = typeA | typeB

@discriminator('nonexistent')
type discriminatorPropertyNotExistAtAll = typeA | typeB

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
type discriminatorMemberHasAdditionalProperties = typeA | typeE

@discriminator('type')
type discriminatorSelfCycle = typeA | discriminatorSelfCycle

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
