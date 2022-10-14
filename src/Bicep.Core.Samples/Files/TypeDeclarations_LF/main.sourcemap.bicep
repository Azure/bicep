@description('The foo type')
//@[66:66]         "description": "The foo type"
type foo = {
//@[13:68]     "foo": {
  @minLength(3)
//@[59:59]           "minLength": 3
  @maxLength(10)
//@[58:58]           "maxLength": 10,
  @description('A string property')
//@[56:56]             "description": "A string property"
  stringProp: string

  objectProp: {
    @minValue(1)
//@[46:46]               "minValue": 1
    intProp: int

    intArrayArrayProp?: int [] []
  }

  typeRefProp: bar

  literalProp: 'literal'

  recursion: foo
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

param inlineObjectParam {
//@[149:179]     "inlineObjectParam": {
  foo: string
  bar: 100|200|300|400|500
  baz: bool
} = {
  foo: 'foo'
//@[175:175]         "foo": "foo",
  bar: 300
//@[176:176]         "bar": 300,
  baz: false
//@[177:177]         "baz": false
}

