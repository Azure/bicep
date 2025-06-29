/*
//@[000:3213) ProgramExpression
  This is a block comment.
*/

// parameters without default value
@sys.description('''
//@[000:0097) ├─DeclaredParameterExpression { Name = myString }
//@[017:0074) | ├─StringLiteralExpression { Value = this is my multi line\ndescription for my myString\n }
this is my multi line
description for my myString
''')
param myString string
//@[015:0021) | └─AmbientTypeReferenceExpression { Name = string }
param myInt int
//@[000:0015) ├─DeclaredParameterExpression { Name = myInt }
//@[012:0015) | └─AmbientTypeReferenceExpression { Name = int }
param myBool bool
//@[000:0017) ├─DeclaredParameterExpression { Name = myBool }
//@[013:0017) | └─AmbientTypeReferenceExpression { Name = bool }

// parameters with default value
@sys.description('this is myString2')
//@[000:0135) ├─DeclaredParameterExpression { Name = myString2 }
//@[017:0036) | ├─StringLiteralExpression { Value = this is myString2 }
@metadata({
//@[010:0056) | ├─ObjectExpression
  description: 'overwrite but still valid'
//@[002:0042) | | └─ObjectPropertyExpression
//@[002:0013) | |   ├─StringLiteralExpression { Value = description }
//@[015:0042) | |   └─StringLiteralExpression { Value = overwrite but still valid }
})
param myString2 string = 'string value'
//@[016:0022) | ├─AmbientTypeReferenceExpression { Name = string }
//@[025:0039) | └─StringLiteralExpression { Value = string value }
param myInt2 int = 42
//@[000:0021) ├─DeclaredParameterExpression { Name = myInt2 }
//@[013:0016) | ├─AmbientTypeReferenceExpression { Name = int }
//@[019:0021) | └─IntegerLiteralExpression { Value = 42 }
param myTruth bool = true
//@[000:0025) ├─DeclaredParameterExpression { Name = myTruth }
//@[014:0018) | ├─AmbientTypeReferenceExpression { Name = bool }
//@[021:0025) | └─BooleanLiteralExpression { Value = True }
param myFalsehood bool = false
//@[000:0030) ├─DeclaredParameterExpression { Name = myFalsehood }
//@[018:0022) | ├─AmbientTypeReferenceExpression { Name = bool }
//@[025:0030) | └─BooleanLiteralExpression { Value = False }
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[000:0067) ├─DeclaredParameterExpression { Name = myEscapedString }
//@[022:0028) | ├─AmbientTypeReferenceExpression { Name = string }
//@[031:0067) | └─StringLiteralExpression { Value = First line\r\nSecond\ttabbed\tline }

