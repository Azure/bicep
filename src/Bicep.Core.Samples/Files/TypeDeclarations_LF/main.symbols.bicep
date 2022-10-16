// @description('The foo type')
// @sealed()
type foo = {
//@[5:08) Type foo. Type: { stringProp: string, objectProp: { intProp: int, intArrayArrayProp: int[][] }, typeRefProp: bar, literalProp: 'literal', recursion: foo }. Declaration start char: 0, length: 261
  @minLength(3)
  @maxLength(10)
  // @description('A string property')
  stringProp: string

  objectProp: {
    @minValue(1)
    intProp: int

    intArrayArrayProp?: int [] []
  }

  typeRefProp: bar

  literalProp: 'literal'

  recursion: foo
}

@minLength(3)
// @description('An array of array of arrays of arrays of ints')
// @metadata({
//   examples: [
//     [[[[1]]], [[[2]]], [[[3]]]]
//   ]
// })
type bar = int[][][][]
//@[5:08) Type bar. Type: int[][][][]. Declaration start char: 0, length: 181

type aUnion = 'snap'|'crackle'|'pop'
//@[5:11) Type aUnion. Type: 'crackle' | 'pop' | 'snap'. Declaration start char: 0, length: 36

type expandedUnion = aUnion|'fizz'|'buzz'|'pop'
//@[5:18) Type expandedUnion. Type: 'buzz' | 'crackle' | 'fizz' | 'pop' | 'snap'. Declaration start char: 0, length: 47

type mixedArray = ('heffalump'|'woozle'|{ shape: '*', size: '*'}|10|-10|true|!true|null)[]
//@[5:15) Type mixedArray. Type: ('heffalump' | 'woozle' | -10 | 10 | false | null | true | { shape: '*', size: '*' })[]. Declaration start char: 0, length: 90

param inlineObjectParam {
//@[6:23) Parameter inlineObjectParam. Type: { foo: string, bar: 100 | 200 | 300 | 400 | 500, baz: bool }. Declaration start char: 0, length: 123
  foo: string
  bar: 100|200|300|400|500
  baz: bool
} = {
  foo: 'foo'
  bar: 300
  baz: false
}

