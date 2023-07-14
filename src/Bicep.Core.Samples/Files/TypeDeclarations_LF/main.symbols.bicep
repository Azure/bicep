@description('The foo type')
@sealed()
type foo = {
//@[5:08) TypeAlias foo. Type: Type<{ stringProp: string, objectProp: { intProp: int, intArrayArrayProp: int[][] | null }, typeRefProp: bar, literalProp: 'literal', recursion: foo? }>. Declaration start char: 0, length: 299
  @minLength(3)
  @maxLength(10)
  @description('A string property')
  stringProp: string

  objectProp: {
    @minValue(1)
    intProp: int

    intArrayArrayProp: int [] [] ?
  }

  typeRefProp: bar

  literalProp: 'literal'

  recursion: foo?
}

@minLength(3)
@description('An array of array of arrays of arrays of ints')
@metadata({
  examples: [
    [[[[1]]], [[[2]]], [[[3]]]]
  ]
})
type bar = int[][][][]
//@[5:08) TypeAlias bar. Type: Type<int[][][][]>. Declaration start char: 0, length: 163

type aUnion = 'snap'|'crackle'|'pop'
//@[5:11) TypeAlias aUnion. Type: Type<'crackle' | 'pop' | 'snap'>. Declaration start char: 0, length: 36

type expandedUnion = aUnion|'fizz'|'buzz'|'pop'
//@[5:18) TypeAlias expandedUnion. Type: Type<'buzz' | 'crackle' | 'fizz' | 'pop' | 'snap'>. Declaration start char: 0, length: 47

type tupleUnion = ['foo', 'bar', 'baz']
//@[5:15) TypeAlias tupleUnion. Type: Type<['fizz', 'buzz'] | ['foo', 'bar', 'baz'] | ['snap', 'crackle', 'pop']>. Declaration start char: 0, length: 85
|['fizz', 'buzz']
|['snap', 'crackle', 'pop']

type mixedArray = ('heffalump'|'woozle'|{ shape: '*', size: '*'}|10|-10|true|!true|null)[]
//@[5:15) TypeAlias mixedArray. Type: Type<('heffalump' | 'woozle' | -10 | 10 | false | null | true | { shape: '*', size: '*' })[]>. Declaration start char: 0, length: 90

type bool = string
//@[5:09) TypeAlias bool. Type: Type<string>. Declaration start char: 0, length: 18

param inlineObjectParam {
//@[6:23) Parameter inlineObjectParam. Type: { foo: string, bar: 100 | 200 | 300 | 400 | 500, baz: bool }. Declaration start char: 0, length: 127
  foo: string
  bar: 100|200|300|400|500
  baz: sys.bool
} = {
  foo: 'foo'
  bar: 300
  baz: false
}

param unionParam {property: 'ping'}|{property: 'pong'} = {property: 'pong'}
//@[6:16) Parameter unionParam. Type: { property: 'ping' } | { property: 'pong' }. Declaration start char: 0, length: 75

param paramUsingType mixedArray
//@[6:20) Parameter paramUsingType. Type: ('heffalump' | 'woozle' | -10 | 10 | false | null | true | { shape: '*', size: '*' })[]. Declaration start char: 0, length: 31

output outputUsingType mixedArray = paramUsingType
//@[7:22) Output outputUsingType. Type: ('heffalump' | 'woozle' | -10 | 10 | false | null | true | { shape: '*', size: '*' })[]. Declaration start char: 0, length: 50

type tuple = [
//@[5:10) TypeAlias tuple. Type: Type<[string, bar]>. Declaration start char: 0, length: 129
    @description('A leading string')
    string

    @description('A second element using a type alias')
    bar
]

type stringStringDictionary = {
//@[5:27) TypeAlias stringStringDictionary. Type: Type<{ *: string }>. Declaration start char: 0, length: 47
    *: string
}

@minValue(1)
@maxValue(10)
type constrainedInt = int
//@[5:19) TypeAlias constrainedInt. Type: Type<int>. Declaration start char: 0, length: 52

param mightIncludeNull ({key: 'value'} | null)[]
//@[6:22) Parameter mightIncludeNull. Type: (null | { key: 'value' })[]. Declaration start char: 0, length: 48

