type 44
//@[05:07) TypeAlias <error>. Type: Type<any>. Declaration start char: 0, length: 7

type noAssignment
//@[05:17) TypeAlias noAssignment. Type: Type<any>. Declaration start char: 0, length: 17

type incompleteAssignment =
//@[05:25) TypeAlias incompleteAssignment. Type: Type<any>. Declaration start char: 0, length: 27

type resource = bool
//@[05:13) TypeAlias resource. Type: Type<bool>. Declaration start char: 0, length: 20

@sealed()
type sealedString = string
//@[05:17) TypeAlias sealedString. Type: Type<string>. Declaration start char: 0, length: 36

@sealed()
type sealedDictionary = {
//@[05:21) TypeAlias sealedDictionary. Type: error. Declaration start char: 0, length: 48
	*: string
}

type disallowedUnion = 'foo'|21
//@[05:20) TypeAlias disallowedUnion. Type: Type<'foo' | 21>. Declaration start char: 0, length: 31

type validStringLiteralUnion = 'foo'|'bar'|'baz'
//@[05:28) TypeAlias validStringLiteralUnion. Type: Type<'bar' | 'baz' | 'foo'>. Declaration start char: 0, length: 48

type validUnionInvalidAddition = validStringLiteralUnion|10
//@[05:30) TypeAlias validUnionInvalidAddition. Type: Type<'bar' | 'baz' | 'foo' | 10>. Declaration start char: 0, length: 59

type invalidUnionInvalidAddition = disallowedUnion|true
//@[05:32) TypeAlias invalidUnionInvalidAddition. Type: Type<'foo' | 21 | true>. Declaration start char: 0, length: 55

type nullLiteral = null
//@[05:16) TypeAlias nullLiteral. Type: error. Declaration start char: 0, length: 23

type unionOfNulls = null|null
//@[05:17) TypeAlias unionOfNulls. Type: Type<null>. Declaration start char: 0, length: 29

@minLength(3)
type lengthConstrainedInt = int
//@[05:25) TypeAlias lengthConstrainedInt. Type: Type<int>. Declaration start char: 0, length: 45

@minValue(3)
type valueConstrainedString = string
//@[05:27) TypeAlias valueConstrainedString. Type: Type<string>. Declaration start char: 0, length: 49

type tautology = tautology
//@[05:14) TypeAlias tautology. Type: error. Declaration start char: 0, length: 26

type tautologicalUnion = tautologicalUnion|'foo'
//@[05:22) TypeAlias tautologicalUnion. Type: error. Declaration start char: 0, length: 48

type tautologicalArray = tautologicalArray[]
//@[05:22) TypeAlias tautologicalArray. Type: Type<tautologicalArray[]>. Declaration start char: 0, length: 44

type directCycleStart = directCycleReturn
//@[05:21) TypeAlias directCycleStart. Type: error. Declaration start char: 0, length: 41

type directCycleReturn = directCycleStart
//@[05:22) TypeAlias directCycleReturn. Type: error. Declaration start char: 0, length: 41

type cycleRoot = connector
//@[05:14) TypeAlias cycleRoot. Type: error. Declaration start char: 0, length: 26

type connector = cycleBack
//@[05:14) TypeAlias connector. Type: error. Declaration start char: 0, length: 26

type cycleBack = cycleRoot
//@[05:14) TypeAlias cycleBack. Type: error. Declaration start char: 0, length: 26

type objectWithInvalidPropertyDecorators = {
//@[05:40) TypeAlias objectWithInvalidPropertyDecorators. Type: Type<{ fooProp: string, barProp: string, krispyProp: string }>. Declaration start char: 0, length: 168
  @sealed()
  fooProp: string

  @secure()
  barProp: string

  @allowed(['snap', 'crackle', 'pop'])
  krispyProp: string
}

type objectWithInvalidRecursion = {
//@[05:31) TypeAlias objectWithInvalidRecursion. Type: Type<{ requiredAndRecursiveProp: objectWithInvalidRecursion }>. Declaration start char: 0, length: 92
  requiredAndRecursiveProp: objectWithInvalidRecursion
}

type arrayWithInvalidMember = objectWithInvalidRecursion[]
//@[05:27) TypeAlias arrayWithInvalidMember. Type: Type<objectWithInvalidRecursion[]>. Declaration start char: 0, length: 58

@sealed()
param sealedStringParam string
//@[06:23) Parameter sealedStringParam. Type: string. Declaration start char: 0, length: 40

param disallowedUnionParam 'foo'|-99
//@[06:26) Parameter disallowedUnionParam. Type: 'foo' | -99. Declaration start char: 0, length: 36

param objectWithInvalidRecursionParam objectWithInvalidRecursion
//@[06:37) Parameter objectWithInvalidRecursionParam. Type: { requiredAndRecursiveProp: objectWithInvalidRecursion }. Declaration start char: 0, length: 64

func invalidArgs = (validStringLiteralUnion a, string b) => string a
//@[44:45) Local a. Type: 'bar' | 'baz' | 'foo'. Declaration start char: 20, length: 25
//@[54:55) Local b. Type: string. Declaration start char: 47, length: 8
//@[05:16) Function invalidArgs. Type: (('bar' | 'baz' | 'foo'), string) => ('bar' | 'baz' | 'foo'). Declaration start char: 0, length: 68

func invalidOutput = () => validStringLiteralUnion 'foo'
//@[05:18) Function invalidOutput. Type: () => 'foo'. Declaration start char: 0, length: 56

