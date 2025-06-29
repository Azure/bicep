@description('The foo type')
//@[000:5153) ProgramExpression
//@[000:0299) ├─DeclaredTypeExpression { Name = foo }
//@[013:0027) | ├─StringLiteralExpression { Value = The foo type }
@sealed()
//@[001:0009) | ├─FunctionCallExpression { Name = sealed }
type foo = {
//@[011:0260) | └─ObjectTypeExpression { Name = { stringProp: string, objectProp: { intProp: int, intArrayArrayProp: int[][] | null }, typeRefProp: bar, literalProp: 'literal', recursion: foo? } }
  @minLength(3)
//@[002:0089) |   ├─ObjectTypePropertyExpression
//@[013:0014) |   | ├─IntegerLiteralExpression { Value = 3 }
  @maxLength(10)
//@[013:0015) |   | ├─IntegerLiteralExpression { Value = 10 }
  @description('A string property')
//@[015:0034) |   | ├─StringLiteralExpression { Value = A string property }
  stringProp: string
//@[014:0020) |   | └─AmbientTypeReferenceExpression { Name = string }

  objectProp: {
//@[002:0089) |   ├─ObjectTypePropertyExpression
//@[014:0089) |   | └─ObjectTypeExpression { Name = { intProp: int, intArrayArrayProp: int[][] | null } }
    @minValue(1)
//@[004:0033) |   |   ├─ObjectTypePropertyExpression
//@[014:0015) |   |   | ├─IntegerLiteralExpression { Value = 1 }
    intProp: int
//@[013:0016) |   |   | └─AmbientTypeReferenceExpression { Name = int }

    intArrayArrayProp: int [] [] ?
//@[004:0034) |   |   └─ObjectTypePropertyExpression
//@[023:0034) |   |     └─NullableTypeExpression { Name = int[][] | null }
//@[023:0032) |   |       └─ArrayTypeExpression { Name = int[][] }
//@[023:0029) |   |         └─ArrayTypeExpression { Name = int[] }
//@[023:0026) |   |           └─AmbientTypeReferenceExpression { Name = int }
  }

  typeRefProp: bar
//@[002:0018) |   ├─ObjectTypePropertyExpression
//@[015:0018) |   | └─TypeAliasReferenceExpression { Name = bar }

  literalProp: 'literal'
//@[002:0024) |   ├─ObjectTypePropertyExpression
//@[015:0024) |   | └─StringLiteralTypeExpression { Name = 'literal' }

  recursion: foo?
//@[002:0017) |   └─ObjectTypePropertyExpression
//@[013:0017) |     └─NullableTypeExpression { Name = null | { stringProp: string, objectProp: { intProp: int, intArrayArrayProp: int[][] | null }, typeRefProp: bar, literalProp: 'literal', recursion: foo? } }
//@[013:0016) |       └─TypeAliasReferenceExpression { Name = foo }
}

type fooProperty = foo.objectProp.intProp
//@[000:0041) ├─DeclaredTypeExpression { Name = fooProperty }
//@[019:0041) | └─TypeReferencePropertyAccessExpression { Name = int }
//@[019:0033) |   └─TypeReferencePropertyAccessExpression { Name = { intProp: int, intArrayArrayProp: int[][] | null } }
//@[019:0022) |     └─TypeAliasReferenceExpression { Name = foo }

@minLength(3)
//@[000:0163) ├─DeclaredTypeExpression { Name = bar }
//@[011:0012) | ├─IntegerLiteralExpression { Value = 3 }
@description('An array of array of arrays of arrays of ints')
//@[013:0060) | ├─StringLiteralExpression { Value = An array of array of arrays of arrays of ints }
@metadata({
//@[010:0063) | ├─ObjectExpression
  examples: [
//@[002:0049) | | └─ObjectPropertyExpression
//@[002:0010) | |   ├─StringLiteralExpression { Value = examples }
//@[012:0049) | |   └─ArrayExpression
    [[[[1]]], [[[2]]], [[[3]]]]
//@[004:0031) | |     └─ArrayExpression
//@[005:0012) | |       ├─ArrayExpression
//@[006:0011) | |       | └─ArrayExpression
//@[007:0010) | |       |   └─ArrayExpression
//@[008:0009) | |       |     └─IntegerLiteralExpression { Value = 1 }
//@[014:0021) | |       ├─ArrayExpression
//@[015:0020) | |       | └─ArrayExpression
//@[016:0019) | |       |   └─ArrayExpression
//@[017:0018) | |       |     └─IntegerLiteralExpression { Value = 2 }
//@[023:0030) | |       └─ArrayExpression
//@[024:0029) | |         └─ArrayExpression
//@[025:0028) | |           └─ArrayExpression
//@[026:0027) | |             └─IntegerLiteralExpression { Value = 3 }
  ]
})
type bar = int[][][][]
//@[011:0022) | └─ArrayTypeExpression { Name = int[][][][] }
//@[011:0020) |   └─ArrayTypeExpression { Name = int[][][] }
//@[011:0018) |     └─ArrayTypeExpression { Name = int[][] }
//@[011:0016) |       └─ArrayTypeExpression { Name = int[] }
//@[011:0014) |         └─AmbientTypeReferenceExpression { Name = int }

type barElement = bar[*]
//@[000:0024) ├─DeclaredTypeExpression { Name = barElement }
//@[018:0024) | └─TypeReferenceItemsAccessExpression { Name = int[][][] }
//@[018:0021) |   └─TypeAliasReferenceExpression { Name = bar }

type aUnion = 'snap'|'crackle'|'pop'
//@[000:0036) ├─DeclaredTypeExpression { Name = aUnion }
//@[014:0036) | └─UnionTypeExpression { Name = 'crackle' | 'pop' | 'snap' }
//@[014:0020) |   ├─StringLiteralTypeExpression { Name = 'snap' }
//@[021:0030) |   ├─StringLiteralTypeExpression { Name = 'crackle' }
//@[031:0036) |   └─StringLiteralTypeExpression { Name = 'pop' }

