@description('The foo type')
//@[00:1534) ProgramExpression
//@[00:0299) ├─DeclaredTypeExpression { Name = foo }
@sealed()
type foo = {
//@[11:0260) | └─ObjectTypeExpression { Name = { stringProp: string, objectProp: { intProp: int, intArrayArrayProp: int[][] | null }, typeRefProp: bar, literalProp: 'literal', recursion: foo? } }
  @minLength(3)
//@[02:0089) |   ├─ObjectTypePropertyExpression
  @maxLength(10)
  @description('A string property')
  stringProp: string
//@[14:0020) |   | └─AmbientTypeReferenceExpression { Name = string }

  objectProp: {
//@[02:0089) |   ├─ObjectTypePropertyExpression
//@[14:0089) |   | └─ObjectTypeExpression { Name = { intProp: int, intArrayArrayProp: int[][] | null } }
    @minValue(1)
//@[04:0033) |   |   ├─ObjectTypePropertyExpression
    intProp: int
//@[13:0016) |   |   | └─AmbientTypeReferenceExpression { Name = int }

    intArrayArrayProp: int [] [] ?
//@[04:0034) |   |   └─ObjectTypePropertyExpression
//@[23:0034) |   |     └─NullableTypeExpression { Name = int[][] | null }
//@[23:0032) |   |       └─ArrayTypeExpression { Name = int[][] }
//@[23:0029) |   |         └─ArrayTypeExpression { Name = int[] }
//@[23:0026) |   |           └─AmbientTypeReferenceExpression { Name = int }
  }

  typeRefProp: bar
//@[02:0018) |   ├─ObjectTypePropertyExpression
//@[15:0018) |   | └─TypeAliasReferenceExpression { Name = bar }

  literalProp: 'literal'
//@[02:0024) |   ├─ObjectTypePropertyExpression
//@[15:0024) |   | └─StringLiteralTypeExpression { Name = 'literal' }

  recursion: foo?
//@[02:0017) |   └─ObjectTypePropertyExpression
//@[13:0017) |     └─NullableTypeExpression { Name = Type<{ stringProp: string, objectProp: { intProp: int, intArrayArrayProp: int[][] | null }, typeRefProp: bar, literalProp: 'literal', recursion: foo? }> | null }
//@[13:0016) |       └─TypeAliasReferenceExpression { Name = foo }
}

@minLength(3)
//@[00:0163) ├─DeclaredTypeExpression { Name = bar }
@description('An array of array of arrays of arrays of ints')
@metadata({
  examples: [
    [[[[1]]], [[[2]]], [[[3]]]]
  ]
})
type bar = int[][][][]
//@[11:0022) | └─ArrayTypeExpression { Name = int[][][][] }
//@[11:0020) |   └─ArrayTypeExpression { Name = int[][][] }
//@[11:0018) |     └─ArrayTypeExpression { Name = int[][] }
//@[11:0016) |       └─ArrayTypeExpression { Name = int[] }
//@[11:0014) |         └─AmbientTypeReferenceExpression { Name = int }

type aUnion = 'snap'|'crackle'|'pop'
//@[00:0036) ├─DeclaredTypeExpression { Name = aUnion }
//@[14:0036) | └─UnionTypeExpression { Name = 'crackle' | 'pop' | 'snap' }
//@[14:0020) |   ├─StringLiteralTypeExpression { Name = 'snap' }
//@[21:0030) |   ├─StringLiteralTypeExpression { Name = 'crackle' }
//@[31:0036) |   └─StringLiteralTypeExpression { Name = 'pop' }

type expandedUnion = aUnion|'fizz'|'buzz'|'pop'
//@[00:0047) ├─DeclaredTypeExpression { Name = expandedUnion }
//@[21:0047) | └─UnionTypeExpression { Name = 'buzz' | 'crackle' | 'fizz' | 'pop' | 'snap' }
//@[21:0027) |   ├─TypeAliasReferenceExpression { Name = aUnion }
//@[28:0034) |   ├─StringLiteralTypeExpression { Name = 'fizz' }
//@[35:0041) |   ├─StringLiteralTypeExpression { Name = 'buzz' }
//@[42:0047) |   └─StringLiteralTypeExpression { Name = 'pop' }

