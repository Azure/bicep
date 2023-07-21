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

    intArrayArrayProp: int [] []
  }

  @sealed()
  sealedObjectProp: {
    fizz: int
    buzz: bool
    pop: bar
  }
}

@minLength(3)
@description('An array of array of arrays of arrays of ints')
@metadata({
  examples: [
    [[[[1]]], [[[2]]], [[[3]]]]
  ]
})
type bar = int[][][][]
