/*
  This is a block comment.
*/
//@[002:004) NewLine |\n\n|

// parameters without default value
//@[035:036) NewLine |\n|
@sys.description('''
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:074) StringComplete |'''\nthis is my multi line\ndescription for my myString\n'''|
this is my multi line
description for my myString
''')
//@[003:004) RightParen |)|
//@[004:005) NewLine |\n|
param myString string
//@[000:005) Identifier |param|
//@[006:014) Identifier |myString|
//@[015:021) Identifier |string|
//@[021:022) NewLine |\n|
param myInt int
//@[000:005) Identifier |param|
//@[006:011) Identifier |myInt|
//@[012:015) Identifier |int|
//@[015:016) NewLine |\n|
param myBool bool
//@[000:005) Identifier |param|
//@[006:012) Identifier |myBool|
//@[013:017) Identifier |bool|
//@[017:019) NewLine |\n\n|

// parameters with default value
//@[032:033) NewLine |\n|
@sys.description('this is myString2')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:036) StringComplete |'this is myString2'|
//@[036:037) RightParen |)|
//@[037:038) NewLine |\n|
@metadata({
//@[000:001) At |@|
//@[001:009) Identifier |metadata|
//@[009:010) LeftParen |(|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
  description: 'overwrite but still valid'
//@[002:013) Identifier |description|
//@[013:014) Colon |:|
//@[015:042) StringComplete |'overwrite but still valid'|
//@[042:043) NewLine |\n|
})
//@[000:001) RightBrace |}|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
param myString2 string = 'string value'
//@[000:005) Identifier |param|
//@[006:015) Identifier |myString2|
//@[016:022) Identifier |string|
//@[023:024) Assignment |=|
//@[025:039) StringComplete |'string value'|
//@[039:040) NewLine |\n|
param myInt2 int = 42
//@[000:005) Identifier |param|
//@[006:012) Identifier |myInt2|
//@[013:016) Identifier |int|
//@[017:018) Assignment |=|
//@[019:021) Integer |42|
//@[021:022) NewLine |\n|
param myTruth bool = true
//@[000:005) Identifier |param|
//@[006:013) Identifier |myTruth|
//@[014:018) Identifier |bool|
//@[019:020) Assignment |=|
//@[021:025) TrueKeyword |true|
//@[025:026) NewLine |\n|
param myFalsehood bool = false
//@[000:005) Identifier |param|
//@[006:017) Identifier |myFalsehood|
//@[018:022) Identifier |bool|
//@[023:024) Assignment |=|
//@[025:030) FalseKeyword |false|
//@[030:031) NewLine |\n|
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[000:005) Identifier |param|
//@[006:021) Identifier |myEscapedString|
//@[022:028) Identifier |string|
//@[029:030) Assignment |=|
//@[031:067) StringComplete |'First line\r\nSecond\ttabbed\tline'|
//@[067:069) NewLine |\n\n|

// object default value
//@[023:024) NewLine |\n|
@sys.description('this is foo')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:030) StringComplete |'this is foo'|
//@[030:031) RightParen |)|
//@[031:032) NewLine |\n|
@metadata({
//@[000:001) At |@|
//@[001:009) Identifier |metadata|
//@[009:010) LeftParen |(|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
  description: 'overwrite but still valid'
//@[002:013) Identifier |description|
//@[013:014) Colon |:|
//@[015:042) StringComplete |'overwrite but still valid'|
//@[042:043) NewLine |\n|
  another: 'just for fun'
//@[002:009) Identifier |another|
//@[009:010) Colon |:|
//@[011:025) StringComplete |'just for fun'|
//@[025:026) NewLine |\n|
})
//@[000:001) RightBrace |}|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
param foo object = {
//@[000:005) Identifier |param|
//@[006:009) Identifier |foo|
//@[010:016) Identifier |object|
//@[017:018) Assignment |=|
//@[019:020) LeftBrace |{|
//@[020:021) NewLine |\n|
  enabled: true
//@[002:009) Identifier |enabled|
//@[009:010) Colon |:|
//@[011:015) TrueKeyword |true|
//@[015:016) NewLine |\n|
  name: 'this is my object'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:027) StringComplete |'this is my object'|
//@[027:028) NewLine |\n|
  priority: 3