type tupleUnion = ['foo', 'bar', 'baz']
//@[00:0085) ├─DeclaredTypeExpression { Name = tupleUnion }
//@[18:0085) | └─UnionTypeExpression { Name = ['fizz', 'buzz'] | ['foo', 'bar', 'baz'] | ['snap', 'crackle', 'pop'] }
//@[18:0039) |   ├─TupleTypeExpression { Name = ['foo', 'bar', 'baz'] }
//@[19:0024) |   | ├─TupleTypeItemExpression
//@[19:0024) |   | | └─StringLiteralTypeExpression { Name = 'foo' }
//@[26:0031) |   | ├─TupleTypeItemExpression
//@[26:0031) |   | | └─StringLiteralTypeExpression { Name = 'bar' }
//@[33:0038) |   | └─TupleTypeItemExpression
//@[33:0038) |   |   └─StringLiteralTypeExpression { Name = 'baz' }
|['fizz', 'buzz']
//@[01:0017) |   ├─TupleTypeExpression { Name = ['fizz', 'buzz'] }
//@[02:0008) |   | ├─TupleTypeItemExpression
//@[02:0008) |   | | └─StringLiteralTypeExpression { Name = 'fizz' }
//@[10:0016) |   | └─TupleTypeItemExpression
//@[10:0016) |   |   └─StringLiteralTypeExpression { Name = 'buzz' }
|['snap', 'crackle', 'pop']
//@[01:0027) |   └─TupleTypeExpression { Name = ['snap', 'crackle', 'pop'] }
//@[02:0008) |     ├─TupleTypeItemExpression
//@[02:0008) |     | └─StringLiteralTypeExpression { Name = 'snap' }
//@[10:0019) |     ├─TupleTypeItemExpression
//@[10:0019) |     | └─StringLiteralTypeExpression { Name = 'crackle' }
//@[21:0026) |     └─TupleTypeItemExpression
//@[21:0026) |       └─StringLiteralTypeExpression { Name = 'pop' }

type mixedArray = ('heffalump'|'woozle'|{ shape: '*', size: '*'}|10|-10|true|!true|null)[]
//@[00:0090) ├─DeclaredTypeExpression { Name = mixedArray }
//@[18:0090) | └─ArrayTypeExpression { Name = ('heffalump' | 'woozle' | -10 | 10 | false | null | true | { shape: '*', size: '*' })[] }
//@[19:0087) |   └─UnionTypeExpression { Name = 'heffalump' | 'woozle' | -10 | 10 | false | null | true | { shape: '*', size: '*' } }
//@[19:0030) |     ├─StringLiteralTypeExpression { Name = 'heffalump' }
//@[31:0039) |     ├─StringLiteralTypeExpression { Name = 'woozle' }
//@[40:0064) |     ├─ObjectTypeExpression { Name = { shape: '*', size: '*' } }
//@[42:0052) |     | ├─ObjectTypePropertyExpression
//@[49:0052) |     | | └─StringLiteralTypeExpression { Name = '*' }
//@[54:0063) |     | └─ObjectTypePropertyExpression
//@[60:0063) |     |   └─StringLiteralTypeExpression { Name = '*' }
//@[65:0067) |     ├─IntegerLiteralTypeExpression { Name = 10 }
//@[68:0071) |     ├─IntegerLiteralTypeExpression { Name = -10 }
//@[72:0076) |     ├─BooleanLiteralTypeExpression { Name = true }
//@[77:0082) |     ├─BooleanLiteralTypeExpression { Name = false }
//@[83:0087) |     └─NullLiteralTypeExpression { Name = null }

type bool = string
//@[00:0018) ├─DeclaredTypeExpression { Name = bool }
//@[12:0018) | └─AmbientTypeReferenceExpression { Name = string }

