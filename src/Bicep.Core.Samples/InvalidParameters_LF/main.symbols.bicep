/* 
  Valid and invalid code is mixed together to validate recovery logic. It can even contain ** * *** **.
*/

param myString string
//@[6:14) Parameter myString. Declaration start char: 0, length: 22
wrong

param myInt int
//@[6:11) Parameter myInt. Declaration start char: 0, length: 16
param

param myBool bool
//@[6:12) Parameter myBool. Declaration start char: 0, length: 19

param missingType

param myString2 string = 'string value'
//@[6:15) Parameter myString2. Declaration start char: 0, length: 41

param wrongDefaultValue string = 42
//@[6:23) Parameter wrongDefaultValue. Declaration start char: 0, length: 37

param myInt2 int = 42
//@[6:12) Parameter myInt2. Declaration start char: 0, length: 22
param noValueAfterColon int =   

param myTruth bool = 'not a boolean'
//@[6:13) Parameter myTruth. Declaration start char: 0, length: 37
param myFalsehood bool = 'false'
//@[6:17) Parameter myFalsehood. Declaration start char: 0, length: 34

param wrongAssignmentToken string: 'hello'

param WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong string = 'why not?'
//@[6:267) Parameter WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong. Declaration start char: 0, length: 289

// badly escaped string
param wrongType fluffyBunny = 'what's up doc?'

// invalid escape
param wrongType fluffyBunny = 'what\s up doc?'
//@[6:15) Parameter wrongType. Declaration start char: 0, length: 48

// unterminated string 
param wrongType fluffyBunny = 'what\'s up doc?
//@[6:15) Parameter wrongType. Declaration start char: 0, length: 48

// unterminated interpolated string
param wrongType fluffyBunny = 'what\'s ${
//@[6:15) Parameter wrongType. Declaration start char: 0, length: 42
param wrongType fluffyBunny = 'what\'s ${up
//@[6:15) Parameter wrongType. Declaration start char: 0, length: 44
param wrongType fluffyBunny = 'what\'s ${up}
//@[6:15) Parameter wrongType. Declaration start char: 0, length: 45
param wrongType fluffyBunny = 'what\'s ${'up
//@[6:15) Parameter wrongType. Declaration start char: 0, length: 46