//@[002:010) Identifier |priority|
//@[010:011) Colon |:|
//@[012:013) Integer |3|
//@[013:014) NewLine |\n|
  info: {
//@[002:006) Identifier |info|
//@[006:007) Colon |:|
//@[008:009) LeftBrace |{|
//@[009:010) NewLine |\n|
    a: 'b'
//@[004:005) Identifier |a|
//@[005:006) Colon |:|
//@[007:010) StringComplete |'b'|
//@[010:011) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  empty: {
//@[002:007) Identifier |empty|
//@[007:008) Colon |:|
//@[009:010) LeftBrace |{|
//@[010:011) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  array: [
//@[002:007) Identifier |array|
//@[007:008) Colon |:|
//@[009:010) LeftSquare |[|
//@[010:011) NewLine |\n|
    'string item'
//@[004:017) StringComplete |'string item'|
//@[017:018) NewLine |\n|
    12
//@[004:006) Integer |12|
//@[006:007) NewLine |\n|
    true
//@[004:008) TrueKeyword |true|
//@[008:009) NewLine |\n|
    [
//@[004:005) LeftSquare |[|
//@[005:006) NewLine |\n|
      'inner'
//@[006:013) StringComplete |'inner'|
//@[013:014) NewLine |\n|
      false
//@[006:011) FalseKeyword |false|
//@[011:012) NewLine |\n|
    ]
//@[004:005) RightSquare |]|
//@[005:006) NewLine |\n|
    {
//@[004:005) LeftBrace |{|
//@[005:006) NewLine |\n|
      a: 'b'
//@[006:007) Identifier |a|
//@[007:008) Colon |:|
//@[009:012) StringComplete |'b'|
//@[012:013) NewLine |\n|
    }
//@[004:005) RightBrace |}|
//@[005:006) NewLine |\n|
  ]
//@[002:003) RightSquare |]|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

