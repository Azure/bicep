﻿/* 
  Valid and invalid code is mixed together to validate recovery logic. It can even contain ** * *** **.
*/

parameter myString string
wrong

parameter myInt int
parameter

parameter myBool bool

parameter missingType

parameter myString2 string = 'string value'

parameter wrongDefaultValue string = 42

parameter myInt2 int = 42
parameter noValueAfterColon int =   

parameter myTruth bool = 'not a boolean'
parameter myFalsehood bool = 'false'

parameter wrongAssignmentToken string: 'hello'

parameter WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong string = 'why not?'

// badly escaped string
parameter wrongType fluffyBunny = 'what's up doc?'

// invalid escape
parameter wrongType fluffyBunny = 'what\s up doc?'

// unterminated string 
parameter wrongType fluffyBunny = 'what\'s up doc?

// unterminated interpolated string
parameter wrongType fluffyBunny = 'what\'s ${
parameter wrongType fluffyBunny = 'what\'s ${up
parameter wrongType fluffyBunny = 'what\'s ${up}
parameter wrongType fluffyBunny = 'what\'s ${'up

// unterminated nested interpolated string
parameter wrongType fluffyBunny = 'what\'s ${'up${
parameter wrongType fluffyBunny = 'what\'s ${'up${
parameter wrongType fluffyBunny = 'what\'s ${'up${doc
parameter wrongType fluffyBunny = 'what\'s ${'up${doc}
parameter wrongType fluffyBunny = 'what\'s ${'up${doc}'
parameter wrongType fluffyBunny = 'what\'s ${'up${doc}'}?

// object literal inside interpolated string
parameter wrongType fluffyBunny = '${{this: doesnt}.work}'

parameter wrongType fluffyBunny = 'what\'s up doc?'

// modifier on an invalid type
parameter someArray arra {
  minLength: 3
  maxLength: 24
}

// duplicate modifier property
parameter duplicatedModifierProperty string {
  minLength: 3
  minLength: 24
}

// non-existent modifiers
parameter secureInt int {
  secure: true
  minLength: 3
  maxLength: 123
}

// wrong modifier value types
parameter wrongIntModifier int {
  defaultValue: true
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
parameter wrongMetadataSchema string {
  metadata: {
    description: true
  }
}

// expression in modifier
parameter expressionInModifier string {
  defaultValue: 2 + 3
  maxLength: a + 2
  minLength: foo()
  allowedValues: [
    i
  ]
}

// unterminated multi-line comment
/*    