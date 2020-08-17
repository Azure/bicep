﻿/* 
  Valid and invalid code is mixed together to validate recovery logic. It can even contain ** * *** **.
*/

param myString string
wrong

param myInt int
parameter

param myBool bool

param missingType

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
param wrongMetadataSchema string {
  metadata: {
    description: true
  }
}

// expression in modifier
param expressionInModifier string {
  defaultValue: 2 + 3
  maxLength: a + 2
  minLength: foo()
  allowedValues: [
    i
  ]
}

// unterminated multi-line comment
/*    