// array default value
//@[022:023) NewLine |\n|
param myArrayParam array = [
//@[000:005) Identifier |param|
//@[006:018) Identifier |myArrayParam|
//@[019:024) Identifier |array|
//@[025:026) Assignment |=|
//@[027:028) LeftSquare |[|
//@[028:029) NewLine |\n|
  'a'
//@[002:005) StringComplete |'a'|
//@[005:006) NewLine |\n|
  'b'
//@[002:005) StringComplete |'b'|
//@[005:006) NewLine |\n|
  'c'
//@[002:005) StringComplete |'c'|
//@[005:006) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\n\n|

// secure string
//@[016:017) NewLine |\n|
@secure()
//@[000:001) At |@|
//@[001:007) Identifier |secure|
//@[007:008) LeftParen |(|
//@[008:009) RightParen |)|
//@[009:010) NewLine |\n|
param password string
//@[000:005) Identifier |param|
//@[006:014) Identifier |password|
//@[015:021) Identifier |string|
//@[021:023) NewLine |\n\n|

// secure object
//@[016:017) NewLine |\n|
@secure()
//@[000:001) At |@|
//@[001:007) Identifier |secure|
//@[007:008) LeftParen |(|
//@[008:009) RightParen |)|
//@[009:010) NewLine |\n|
param secretObject object
//@[000:005) Identifier |param|
//@[006:018) Identifier |secretObject|
//@[019:025) Identifier |object|
//@[025:027) NewLine |\n\n|

// enum parameter
//@[017:018) NewLine |\n|
@allowed([
//@[000:001) At |@|
//@[001:008) Identifier |allowed|
//@[008:009) LeftParen |(|
//@[009:010) LeftSquare |[|
//@[010:011) NewLine |\n|
  'Standard_LRS'
//@[002:016) StringComplete |'Standard_LRS'|
//@[016:017) NewLine |\n|
  'Standard_GRS'
//@[002:016) StringComplete |'Standard_GRS'|
//@[016:017) NewLine |\n|
])
//@[000:001) RightSquare |]|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
param storageSku string
//@[000:005) Identifier |param|
//@[006:016) Identifier |storageSku|
//@[017:023) Identifier |string|
//@[023:025) NewLine |\n\n|

@allowed([
//@[000:001) At |@|
//@[001:008) Identifier |allowed|
//@[008:009) LeftParen |(|
//@[009:010) LeftSquare |[|
//@[010:011) NewLine |\n|
  1
//@[002:003) Integer |1|
//@[003:004) NewLine |\n|
  2
//@[002:003) Integer |2|
//@[003:004) NewLine |\n|
  3
//@[002:003) Integer |3|
//@[003:004) NewLine |\n|
])
//@[000:001) RightSquare |]|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
param intEnum int
//@[000:005) Identifier |param|
//@[006:013) Identifier |intEnum|
//@[014:017) Identifier |int|
//@[017:019) NewLine |\n\n|

// length constraint on a string
//@[032:033) NewLine |\n|
@minLength(3)
//@[000:001) At |@|
//@[001:010) Identifier |minLength|
//@[010:011) LeftParen |(|
//@[011:012) Integer |3|
//@[012:013) RightParen |)|
//@[013:014) NewLine |\n|
@maxLength(24)
//@[000:001) At |@|
//@[001:010) Identifier |maxLength|
//@[010:011) LeftParen |(|
//@[011:013) Integer |24|
//@[013:014) RightParen |)|
//@[014:015) NewLine |\n|
param storageName string
//@[000:005) Identifier |param|
//@[006:017) Identifier |storageName|
//@[018:024) Identifier |string|
//@[024:026) NewLine |\n\n|

// length constraint on an array
//@[032:033) NewLine |\n|
@minLength(3)
//@[000:001) At |@|
//@[001:010) Identifier |minLength|
//@[010:011) LeftParen |(|
//@[011:012) Integer |3|
//@[012:013) RightParen |)|
//@[013:014) NewLine |\n|
@maxLength(24)
//@[000:001) At |@|
//@[001:010) Identifier |maxLength|
//@[010:011) LeftParen |(|
//@[011:013) Integer |24|
//@[013:014) RightParen |)|
//@[014:015) NewLine |\n|
param someArray array
//@[000:005) Identifier |param|
//@[006:015) Identifier |someArray|
//@[016:021) Identifier |array|
//@[021:023) NewLine |\n\n|

// empty metadata
//@[017:018) NewLine |\n|
@metadata({})
//@[000:001) At |@|
//@[001:009) Identifier |metadata|
//@[009:010) LeftParen |(|
//@[010:011) LeftBrace |{|
//@[011:012) RightBrace |}|
//@[012:013) RightParen |)|
//@[013:014) NewLine |\n|
param emptyMetadata string
//@[000:005) Identifier |param|
//@[006:019) Identifier |emptyMetadata|
//@[020:026) Identifier |string|
//@[026:028) NewLine |\n\n|

// description
//@[014:015) NewLine |\n|
@metadata({
//@[000:001) At |@|
//@[001:009) Identifier |metadata|
//@[009:010) LeftParen |(|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
  description: 'my description'
//@[002:013) Identifier |description|
//@[013:014) Colon |:|
//@[015:031) StringComplete |'my description'|
//@[031:032) NewLine |\n|
})
//@[000:001) RightBrace |}|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
param description string
//@[000:005) Identifier |param|
//@[006:017) Identifier |description|
//@[018:024) Identifier |string|
//@[024:026) NewLine |\n\n|

@sys.description('my description')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:033) StringComplete |'my description'|
//@[033:034) RightParen |)|
//@[034:035) NewLine |\n|
param description2 string
//@[000:005) Identifier |param|
//@[006:018) Identifier |description2|
//@[019:025) Identifier |string|
//@[025:027) NewLine |\n\n|

