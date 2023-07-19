@description('The foo type')
//@[000:4407) ProgramExpression
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
//@[000:0127) ├─DeclaredParameterExpression { Name = inlineObjectParam }
  foo: string
  bar: 100|200|300|400|500
  baz: sys.bool
} = {
//@[004:0044) | └─ObjectExpression
  foo: 'foo'
//@[002:0012) |   ├─ObjectPropertyExpression
//@[002:0005) |   | ├─StringLiteralExpression { Value = foo }
//@[007:0012) |   | └─StringLiteralExpression { Value = foo }
  bar: 300
//@[002:0010) |   ├─ObjectPropertyExpression
//@[002:0005) |   | ├─StringLiteralExpression { Value = bar }
//@[007:0010) |   | └─IntegerLiteralExpression { Value = 300 }
  baz: false
//@[002:0012) |   └─ObjectPropertyExpression
//@[002:0005) |     ├─StringLiteralExpression { Value = baz }
//@[007:0012) |     └─BooleanLiteralExpression { Value = False }
}

param unionParam {property: 'ping'}|{property: 'pong'} = {property: 'pong'}
//@[000:0075) ├─DeclaredParameterExpression { Name = unionParam }
//@[057:0075) | └─ObjectExpression
//@[058:0074) |   └─ObjectPropertyExpression
//@[058:0066) |     ├─StringLiteralExpression { Value = property }
//@[068:0074) |     └─StringLiteralExpression { Value = pong }

param paramUsingType mixedArray
//@[000:0031) ├─DeclaredParameterExpression { Name = paramUsingType }

output outputUsingType mixedArray = paramUsingType
//@[000:0050) ├─DeclaredOutputExpression { Name = outputUsingType }
//@[036:0050) | └─ParametersReferenceExpression { Parameter = paramUsingType }

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
//@[000:0048) ├─DeclaredParameterExpression { Name = mightIncludeNull }

var nonNull = mightIncludeNull[0]!.key
//@[000:0038) ├─DeclaredVariableExpression { Name = nonNull }
//@[014:0038) | └─AccessChainExpression
//@[014:0033) |   ├─ArrayAccessExpression
//@[031:0032) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[014:0030) |   | └─ParametersReferenceExpression { Parameter = mightIncludeNull }
//@[035:0038) |   └─StringLiteralExpression { Value = key }

output nonNull string = nonNull
//@[000:0031) ├─DeclaredOutputExpression { Name = nonNull }
//@[024:0031) | └─VariableReferenceExpression { Variable = nonNull }

var maybeNull = mightIncludeNull[0].?key
//@[000:0040) ├─DeclaredVariableExpression { Name = maybeNull }
//@[016:0040) | └─PropertyAccessExpression { PropertyName = key }
//@[016:0035) |   └─ArrayAccessExpression
//@[033:0034) |     ├─IntegerLiteralExpression { Value = 0 }
//@[016:0032) |     └─ParametersReferenceExpression { Parameter = mightIncludeNull }

output maybeNull string? = maybeNull
//@[000:0036) ├─DeclaredOutputExpression { Name = maybeNull }
//@[027:0036) | └─VariableReferenceExpression { Variable = maybeNull }

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

@discriminator('type')
type discriminatorInnerSelfOptionalCycle1 = typeA | {
  type: 'b'
  value: discriminatorInnerSelfOptionalCycle1?
}

type discriminatedUnionInlineAdditionalProps1 = {
  @discriminator('type')
  *: typeA | typeB
}

type discriminatedUnionInlineAdditionalProps2 = {
  @discriminator('type')
  *: (typeA | typeB)?
}

type discriminatedUnionCycle1 = {
  type: 'b'
  @discriminator('type')
  prop: (typeA | discriminatedUnionCycle1)?
}

param paramDiscriminatedUnionTypeAlias1 discriminatedUnion1
//@[000:0059) ├─DeclaredParameterExpression { Name = paramDiscriminatedUnionTypeAlias1 }
param paramDiscriminatedUnionTypeAlias2 discriminatedUnion5
//@[000:0059) ├─DeclaredParameterExpression { Name = paramDiscriminatedUnionTypeAlias2 }

@discriminator('type')
//@[000:0073) ├─DeclaredParameterExpression { Name = paramInlineDiscriminatedUnion1 }
param paramInlineDiscriminatedUnion1 typeA | typeB

