@description('The foo type')
//@[00:2835) ProgramExpression
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
//@[00:0127) ├─DeclaredParameterExpression { Name = inlineObjectParam }
  foo: string
  bar: 100|200|300|400|500
  baz: sys.bool
} = {
//@[04:0044) | └─ObjectExpression
  foo: 'foo'
//@[02:0012) |   ├─ObjectPropertyExpression
//@[02:0005) |   | ├─StringLiteralExpression { Value = foo }
//@[07:0012) |   | └─StringLiteralExpression { Value = foo }
  bar: 300
//@[02:0010) |   ├─ObjectPropertyExpression
//@[02:0005) |   | ├─StringLiteralExpression { Value = bar }
//@[07:0010) |   | └─IntegerLiteralExpression { Value = 300 }
  baz: false
//@[02:0012) |   └─ObjectPropertyExpression
//@[02:0005) |     ├─StringLiteralExpression { Value = baz }
//@[07:0012) |     └─BooleanLiteralExpression { Value = False }
}

param unionParam {property: 'ping'}|{property: 'pong'} = {property: 'pong'}
//@[00:0075) ├─DeclaredParameterExpression { Name = unionParam }
//@[57:0075) | └─ObjectExpression
//@[58:0074) |   └─ObjectPropertyExpression
//@[58:0066) |     ├─StringLiteralExpression { Value = property }
//@[68:0074) |     └─StringLiteralExpression { Value = pong }

param paramUsingType mixedArray
//@[00:0031) ├─DeclaredParameterExpression { Name = paramUsingType }

output outputUsingType mixedArray = paramUsingType
//@[00:0050) ├─DeclaredOutputExpression { Name = outputUsingType }
//@[36:0050) | └─ParametersReferenceExpression { Parameter = paramUsingType }

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
//@[00:0048) ├─DeclaredParameterExpression { Name = mightIncludeNull }

var nonNull = mightIncludeNull[0]!.key
//@[00:0038) ├─DeclaredVariableExpression { Name = nonNull }
//@[14:0038) | └─AccessChainExpression
//@[14:0033) |   ├─ArrayAccessExpression
//@[31:0032) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[14:0030) |   | └─ParametersReferenceExpression { Parameter = mightIncludeNull }
//@[35:0038) |   └─StringLiteralExpression { Value = key }

output nonNull string = nonNull
//@[00:0031) ├─DeclaredOutputExpression { Name = nonNull }
//@[24:0031) | └─VariableReferenceExpression { Variable = nonNull }

var maybeNull = mightIncludeNull[0].?key
//@[00:0040) ├─DeclaredVariableExpression { Name = maybeNull }
//@[16:0040) | └─PropertyAccessExpression { PropertyName = key }
//@[16:0035) |   └─ArrayAccessExpression
//@[33:0034) |     ├─IntegerLiteralExpression { Value = 0 }
//@[16:0032) |     └─ParametersReferenceExpression { Parameter = mightIncludeNull }

output maybeNull string? = maybeNull
//@[00:0036) └─DeclaredOutputExpression { Name = maybeNull }
//@[27:0036)   └─VariableReferenceExpression { Variable = maybeNull }

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
type discriminatedUnion2 = { type: 'c', value: string } | { type: 'd', value: bool }

@discriminator('type')
type discriminatedUnion3 = discriminatedUnion1 | discriminatedUnion2 | { type: 'e', *: string }

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