type singleMemberUnion = | 'alone'
//@[000:0034) ├─DeclaredTypeExpression { Name = singleMemberUnion }
//@[025:0034) | └─UnionTypeExpression { Name =  }
//@[027:0034) |   └─StringLiteralTypeExpression { Name = 'alone' }

type expandedUnion = aUnion|'fizz'|'buzz'|'pop'
//@[000:0047) ├─DeclaredTypeExpression { Name = expandedUnion }
//@[021:0047) | └─UnionTypeExpression { Name = 'buzz' | 'crackle' | 'fizz' | 'pop' | 'snap' }
//@[021:0027) |   ├─TypeAliasReferenceExpression { Name = aUnion }
//@[028:0034) |   ├─StringLiteralTypeExpression { Name = 'fizz' }
//@[035:0041) |   ├─StringLiteralTypeExpression { Name = 'buzz' }
//@[042:0047) |   └─StringLiteralTypeExpression { Name = 'pop' }

type tupleUnion = ['foo', 'bar', 'baz']
//@[000:0085) ├─DeclaredTypeExpression { Name = tupleUnion }
//@[018:0085) | └─UnionTypeExpression { Name = ['fizz', 'buzz'] | ['foo', 'bar', 'baz'] | ['snap', 'crackle', 'pop'] }
//@[018:0039) |   ├─TupleTypeExpression { Name = ['foo', 'bar', 'baz'] }
//@[019:0024) |   | ├─TupleTypeItemExpression
//@[019:0024) |   | | └─StringLiteralTypeExpression { Name = 'foo' }
//@[026:0031) |   | ├─TupleTypeItemExpression
//@[026:0031) |   | | └─StringLiteralTypeExpression { Name = 'bar' }
//@[033:0038) |   | └─TupleTypeItemExpression
//@[033:0038) |   |   └─StringLiteralTypeExpression { Name = 'baz' }
|['fizz', 'buzz']
//@[001:0017) |   ├─TupleTypeExpression { Name = ['fizz', 'buzz'] }
//@[002:0008) |   | ├─TupleTypeItemExpression
//@[002:0008) |   | | └─StringLiteralTypeExpression { Name = 'fizz' }
//@[010:0016) |   | └─TupleTypeItemExpression
//@[010:0016) |   |   └─StringLiteralTypeExpression { Name = 'buzz' }
|['snap', 'crackle', 'pop']
//@[001:0027) |   └─TupleTypeExpression { Name = ['snap', 'crackle', 'pop'] }
//@[002:0008) |     ├─TupleTypeItemExpression
//@[002:0008) |     | └─StringLiteralTypeExpression { Name = 'snap' }
//@[010:0019) |     ├─TupleTypeItemExpression
//@[010:0019) |     | └─StringLiteralTypeExpression { Name = 'crackle' }
//@[021:0026) |     └─TupleTypeItemExpression
//@[021:0026) |       └─StringLiteralTypeExpression { Name = 'pop' }

type mixedArray = ('heffalump'|'woozle'|{ shape: '*', size: '*'}|10|-10|true|!true|null)[]
//@[000:0090) ├─DeclaredTypeExpression { Name = mixedArray }
//@[018:0090) | └─ArrayTypeExpression { Name = ('heffalump' | 'woozle' | -10 | 10 | false | null | true | { shape: '*', size: '*' })[] }
//@[019:0087) |   └─UnionTypeExpression { Name = 'heffalump' | 'woozle' | -10 | 10 | false | null | true | { shape: '*', size: '*' } }
//@[019:0030) |     ├─StringLiteralTypeExpression { Name = 'heffalump' }
//@[031:0039) |     ├─StringLiteralTypeExpression { Name = 'woozle' }
//@[040:0064) |     ├─ObjectTypeExpression { Name = { shape: '*', size: '*' } }
//@[042:0052) |     | ├─ObjectTypePropertyExpression
//@[049:0052) |     | | └─StringLiteralTypeExpression { Name = '*' }
//@[054:0063) |     | └─ObjectTypePropertyExpression
//@[060:0063) |     |   └─StringLiteralTypeExpression { Name = '*' }
//@[065:0067) |     ├─IntegerLiteralTypeExpression { Name = 10 }
//@[068:0071) |     ├─IntegerLiteralTypeExpression { Name = -10 }
//@[072:0076) |     ├─BooleanLiteralTypeExpression { Name = true }
//@[077:0082) |     ├─BooleanLiteralTypeExpression { Name = false }
//@[083:0087) |     └─NullLiteralTypeExpression { Name = null }

type bool = string
//@[000:0018) ├─DeclaredTypeExpression { Name = bool }
//@[012:0018) | └─AmbientTypeReferenceExpression { Name = string }