// random extra metadata
//@[024:025) NewLine |\n|
@metadata({
//@[000:001) At |@|
//@[001:009) Identifier |metadata|
//@[009:010) LeftParen |(|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
  description: 'my description'
//@[002:013) Identifier |description|
//@[013:014) Colon |:|
//@[015:031) StringComplete |'my description'|
//@[031:032) NewLine |\n|
  a: 1
//@[002:003) Identifier |a|
//@[003:004) Colon |:|
//@[005:006) Integer |1|
//@[006:007) NewLine |\n|
  b: true
//@[002:003) Identifier |b|
//@[003:004) Colon |:|
//@[005:009) TrueKeyword |true|
//@[009:010) NewLine |\n|
  c: [
//@[002:003) Identifier |c|
//@[003:004) Colon |:|
//@[005:006) LeftSquare |[|
//@[006:007) NewLine |\n|
  ]
//@[002:003) RightSquare |]|
//@[003:004) NewLine |\n|
  d: {
//@[002:003) Identifier |d|
//@[003:004) Colon |:|
//@[005:006) LeftBrace |{|
//@[006:007) NewLine |\n|
    test: 'abc'
//@[004:008) Identifier |test|
//@[008:009) Colon |:|
//@[010:015) StringComplete |'abc'|
//@[015:016) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
})
//@[000:001) RightBrace |}|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
param additionalMetadata string
//@[000:005) Identifier |param|
//@[006:024) Identifier |additionalMetadata|
//@[025:031) Identifier |string|
//@[031:033) NewLine |\n\n|

// all modifiers together
//@[025:026) NewLine |\n|
@secure()
//@[000:001) At |@|
//@[001:007) Identifier |secure|
//@[007:008) LeftParen |(|
//@[008:009) RightParen |)|
//@[009:010) NewLine |\n|
@minLength(3)
//@[000:001) At |@|
//@[001:010) Identifier |minLength|
//@[010:011) LeftParen |(|
//@[011:012) Integer |3|
//@[012:013) RightParen |)|
//@[013:014) NewLine |\n|
@maxLength(24)
//@[000:001) At |@|
//@[001:010) Identifier |maxLength|
//@[010:011) LeftParen |(|
//@[011:013) Integer |24|
//@[013:014) RightParen |)|
//@[014:015) NewLine |\n|
@allowed([
//@[000:001) At |@|
//@[001:008) Identifier |allowed|
//@[008:009) LeftParen |(|
//@[009:010) LeftSquare |[|
//@[010:011) NewLine |\n|
  'one'
//@[002:007) StringComplete |'one'|
//@[007:008) NewLine |\n|
  'two'
//@[002:007) StringComplete |'two'|
//@[007:008) NewLine |\n|
  'three'
//@[002:009) StringComplete |'three'|
//@[009:010) NewLine |\n|
])
//@[000:001) RightSquare |]|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
@metadata({
//@[000:001) At |@|
//@[001:009) Identifier |metadata|
//@[009:010) LeftParen |(|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
  description: 'Name of the storage account'
//@[002:013) Identifier |description|
//@[013:014) Colon |:|
//@[015:044) StringComplete |'Name of the storage account'|
//@[044:045) NewLine |\n|
})
//@[000:001) RightBrace |}|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
param someParameter string
//@[000:005) Identifier |param|
//@[006:019) Identifier |someParameter|
//@[020:026) Identifier |string|
//@[026:028) NewLine |\n\n|

param defaultExpression bool = 18 != (true || false)
//@[000:005) Identifier |param|
//@[006:023) Identifier |defaultExpression|
//@[024:028) Identifier |bool|
//@[029:030) Assignment |=|
//@[031:033) Integer |18|
//@[034:036) NotEquals |!=|
//@[037:038) LeftParen |(|
//@[038:042) TrueKeyword |true|
//@[043:045) LogicalOr ||||
//@[046:051) FalseKeyword |false|
//@[051:052) RightParen |)|
//@[052:054) NewLine |\n\n|

@allowed([
//@[000:001) At |@|
//@[001:008) Identifier |allowed|
//@[008:009) LeftParen |(|
//@[009:010) LeftSquare |[|
//@[010:011) NewLine |\n|
  'abc'
//@[002:007) StringComplete |'abc'|
//@[007:008) NewLine |\n|
  'def'
//@[002:007) StringComplete |'def'|
//@[007:008) NewLine |\n|
])
//@[000:001) RightSquare |]|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
param stringLiteral string
//@[000:005) Identifier |param|
//@[006:019) Identifier |stringLiteral|
//@[020:026) Identifier |string|
//@[026:028) NewLine |\n\n|

@allowed(
//@[000:001) At |@|
//@[001:008) Identifier |allowed|
//@[008:009) LeftParen |(|
//@[009:010) NewLine |\n|
    // some comment
//@[019:020) NewLine |\n|
    [
//@[004:005) LeftSquare |[|
//@[005:006) NewLine |\n|
  'abc'
//@[002:007) StringComplete |'abc'|
//@[007:008) NewLine |\n|
  'def'
//@[002:007) StringComplete |'def'|
//@[007:008) NewLine |\n|
  'ghi'
//@[002:007) StringComplete |'ghi'|
//@[007:008) NewLine |\n|
])
//@[000:001) RightSquare |]|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
param stringLiteralWithAllowedValuesSuperset string = stringLiteral
//@[000:005) Identifier |param|
//@[006:044) Identifier |stringLiteralWithAllowedValuesSuperset|
//@[045:051) Identifier |string|
//@[052:053) Assignment |=|
//@[054:067) Identifier |stringLiteral|
//@[067:069) NewLine |\n\n|

@secure()
//@[000:001) At |@|
//@[001:007) Identifier |secure|
//@[007:008) LeftParen |(|
//@[008:009) RightParen |)|
//@[009:010) NewLine |\n|
@minLength(2)
//@[000:001) At |@|
//@[001:010) Identifier |minLength|
//@[010:011) LeftParen |(|
//@[011:012) Integer |2|
//@[012:013) RightParen |)|
//@[013:014) NewLine |\n|
  @maxLength(10)
//@[002:003) At |@|
//@[003:012) Identifier |maxLength|
//@[012:013) LeftParen |(|
//@[013:015) Integer |10|
//@[015:016) RightParen |)|
//@[016:017) NewLine |\n|
@allowed([
//@[000:001) At |@|
//@[001:008) Identifier |allowed|
//@[008:009) LeftParen |(|
//@[009:010) LeftSquare |[|
//@[010:011) NewLine |\n|
  'Apple'
//@[002:009) StringComplete |'Apple'|
//@[009:010) NewLine |\n|
  'Banana'
//@[002:010) StringComplete |'Banana'|
//@[010:011) NewLine |\n|
])
//@[000:001) RightSquare |]|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
param decoratedString string
//@[000:005) Identifier |param|
//@[006:021) Identifier |decoratedString|
//@[022:028) Identifier |string|
//@[028:030) NewLine |\n\n|

@minValue(100)
//@[000:001) At |@|
//@[001:009) Identifier |minValue|
//@[009:010) LeftParen |(|
//@[010:013) Integer |100|
//@[013:014) RightParen |)|
//@[014:015) NewLine |\n|
param decoratedInt int = 123
//@[000:005) Identifier |param|
//@[006:018) Identifier |decoratedInt|
//@[019:022) Identifier |int|
//@[023:024) Assignment |=|
//@[025:028) Integer |123|
//@[028:030) NewLine |\n\n|

// negative integer literals are allowed as decorator values
//@[060:061) NewLine |\n|
@minValue(-10)
//@[000:001) At |@|
//@[001:009) Identifier |minValue|
//@[009:010) LeftParen |(|
//@[010:011) Minus |-|
//@[011:013) Integer |10|
//@[013:014) RightParen |)|
//@[014:015) NewLine |\n|
@maxValue(-3)
//@[000:001) At |@|
//@[001:009) Identifier |maxValue|
//@[009:010) LeftParen |(|
//@[010:011) Minus |-|
//@[011:012) Integer |3|
//@[012:013) RightParen |)|
//@[013:014) NewLine |\n|
param negativeValues int
//@[000:005) Identifier |param|
//@[006:020) Identifier |negativeValues|
//@[021:024) Identifier |int|
//@[024:026) NewLine |\n\n|

@sys.description('A boolean.')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:029) StringComplete |'A boolean.'|
//@[029:030) RightParen |)|
//@[030:031) NewLine |\n|
@metadata({
//@[000:001) At |@|
//@[001:009) Identifier |metadata|
//@[009:010) LeftParen |(|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    description: 'I will be overrode.'
//@[004:015) Identifier |description|
//@[015:016) Colon |:|
//@[017:038) StringComplete |'I will be overrode.'|
//@[038:039) NewLine |\n|
    foo: 'something'
//@[004:007) Identifier |foo|
//@[007:008) Colon |:|
//@[009:020) StringComplete |'something'|
//@[020:021) NewLine |\n|
    bar: [
//@[004:007) Identifier |bar|
//@[007:008) Colon |:|
//@[009:010) LeftSquare |[|
//@[010:011) NewLine |\n|
        {          }
//@[008:009) LeftBrace |{|
//@[019:020) RightBrace |}|
//@[020:021) NewLine |\n|
        true
//@[008:012) TrueKeyword |true|
//@[012:013) NewLine |\n|
        123
//@[008:011) Integer |123|
//@[011:012) NewLine |\n|
    ]
//@[004:005) RightSquare |]|
//@[005:006) NewLine |\n|
})
//@[000:001) RightBrace |}|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
param decoratedBool bool = /* comment1 */ /* comment2*/      /* comment3 */ /* comment4 */ (true && false) != true
//@[000:005) Identifier |param|
//@[006:019) Identifier |decoratedBool|
//@[020:024) Identifier |bool|
//@[025:026) Assignment |=|
//@[091:092) LeftParen |(|
//@[092:096) TrueKeyword |true|
//@[097:099) LogicalAnd |&&|
//@[100:105) FalseKeyword |false|
//@[105:106) RightParen |)|
//@[107:109) NotEquals |!=|
//@[110:114) TrueKeyword |true|
//@[114:116) NewLine |\n\n|

