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

type aUnion = 'snap'|'crackle'|'pop'

type expandedUnion = aUnion|'fizz'|'buzz'|'pop'

type tupleUnion = ['foo', 'bar', 'baz']
|['fizz', 'buzz']
|['snap', 'crackle', 'pop']

type mixedArray = ('heffalump'|'woozle'|{ shape: '*', size: '*'}|10|-10|true|!true|null)[]

type bool = string

param inlineObjectParam {
//@[6:23) [no-unused-params (Warning)] Parameter "inlineObjectParam" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |inlineObjectParam|
  foo: string
  bar: 100|200|300|400|500
  baz: sys.bool
} = {
  foo: 'foo'
  bar: 300
  baz: false
}

param unionParam {property: 'ping'}|{property: 'pong'} = {property: 'pong'}
//@[6:16) [no-unused-params (Warning)] Parameter "unionParam" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |unionParam|

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

param mightIncludeNull ({key: 'value'} | null)[]

var nonNull = mightIncludeNull[0]!.key

output nonNull string = nonNull

var maybeNull = mightIncludeNull[0].?key

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
  value: 'a' | 'b'
}

type typeF = {
  type: 'f'
  *: string
}

@discriminator('type')
type discriminatedUnion1 = typeA | typeB

@discriminator('type')
type discriminatedUnion2 = { type: 'c', value: string } | { type: 'd', value: bool }

@discriminator('type')
type discriminatedUnion3 = discriminatedUnion1 | discriminatedUnion2 | { type: 'e', value: string }

@discriminator('type')
type discriminatedUnion4 = discriminatedUnion1 | (discriminatedUnion2 | typeE)

@discriminator('type')
type discriminatedUnion5 = (typeA | typeB)?

@discriminator('type')
type discriminatedUnion6 = (typeA | typeB)!

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

type inlineDiscriminatedUnion4 = {
  @discriminator('type')
  prop: (typeA | typeC)?
}

type discriminatorUnionAsPropertyType = {
  prop1: discriminatedUnion1
  prop2: discriminatedUnion3
}

type discriminatedUnionInlineAdditionalProps1 = {
  @discriminator('type')
  *: typeA | typeB
}

type discriminatedUnionInlineAdditionalProps2 = {
  @discriminator('type')
  *: (typeA | typeB)?
}

@discriminator('type')
type discriminatorMemberHasAdditionalProperties1 = typeA | typeF | { type: 'g', *: int }

@discriminator('type')
type discriminatorInnerSelfOptionalCycle1 = typeA | {
  type: 'b'
  value: discriminatorInnerSelfOptionalCycle1?
}

type discriminatedUnionMemberOptionalCycle1 = {
  type: 'b'
  @discriminator('type')
  prop: (typeA | discriminatedUnionMemberOptionalCycle1)?
}

type discriminatedUnionTuple1 = [
  discriminatedUnion1
  string
]

type discriminatedUnionInlineTuple1 = [
  @discriminator('type')
  typeA | typeB | { type: 'c', value: object }
  string
]

param paramDiscriminatedUnionTypeAlias1 discriminatedUnion1
//@[6:39) [no-unused-params (Warning)] Parameter "paramDiscriminatedUnionTypeAlias1" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |paramDiscriminatedUnionTypeAlias1|
param paramDiscriminatedUnionTypeAlias2 discriminatedUnion5
//@[6:39) [no-unused-params (Warning)] Parameter "paramDiscriminatedUnionTypeAlias2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |paramDiscriminatedUnionTypeAlias2|

@discriminator('type')
param paramInlineDiscriminatedUnion1 typeA | typeB
//@[6:36) [no-unused-params (Warning)] Parameter "paramInlineDiscriminatedUnion1" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |paramInlineDiscriminatedUnion1|

@discriminator('type')
param paramInlineDiscriminatedUnion2 (typeA | typeB) = { type: 'b', value: 0 }
//@[6:36) [no-unused-params (Warning)] Parameter "paramInlineDiscriminatedUnion2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |paramInlineDiscriminatedUnion2|

@discriminator('type')
param paramInlineDiscriminatedUnion3 (typeA | typeB)?
//@[6:36) [no-unused-params (Warning)] Parameter "paramInlineDiscriminatedUnion3" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |paramInlineDiscriminatedUnion3|

output outputDiscriminatedUnionTypeAlias1 discriminatedUnion1 = { type: 'a', value: 'str' }
@discriminator('type')
output outputDiscriminatedUnionTypeAlias2 discriminatedUnion1 = { type: 'a', value: 'str' }
output outputDiscriminatedUnionTypeAlias3 discriminatedUnion5 = null

@discriminator('type')
output outputInlineDiscriminatedUnion1 typeA | typeB | { type: 'c', value: int } = { type: 'a', value: 'a' }

@discriminator('type')
output outputInlineDiscriminatedUnion2 typeA | typeB | ({ type: 'c', value: int }) = { type: 'c', value: 1 }

@discriminator('type')
output outputInlineDiscriminatedUnion3 (typeA | typeB)? = null

