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