param inlineObjectParam {
//@[00:0127) ├─DeclaredParameterExpression { Name = inlineObjectParam }
//@[24:0084) | ├─ObjectTypeExpression { Name = { foo: string, bar: 100 | 200 | 300 | 400 | 500, baz: bool } }
  foo: string
//@[02:0013) | | ├─ObjectTypePropertyExpression
//@[07:0013) | | | └─AmbientTypeReferenceExpression { Name = string }
  bar: 100|200|300|400|500
//@[02:0026) | | ├─ObjectTypePropertyExpression
//@[07:0026) | | | └─UnionTypeExpression { Name = 100 | 200 | 300 | 400 | 500 }
//@[07:0010) | | |   ├─IntegerLiteralTypeExpression { Name = 100 }
//@[11:0014) | | |   ├─IntegerLiteralTypeExpression { Name = 200 }
//@[15:0018) | | |   ├─IntegerLiteralTypeExpression { Name = 300 }
//@[19:0022) | | |   ├─IntegerLiteralTypeExpression { Name = 400 }
//@[23:0026) | | |   └─IntegerLiteralTypeExpression { Name = 500 }
  baz: sys.bool
//@[02:0015) | | └─ObjectTypePropertyExpression
//@[07:0015) | |   └─FullyQualifiedAmbientTypeReferenceExpression { Name = sys.bool }
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
//@[17:0054) | ├─UnionTypeExpression { Name = { property: 'ping' } | { property: 'pong' } }
//@[17:0035) | | ├─ObjectTypeExpression { Name = { property: 'ping' } }
//@[18:0034) | | | └─ObjectTypePropertyExpression
//@[28:0034) | | |   └─StringLiteralTypeExpression { Name = 'ping' }
//@[36:0054) | | └─ObjectTypeExpression { Name = { property: 'pong' } }
//@[37:0053) | |   └─ObjectTypePropertyExpression
//@[47:0053) | |     └─StringLiteralTypeExpression { Name = 'pong' }
//@[57:0075) | └─ObjectExpression
//@[58:0074) |   └─ObjectPropertyExpression
//@[58:0066) |     ├─StringLiteralExpression { Value = property }
//@[68:0074) |     └─StringLiteralExpression { Value = pong }

param paramUsingType mixedArray
//@[00:0031) ├─DeclaredParameterExpression { Name = paramUsingType }
//@[21:0031) | └─TypeAliasReferenceExpression { Name = mixedArray }

output outputUsingType mixedArray = paramUsingType
//@[00:0050) ├─DeclaredOutputExpression { Name = outputUsingType }
//@[23:0033) | ├─TypeAliasReferenceExpression { Name = mixedArray }
//@[36:0050) | └─ParametersReferenceExpression { Parameter = paramUsingType }

type tuple = [
//@[00:0129) ├─DeclaredTypeExpression { Name = tuple }
//@[13:0129) | └─TupleTypeExpression { Name = [string, bar] }
    @description('A leading string')
//@[04:0047) |   ├─TupleTypeItemExpression
    string
//@[04:0010) |   | └─AmbientTypeReferenceExpression { Name = string }

    @description('A second element using a type alias')
//@[04:0063) |   └─TupleTypeItemExpression
    bar
//@[04:0007) |     └─TypeAliasReferenceExpression { Name = bar }
]

type stringStringDictionary = {
//@[00:0047) ├─DeclaredTypeExpression { Name = stringStringDictionary }
//@[30:0047) | └─ObjectTypeExpression { Name = { *: string } }
    *: string
//@[04:0013) |   └─ObjectTypeAdditionalPropertiesExpression
//@[07:0013) |     └─AmbientTypeReferenceExpression { Name = string }
}

@minValue(1)
//@[00:0052) ├─DeclaredTypeExpression { Name = constrainedInt }
@maxValue(10)
type constrainedInt = int
//@[22:0025) | └─AmbientTypeReferenceExpression { Name = int }

param mightIncludeNull ({key: 'value'} | null)[]
//@[00:0048) ├─DeclaredParameterExpression { Name = mightIncludeNull }
//@[23:0048) | └─ArrayTypeExpression { Name = (null | { key: 'value' })[] }
//@[24:0045) |   └─UnionTypeExpression { Name = null | { key: 'value' } }
//@[24:0038) |     ├─ObjectTypeExpression { Name = { key: 'value' } }
//@[25:0037) |     | └─ObjectTypePropertyExpression
//@[30:0037) |     |   └─StringLiteralTypeExpression { Name = 'value' }
//@[41:0045) |     └─NullLiteralTypeExpression { Name = null }

var nonNull = mightIncludeNull[0]!.key
//@[00:0038) ├─DeclaredVariableExpression { Name = nonNull }
//@[14:0038) | └─AccessChainExpression
//@[14:0033) |   ├─ArrayAccessExpression
//@[31:0032) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[14:0030) |   | └─ParametersReferenceExpression { Parameter = mightIncludeNull }
//@[35:0038) |   └─StringLiteralExpression { Value = key }

output nonNull string = nonNull
//@[00:0031) ├─DeclaredOutputExpression { Name = nonNull }
//@[15:0021) | ├─AmbientTypeReferenceExpression { Name = string }
//@[24:0031) | └─VariableReferenceExpression { Variable = nonNull }

var maybeNull = mightIncludeNull[0].?key
//@[00:0040) ├─DeclaredVariableExpression { Name = maybeNull }
//@[16:0040) | └─PropertyAccessExpression { PropertyName = key }
//@[16:0035) |   └─ArrayAccessExpression
//@[33:0034) |     ├─IntegerLiteralExpression { Value = 0 }
//@[16:0032) |     └─ParametersReferenceExpression { Parameter = mightIncludeNull }

output maybeNull string? = maybeNull
//@[00:0036) └─DeclaredOutputExpression { Name = maybeNull }
//@[17:0024)   ├─NullableTypeExpression { Name = Type<string> | null }
//@[17:0023)   | └─AmbientTypeReferenceExpression { Name = string }
//@[27:0036)   └─VariableReferenceExpression { Variable = maybeNull }

type nullable = string?
//@[00:0023) ├─DeclaredTypeExpression { Name = nullable }
//@[16:0023) | └─NullableTypeExpression { Name = Type<string> | null }
//@[16:0022) |   └─AmbientTypeReferenceExpression { Name = string }

type nonNullable = nullable!
//@[00:0028) ├─DeclaredTypeExpression { Name = nonNullable }
//@[19:0028) | └─NonNullableTypeExpression { Name = Type<null | string> }
//@[19:0027) |   └─TypeAliasReferenceExpression { Name = nullable }

