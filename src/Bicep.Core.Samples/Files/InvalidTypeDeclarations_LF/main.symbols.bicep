type 44
//@[5:07) TypeAlias <error>. Type: Type<any>. Declaration start char: 0, length: 7

type noAssignment
//@[5:17) TypeAlias noAssignment. Type: Type<any>. Declaration start char: 0, length: 17

type incompleteAssignment =
//@[5:25) TypeAlias incompleteAssignment. Type: Type<any>. Declaration start char: 0, length: 27

type resource = bool
//@[5:13) TypeAlias resource. Type: Type<bool>. Declaration start char: 0, length: 20

@sealed()
type sealedString = string
//@[5:17) TypeAlias sealedString. Type: Type<string>. Declaration start char: 0, length: 36

@sealed()
type sealedDictionary = {
//@[5:21) TypeAlias sealedDictionary. Type: error. Declaration start char: 0, length: 48
	*: string
}

type disallowedUnion = 'foo'|21
//@[5:20) TypeAlias disallowedUnion. Type: Type<'foo' | 21>. Declaration start char: 0, length: 31

type validStringLiteralUnion = 'foo'|'bar'|'baz'
//@[5:28) TypeAlias validStringLiteralUnion. Type: Type<'bar' | 'baz' | 'foo'>. Declaration start char: 0, length: 48

type validUnionInvalidAddition = validStringLiteralUnion|10
//@[5:30) TypeAlias validUnionInvalidAddition. Type: Type<'bar' | 'baz' | 'foo' | 10>. Declaration start char: 0, length: 59

type invalidUnionInvalidAddition = disallowedUnion|true
//@[5:32) TypeAlias invalidUnionInvalidAddition. Type: Type<'foo' | 21 | true>. Declaration start char: 0, length: 55

type nullLiteral = null
//@[5:16) TypeAlias nullLiteral. Type: error. Declaration start char: 0, length: 23

type unionOfNulls = null|null
//@[5:17) TypeAlias unionOfNulls. Type: Type<null>. Declaration start char: 0, length: 29

@minLength(3)
type lengthConstrainedInt = int
//@[5:25) TypeAlias lengthConstrainedInt. Type: Type<int>. Declaration start char: 0, length: 45

@minValue(3)
type valueConstrainedString = string
//@[5:27) TypeAlias valueConstrainedString. Type: Type<string>. Declaration start char: 0, length: 49

type tautology = tautology
//@[5:14) TypeAlias tautology. Type: error. Declaration start char: 0, length: 26

type tautologicalUnion = tautologicalUnion|'foo'
//@[5:22) TypeAlias tautologicalUnion. Type: error. Declaration start char: 0, length: 48

type tautologicalArray = tautologicalArray[]
//@[5:22) TypeAlias tautologicalArray. Type: Type<tautologicalArray[]>. Declaration start char: 0, length: 44

type directCycleStart = directCycleReturn
//@[5:21) TypeAlias directCycleStart. Type: error. Declaration start char: 0, length: 41

type directCycleReturn = directCycleStart
//@[5:22) TypeAlias directCycleReturn. Type: error. Declaration start char: 0, length: 41

type cycleRoot = connector
//@[5:14) TypeAlias cycleRoot. Type: error. Declaration start char: 0, length: 26

type connector = cycleBack
//@[5:14) TypeAlias connector. Type: error. Declaration start char: 0, length: 26

type cycleBack = cycleRoot
//@[5:14) TypeAlias cycleBack. Type: error. Declaration start char: 0, length: 26

type objectWithInvalidPropertyDecorators = {
//@[5:40) TypeAlias objectWithInvalidPropertyDecorators. Type: Type<{ fooProp: string, barProp: string, krispyProp: string }>. Declaration start char: 0, length: 168
  @sealed()
  fooProp: string

  @secure()
  barProp: string

  @allowed(['snap', 'crackle', 'pop'])
  krispyProp: string
}

type objectWithInvalidRecursion = {
//@[5:31) TypeAlias objectWithInvalidRecursion. Type: Type<{ requiredAndRecursiveProp: objectWithInvalidRecursion }>. Declaration start char: 0, length: 92
  requiredAndRecursiveProp: objectWithInvalidRecursion
}

type arrayWithInvalidMember = objectWithInvalidRecursion[]
//@[5:27) TypeAlias arrayWithInvalidMember. Type: Type<objectWithInvalidRecursion[]>. Declaration start char: 0, length: 58

@sealed()
param sealedStringParam string
//@[6:23) Parameter sealedStringParam. Type: string. Declaration start char: 0, length: 40

param disallowedUnionParam 'foo'|-99
//@[6:26) Parameter disallowedUnionParam. Type: 'foo' | -99. Declaration start char: 0, length: 36

param objectWithInvalidRecursionParam objectWithInvalidRecursion
//@[6:37) Parameter objectWithInvalidRecursionParam. Type: { requiredAndRecursiveProp: objectWithInvalidRecursion }. Declaration start char: 0, length: 64

type typeA = {
//@[5:10) TypeAlias typeA. Type: Type<{ type: 'a', value: string }>. Declaration start char: 0, length: 44
  type: 'a'
  value: string
}

