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
