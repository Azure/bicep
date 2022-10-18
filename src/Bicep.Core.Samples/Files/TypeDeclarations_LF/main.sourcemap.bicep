// @description('The foo type')
// @sealed()
type foo = {
//@[13:61]     "foo": {
  @minLength(3)
//@[55:55]           "minLength": 3
  @maxLength(10)
//@[54:54]           "maxLength": 10,
  // @description('A string property')
  stringProp: string

  objectProp: {
    @minValue(1)
//@[45:45]               "minValue": 1
    intProp: int

    intArrayArrayProp?: int [] []
  }

  typeRefProp: bar

  literalProp: 'literal'

  recursion?: foo
}

@minLength(3)
//@[76:76]       "minLength": 3
// @description('An array of array of arrays of arrays of ints')
// @metadata({
//   examples: [
//     [[[[1]]], [[[2]]], [[[3]]]]
//   ]
// })
type bar = int[][][][]
//@[62:77]     "bar": {

type aUnion = 'snap'|'crackle'|'pop'
//@[78:85]     "aUnion": {

type expandedUnion = aUnion|'fizz'|'buzz'|'pop'
//@[86:95]     "expandedUnion": {

type mixedArray = ('heffalump'|'woozle'|{ shape: '*', size: '*'}|10|-10|true|!true|null)[]
//@[96:111]     "mixedArray": {

param inlineObjectParam {
//@[114:144]     "inlineObjectParam": {
  foo: string
  bar: 100|200|300|400|500
  baz: bool
} = {
  foo: 'foo'
//@[140:140]         "foo": "foo",
  bar: 300
//@[141:141]         "bar": 300,
  baz: false
//@[142:142]         "baz": false
}

