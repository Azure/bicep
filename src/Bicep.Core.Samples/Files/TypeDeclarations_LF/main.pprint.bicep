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
@metadata(
  {
    examples: [[[[[1]]], [[[2]]], [[[3]]]]]
  }
)
type bar = int[][][][]

type aUnion = 'snap' | 'crackle' | 'pop'

type expandedUnion = aUnion | 'fizz' | 'buzz' | 'pop'

type tupleUnion = ['foo', 'bar', 'baz']
 | ['fizz', 'buzz']
 | ['snap', 'crackle', 'pop']

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

param unionParam { property: 'ping' } | { property: 'pong' } = {
  property: 'pong'
}

param paramUsingType mixedArray

output outputUsingType mixedArray = paramUsingType

type tuple = [
  @description('A leading string')
  string

  @description('A second element using a type alias')
  bar
]

type stringStringDictionary = {
  *: string
}

@minValue(1)
@maxValue(10)
type constrainedInt = int

param mightIncludeNull ({ key: 'value' } | null)[]

var nonNull = mightIncludeNull[0]!.key

output nonNull string = nonNull

var maybeNull = mightIncludeNull[0].key

output maybeNull string? = maybeNull

type nullable = string?

type nonNullable = nullable!

type typeA = {
  type: 'a'
  value: string
}

type typeB = {
  type: 'b'
  value: int
}

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

@discriminator('type')
type discriminatedUnion1 = typeA | typeB

@discriminator('type')
type discriminatedUnion2 = { type: 'c', value: string } | {
  type: 'd'
  value: bool
}

@discriminator('type')
type discriminatedUnion3 = discriminatedUnion1 | discriminatedUnion2 | {
  type: 'e'
  *: string
}

@discriminator('type')
type discriminatedUnion4 = discriminatedUnion1 | (discriminatedUnion2 | typeE)

type inlineDiscriminatedUnion1 = {
  @discriminator('type')
  prop: typeA | typeC
}

type inlineDiscriminatedUnion2 = {
  @discriminator('type')
  prop: { type: 'a', value: bool } | typeB
}

@discriminator('type')
type inlineDiscriminatedUnion3 = {
  type: 'a'
  @discriminator('type')
  prop: { type: 'a', value: bool } | typeB
} | {
  type: 'b'
  @discriminator('type')
  prop: discriminatedUnion1 | discriminatedUnion2
}

type discriminatorUnionAsPropertyType = {
  prop1: discriminatedUnion1
  prop2: discriminatedUnion3
}

@discriminator('type')
type discriminatorInnerSelfOptionalCycle1 = typeA | {
  type: 'b'
  value: discriminatorInnerSelfOptionalCycle1?
}