// object default value
@sys.description('this is foo')
//@[000:0348) ├─DeclaredParameterExpression { Name = foo }
//@[017:0030) | ├─StringLiteralExpression { Value = this is foo }
@metadata({
//@[010:0082) | ├─ObjectExpression
  description: 'overwrite but still valid'
//@[002:0042) | | ├─ObjectPropertyExpression
//@[002:0013) | | | ├─StringLiteralExpression { Value = description }
//@[015:0042) | | | └─StringLiteralExpression { Value = overwrite but still valid }
  another: 'just for fun'
//@[002:0025) | | └─ObjectPropertyExpression
//@[002:0009) | |   ├─StringLiteralExpression { Value = another }
//@[011:0025) | |   └─StringLiteralExpression { Value = just for fun }
})
param foo object = {
//@[010:0016) | ├─AmbientTypeReferenceExpression { Name = object }
//@[019:0232) | └─ObjectExpression
  enabled: true
//@[002:0015) |   ├─ObjectPropertyExpression
//@[002:0009) |   | ├─StringLiteralExpression { Value = enabled }
//@[011:0015) |   | └─BooleanLiteralExpression { Value = True }
  name: 'this is my object'
//@[002:0027) |   ├─ObjectPropertyExpression
//@[002:0006) |   | ├─StringLiteralExpression { Value = name }
//@[008:0027) |   | └─StringLiteralExpression { Value = this is my object }
  priority: 3
//@[002:0013) |   ├─ObjectPropertyExpression
//@[002:0010) |   | ├─StringLiteralExpression { Value = priority }
//@[012:0013) |   | └─IntegerLiteralExpression { Value = 3 }
  info: {
//@[002:0024) |   ├─ObjectPropertyExpression
//@[002:0006) |   | ├─StringLiteralExpression { Value = info }
//@[008:0024) |   | └─ObjectExpression
    a: 'b'
//@[004:0010) |   |   └─ObjectPropertyExpression
//@[004:0005) |   |     ├─StringLiteralExpression { Value = a }
//@[007:0010) |   |     └─StringLiteralExpression { Value = b }
  }
  empty: {
//@[002:0014) |   ├─ObjectPropertyExpression
//@[002:0007) |   | ├─StringLiteralExpression { Value = empty }
//@[009:0014) |   | └─ObjectExpression
  }
  array: [
//@[002:0111) |   └─ObjectPropertyExpression
//@[002:0007) |     ├─StringLiteralExpression { Value = array }
//@[009:0111) |     └─ArrayExpression
    'string item'
//@[004:0017) |       ├─StringLiteralExpression { Value = string item }
    12
//@[004:0006) |       ├─IntegerLiteralExpression { Value = 12 }
    true
//@[004:0008) |       ├─BooleanLiteralExpression { Value = True }
    [
//@[004:0037) |       ├─ArrayExpression
      'inner'
//@[006:0013) |       | ├─StringLiteralExpression { Value = inner }
      false
//@[006:0011) |       | └─BooleanLiteralExpression { Value = False }
    ]
    {
//@[004:0024) |       └─ObjectExpression
      a: 'b'
//@[006:0012) |         └─ObjectPropertyExpression
//@[006:0007) |           ├─StringLiteralExpression { Value = a }
//@[009:0012) |           └─StringLiteralExpression { Value = b }
    }
  ]
}

// array default value
param myArrayParam array = [
//@[000:0048) ├─DeclaredParameterExpression { Name = myArrayParam }
//@[019:0024) | ├─AmbientTypeReferenceExpression { Name = array }
//@[027:0048) | └─ArrayExpression
  'a'
//@[002:0005) |   ├─StringLiteralExpression { Value = a }
  'b'
//@[002:0005) |   ├─StringLiteralExpression { Value = b }
  'c'
//@[002:0005) |   └─StringLiteralExpression { Value = c }
]

// secure string
@secure()
//@[000:0031) ├─DeclaredParameterExpression { Name = password }
//@[001:0009) | ├─FunctionCallExpression { Name = secure }
param password string
//@[015:0021) | └─AmbientTypeReferenceExpression { Name = string }

// secure object
@secure()
//@[000:0035) ├─DeclaredParameterExpression { Name = secretObject }
//@[001:0009) | ├─FunctionCallExpression { Name = secure }
param secretObject object
//@[019:0025) | └─AmbientTypeReferenceExpression { Name = object }

// enum parameter
@allowed([
//@[000:0071) ├─DeclaredParameterExpression { Name = storageSku }
  'Standard_LRS'
  'Standard_GRS'
])
param storageSku string
//@[017:0023) | └─AmbientTypeReferenceExpression { Name = string }

@allowed([
//@[000:0043) ├─DeclaredParameterExpression { Name = intEnum }
  1
  2
  3
])
param intEnum int
//@[014:0017) | └─AmbientTypeReferenceExpression { Name = int }

// length constraint on a string
@minLength(3)
//@[000:0053) ├─DeclaredParameterExpression { Name = storageName }
//@[011:0012) | ├─IntegerLiteralExpression { Value = 3 }
@maxLength(24)
//@[011:0013) | ├─IntegerLiteralExpression { Value = 24 }
param storageName string
//@[018:0024) | └─AmbientTypeReferenceExpression { Name = string }

// length constraint on an array
@minLength(3)
//@[000:0050) ├─DeclaredParameterExpression { Name = someArray }
//@[011:0012) | ├─IntegerLiteralExpression { Value = 3 }
@maxLength(24)
//@[011:0013) | ├─IntegerLiteralExpression { Value = 24 }
param someArray array
//@[016:0021) | └─AmbientTypeReferenceExpression { Name = array }

