/* 
  Valid and invalid code is mixed together to validate recovery logic. It can even contain ** * *** **.
*/

param myString string
wrong

param myInt int
param

param 3
param % string
param % string 3 = 's'

param myBool bool

param missingType

// space after identifier #completionTest(32) -> paramTypes
param missingTypeWithSpaceAfter 

// tab after identifier #completionTest(30) -> paramTypes
param missingTypeWithTabAfter	

// partial type #completionTest(18, 19, 20, 21) -> paramTypes
param partialType str

param malformedType 44

// malformed type but type check should still happen
param malformedType2 44 = f

// malformed type but type check should still happen
param malformedModifier 44 {
  secure: 's'
}

param myString2 string = 'string value'

param wrongDefaultValue string = 42

param myInt2 int = 42
param noValueAfterColon int =   

param myTruth bool = 'not a boolean'
param myFalsehood bool = 'false'

param wrongAssignmentToken string: 'hello'

param WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong string = 'why not?'

// badly escaped string
param wrongType fluffyBunny = 'what's up doc?'

// invalid escape
param wrongType fluffyBunny = 'what\s up doc?'

// unterminated string 
param wrongType fluffyBunny = 'what\'s up doc?

// unterminated interpolated string
param wrongType fluffyBunny = 'what\'s ${
param wrongType fluffyBunny = 'what\'s ${up
param wrongType fluffyBunny = 'what\'s ${up}
param wrongType fluffyBunny = 'what\'s ${'up

// unterminated nested interpolated string
param wrongType fluffyBunny = 'what\'s ${'up${
param wrongType fluffyBunny = 'what\'s ${'up${
param wrongType fluffyBunny = 'what\'s ${'up${doc
param wrongType fluffyBunny = 'what\'s ${'up${doc}
param wrongType fluffyBunny = 'what\'s ${'up${doc}'
param wrongType fluffyBunny = 'what\'s ${'up${doc}'}?

// object literal inside interpolated string
param wrongType fluffyBunny = '${{this: doesnt}.work}'

// bad interpolated string format
param badInterpolatedString string = 'hello ${}!'
param badInterpolatedString2 string = 'hello ${a b c}!'

param wrongType fluffyBunny = 'what\'s up doc?'

// modifier on an invalid type
param someArray arra {
  minLength: 3
  maxLength: 24
}

// duplicate modifier property
param duplicatedModifierProperty string {
  minLength: 3
  minLength: 24
}

// non-existent modifiers
param secureInt int {
  secure: true
  minLength: 3
  maxLength: 123
}

// wrong modifier value types
param wrongIntModifier int {
  default: true
  allowed: [
    'test'
    true
  ]
  minValue: {
  }
  maxValue: [
  ]
  metadata: 'wrong'
}

// wrong metadata schema
param wrongMetadataSchema string {
  metadata: {
    description: true
  }
}

// expression in modifier
param expressionInModifier string {
  default: 2 + 3
  maxLength: a + 2
  minLength: foo()
  allowed: [
    i
  ]
}

// 1-cycle in params
param paramDefaultOneCycle string = paramDefaultOneCycle

// 2-cycle in params
param paramDefaultTwoCycle1 string = paramDefaultTwoCycle2
param paramDefaultTwoCycle2 string = paramDefaultTwoCycle1

// 1-cycle in modifier params
param paramModifierOneCycle string {
  default: paramModifierOneCycle
}

// 1-cycle in modifier with non-default property
param paramModifierSelfCycle string {
  allowed: [
    paramModifierSelfCycle
  ]
}

// 2-cycle in modifier params
param paramModifierTwoCycle1 string {
  default: paramModifierTwoCycle2
}
param paramModifierTwoCycle2 string {
  default: paramModifierTwoCycle1
}

// 2-cycle mixed param syntaxes
param paramMixedTwoCycle1 string = paramMixedTwoCycle2
param paramMixedTwoCycle2 string {
  default: paramMixedTwoCycle1
}

// wrong types of "variable"/identifier access
var sampleVar = 'sample'
resource sampleResource 'Microsoft.Foo/foos@2020-02-02' = {
  name: 'foo'
}
output sampleOutput string = 'hello'

param paramAccessingVar string = concat(sampleVar, 's')
param paramAccessingVar2 string {
  default: 'foo ${sampleVar} foo'
}

param paramAccessingResource string = sampleResource
param paramAccessingResource2 string {
  default: base64(sampleResource.properties.foo)
}

param paramAccessingOutput string = sampleOutput
param paramAccessingOutput2 string {
  default: sampleOutput
}

param stringLiteral string {
  allowed: [
    'def'
  ]
}

param stringLiteral2 string {
  allowed: [
    'abc'
    'def'
  ]
  default: stringLiteral
}

param stringLiteral3 string {
  allowed: [
    'abc'
  ]
  default: stringLiteral2
}

// unterminated multi-line comment
/*    