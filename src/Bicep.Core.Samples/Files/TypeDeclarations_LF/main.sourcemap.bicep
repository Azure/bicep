// @description('The foo type')
// @sealed()
type foo = {
//@[13:62]     "foo": {
  @minLength(3)
//@[56:56]           "minLength": 3
  @maxLength(10)
//@[55:55]           "maxLength": 10,
  // @description('A string property')
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
//@[77:77]       "minLength": 3
// @description('An array of array of arrays of arrays of ints')
// @metadata({
//   examples: [
//     [[[[1]]], [[[2]]], [[[3]]]]
//   ]
// })
type bar = int[][][][]
//@[63:78]     "bar": {

type aUnion = 'snap'|'crackle'|'pop'
//@[79:86]     "aUnion": {

type expandedUnion = aUnion|'fizz'|'buzz'|'pop'
//@[87:96]     "expandedUnion": {

type mixedArray = ('heffalump'|'woozle'|{ shape: '*', size: '*'}|10|-10|true|!true|null)[]
//@[97:112]     "mixedArray": {

param inlineObjectParam {
//@[115:145]     "inlineObjectParam": {
  foo: string
  bar: 100|200|300|400|500
  baz: bool
} = {
  foo: 'foo'
//@[141:141]         "foo": "foo",
  bar: 300
//@[142:142]         "bar": 300,
  baz: false
//@[143:143]         "baz": false
}