// empty metadata
@metadata({})
//@[000:0040) ├─DeclaredParameterExpression { Name = emptyMetadata }
//@[010:0012) | ├─ObjectExpression
param emptyMetadata string
//@[020:0026) | └─AmbientTypeReferenceExpression { Name = string }

// description
@metadata({
//@[000:0071) ├─DeclaredParameterExpression { Name = description }
//@[010:0045) | ├─ObjectExpression
  description: 'my description'
//@[002:0031) | | └─ObjectPropertyExpression
//@[002:0013) | |   ├─StringLiteralExpression { Value = description }
//@[015:0031) | |   └─StringLiteralExpression { Value = my description }
})
param description string
//@[018:0024) | └─AmbientTypeReferenceExpression { Name = string }

@sys.description('my description')
//@[000:0060) ├─DeclaredParameterExpression { Name = description2 }
//@[017:0033) | ├─StringLiteralExpression { Value = my description }
param description2 string
//@[019:0025) | └─AmbientTypeReferenceExpression { Name = string }

// random extra metadata
@metadata({
//@[000:0133) ├─DeclaredParameterExpression { Name = additionalMetadata }
//@[010:0100) | ├─ObjectExpression
  description: 'my description'
//@[002:0031) | | ├─ObjectPropertyExpression
//@[002:0013) | | | ├─StringLiteralExpression { Value = description }
//@[015:0031) | | | └─StringLiteralExpression { Value = my description }
  a: 1
//@[002:0006) | | ├─ObjectPropertyExpression
//@[002:0003) | | | ├─StringLiteralExpression { Value = a }
//@[005:0006) | | | └─IntegerLiteralExpression { Value = 1 }
  b: true
//@[002:0009) | | ├─ObjectPropertyExpression
//@[002:0003) | | | ├─StringLiteralExpression { Value = b }
//@[005:0009) | | | └─BooleanLiteralExpression { Value = True }
  c: [
//@[002:0010) | | ├─ObjectPropertyExpression
//@[002:0003) | | | ├─StringLiteralExpression { Value = c }
//@[005:0010) | | | └─ArrayExpression
  ]
  d: {
//@[002:0026) | | └─ObjectPropertyExpression
//@[002:0003) | |   ├─StringLiteralExpression { Value = d }
//@[005:0026) | |   └─ObjectExpression
    test: 'abc'
//@[004:0015) | |     └─ObjectPropertyExpression
//@[004:0008) | |       ├─StringLiteralExpression { Value = test }
//@[010:0015) | |       └─StringLiteralExpression { Value = abc }
  }
})
param additionalMetadata string
//@[025:0031) | └─AmbientTypeReferenceExpression { Name = string }

// all modifiers together
@secure()
//@[000:0165) ├─DeclaredParameterExpression { Name = someParameter }
//@[001:0009) | ├─FunctionCallExpression { Name = secure }
@minLength(3)
//@[011:0012) | ├─IntegerLiteralExpression { Value = 3 }
@maxLength(24)
//@[011:0013) | ├─IntegerLiteralExpression { Value = 24 }
@allowed([
  'one'
  'two'
  'three'
])
@metadata({
//@[010:0058) | ├─ObjectExpression
  description: 'Name of the storage account'
//@[002:0044) | | └─ObjectPropertyExpression
//@[002:0013) | |   ├─StringLiteralExpression { Value = description }
//@[015:0044) | |   └─StringLiteralExpression { Value = Name of the storage account }
})
param someParameter string
//@[020:0026) | └─AmbientTypeReferenceExpression { Name = string }

param defaultExpression bool = 18 != (true || false)
//@[000:0052) ├─DeclaredParameterExpression { Name = defaultExpression }
//@[024:0028) | ├─AmbientTypeReferenceExpression { Name = bool }
//@[031:0052) | └─BinaryExpression { Operator = NotEquals }
//@[031:0033) |   ├─IntegerLiteralExpression { Value = 18 }
//@[038:0051) |   └─BinaryExpression { Operator = LogicalOr }
//@[038:0042) |     ├─BooleanLiteralExpression { Value = True }
//@[046:0051) |     └─BooleanLiteralExpression { Value = False }