@secure()
//@[000:001) At |@|
//@[001:007) Identifier |secure|
//@[007:008) LeftParen |(|
//@[008:009) RightParen |)|
//@[009:010) NewLine |\n|
param decoratedObject object = {
//@[000:005) Identifier |param|
//@[006:021) Identifier |decoratedObject|
//@[022:028) Identifier |object|
//@[029:030) Assignment |=|
//@[031:032) LeftBrace |{|
//@[032:033) NewLine |\n|
  enabled: true
//@[002:009) Identifier |enabled|
//@[009:010) Colon |:|
//@[011:015) TrueKeyword |true|
//@[015:016) NewLine |\n|
  name: 'this is my object'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:027) StringComplete |'this is my object'|
//@[027:028) NewLine |\n|
  priority: 3
//@[002:010) Identifier |priority|
//@[010:011) Colon |:|
//@[012:013) Integer |3|
//@[013:014) NewLine |\n|
  info: {
//@[002:006) Identifier |info|
//@[006:007) Colon |:|
//@[008:009) LeftBrace |{|
//@[009:010) NewLine |\n|
    a: 'b'
//@[004:005) Identifier |a|
//@[005:006) Colon |:|
//@[007:010) StringComplete |'b'|
//@[010:011) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  empty: {
//@[002:007) Identifier |empty|
//@[007:008) Colon |:|
//@[009:010) LeftBrace |{|
//@[010:011) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  array: [
//@[002:007) Identifier |array|
//@[007:008) Colon |:|
//@[009:010) LeftSquare |[|
//@[010:011) NewLine |\n|
    'string item'
//@[004:017) StringComplete |'string item'|
//@[017:018) NewLine |\n|
    12
//@[004:006) Integer |12|
//@[006:007) NewLine |\n|
    true
//@[004:008) TrueKeyword |true|
//@[008:009) NewLine |\n|
    [
//@[004:005) LeftSquare |[|
//@[005:006) NewLine |\n|
      'inner'
//@[006:013) StringComplete |'inner'|
//@[013:014) NewLine |\n|
      false
//@[006:011) FalseKeyword |false|
//@[011:012) NewLine |\n|
    ]
//@[004:005) RightSquare |]|
//@[005:006) NewLine |\n|
    {
//@[004:005) LeftBrace |{|
//@[005:006) NewLine |\n|
      a: 'b'
//@[006:007) Identifier |a|
//@[007:008) Colon |:|
//@[009:012) StringComplete |'b'|
//@[012:013) NewLine |\n|
    }
//@[004:005) RightBrace |}|
//@[005:006) NewLine |\n|
  ]