type typeB = {
//@[5:10) TypeAlias typeB. Type: Type<{ type: 'b', value: int }>. Declaration start char: 0, length: 41
  type: 'b'
  value: int
}

@discriminator('type')
type unionAB = typeA | typeB
//@[5:12) TypeAlias unionAB. Type: Type<{ type: 'a', value: string } | { type: 'b', value: int }>. Declaration start char: 0, length: 51

type typeC = {
//@[5:10) TypeAlias typeC. Type: Type<{ type: 'c', value: bool, value2: string }>. Declaration start char: 0, length: 59
  type: 'c'
  value: bool
  value2: string
}

type typeD = {
//@[5:10) TypeAlias typeD. Type: Type<{ type: 'd', value: object }>. Declaration start char: 0, length: 44
  type: 'd'
  value: object
}

type typeE = {
//@[5:10) TypeAlias typeE. Type: Type<{ type: 'e', *: string }>. Declaration start char: 0, length: 40
  type: 'e'
  *: string
}

type typeF = {
//@[5:10) TypeAlias typeF. Type: Type<{ type: 0, value: string }>. Declaration start char: 0, length: 42
  type: 0
  value: string
}

type typeG = {
//@[5:10) TypeAlias typeG. Type: Type<{ type: 'g' | null, value: string }>. Declaration start char: 0, length: 45
  type: 'g'?
  value: string
}

type objectUnion = typeA | typeB
//@[5:16) TypeAlias objectUnion. Type: Type<{ type: 'a', value: string } | { type: 'b', value: int }>. Declaration start char: 0, length: 32

@discriminator()
type noDiscriminatorParam = typeA | typeB
//@[5:25) TypeAlias noDiscriminatorParam. Type: error. Declaration start char: 0, length: 58

@discriminator(true)
type wrongDiscriminatorParamType = typeA | typeB
//@[5:32) TypeAlias wrongDiscriminatorParamType. Type: error. Declaration start char: 0, length: 69

@discriminator('nonexistent')
type discriminatorPropertyNotExistAtAll = typeA | typeB
//@[5:39) TypeAlias discriminatorPropertyNotExistAtAll. Type: error. Declaration start char: 0, length: 85

@discriminator('type')
type discriminatorPropertyNotExistOnAtLeastOne = typeA | { value: bool }
//@[5:46) TypeAlias discriminatorPropertyNotExistOnAtLeastOne. Type: error. Declaration start char: 0, length: 95

@discriminator('type')
type discriminatorWithOnlyOneMember = typeA
//@[5:35) TypeAlias discriminatorWithOnlyOneMember. Type: Type<{ type: 'a', value: string }>. Declaration start char: 0, length: 66

@discriminator('type')
type discriminatorPropertyNotRequiredStringLiteral1 = typeA | typeF
//@[5:51) TypeAlias discriminatorPropertyNotRequiredStringLiteral1. Type: error. Declaration start char: 0, length: 90

@discriminator('type')
type discriminatorPropertyNotRequiredStringLiteral2 = typeA | typeG
//@[5:51) TypeAlias discriminatorPropertyNotRequiredStringLiteral2. Type: error. Declaration start char: 0, length: 90

@discriminator('type')
type discriminatorDuplicatedMember1 = typeA | typeA
//@[5:35) TypeAlias discriminatorDuplicatedMember1. Type: error. Declaration start char: 0, length: 74

@discriminator('type')
type discriminatorDuplicatedMember2 = typeA | { type: 'a', config: object }
//@[5:35) TypeAlias discriminatorDuplicatedMember2. Type: error. Declaration start char: 0, length: 98

@discriminator('type')
type discriminatorSelfCycle = typeA | discriminatorSelfCycle
//@[5:27) TypeAlias discriminatorSelfCycle. Type: error. Declaration start char: 0, length: 83

@discriminator('type')
type discriminatorTopLevelCycleA = typeA | discriminatorTopLevelCycleB
//@[5:32) TypeAlias discriminatorTopLevelCycleA. Type: error. Declaration start char: 0, length: 93
@discriminator('type')
type discriminatorTopLevelCycleB = typeB | discriminatorTopLevelCycleA
//@[5:32) TypeAlias discriminatorTopLevelCycleB. Type: error. Declaration start char: 0, length: 93

@discriminator('type')
type discriminatorInnerSelfCycle1 = typeA | {
//@[5:33) TypeAlias discriminatorInnerSelfCycle1. Type: Type<{ type: 'a', value: string } | { type: 'b', value: discriminatorInnerSelfCycle1 }>. Declaration start char: 0, length: 120
  type: 'b'
  value: discriminatorInnerSelfCycle1
}

type discriminatorInnerSelfCycle2Helper = {
//@[5:39) TypeAlias discriminatorInnerSelfCycle2Helper. Type: Type<{ type: 'b', value: discriminatorInnerSelfCycle2 }>. Declaration start char: 0, length: 95
  type: 'b'
  value: discriminatorInnerSelfCycle2
}
@discriminator('type')
type discriminatorInnerSelfCycle2 = typeA | discriminatorInnerSelfCycle2Helper
//@[5:33) TypeAlias discriminatorInnerSelfCycle2. Type: Type<{ type: 'a', value: string } | { type: 'b', value: discriminatorInnerSelfCycle2 }>. Declaration start char: 0, length: 101