var nonNull = mightIncludeNull[0]!.key
//@[4:11) Variable nonNull. Type: 'value'. Declaration start char: 0, length: 38

output nonNull string = nonNull
//@[7:14) Output nonNull. Type: string. Declaration start char: 0, length: 31

var maybeNull = mightIncludeNull[0].?key
//@[4:13) Variable maybeNull. Type: 'value' | null. Declaration start char: 0, length: 40

output maybeNull string? = maybeNull
//@[7:16) Output maybeNull. Type: null | string. Declaration start char: 0, length: 36

type nullable = string?
//@[5:13) TypeAlias nullable. Type: Type<null | string>. Declaration start char: 0, length: 23

type nonNullable = nullable!
//@[5:16) TypeAlias nonNullable. Type: Type<string>. Declaration start char: 0, length: 28

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

@discriminator('type')
type discriminatedUnion1 = typeA | typeB
//@[5:24) TypeAlias discriminatedUnion1. Type: Type<{ type: 'a', value: string } | { type: 'b', value: int }>. Declaration start char: 0, length: 63

@discriminator('type')
type discriminatedUnion2 = { type: 'c', value: string } | { type: 'd', value: bool }
//@[5:24) TypeAlias discriminatedUnion2. Type: Type<{ type: 'c', value: string } | { type: 'd', value: bool }>. Declaration start char: 0, length: 107

@discriminator('type')
type discriminatedUnion3 = discriminatedUnion1 | discriminatedUnion2 | { type: 'e', *: string }
//@[5:24) TypeAlias discriminatedUnion3. Type: Type<{ type: 'a', value: string } | { type: 'b', value: int } | { type: 'c', value: string } | { type: 'd', value: bool } | { type: 'e', *: string }>. Declaration start char: 0, length: 118

@discriminator('type')
type discriminatedUnion4 = discriminatedUnion1 | (discriminatedUnion2 | typeE)
//@[5:24) TypeAlias discriminatedUnion4. Type: Type<{ type: 'a', value: string } | { type: 'b', value: int } | { type: 'c', value: string } | { type: 'd', value: bool } | { type: 'e', *: string }>. Declaration start char: 0, length: 101

type inlineDiscriminatedUnion1 = {
//@[5:30) TypeAlias inlineDiscriminatedUnion1. Type: Type<{ prop: typeA | typeC }>. Declaration start char: 0, length: 83
  @discriminator('type')
  prop: typeA | typeC
}

type inlineDiscriminatedUnion2 = {
//@[5:30) TypeAlias inlineDiscriminatedUnion2. Type: Type<{ prop: { type: 'a', value: bool } | typeB }>. Declaration start char: 0, length: 104
  @discriminator('type')
  prop: { type: 'a', value: bool } | typeB
}

@discriminator('type')
type inlineDiscriminatedUnion3 = {
//@[5:30) TypeAlias inlineDiscriminatedUnion3. Type: Type<{ type: 'a', prop: { type: 'a', value: bool } | typeB } | { type: 'b', prop: discriminatedUnion1 | discriminatedUnion2 }>. Declaration start char: 0, length: 232
  type: 'a'
  @discriminator('type')
  prop: { type: 'a', value: bool } | typeB
} | {
  type: 'b'
  @discriminator('type')
  prop: discriminatedUnion1 | discriminatedUnion2
}

type discriminatorUnionAsPropertyType = {
//@[5:37) TypeAlias discriminatorUnionAsPropertyType. Type: Type<{ prop1: discriminatedUnion1, prop2: discriminatedUnion3 }>. Declaration start char: 0, length: 101
  prop1: discriminatedUnion1
  prop2: discriminatedUnion3
}

@discriminator('type')
type discriminatorInnerSelfOptionalCycle1 = typeA | {
//@[5:41) TypeAlias discriminatorInnerSelfOptionalCycle1. Type: Type<{ type: 'a', value: string } | { type: 'b', value: discriminatorInnerSelfOptionalCycle1? }>. Declaration start char: 0, length: 137
  type: 'b'
  value: discriminatorInnerSelfOptionalCycle1?
}
