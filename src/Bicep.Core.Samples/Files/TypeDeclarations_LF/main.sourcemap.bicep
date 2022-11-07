@description('The foo type')
//@[66:66]         "description": "The foo type"
@sealed()
type foo = {
//@[13:68]     "foo": {
  @minLength(3)
//@[28:28]           "minLength": 3
  @maxLength(10)
//@[27:27]           "maxLength": 10,
  @description('A string property')
//@[25:25]             "description": "A string property"
  stringProp: string

  objectProp: {
    @minValue(1)
//@[38:38]               "minValue": 1
    intProp: int

    intArrayArrayProp?: int [] []
  }

  typeRefProp: bar

  literalProp: 'literal'
//@[57:57]             "literal"

  recursion?: foo
}

@minLength(3)
//@[111:111]       "minLength": 3
@description('An array of array of arrays of arrays of ints')
//@[109:109]         "description": "An array of array of arrays of arrays of ints"
@metadata({
  examples: [
//@[84:108]         "examples": [
    [[[[1]]], [[[2]]], [[[3]]]]
//@[85:107]                   1
  ]
})
type bar = int[][][][]
//@[69:112]     "bar": {

type aUnion = 'snap'|'crackle'|'pop'
//@[113:120]     "aUnion": {

type expandedUnion = aUnion|'fizz'|'buzz'|'pop'
//@[121:130]     "expandedUnion": {

type mixedArray = ('heffalump'|'woozle'|{ shape: '*', size: '*'}|10|-10|true|!true|null)[]
//@[131:146]     "mixedArray": {

type bool = string
//@[147:149]     "bool": {

param inlineObjectParam {
//@[152:182]     "inlineObjectParam": {
  foo: string
  bar: 100|200|300|400|500
  baz: sys.bool
} = {
  foo: 'foo'
//@[178:178]         "foo": "foo",
  bar: 300
//@[179:179]         "bar": 300,
  baz: false
//@[180:180]         "baz": false
}

param unionParam {property: 'ping'}|{property: 'pong'} = {property: 'pong'}
//@[183:196]     "unionParam": {

param paramUsingType mixedArray
//@[197:199]     "paramUsingType": {

