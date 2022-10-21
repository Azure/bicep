type 44
//@[5:07) Type <error>. Type: any. Declaration start char: 0, length: 7

type noAssignment
//@[5:17) Type noAssignment. Type: any. Declaration start char: 0, length: 17

type incompleteAssignment =
//@[5:25) Type incompleteAssignment. Type: any. Declaration start char: 0, length: 27

type string = string
//@[5:11) Type string. Type: string. Declaration start char: 0, length: 20

@sealed()
type sealedString = string
//@[5:17) Type sealedString. Type: string. Declaration start char: 0, length: 36

type disallowedUnion = 'foo'|21
//@[5:20) Type disallowedUnion. Type: error. Declaration start char: 0, length: 31

type validStringLiteralUnion = 'foo'|'bar'|'baz'
//@[5:28) Type validStringLiteralUnion. Type: 'bar' | 'baz' | 'foo'. Declaration start char: 0, length: 48

type validUnionInvalidAddition = validStringLiteralUnion|10
//@[5:30) Type validUnionInvalidAddition. Type: error. Declaration start char: 0, length: 59

type invalidUnionInvalidAddition = disallowedUnion|true
//@[5:32) Type invalidUnionInvalidAddition. Type: error. Declaration start char: 0, length: 55

type nullLiteral = null
//@[5:16) Type nullLiteral. Type: error. Declaration start char: 0, length: 23

type unionOfNulls = null|null
//@[5:17) Type unionOfNulls. Type: error. Declaration start char: 0, length: 29

@minLength(3)
type lengthConstrainedInt = int
//@[5:25) Type lengthConstrainedInt. Type: int. Declaration start char: 0, length: 45

@minValue(3)
type valueConstrainedString = string
//@[5:27) Type valueConstrainedString. Type: string. Declaration start char: 0, length: 49

type tautology = tautology
//@[5:14) Type tautology. Type: error. Declaration start char: 0, length: 26

type tautologicalUnion = tautologicalUnion|'foo'
//@[5:22) Type tautologicalUnion. Type: error. Declaration start char: 0, length: 48

type tautologicalArray = tautologicalArray[]
//@[5:22) Type tautologicalArray. Type: error. Declaration start char: 0, length: 44

type directCycleStart = directCycleReturn
//@[5:21) Type directCycleStart. Type: error. Declaration start char: 0, length: 41

type directCycleReturn = directCycleStart
//@[5:22) Type directCycleReturn. Type: error. Declaration start char: 0, length: 41

type cycleRoot = connector
//@[5:14) Type cycleRoot. Type: error. Declaration start char: 0, length: 26

type connector = cycleBack
//@[5:14) Type connector. Type: error. Declaration start char: 0, length: 26

type cycleBack = cycleRoot
//@[5:14) Type cycleBack. Type: error. Declaration start char: 0, length: 26

type objectWithInvalidPropertyDecorators = {
//@[5:40) Type objectWithInvalidPropertyDecorators. Type: { fooProp: string, barProp: string, krispyProp: string }. Declaration start char: 0, length: 168
  @sealed()
  fooProp: string

  @secure()
  barProp: string

  @allowed(['snap', 'crackle', 'pop'])
  krispyProp: string
}

type objectWithInvalidRecursion = {
//@[5:31) Type objectWithInvalidRecursion. Type: error. Declaration start char: 0, length: 92
  requiredAndRecursiveProp: objectWithInvalidRecursion
}

@sealed()
param sealedStringParam string
//@[6:23) Parameter sealedStringParam. Type: string. Declaration start char: 0, length: 40

param disallowedUnionParam 'foo'|-99
//@[6:26) Parameter disallowedUnionParam. Type: error. Declaration start char: 0, length: 36