// unterminated nested interpolated string
param wrongType fluffyBunny = 'what\'s ${'up${
//@[6:15) Parameter wrongType. Declaration start char: 0, length: 47
param wrongType fluffyBunny = 'what\'s ${'up${
//@[6:15) Parameter wrongType. Declaration start char: 0, length: 47
param wrongType fluffyBunny = 'what\'s ${'up${doc
//@[6:15) Parameter wrongType. Declaration start char: 0, length: 50
param wrongType fluffyBunny = 'what\'s ${'up${doc}
//@[6:15) Parameter wrongType. Declaration start char: 0, length: 51
param wrongType fluffyBunny = 'what\'s ${'up${doc}'
//@[6:15) Parameter wrongType. Declaration start char: 0, length: 52
param wrongType fluffyBunny = 'what\'s ${'up${doc}'}?
//@[6:15) Parameter wrongType. Declaration start char: 0, length: 55

// object literal inside interpolated string
param wrongType fluffyBunny = '${{this: doesnt}.work}'
//@[6:15) Parameter wrongType. Declaration start char: 0, length: 56

// bad interpolated string format
param badInterpolatedString string = 'hello ${}!'
//@[6:27) Parameter badInterpolatedString. Declaration start char: 0, length: 50
param badInterpolatedString2 string = 'hello ${a b c}!'
//@[6:28) Parameter badInterpolatedString2. Declaration start char: 0, length: 57

param wrongType fluffyBunny = 'what\'s up doc?'
//@[6:15) Parameter wrongType. Declaration start char: 0, length: 49

// modifier on an invalid type
param someArray arra {
//@[6:15) Parameter someArray. Declaration start char: 0, length: 57
  minLength: 3
  maxLength: 24
}

// duplicate modifier property
param duplicatedModifierProperty string {
//@[6:32) Parameter duplicatedModifierProperty. Declaration start char: 0, length: 76
  minLength: 3
  minLength: 24
}

// non-existent modifiers
param secureInt int {
//@[6:15) Parameter secureInt. Declaration start char: 0, length: 72
  secure: true
  minLength: 3
  maxLength: 123
}

// wrong modifier value types
param wrongIntModifier int {
//@[6:22) Parameter wrongIntModifier. Declaration start char: 0, length: 141
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
//@[6:25) Parameter wrongMetadataSchema. Declaration start char: 0, length: 78
  metadata: {
    description: true
  }
}

// expression in modifier
param expressionInModifier string {
//@[6:26) Parameter expressionInModifier. Declaration start char: 0, length: 117
  default: 2 + 3
  maxLength: a + 2
  minLength: foo()
  allowed: [
    i
  ]
}

// 1-cycle in params
param paramDefaultOneCycle string = paramDefaultOneCycle
//@[6:26) Parameter paramDefaultOneCycle. Declaration start char: 0, length: 58

// 2-cycle in params
param paramDefaultTwoCycle1 string = paramDefaultTwoCycle2
//@[6:27) Parameter paramDefaultTwoCycle1. Declaration start char: 0, length: 59
param paramDefaultTwoCycle2 string = paramDefaultTwoCycle1
//@[6:27) Parameter paramDefaultTwoCycle2. Declaration start char: 0, length: 60

// 1-cycle in modifier params
param paramModifierOneCycle string {
//@[6:27) Parameter paramModifierOneCycle. Declaration start char: 0, length: 73
  default: paramModifierOneCycle
}

// 1-cycle in modifier with non-default property
param paramModifierSelfCycle string {
//@[6:28) Parameter paramModifierSelfCycle. Declaration start char: 0, length: 85
  allowed: [
    paramModifierSelfCycle
  ]
}

// 2-cycle in modifier params
param paramModifierTwoCycle1 string {
//@[6:28) Parameter paramModifierTwoCycle1. Declaration start char: 0, length: 74
  default: paramModifierTwoCycle2
}
param paramModifierTwoCycle2 string {
//@[6:28) Parameter paramModifierTwoCycle2. Declaration start char: 0, length: 75
  default: paramModifierTwoCycle1
}

// 2-cycle mixed param syntaxes
param paramMixedTwoCycle1 string = paramMixedTwoCycle2
//@[6:25) Parameter paramMixedTwoCycle1. Declaration start char: 0, length: 55
param paramMixedTwoCycle2 string {
//@[6:25) Parameter paramMixedTwoCycle2. Declaration start char: 0, length: 69
  default: paramMixedTwoCycle1
}

// wrong types of "variable"/identifier access
var sampleVar = 'sample'
//@[4:13) Variable sampleVar. Declaration start char: 0, length: 25
resource sampleResource 'Microsoft.Foo/foos@2020-02-02' = {
//@[9:23) Resource sampleResource. Declaration start char: 0, length: 76
  name: 'foo'
}
output sampleOutput string = 'hello'
//@[7:19) Output sampleOutput. Declaration start char: 0, length: 38

param paramAccessingVar string = concat(sampleVar, 's')
//@[6:23) Parameter paramAccessingVar. Declaration start char: 0, length: 56
param paramAccessingVar2 string {
//@[6:24) Parameter paramAccessingVar2. Declaration start char: 0, length: 71
  default: 'foo ${sampleVar} foo'
}

param paramAccessingResource string = sampleResource
//@[6:28) Parameter paramAccessingResource. Declaration start char: 0, length: 53
param paramAccessingResource2 string {
//@[6:29) Parameter paramAccessingResource2. Declaration start char: 0, length: 91
  default: base64(sampleResource.properties.foo)
}

param paramAccessingOutput string = sampleOutput
//@[6:26) Parameter paramAccessingOutput. Declaration start char: 0, length: 49
param paramAccessingOutput2 string {
//@[6:27) Parameter paramAccessingOutput2. Declaration start char: 0, length: 64
  default: sampleOutput
}

// unterminated multi-line comment
/*    