//@[002:003) RightSquare |]|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

@sys.metadata({
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:013) Identifier |metadata|
//@[013:014) LeftParen |(|
//@[014:015) LeftBrace |{|
//@[015:016) NewLine |\n|
    description: 'I will be overrode.'
//@[004:015) Identifier |description|
//@[015:016) Colon |:|
//@[017:038) StringComplete |'I will be overrode.'|
//@[038:039) NewLine |\n|
})
//@[000:001) RightBrace |}|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
@sys.maxLength(20)
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:014) Identifier |maxLength|
//@[014:015) LeftParen |(|
//@[015:017) Integer |20|
//@[017:018) RightParen |)|
//@[018:019) NewLine |\n|
@sys.description('An array.')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:028) StringComplete |'An array.'|
//@[028:029) RightParen |)|
//@[029:030) NewLine |\n|
param decoratedArray array = [
//@[000:005) Identifier |param|
//@[006:020) Identifier |decoratedArray|
//@[021:026) Identifier |array|
//@[027:028) Assignment |=|
//@[029:030) LeftSquare |[|
//@[030:031) NewLine |\n|
    utcNow()
//@[004:010) Identifier |utcNow|
//@[010:011) LeftParen |(|
//@[011:012) RightParen |)|
//@[012:013) NewLine |\n|
    newGuid()
//@[004:011) Identifier |newGuid|
//@[011:012) LeftParen |(|
//@[012:013) RightParen |)|
//@[013:014) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\n\n|

param nameofParam string = nameof(decoratedArray)
//@[000:005) Identifier |param|
//@[006:017) Identifier |nameofParam|
//@[018:024) Identifier |string|
//@[025:026) Assignment |=|
//@[027:033) Identifier |nameof|
//@[033:034) LeftParen |(|
//@[034:048) Identifier |decoratedArray|
//@[048:049) RightParen |)|
//@[049:050) NewLine |\n|

//@[000:000) EndOfFile ||