param inlineObjectParam {
//@[000:0127) ├─DeclaredParameterExpression { Name = inlineObjectParam }
//@[024:0084) | ├─ObjectTypeExpression { Name = { foo: string, bar: 100 | 200 | 300 | 400 | 500, baz: bool } }
  foo: string
//@[002:0013) | | ├─ObjectTypePropertyExpression
//@[007:0013) | | | └─AmbientTypeReferenceExpression { Name = string }
  bar: 100|200|300|400|500
//@[002:0026) | | ├─ObjectTypePropertyExpression
//@[007:0026) | | | └─UnionTypeExpression { Name = 100 | 200 | 300 | 400 | 500 }
//@[007:0010) | | |   ├─IntegerLiteralTypeExpression { Name = 100 }
//@[011:0014) | | |   ├─IntegerLiteralTypeExpression { Name = 200 }
//@[015:0018) | | |   ├─IntegerLiteralTypeExpression { Name = 300 }
//@[019:0022) | | |   ├─IntegerLiteralTypeExpression { Name = 400 }
//@[023:0026) | | |   └─IntegerLiteralTypeExpression { Name = 500 }
  baz: sys.bool
//@[002:0015) | | └─ObjectTypePropertyExpression
//@[007:0015) | |   └─FullyQualifiedAmbientTypeReferenceExpression { Name = sys.bool }
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
//@[017:0054) | ├─UnionTypeExpression { Name = { property: 'ping' } | { property: 'pong' } }
//@[017:0035) | | ├─ObjectTypeExpression { Name = { property: 'ping' } }
//@[018:0034) | | | └─ObjectTypePropertyExpression
//@[028:0034) | | |   └─StringLiteralTypeExpression { Name = 'ping' }
//@[036:0054) | | └─ObjectTypeExpression { Name = { property: 'pong' } }
//@[037:0053) | |   └─ObjectTypePropertyExpression
//@[047:0053) | |     └─StringLiteralTypeExpression { Name = 'pong' }
//@[057:0075) | └─ObjectExpression
//@[058:0074) |   └─ObjectPropertyExpression
//@[058:0066) |     ├─StringLiteralExpression { Value = property }
//@[068:0074) |     └─StringLiteralExpression { Value = pong }

param paramUsingType mixedArray
//@[000:0031) ├─DeclaredParameterExpression { Name = paramUsingType }
//@[021:0031) | └─TypeAliasReferenceExpression { Name = mixedArray }

output outputUsingType mixedArray = paramUsingType
//@[000:0050) ├─DeclaredOutputExpression { Name = outputUsingType }
//@[023:0033) | ├─TypeAliasReferenceExpression { Name = mixedArray }
//@[036:0050) | └─ParametersReferenceExpression { Parameter = paramUsingType }

type tuple = [
//@[000:0129) ├─DeclaredTypeExpression { Name = tuple }
//@[013:0129) | └─TupleTypeExpression { Name = [string, bar] }
    @description('A leading string')
//@[004:0047) |   ├─TupleTypeItemExpression
//@[017:0035) |   | ├─StringLiteralExpression { Value = A leading string }
    string
//@[004:0010) |   | └─AmbientTypeReferenceExpression { Name = string }

    @description('A second element using a type alias')
//@[004:0063) |   └─TupleTypeItemExpression
//@[017:0054) |     ├─StringLiteralExpression { Value = A second element using a type alias }
    bar
//@[004:0007) |     └─TypeAliasReferenceExpression { Name = bar }
]

type tupleSecondItem = tuple[1]
//@[000:0031) ├─DeclaredTypeExpression { Name = tupleSecondItem }
//@[023:0031) | └─TypeReferenceIndexAccessExpression { Name = int[][][][] }
//@[023:0028) |   └─TypeAliasReferenceExpression { Name = tuple }

type stringStringDictionary = {
//@[000:0047) ├─DeclaredTypeExpression { Name = stringStringDictionary }
//@[030:0047) | └─ObjectTypeExpression { Name = { *: string } }
    *: string
//@[004:0013) |   └─ObjectTypeAdditionalPropertiesExpression
//@[007:0013) |     └─AmbientTypeReferenceExpression { Name = string }
}

type stringStringDictionaryValue = stringStringDictionary.*
//@[000:0059) ├─DeclaredTypeExpression { Name = stringStringDictionaryValue }
//@[035:0059) | └─TypeReferenceAdditionalPropertiesAccessExpression { Name = string }
//@[035:0057) |   └─TypeAliasReferenceExpression { Name = stringStringDictionary }

@minValue(1)
//@[000:0052) ├─DeclaredTypeExpression { Name = constrainedInt }
//@[010:0011) | ├─IntegerLiteralExpression { Value = 1 }
@maxValue(10)
//@[010:0012) | ├─IntegerLiteralExpression { Value = 10 }
type constrainedInt = int
//@[022:0025) | └─AmbientTypeReferenceExpression { Name = int }

param mightIncludeNull ({key: 'value'} | null)[]
//@[000:0048) ├─DeclaredParameterExpression { Name = mightIncludeNull }
//@[023:0048) | └─ArrayTypeExpression { Name = (null | { key: 'value' })[] }
//@[024:0045) |   └─UnionTypeExpression { Name = null | { key: 'value' } }
//@[024:0038) |     ├─ObjectTypeExpression { Name = { key: 'value' } }
//@[025:0037) |     | └─ObjectTypePropertyExpression
//@[030:0037) |     |   └─StringLiteralTypeExpression { Name = 'value' }
//@[041:0045) |     └─NullLiteralTypeExpression { Name = null }

var nonNull = mightIncludeNull[0]!.key
//@[000:0038) ├─DeclaredVariableExpression { Name = nonNull }
//@[014:0038) | └─PropertyAccessExpression { PropertyName = key }
//@[014:0033) |   └─ArrayAccessExpression
//@[031:0032) |     ├─IntegerLiteralExpression { Value = 0 }
//@[014:0030) |     └─ParametersReferenceExpression { Parameter = mightIncludeNull }

output nonNull string = nonNull
//@[000:0031) ├─DeclaredOutputExpression { Name = nonNull }
//@[015:0021) | ├─AmbientTypeReferenceExpression { Name = string }
//@[024:0031) | └─VariableReferenceExpression { Variable = nonNull }

var maybeNull = mightIncludeNull[0].?key
//@[000:0040) ├─DeclaredVariableExpression { Name = maybeNull }
//@[016:0040) | └─PropertyAccessExpression { PropertyName = key }
//@[016:0035) |   └─ArrayAccessExpression
//@[033:0034) |     ├─IntegerLiteralExpression { Value = 0 }
//@[016:0032) |     └─ParametersReferenceExpression { Parameter = mightIncludeNull }

var maybeNull2 = mightIncludeNull[0][?'key']
//@[000:0044) ├─DeclaredVariableExpression { Name = maybeNull2 }
//@[017:0044) | └─PropertyAccessExpression { PropertyName = key }
//@[017:0036) |   └─ArrayAccessExpression
//@[034:0035) |     ├─IntegerLiteralExpression { Value = 0 }
//@[017:0033) |     └─ParametersReferenceExpression { Parameter = mightIncludeNull }

output maybeNull string? = maybeNull
//@[000:0036) ├─DeclaredOutputExpression { Name = maybeNull }
//@[017:0024) | ├─NullableTypeExpression { Name = null | string }
//@[017:0023) | | └─AmbientTypeReferenceExpression { Name = string }
//@[027:0036) | └─VariableReferenceExpression { Variable = maybeNull }

type nullable = string?
//@[000:0023) ├─DeclaredTypeExpression { Name = nullable }
//@[016:0023) | └─NullableTypeExpression { Name = null | string }
//@[016:0022) |   └─AmbientTypeReferenceExpression { Name = string }

type nonNullable = nullable!
//@[000:0028) ├─DeclaredTypeExpression { Name = nonNullable }
//@[019:0028) | └─NonNullableTypeExpression { Name = null | string }
//@[019:0027) |   └─TypeAliasReferenceExpression { Name = nullable }

type typeA = {
//@[000:0044) ├─DeclaredTypeExpression { Name = typeA }
//@[013:0044) | └─ObjectTypeExpression { Name = { type: 'a', value: string } }
  type: 'a'
//@[002:0011) |   ├─ObjectTypePropertyExpression
//@[008:0011) |   | └─StringLiteralTypeExpression { Name = 'a' }
  value: string
//@[002:0015) |   └─ObjectTypePropertyExpression
//@[009:0015) |     └─AmbientTypeReferenceExpression { Name = string }
}

type typeB = {
//@[000:0041) ├─DeclaredTypeExpression { Name = typeB }
//@[013:0041) | └─ObjectTypeExpression { Name = { type: 'b', value: int } }
  type: 'b'
//@[002:0011) |   ├─ObjectTypePropertyExpression
//@[008:0011) |   | └─StringLiteralTypeExpression { Name = 'b' }
  value: int
//@[002:0012) |   └─ObjectTypePropertyExpression
//@[009:0012) |     └─AmbientTypeReferenceExpression { Name = int }
}

type typeC = {
//@[000:0059) ├─DeclaredTypeExpression { Name = typeC }
//@[013:0059) | └─ObjectTypeExpression { Name = { type: 'c', value: bool, value2: string } }
  type: 'c'
//@[002:0011) |   ├─ObjectTypePropertyExpression
//@[008:0011) |   | └─StringLiteralTypeExpression { Name = 'c' }
  value: bool
//@[002:0013) |   ├─ObjectTypePropertyExpression
//@[009:0013) |   | └─TypeAliasReferenceExpression { Name = bool }
  value2: string
//@[002:0016) |   └─ObjectTypePropertyExpression
//@[010:0016) |     └─AmbientTypeReferenceExpression { Name = string }
}

type typeD = {
//@[000:0044) ├─DeclaredTypeExpression { Name = typeD }
//@[013:0044) | └─ObjectTypeExpression { Name = { type: 'd', value: object } }
  type: 'd'
//@[002:0011) |   ├─ObjectTypePropertyExpression
//@[008:0011) |   | └─StringLiteralTypeExpression { Name = 'd' }
  value: object
//@[002:0015) |   └─ObjectTypePropertyExpression
//@[009:0015) |     └─AmbientTypeReferenceExpression { Name = object }
}

type typeE = {
//@[000:0047) ├─DeclaredTypeExpression { Name = typeE }
//@[013:0047) | └─ObjectTypeExpression { Name = { type: 'e', value: 'a' | 'b' } }
  type: 'e'
//@[002:0011) |   ├─ObjectTypePropertyExpression
//@[008:0011) |   | └─StringLiteralTypeExpression { Name = 'e' }
  value: 'a' | 'b'
//@[002:0018) |   └─ObjectTypePropertyExpression
//@[009:0018) |     └─UnionTypeExpression { Name = 'a' | 'b' }
//@[009:0012) |       ├─StringLiteralTypeExpression { Name = 'a' }
//@[015:0018) |       └─StringLiteralTypeExpression { Name = 'b' }
}

type typeF = {
//@[000:0040) ├─DeclaredTypeExpression { Name = typeF }
//@[013:0040) | └─ObjectTypeExpression { Name = { type: 'f', *: string } }
  type: 'f'
//@[002:0011) |   ├─ObjectTypePropertyExpression
//@[008:0011) |   | └─StringLiteralTypeExpression { Name = 'f' }
  *: string
//@[002:0011) |   └─ObjectTypeAdditionalPropertiesExpression
//@[005:0011) |     └─AmbientTypeReferenceExpression { Name = string }
}

@discriminator('type')
//@[000:0063) ├─DeclaredTypeExpression { Name = discriminatedUnion1 }
type discriminatedUnion1 = typeA | typeB
//@[027:0040) | └─DiscriminatedObjectTypeExpression { Name = { type: 'a', value: string } | { type: 'b', value: int } }
//@[027:0032) |   ├─TypeAliasReferenceExpression { Name = typeA }
//@[035:0040) |   └─TypeAliasReferenceExpression { Name = typeB }

@discriminator('type')
//@[000:0107) ├─DeclaredTypeExpression { Name = discriminatedUnion2 }
type discriminatedUnion2 = { type: 'c', value: string } | { type: 'd', value: bool }
//@[027:0084) | └─DiscriminatedObjectTypeExpression { Name = { type: 'c', value: string } | { type: 'd', value: bool } }
//@[027:0055) |   ├─ObjectTypeExpression { Name = { type: 'c', value: string } }
//@[029:0038) |   | ├─ObjectTypePropertyExpression
//@[035:0038) |   | | └─StringLiteralTypeExpression { Name = 'c' }
//@[040:0053) |   | └─ObjectTypePropertyExpression
//@[047:0053) |   |   └─AmbientTypeReferenceExpression { Name = string }
//@[058:0084) |   └─ObjectTypeExpression { Name = { type: 'd', value: bool } }
//@[060:0069) |     ├─ObjectTypePropertyExpression
//@[066:0069) |     | └─StringLiteralTypeExpression { Name = 'd' }
//@[071:0082) |     └─ObjectTypePropertyExpression
//@[078:0082) |       └─TypeAliasReferenceExpression { Name = bool }

@discriminator('type')
//@[000:0122) ├─DeclaredTypeExpression { Name = discriminatedUnion3 }
type discriminatedUnion3 = discriminatedUnion1 | discriminatedUnion2 | { type: 'e', value: string }
//@[027:0099) | └─DiscriminatedObjectTypeExpression { Name = { type: 'a', value: string } | { type: 'b', value: int } | { type: 'c', value: string } | { type: 'd', value: bool } | { type: 'e', value: string } }
//@[027:0046) |   ├─TypeAliasReferenceExpression { Name = discriminatedUnion1 }
//@[049:0068) |   ├─TypeAliasReferenceExpression { Name = discriminatedUnion2 }
//@[071:0099) |   └─ObjectTypeExpression { Name = { type: 'e', value: string } }
//@[073:0082) |     ├─ObjectTypePropertyExpression
//@[079:0082) |     | └─StringLiteralTypeExpression { Name = 'e' }
//@[084:0097) |     └─ObjectTypePropertyExpression
//@[091:0097) |       └─AmbientTypeReferenceExpression { Name = string }

@discriminator('type')
//@[000:0101) ├─DeclaredTypeExpression { Name = discriminatedUnion4 }
type discriminatedUnion4 = discriminatedUnion1 | (discriminatedUnion2 | typeE)
//@[027:0078) | └─DiscriminatedObjectTypeExpression { Name = { type: 'a', value: string } | { type: 'b', value: int } | { type: 'c', value: string } | { type: 'd', value: bool } | { type: 'e', value: 'a' | 'b' } }
//@[027:0046) |   ├─TypeAliasReferenceExpression { Name = discriminatedUnion1 }
//@[050:0077) |   └─DiscriminatedObjectTypeExpression { Name = { type: 'c', value: string } | { type: 'd', value: bool } | { type: 'e', value: 'a' | 'b' } }
//@[050:0069) |     ├─TypeAliasReferenceExpression { Name = discriminatedUnion2 }
//@[072:0077) |     └─TypeAliasReferenceExpression { Name = typeE }

@discriminator('type')
//@[000:0066) ├─DeclaredTypeExpression { Name = discriminatedUnion5 }
type discriminatedUnion5 = (typeA | typeB)?
//@[027:0043) | └─NullableTypeExpression { Name = null | ({ type: 'a', value: string } | { type: 'b', value: int }) }
//@[028:0041) |   └─DiscriminatedObjectTypeExpression { Name = { type: 'a', value: string } | { type: 'b', value: int } }
//@[028:0033) |     ├─TypeAliasReferenceExpression { Name = typeA }
//@[036:0041) |     └─TypeAliasReferenceExpression { Name = typeB }

@discriminator('type')
//@[000:0066) ├─DeclaredTypeExpression { Name = discriminatedUnion6 }
type discriminatedUnion6 = (typeA | typeB)!
//@[027:0043) | └─NonNullableTypeExpression { Name = { type: 'a', value: string } | { type: 'b', value: int } }
//@[028:0041) |   └─DiscriminatedObjectTypeExpression { Name = { type: 'a', value: string } | { type: 'b', value: int } }
//@[028:0033) |     ├─TypeAliasReferenceExpression { Name = typeA }
//@[036:0041) |     └─TypeAliasReferenceExpression { Name = typeB }

type inlineDiscriminatedUnion1 = {
//@[000:0083) ├─DeclaredTypeExpression { Name = inlineDiscriminatedUnion1 }
//@[033:0083) | └─ObjectTypeExpression { Name = { prop: typeA | typeC } }
  @discriminator('type')
//@[002:0046) |   └─ObjectTypePropertyExpression
  prop: typeA | typeC
//@[008:0021) |     └─DiscriminatedObjectTypeExpression { Name = { type: 'a', value: string } | { type: 'c', value: bool, value2: string } }
//@[008:0013) |       ├─TypeAliasReferenceExpression { Name = typeA }
//@[016:0021) |       └─TypeAliasReferenceExpression { Name = typeC }
}

type inlineDiscriminatedUnion2 = {
//@[000:0104) ├─DeclaredTypeExpression { Name = inlineDiscriminatedUnion2 }
//@[033:0104) | └─ObjectTypeExpression { Name = { prop: { type: 'a', value: bool } | typeB } }
  @discriminator('type')
//@[002:0067) |   └─ObjectTypePropertyExpression
  prop: { type: 'a', value: bool } | typeB
//@[008:0042) |     └─DiscriminatedObjectTypeExpression { Name = { type: 'a', value: bool } | { type: 'b', value: int } }
//@[008:0034) |       ├─ObjectTypeExpression { Name = { type: 'a', value: bool } }
//@[010:0019) |       | ├─ObjectTypePropertyExpression
//@[016:0019) |       | | └─StringLiteralTypeExpression { Name = 'a' }
//@[021:0032) |       | └─ObjectTypePropertyExpression
//@[028:0032) |       |   └─TypeAliasReferenceExpression { Name = bool }
//@[037:0042) |       └─TypeAliasReferenceExpression { Name = typeB }
}

@discriminator('type')
//@[000:0232) ├─DeclaredTypeExpression { Name = inlineDiscriminatedUnion3 }
type inlineDiscriminatedUnion3 = {
//@[033:0209) | └─DiscriminatedObjectTypeExpression { Name = { type: 'a', prop: { type: 'a', value: bool } | typeB } | { type: 'b', prop: discriminatedUnion1 | discriminatedUnion2 } }
//@[033:0116) |   ├─ObjectTypeExpression { Name = { type: 'a', prop: { type: 'a', value: bool } | typeB } }
  type: 'a'
//@[002:0011) |   | ├─ObjectTypePropertyExpression
//@[008:0011) |   | | └─StringLiteralTypeExpression { Name = 'a' }
  @discriminator('type')
//@[002:0067) |   | └─ObjectTypePropertyExpression
  prop: { type: 'a', value: bool } | typeB
//@[008:0042) |   |   └─DiscriminatedObjectTypeExpression { Name = { type: 'a', value: bool } | { type: 'b', value: int } }
//@[008:0034) |   |     ├─ObjectTypeExpression { Name = { type: 'a', value: bool } }
//@[010:0019) |   |     | ├─ObjectTypePropertyExpression
//@[016:0019) |   |     | | └─StringLiteralTypeExpression { Name = 'a' }
//@[021:0032) |   |     | └─ObjectTypePropertyExpression
//@[028:0032) |   |     |   └─TypeAliasReferenceExpression { Name = bool }
//@[037:0042) |   |     └─TypeAliasReferenceExpression { Name = typeB }
} | {
//@[004:0094) |   └─ObjectTypeExpression { Name = { type: 'b', prop: discriminatedUnion1 | discriminatedUnion2 } }
  type: 'b'
//@[002:0011) |     ├─ObjectTypePropertyExpression
//@[008:0011) |     | └─StringLiteralTypeExpression { Name = 'b' }
  @discriminator('type')
//@[002:0074) |     └─ObjectTypePropertyExpression
  prop: discriminatedUnion1 | discriminatedUnion2
//@[008:0049) |       └─DiscriminatedObjectTypeExpression { Name = { type: 'a', value: string } | { type: 'b', value: int } | { type: 'c', value: string } | { type: 'd', value: bool } }
//@[008:0027) |         ├─TypeAliasReferenceExpression { Name = discriminatedUnion1 }
//@[030:0049) |         └─TypeAliasReferenceExpression { Name = discriminatedUnion2 }
}

type inlineDiscriminatedUnion4 = {
//@[000:0086) ├─DeclaredTypeExpression { Name = inlineDiscriminatedUnion4 }
//@[033:0086) | └─ObjectTypeExpression { Name = { prop: (typeA | typeC)? } }
  @discriminator('type')
//@[002:0049) |   └─ObjectTypePropertyExpression
  prop: (typeA | typeC)?
//@[008:0024) |     └─NullableTypeExpression { Name = null | ({ type: 'a', value: string } | { type: 'c', value: bool, value2: string }) }
//@[009:0022) |       └─DiscriminatedObjectTypeExpression { Name = { type: 'a', value: string } | { type: 'c', value: bool, value2: string } }
//@[009:0014) |         ├─TypeAliasReferenceExpression { Name = typeA }
//@[017:0022) |         └─TypeAliasReferenceExpression { Name = typeC }
}

type discriminatorUnionAsPropertyType = {
//@[000:0101) ├─DeclaredTypeExpression { Name = discriminatorUnionAsPropertyType }
//@[040:0101) | └─ObjectTypeExpression { Name = { prop1: discriminatedUnion1, prop2: discriminatedUnion3 } }
  prop1: discriminatedUnion1
//@[002:0028) |   ├─ObjectTypePropertyExpression
//@[009:0028) |   | └─TypeAliasReferenceExpression { Name = discriminatedUnion1 }
  prop2: discriminatedUnion3
//@[002:0028) |   └─ObjectTypePropertyExpression
//@[009:0028) |     └─TypeAliasReferenceExpression { Name = discriminatedUnion3 }
}

type discriminatedUnionInlineAdditionalProps1 = {
//@[000:0095) ├─DeclaredTypeExpression { Name = discriminatedUnionInlineAdditionalProps1 }
//@[048:0095) | └─ObjectTypeExpression { Name = { *: typeA | typeB } }
  @discriminator('type')
//@[002:0043) |   └─ObjectTypeAdditionalPropertiesExpression
  *: typeA | typeB
//@[005:0018) |     └─DiscriminatedObjectTypeExpression { Name = { type: 'a', value: string } | { type: 'b', value: int } }
//@[005:0010) |       ├─TypeAliasReferenceExpression { Name = typeA }
//@[013:0018) |       └─TypeAliasReferenceExpression { Name = typeB }
}

type discriminatedUnionInlineAdditionalProps2 = {
//@[000:0098) ├─DeclaredTypeExpression { Name = discriminatedUnionInlineAdditionalProps2 }
//@[048:0098) | └─ObjectTypeExpression { Name = { *: (typeA | typeB)? } }
  @discriminator('type')
//@[002:0046) |   └─ObjectTypeAdditionalPropertiesExpression
  *: (typeA | typeB)?
//@[005:0021) |     └─NullableTypeExpression { Name = null | ({ type: 'a', value: string } | { type: 'b', value: int }) }
//@[006:0019) |       └─DiscriminatedObjectTypeExpression { Name = { type: 'a', value: string } | { type: 'b', value: int } }
//@[006:0011) |         ├─TypeAliasReferenceExpression { Name = typeA }
//@[014:0019) |         └─TypeAliasReferenceExpression { Name = typeB }
}

@discriminator('type')
//@[000:0111) ├─DeclaredTypeExpression { Name = discriminatorMemberHasAdditionalProperties1 }
type discriminatorMemberHasAdditionalProperties1 = typeA | typeF | { type: 'g', *: int }
//@[051:0088) | └─DiscriminatedObjectTypeExpression { Name = { type: 'a', value: string } | { type: 'f', *: string } | { type: 'g', *: int } }
//@[051:0056) |   ├─TypeAliasReferenceExpression { Name = typeA }
//@[059:0064) |   ├─TypeAliasReferenceExpression { Name = typeF }
//@[067:0088) |   └─ObjectTypeExpression { Name = { type: 'g', *: int } }
//@[069:0078) |     ├─ObjectTypePropertyExpression
//@[075:0078) |     | └─StringLiteralTypeExpression { Name = 'g' }
//@[080:0086) |     └─ObjectTypeAdditionalPropertiesExpression
//@[083:0086) |       └─AmbientTypeReferenceExpression { Name = int }

@discriminator('type')
//@[000:0137) ├─DeclaredTypeExpression { Name = discriminatorInnerSelfOptionalCycle1 }
type discriminatorInnerSelfOptionalCycle1 = typeA | {
//@[044:0114) | └─DiscriminatedObjectTypeExpression { Name = { type: 'a', value: string } | { type: 'b', value: discriminatorInnerSelfOptionalCycle1? } }
//@[044:0049) |   ├─TypeAliasReferenceExpression { Name = typeA }
//@[052:0114) |   └─ObjectTypeExpression { Name = { type: 'b', value: discriminatorInnerSelfOptionalCycle1? } }
  type: 'b'
//@[002:0011) |     ├─ObjectTypePropertyExpression
//@[008:0011) |     | └─StringLiteralTypeExpression { Name = 'b' }
  value: discriminatorInnerSelfOptionalCycle1?
//@[002:0046) |     └─ObjectTypePropertyExpression
//@[009:0046) |       └─NullableTypeExpression { Name = null | ({ type: 'a', value: string } | { type: 'b', value: discriminatorInnerSelfOptionalCycle1? }) }
//@[009:0045) |         └─TypeAliasReferenceExpression { Name = discriminatorInnerSelfOptionalCycle1 }
}

type discriminatedUnionMemberOptionalCycle1 = {
//@[000:0144) ├─DeclaredTypeExpression { Name = discriminatedUnionMemberOptionalCycle1 }
//@[046:0144) | └─ObjectTypeExpression { Name = { type: 'b', prop: (typeA | discriminatedUnionMemberOptionalCycle1)? } }
  type: 'b'
//@[002:0011) |   ├─ObjectTypePropertyExpression
//@[008:0011) |   | └─StringLiteralTypeExpression { Name = 'b' }
  @discriminator('type')
//@[002:0082) |   └─ObjectTypePropertyExpression
  prop: (typeA | discriminatedUnionMemberOptionalCycle1)?
//@[008:0057) |     └─NullableTypeExpression { Name = null | ({ type: 'a', value: string } | { type: 'b', prop: (typeA | discriminatedUnionMemberOptionalCycle1)? }) }
//@[009:0055) |       └─DiscriminatedObjectTypeExpression { Name = { type: 'a', value: string } | { type: 'b', prop: (typeA | discriminatedUnionMemberOptionalCycle1)? } }
//@[009:0014) |         ├─TypeAliasReferenceExpression { Name = typeA }
//@[017:0055) |         └─TypeAliasReferenceExpression { Name = discriminatedUnionMemberOptionalCycle1 }
}

type discriminatedUnionMemberOptionalCycle2 = {
//@[000:0138) ├─DeclaredTypeExpression { Name = discriminatedUnionMemberOptionalCycle2 }
//@[046:0138) | └─ObjectTypeExpression { Name = { type: 'b', *: typeA | discriminatedUnionMemberOptionalCycle1 } }
  type: 'b'
//@[002:0011) |   ├─ObjectTypePropertyExpression
//@[008:0011) |   | └─StringLiteralTypeExpression { Name = 'b' }
  @discriminator('type')
//@[002:0076) |   └─ObjectTypeAdditionalPropertiesExpression
  *: typeA | discriminatedUnionMemberOptionalCycle1
//@[005:0051) |     └─DiscriminatedObjectTypeExpression { Name = { type: 'a', value: string } | { type: 'b', prop: (typeA | discriminatedUnionMemberOptionalCycle1)? } }
//@[005:0010) |       ├─TypeAliasReferenceExpression { Name = typeA }
//@[013:0051) |       └─TypeAliasReferenceExpression { Name = discriminatedUnionMemberOptionalCycle1 }
}

type discriminatedUnionTuple1 = [
//@[000:0066) ├─DeclaredTypeExpression { Name = discriminatedUnionTuple1 }
//@[032:0066) | └─TupleTypeExpression { Name = [discriminatedUnion1, string] }
  discriminatedUnion1
//@[002:0021) |   ├─TupleTypeItemExpression
//@[002:0021) |   | └─TypeAliasReferenceExpression { Name = discriminatedUnion1 }
  string
//@[002:0008) |   └─TupleTypeItemExpression
//@[002:0008) |     └─AmbientTypeReferenceExpression { Name = string }
]

type discriminatedUnionInlineTuple1 = [
//@[000:0122) ├─DeclaredTypeExpression { Name = discriminatedUnionInlineTuple1 }
//@[038:0122) | └─TupleTypeExpression { Name = [typeA | typeB | { type: 'c', value: object }, string] }
  @discriminator('type')
//@[002:0071) |   ├─TupleTypeItemExpression
  typeA | typeB | { type: 'c', value: object }
//@[002:0046) |   | └─DiscriminatedObjectTypeExpression { Name = { type: 'a', value: string } | { type: 'b', value: int } | { type: 'c', value: object } }
//@[002:0007) |   |   ├─TypeAliasReferenceExpression { Name = typeA }
//@[010:0015) |   |   ├─TypeAliasReferenceExpression { Name = typeB }
//@[018:0046) |   |   └─ObjectTypeExpression { Name = { type: 'c', value: object } }
//@[020:0029) |   |     ├─ObjectTypePropertyExpression
//@[026:0029) |   |     | └─StringLiteralTypeExpression { Name = 'c' }
//@[031:0044) |   |     └─ObjectTypePropertyExpression
//@[038:0044) |   |       └─AmbientTypeReferenceExpression { Name = object }
  string
//@[002:0008) |   └─TupleTypeItemExpression
//@[002:0008) |     └─AmbientTypeReferenceExpression { Name = string }
]

param paramDiscriminatedUnionTypeAlias1 discriminatedUnion1
//@[000:0059) ├─DeclaredParameterExpression { Name = paramDiscriminatedUnionTypeAlias1 }
//@[040:0059) | └─TypeAliasReferenceExpression { Name = discriminatedUnion1 }
param paramDiscriminatedUnionTypeAlias2 discriminatedUnion5
//@[000:0059) ├─DeclaredParameterExpression { Name = paramDiscriminatedUnionTypeAlias2 }
//@[040:0059) | └─TypeAliasReferenceExpression { Name = discriminatedUnion5 }

@discriminator('type')
//@[000:0073) ├─DeclaredParameterExpression { Name = paramInlineDiscriminatedUnion1 }
param paramInlineDiscriminatedUnion1 typeA | typeB
//@[037:0050) | └─DiscriminatedObjectTypeExpression { Name = { type: 'a', value: string } | { type: 'b', value: int } }
//@[037:0042) |   ├─TypeAliasReferenceExpression { Name = typeA }
//@[045:0050) |   └─TypeAliasReferenceExpression { Name = typeB }

@discriminator('type')
//@[000:0101) ├─DeclaredParameterExpression { Name = paramInlineDiscriminatedUnion2 }
param paramInlineDiscriminatedUnion2 (typeA | typeB) = { type: 'b', value: 0 }
//@[038:0051) | ├─DiscriminatedObjectTypeExpression { Name = { type: 'a', value: string } | { type: 'b', value: int } }
//@[038:0043) | | ├─TypeAliasReferenceExpression { Name = typeA }
//@[046:0051) | | └─TypeAliasReferenceExpression { Name = typeB }
//@[055:0078) | └─ObjectExpression
//@[057:0066) |   ├─ObjectPropertyExpression
//@[057:0061) |   | ├─StringLiteralExpression { Value = type }
//@[063:0066) |   | └─StringLiteralExpression { Value = b }
//@[068:0076) |   └─ObjectPropertyExpression
//@[068:0073) |     ├─StringLiteralExpression { Value = value }
//@[075:0076) |     └─IntegerLiteralExpression { Value = 0 }

@discriminator('type')
//@[000:0076) ├─DeclaredParameterExpression { Name = paramInlineDiscriminatedUnion3 }
param paramInlineDiscriminatedUnion3 (typeA | typeB)?
//@[037:0053) | └─NullableTypeExpression { Name = null | ({ type: 'a', value: string } | { type: 'b', value: int }) }
//@[038:0051) |   └─DiscriminatedObjectTypeExpression { Name = { type: 'a', value: string } | { type: 'b', value: int } }
//@[038:0043) |     ├─TypeAliasReferenceExpression { Name = typeA }
//@[046:0051) |     └─TypeAliasReferenceExpression { Name = typeB }

output outputDiscriminatedUnionTypeAlias1 discriminatedUnion1 = { type: 'a', value: 'str' }
//@[000:0091) ├─DeclaredOutputExpression { Name = outputDiscriminatedUnionTypeAlias1 }
//@[042:0061) | ├─TypeAliasReferenceExpression { Name = discriminatedUnion1 }
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
//@[042:0061) | ├─TypeAliasReferenceExpression { Name = discriminatedUnion1 }
//@[064:0091) | └─ObjectExpression
//@[066:0075) |   ├─ObjectPropertyExpression
//@[066:0070) |   | ├─StringLiteralExpression { Value = type }
//@[072:0075) |   | └─StringLiteralExpression { Value = a }
//@[077:0089) |   └─ObjectPropertyExpression
//@[077:0082) |     ├─StringLiteralExpression { Value = value }
//@[084:0089) |     └─StringLiteralExpression { Value = str }
output outputDiscriminatedUnionTypeAlias3 discriminatedUnion5 = null
//@[000:0068) ├─DeclaredOutputExpression { Name = outputDiscriminatedUnionTypeAlias3 }
//@[042:0061) | ├─TypeAliasReferenceExpression { Name = discriminatedUnion5 }
//@[064:0068) | └─NullLiteralExpression

@discriminator('type')
//@[000:0131) ├─DeclaredOutputExpression { Name = outputInlineDiscriminatedUnion1 }
output outputInlineDiscriminatedUnion1 typeA | typeB | { type: 'c', value: int } = { type: 'a', value: 'a' }
//@[039:0080) | ├─DiscriminatedObjectTypeExpression { Name = { type: 'a', value: string } | { type: 'b', value: int } | { type: 'c', value: int } }
//@[039:0044) | | ├─TypeAliasReferenceExpression { Name = typeA }
//@[047:0052) | | ├─TypeAliasReferenceExpression { Name = typeB }
//@[055:0080) | | └─ObjectTypeExpression { Name = { type: 'c', value: int } }
//@[057:0066) | |   ├─ObjectTypePropertyExpression
//@[063:0066) | |   | └─StringLiteralTypeExpression { Name = 'c' }
//@[068:0078) | |   └─ObjectTypePropertyExpression
//@[075:0078) | |     └─AmbientTypeReferenceExpression { Name = int }
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
//@[039:0082) | ├─DiscriminatedObjectTypeExpression { Name = { type: 'a', value: string } | { type: 'b', value: int } | { type: 'c', value: int } }
//@[039:0044) | | ├─TypeAliasReferenceExpression { Name = typeA }
//@[047:0052) | | ├─TypeAliasReferenceExpression { Name = typeB }
//@[056:0081) | | └─ObjectTypeExpression { Name = { type: 'c', value: int } }
//@[058:0067) | |   ├─ObjectTypePropertyExpression
//@[064:0067) | |   | └─StringLiteralTypeExpression { Name = 'c' }
//@[069:0079) | |   └─ObjectTypePropertyExpression
//@[076:0079) | |     └─AmbientTypeReferenceExpression { Name = int }
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
//@[039:0055)   ├─NullableTypeExpression { Name = null | ({ type: 'a', value: string } | { type: 'b', value: int }) }
//@[040:0053)   | └─DiscriminatedObjectTypeExpression { Name = { type: 'a', value: string } | { type: 'b', value: int } }
//@[040:0045)   |   ├─TypeAliasReferenceExpression { Name = typeA }
//@[048:0053)   |   └─TypeAliasReferenceExpression { Name = typeB }
//@[058:0062)   └─NullLiteralExpression

