/* 
  Valid and invalid code is mixed together to validate recovery logic. It can even contain ** * *** **.
*/

param myString string
//@[6:14] Parameter myString
wrong

param myInt int
//@[6:11] Parameter myInt
param

param myBool bool
//@[6:12] Parameter myBool

param missingType

param myString2 string = 'string value'
//@[6:15] Parameter myString2

param wrongDefaultValue string = 42
//@[6:23] Parameter wrongDefaultValue

param myInt2 int = 42
//@[6:12] Parameter myInt2
param noValueAfterColon int =   

param myTruth bool = 'not a boolean'
//@[6:13] Parameter myTruth
param myFalsehood bool = 'false'
//@[6:17] Parameter myFalsehood

param wrongAssignmentToken string: 'hello'

param WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong string = 'why not?'
//@[6:267] Parameter WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong

// badly escaped string
param wrongType fluffyBunny = 'what's up doc?'

// invalid escape
param wrongType fluffyBunny = 'what\s up doc?'
//@[6:15] Parameter wrongType

// unterminated string 
param wrongType fluffyBunny = 'what\'s up doc?
//@[6:15] Parameter wrongType

// unterminated interpolated string
param wrongType fluffyBunny = 'what\'s ${
param wrongType fluffyBunny = 'what\'s ${up
param wrongType fluffyBunny = 'what\'s ${up}
//@[6:15] Parameter wrongType
param wrongType fluffyBunny = 'what\'s ${'up
//@[6:15] Parameter wrongType

// unterminated nested interpolated string
param wrongType fluffyBunny = 'what\'s ${'up${
param wrongType fluffyBunny = 'what\'s ${'up${
param wrongType fluffyBunny = 'what\'s ${'up${doc
param wrongType fluffyBunny = 'what\'s ${'up${doc}
//@[6:15] Parameter wrongType
param wrongType fluffyBunny = 'what\'s ${'up${doc}'
//@[6:15] Parameter wrongType
param wrongType fluffyBunny = 'what\'s ${'up${doc}'}?
//@[6:15] Parameter wrongType

// object literal inside interpolated string
param wrongType fluffyBunny = '${{this: doesnt}.work}'

param wrongType fluffyBunny = 'what\'s up doc?'

// modifier on an invalid type
param someArray arra {
//@[6:15] Parameter someArray
  minLength: 3
  maxLength: 24
}

// duplicate modifier property
param duplicatedModifierProperty string {
//@[6:32] Parameter duplicatedModifierProperty
  minLength: 3
  minLength: 24
}

// non-existent modifiers
param secureInt int {
//@[6:15] Parameter secureInt
  secure: true
  minLength: 3
  maxLength: 123
}

// wrong modifier value types
param wrongIntModifier int {
//@[6:22] Parameter wrongIntModifier
  default: true
  allowedValues: [
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
//@[6:25] Parameter wrongMetadataSchema
  metadata: {
    description: true
  }
}

// expression in modifier
param expressionInModifier string {
//@[6:26] Parameter expressionInModifier
  default: 2 + 3
  maxLength: a + 2
  minLength: foo()
  allowedValues: [
    i
  ]
}

// unterminated multi-line comment
/*    