@discriminator('type')
//@[000:0101) ├─DeclaredParameterExpression { Name = paramInlineDiscriminatedUnion2 }
param paramInlineDiscriminatedUnion2 (typeA | typeB) = { type: 'b', value: 0 }
//@[055:0078) | └─ObjectExpression
//@[057:0066) |   ├─ObjectPropertyExpression
//@[057:0061) |   | ├─StringLiteralExpression { Value = type }
//@[063:0066) |   | └─StringLiteralExpression { Value = b }
//@[068:0076) |   └─ObjectPropertyExpression
//@[068:0073) |     ├─StringLiteralExpression { Value = value }
//@[075:0076) |     └─IntegerLiteralExpression { Value = 0 }

@discriminator('type')
//@[000:0080) ├─DeclaredParameterExpression { Name = paramInlineDiscriminatedUnion3 }
param paramInlineDiscriminatedUnion3 typeA | typeB | null

@discriminator('type')
//@[000:0076) ├─DeclaredParameterExpression { Name = paramInlineDiscriminatedUnion4 }
param paramInlineDiscriminatedUnion4 (typeA | typeB)?

output outputDiscriminatedUnionTypeAlias1 discriminatedUnion1 = { type: 'a', value: 'str' }
//@[000:0091) ├─DeclaredOutputExpression { Name = outputDiscriminatedUnionTypeAlias1 }
//@[064:0091) | └─ObjectExpression
//@[066:0075) |   ├─ObjectPropertyExpression
//@[066:0070) |   | ├─StringLiteralExpression { Value = type }
//@[072:0075) |   | └─StringLiteralExpression { Value = a }
//@[077:0089) |   └─ObjectPropertyExpression
//@[077:0082) |     ├─StringLiteralExpression { Value = value }
//@[084:0089) |     └─StringLiteralExpression { Value = str }
@discriminator('type')
//@[000:0114) ├─DeclaredOutputExpression { Name = outputDiscriminatedUnionTypeAlias2 }
output outputDiscriminatedUnionTypeAlias2 discriminatedUnion1 = { type: 'a', value: 'str' }
//@[064:0091) | └─ObjectExpression
//@[066:0075) |   ├─ObjectPropertyExpression
//@[066:0070) |   | ├─StringLiteralExpression { Value = type }
//@[072:0075) |   | └─StringLiteralExpression { Value = a }
//@[077:0089) |   └─ObjectPropertyExpression
//@[077:0082) |     ├─StringLiteralExpression { Value = value }
//@[084:0089) |     └─StringLiteralExpression { Value = str }
output outputDiscriminatedUnionTypeAlias3 discriminatedUnion5 = null
//@[000:0068) ├─DeclaredOutputExpression { Name = outputDiscriminatedUnionTypeAlias3 }
//@[064:0068) | └─NullLiteralExpression

@discriminator('type')
//@[000:0131) ├─DeclaredOutputExpression { Name = outputInlineDiscriminatedUnion1 }
output outputInlineDiscriminatedUnion1 typeA | typeB | { type: 'c', value: int } = { type: 'a', value: 'a' }
//@[083:0108) | └─ObjectExpression
//@[085:0094) |   ├─ObjectPropertyExpression
//@[085:0089) |   | ├─StringLiteralExpression { Value = type }
//@[091:0094) |   | └─StringLiteralExpression { Value = a }
//@[096:0106) |   └─ObjectPropertyExpression
//@[096:0101) |     ├─StringLiteralExpression { Value = value }
//@[103:0106) |     └─StringLiteralExpression { Value = a }

@discriminator('type')
//@[000:0131) ├─DeclaredOutputExpression { Name = outputInlineDiscriminatedUnion2 }
output outputInlineDiscriminatedUnion2 typeA | typeB | ({ type: 'c', value: int }) = { type: 'c', value: 1 }
//@[085:0108) | └─ObjectExpression
//@[087:0096) |   ├─ObjectPropertyExpression
//@[087:0091) |   | ├─StringLiteralExpression { Value = type }
//@[093:0096) |   | └─StringLiteralExpression { Value = c }
//@[098:0106) |   └─ObjectPropertyExpression
//@[098:0103) |     ├─StringLiteralExpression { Value = value }
//@[105:0106) |     └─IntegerLiteralExpression { Value = 1 }

@discriminator('type')
//@[000:0085) └─DeclaredOutputExpression { Name = outputInlineDiscriminatedUnion3 }
output outputInlineDiscriminatedUnion3 (typeA | typeB)? = null
//@[058:0062)   └─NullLiteralExpression