@allowed([
//@[000:0056) ├─DeclaredParameterExpression { Name = stringLiteral }
  'abc'
  'def'
])
param stringLiteral string
//@[020:0026) | └─AmbientTypeReferenceExpression { Name = string }

@allowed(
//@[000:0130) ├─DeclaredParameterExpression { Name = stringLiteralWithAllowedValuesSuperset }
    // some comment
    [
  'abc'
  'def'
  'ghi'
])
param stringLiteralWithAllowedValuesSuperset string = stringLiteral
//@[045:0051) | ├─AmbientTypeReferenceExpression { Name = string }
//@[054:0067) | └─ParametersReferenceExpression { Parameter = stringLiteral }

@secure()
//@[000:0104) ├─DeclaredParameterExpression { Name = decoratedString }
//@[001:0009) | ├─FunctionCallExpression { Name = secure }
@minLength(2)
//@[011:0012) | ├─IntegerLiteralExpression { Value = 2 }
  @maxLength(10)
//@[013:0015) | ├─IntegerLiteralExpression { Value = 10 }
@allowed([
  'Apple'
  'Banana'
])
param decoratedString string
//@[022:0028) | └─AmbientTypeReferenceExpression { Name = string }

@minValue(100)
//@[000:0043) ├─DeclaredParameterExpression { Name = decoratedInt }
//@[010:0013) | ├─IntegerLiteralExpression { Value = 100 }
param decoratedInt int = 123
//@[019:0022) | ├─AmbientTypeReferenceExpression { Name = int }
//@[025:0028) | └─IntegerLiteralExpression { Value = 123 }

// negative integer literals are allowed as decorator values
@minValue(-10)
//@[000:0053) ├─DeclaredParameterExpression { Name = negativeValues }
//@[010:0013) | ├─IntegerLiteralExpression { Value = -10 }
@maxValue(-3)
//@[010:0012) | ├─IntegerLiteralExpression { Value = -3 }
param negativeValues int
//@[021:0024) | └─AmbientTypeReferenceExpression { Name = int }

@sys.description('A boolean.')
//@[000:0283) ├─DeclaredParameterExpression { Name = decoratedBool }
//@[017:0029) | ├─StringLiteralExpression { Value = A boolean. }
@metadata({
//@[010:0136) | ├─ObjectExpression
    description: 'I will be overrode.'
//@[004:0038) | | ├─ObjectPropertyExpression
//@[004:0015) | | | ├─StringLiteralExpression { Value = description }
//@[017:0038) | | | └─StringLiteralExpression { Value = I will be overrode. }
    foo: 'something'
//@[004:0020) | | ├─ObjectPropertyExpression
//@[004:0007) | | | ├─StringLiteralExpression { Value = foo }
//@[009:0020) | | | └─StringLiteralExpression { Value = something }
    bar: [
//@[004:0062) | | └─ObjectPropertyExpression
//@[004:0007) | |   ├─StringLiteralExpression { Value = bar }
//@[009:0062) | |   └─ArrayExpression
        {          }
//@[008:0020) | |     ├─ObjectExpression
        true
//@[008:0012) | |     ├─BooleanLiteralExpression { Value = True }
        123
//@[008:0011) | |     └─IntegerLiteralExpression { Value = 123 }
    ]
})
param decoratedBool bool = /* comment1 */ /* comment2*/      /* comment3 */ /* comment4 */ (true && false) != true
//@[020:0024) | ├─AmbientTypeReferenceExpression { Name = bool }
//@[091:0114) | └─BinaryExpression { Operator = NotEquals }
//@[092:0105) |   ├─BinaryExpression { Operator = LogicalAnd }
//@[092:0096) |   | ├─BooleanLiteralExpression { Value = True }
//@[100:0105) |   | └─BooleanLiteralExpression { Value = False }
//@[110:0114) |   └─BooleanLiteralExpression { Value = True }

