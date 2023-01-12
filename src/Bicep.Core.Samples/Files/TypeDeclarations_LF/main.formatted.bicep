@description('The foo type')
@sealed()
type foo = {
  @minLength(3)
  @maxLength(10)
  @description('A string property')
  stringProp: string

  objectProp: {
    @minValue(1)
    intProp: int

    intArrayArrayProp: int[][]?
  }

  typeRefProp: bar

  literalProp: 'literal'

  recursion: foo?
}

@minLength(3)
@description('An array of array of arrays of arrays of ints')
@metadata({
  examples: [
    [ [ [ [ 1 ] ] ], [ [ [ 2 ] ] ], [ [ [ 3 ] ] ] ]
  ]
})
type bar = int[][][][]

type aUnion = 'snap' | 'crackle' | 'pop'

type expandedUnion = aUnion | 'fizz' | 'buzz' | 'pop'

type tupleUnion = [ 'foo', 'bar', 'baz' ]
  | [ 'fizz', 'buzz' ]
  | [ 'snap', 'crackle', 'pop' ]

type mixedArray = ('heffalump' | 'woozle' | { shape: '*', size: '*' } | 10 | -10 | true | !true | null)[]

type bool = string

param inlineObjectParam {
  foo: string
  bar: 100 | 200 | 300 | 400 | 500
  baz: sys.bool
} = {
  foo: 'foo'
  bar: 300
  baz: false
}

param unionParam { property: 'ping' } | { property: 'pong' } = { property: 'pong' }

param paramUsingType mixedArray

type tuple = [
  @description('A leading string')
  string

  @description('A second element using a type alias')
  bar
]

type stringStringDictionary = {
  *: string
}

param nullableParam string?
