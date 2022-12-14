@description('The foo type')
@sealed()
type foo = {
//@[5:08) TypeAlias foo. Type: Type<{ stringProp: string, objectProp: { intProp: int, intArrayArrayProp?: int[][] }, typeRefProp: bar, literalProp: 'literal', recursion?: foo }>. Declaration start char: 0, length: 298
  @minLength(3)
  @maxLength(10)
  @description('A string property')
  stringProp: string

  objectProp: {
    @minValue(1)
    intProp: int

    intArrayArrayProp?: int [] []
  }

  typeRefProp: bar

  literalProp: 'literal'

  recursion?: foo
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

type tupleUnion = ['foo', 'bar', 'baz']|['fizz', 'buzz']|['snap', 'crackle', 'pop']
//@[5:15) TypeAlias tupleUnion. Type: Type<['fizz', 'buzz'] | ['foo', 'bar', 'baz'] | ['snap', 'crackle', 'pop']>. Declaration start char: 0, length: 83

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

type tuple = [
//@[5:10) TypeAlias tuple. Type: Type<[string, bar]>. Declaration start char: 0, length: 129
    @description('A leading string')
    string

    @description('A second element using a type alias')
    bar
]