@secure()
//@[000:0254) ├─DeclaredParameterExpression { Name = decoratedObject }
//@[001:0009) | ├─FunctionCallExpression { Name = secure }
param decoratedObject object = {
//@[022:0028) | ├─AmbientTypeReferenceExpression { Name = object }
//@[031:0244) | └─ObjectExpression
  enabled: true
//@[002:0015) |   ├─ObjectPropertyExpression
//@[002:0009) |   | ├─StringLiteralExpression { Value = enabled }
//@[011:0015) |   | └─BooleanLiteralExpression { Value = True }
  name: 'this is my object'
//@[002:0027) |   ├─ObjectPropertyExpression
//@[002:0006) |   | ├─StringLiteralExpression { Value = name }
//@[008:0027) |   | └─StringLiteralExpression { Value = this is my object }
  priority: 3
//@[002:0013) |   ├─ObjectPropertyExpression
//@[002:0010) |   | ├─StringLiteralExpression { Value = priority }
//@[012:0013) |   | └─IntegerLiteralExpression { Value = 3 }
  info: {
//@[002:0024) |   ├─ObjectPropertyExpression
//@[002:0006) |   | ├─StringLiteralExpression { Value = info }
//@[008:0024) |   | └─ObjectExpression
    a: 'b'
//@[004:0010) |   |   └─ObjectPropertyExpression
//@[004:0005) |   |     ├─StringLiteralExpression { Value = a }
//@[007:0010) |   |     └─StringLiteralExpression { Value = b }
  }
  empty: {
//@[002:0014) |   ├─ObjectPropertyExpression
//@[002:0007) |   | ├─StringLiteralExpression { Value = empty }
//@[009:0014) |   | └─ObjectExpression
  }
  array: [
//@[002:0111) |   └─ObjectPropertyExpression
//@[002:0007) |     ├─StringLiteralExpression { Value = array }
//@[009:0111) |     └─ArrayExpression
    'string item'
//@[004:0017) |       ├─StringLiteralExpression { Value = string item }
    12
//@[004:0006) |       ├─IntegerLiteralExpression { Value = 12 }
    true
//@[004:0008) |       ├─BooleanLiteralExpression { Value = True }
    [
//@[004:0037) |       ├─ArrayExpression
      'inner'
//@[006:0013) |       | ├─StringLiteralExpression { Value = inner }
      false
//@[006:0011) |       | └─BooleanLiteralExpression { Value = False }
    ]
    {
//@[004:0024) |       └─ObjectExpression
      a: 'b'
//@[006:0012) |         └─ObjectPropertyExpression
//@[006:0007) |           ├─StringLiteralExpression { Value = a }
//@[009:0012) |           └─StringLiteralExpression { Value = b }
    }
  ]
}

@sys.metadata({
//@[000:0166) ├─DeclaredParameterExpression { Name = decoratedArray }
//@[014:0056) | ├─ObjectExpression
    description: 'I will be overrode.'
//@[004:0038) | | └─ObjectPropertyExpression
//@[004:0015) | |   ├─StringLiteralExpression { Value = description }
//@[017:0038) | |   └─StringLiteralExpression { Value = I will be overrode. }
})
@sys.maxLength(20)
//@[015:0017) | ├─IntegerLiteralExpression { Value = 20 }
@sys.description('An array.')
//@[017:0028) | ├─StringLiteralExpression { Value = An array. }
param decoratedArray array = [
//@[021:0026) | ├─AmbientTypeReferenceExpression { Name = array }
//@[029:0059) | └─ArrayExpression
    utcNow()
//@[004:0012) |   ├─FunctionCallExpression { Name = utcNow }
    newGuid()
//@[004:0013) |   └─FunctionCallExpression { Name = newGuid }
]

param nameofParam string = nameof(decoratedArray)
//@[000:0049) └─DeclaredParameterExpression { Name = nameofParam }
//@[018:0024)   ├─AmbientTypeReferenceExpression { Name = string }
//@[034:0048)   └─StringLiteralExpression { Value = decoratedArray }

