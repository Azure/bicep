/* 
  Valid and invalid code is mixed together to validate recovery logic. It can even contain ** * *** **.
*/
//@[002:004) NewLine |\n\n|

param myString string
//@[000:005) Identifier |param|
//@[006:014) Identifier |myString|
//@[015:021) Identifier |string|
//@[021:022) NewLine |\n|
wrong
//@[000:005) Identifier |wrong|
//@[005:007) NewLine |\n\n|

param myInt int
//@[000:005) Identifier |param|
//@[006:011) Identifier |myInt|
//@[012:015) Identifier |int|
//@[015:016) NewLine |\n|
param
//@[000:005) Identifier |param|
//@[005:007) NewLine |\n\n|

param 3
//@[000:005) Identifier |param|
//@[006:007) Integer |3|
//@[007:008) NewLine |\n|
param % string
//@[000:005) Identifier |param|
//@[006:007) Modulo |%|
//@[008:014) Identifier |string|
//@[014:015) NewLine |\n|
param % string 3 = 's'
//@[000:005) Identifier |param|
//@[006:007) Modulo |%|
//@[008:014) Identifier |string|
//@[015:016) Integer |3|
//@[017:018) Assignment |=|
//@[019:022) StringComplete |'s'|
//@[022:024) NewLine |\n\n|

param myBool bool
//@[000:005) Identifier |param|
//@[006:012) Identifier |myBool|
//@[013:017) Identifier |bool|
//@[017:019) NewLine |\n\n|

param missingType
//@[000:005) Identifier |param|
//@[006:017) Identifier |missingType|
//@[017:019) NewLine |\n\n|

// space after identifier #completionTest(32) -> paramTypes
//@[059:060) NewLine |\n|
param missingTypeWithSpaceAfter 
//@[000:005) Identifier |param|
//@[006:031) Identifier |missingTypeWithSpaceAfter|
//@[032:034) NewLine |\n\n|

// tab after identifier #completionTest(30) -> paramTypes
//@[057:058) NewLine |\n|
param missingTypeWithTabAfter	
//@[000:005) Identifier |param|
//@[006:029) Identifier |missingTypeWithTabAfter|
//@[030:032) NewLine |\n\n|

// #completionTest(20) -> paramTypes
//@[036:037) NewLine |\n|
param trailingSpace  
//@[000:005) Identifier |param|
//@[006:019) Identifier |trailingSpace|
//@[021:023) NewLine |\n\n|

// partial type #completionTest(18, 19, 20, 21) -> paramTypes
//@[061:062) NewLine |\n|
param partialType str
//@[000:005) Identifier |param|
//@[006:017) Identifier |partialType|
//@[018:021) Identifier |str|
//@[021:023) NewLine |\n\n|

param malformedType 44
//@[000:005) Identifier |param|
//@[006:019) Identifier |malformedType|
//@[020:022) Integer |44|
//@[022:024) NewLine |\n\n|

// malformed type but type check should still happen
//@[052:053) NewLine |\n|
param malformedType2 44 = f
//@[000:005) Identifier |param|
//@[006:020) Identifier |malformedType2|
//@[021:023) Integer |44|
//@[024:025) Assignment |=|
//@[026:027) Identifier |f|
//@[027:029) NewLine |\n\n|

// malformed type but type check should still happen
//@[052:053) NewLine |\n|
@secure('s')
//@[000:001) At |@|
//@[001:007) Identifier |secure|
//@[007:008) LeftParen |(|
//@[008:011) StringComplete |'s'|
//@[011:012) RightParen |)|
//@[012:013) NewLine |\n|
param malformedModifier 44
//@[000:005) Identifier |param|
//@[006:023) Identifier |malformedModifier|
//@[024:026) Integer |44|
//@[026:028) NewLine |\n\n|

param myString2 string = 'string value'
//@[000:005) Identifier |param|
//@[006:015) Identifier |myString2|
//@[016:022) Identifier |string|
//@[023:024) Assignment |=|
//@[025:039) StringComplete |'string value'|
//@[039:041) NewLine |\n\n|

param wrongDefaultValue string = 42
//@[000:005) Identifier |param|
//@[006:023) Identifier |wrongDefaultValue|
//@[024:030) Identifier |string|
//@[031:032) Assignment |=|
//@[033:035) Integer |42|
//@[035:037) NewLine |\n\n|

param myInt2 int = 42
//@[000:005) Identifier |param|
//@[006:012) Identifier |myInt2|
//@[013:016) Identifier |int|
//@[017:018) Assignment |=|
//@[019:021) Integer |42|
//@[021:022) NewLine |\n|
param noValueAfterColon int =   
//@[000:005) Identifier |param|
//@[006:023) Identifier |noValueAfterColon|
//@[024:027) Identifier |int|
//@[028:029) Assignment |=|
//@[032:034) NewLine |\n\n|

param myTruth bool = 'not a boolean'
//@[000:005) Identifier |param|
//@[006:013) Identifier |myTruth|
//@[014:018) Identifier |bool|
//@[019:020) Assignment |=|
//@[021:036) StringComplete |'not a boolean'|
//@[036:037) NewLine |\n|
param myFalsehood bool = 'false'
//@[000:005) Identifier |param|
//@[006:017) Identifier |myFalsehood|
//@[018:022) Identifier |bool|
//@[023:024) Assignment |=|
//@[025:032) StringComplete |'false'|
//@[032:034) NewLine |\n\n|

param wrongAssignmentToken string: 'hello'
//@[000:005) Identifier |param|
//@[006:026) Identifier |wrongAssignmentToken|
//@[027:033) Identifier |string|
//@[033:034) Colon |:|
//@[035:042) StringComplete |'hello'|
//@[042:044) NewLine |\n\n|

param WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong string = 'why not?'
//@[000:005) Identifier |param|
//@[006:267) Identifier |WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong|
//@[268:274) Identifier |string|
//@[275:276) Assignment |=|
//@[277:287) StringComplete |'why not?'|
//@[287:289) NewLine |\n\n|

// #completionTest(28,29) -> boolPlusSymbols
//@[044:045) NewLine |\n|
param boolCompletions bool = 
//@[000:005) Identifier |param|
//@[006:021) Identifier |boolCompletions|
//@[022:026) Identifier |bool|
//@[027:028) Assignment |=|
//@[029:031) NewLine |\n\n|

// #completionTest(30,31) -> arrayPlusSymbols
//@[045:046) NewLine |\n|
param arrayCompletions array = 
//@[000:005) Identifier |param|
//@[006:022) Identifier |arrayCompletions|
//@[023:028) Identifier |array|
//@[029:030) Assignment |=|
//@[031:033) NewLine |\n\n|

// #completionTest(32,33) -> objectPlusSymbols
//@[046:047) NewLine |\n|
param objectCompletions object = 
//@[000:005) Identifier |param|
//@[006:023) Identifier |objectCompletions|
//@[024:030) Identifier |object|
//@[031:032) Assignment |=|
//@[033:035) NewLine |\n\n|

// badly escaped string
//@[023:024) NewLine |\n|
param wrongType fluffyBunny = 'what's up doc?'
//@[000:005) Identifier |param|
//@[006:015) Identifier |wrongType|
//@[016:027) Identifier |fluffyBunny|
//@[028:029) Assignment |=|
//@[030:036) StringComplete |'what'|
//@[036:037) Identifier |s|
//@[038:040) Identifier |up|
//@[041:044) Identifier |doc|
//@[044:045) Question |?|
//@[045:046) StringComplete |'|
//@[046:048) NewLine |\n\n|

// invalid escape
//@[017:018) NewLine |\n|
param wrongType fluffyBunny = 'what\s up doc?'
//@[000:005) Identifier |param|
//@[006:015) Identifier |wrongType|
//@[016:027) Identifier |fluffyBunny|
//@[028:029) Assignment |=|
//@[030:046) StringComplete |'what\s up doc?'|
//@[046:048) NewLine |\n\n|

// unterminated string 
//@[023:024) NewLine |\n|
param wrongType fluffyBunny = 'what\'s up doc?
//@[000:005) Identifier |param|
//@[006:015) Identifier |wrongType|
//@[016:027) Identifier |fluffyBunny|
//@[028:029) Assignment |=|
//@[030:046) StringComplete |'what\'s up doc?|
//@[046:048) NewLine |\n\n|

// unterminated interpolated string
//@[035:036) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${
//@[000:005) Identifier |param|
//@[006:015) Identifier |wrongType|
//@[016:027) Identifier |fluffyBunny|
//@[028:029) Assignment |=|
//@[030:041) StringLeftPiece |'what\'s ${|
//@[041:041) StringRightPiece ||
//@[041:042) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${up
//@[000:005) Identifier |param|
//@[006:015) Identifier |wrongType|
//@[016:027) Identifier |fluffyBunny|
//@[028:029) Assignment |=|
//@[030:041) StringLeftPiece |'what\'s ${|
//@[041:043) Identifier |up|
//@[043:043) StringRightPiece ||
//@[043:044) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${up}
//@[000:005) Identifier |param|
//@[006:015) Identifier |wrongType|
//@[016:027) Identifier |fluffyBunny|
//@[028:029) Assignment |=|
//@[030:041) StringLeftPiece |'what\'s ${|
//@[041:043) Identifier |up|
//@[043:044) StringRightPiece |}|
//@[044:045) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up
//@[000:005) Identifier |param|
//@[006:015) Identifier |wrongType|
//@[016:027) Identifier |fluffyBunny|
//@[028:029) Assignment |=|
//@[030:041) StringLeftPiece |'what\'s ${|
//@[041:044) StringComplete |'up|
//@[044:044) StringRightPiece ||
//@[044:046) NewLine |\n\n|

// unterminated nested interpolated string
//@[042:043) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${
//@[000:005) Identifier |param|
//@[006:015) Identifier |wrongType|
//@[016:027) Identifier |fluffyBunny|
//@[028:029) Assignment |=|
//@[030:041) StringLeftPiece |'what\'s ${|
//@[041:046) StringLeftPiece |'up${|
//@[046:046) StringRightPiece ||
//@[046:047) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${
//@[000:005) Identifier |param|
//@[006:015) Identifier |wrongType|
//@[016:027) Identifier |fluffyBunny|
//@[028:029) Assignment |=|
//@[030:041) StringLeftPiece |'what\'s ${|
//@[041:046) StringLeftPiece |'up${|
//@[046:046) StringRightPiece ||
//@[046:047) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${doc
//@[000:005) Identifier |param|
//@[006:015) Identifier |wrongType|
//@[016:027) Identifier |fluffyBunny|
//@[028:029) Assignment |=|
//@[030:041) StringLeftPiece |'what\'s ${|
//@[041:046) StringLeftPiece |'up${|
//@[046:049) Identifier |doc|
//@[049:049) StringRightPiece ||
//@[049:050) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${doc}
//@[000:005) Identifier |param|
//@[006:015) Identifier |wrongType|
//@[016:027) Identifier |fluffyBunny|
//@[028:029) Assignment |=|
//@[030:041) StringLeftPiece |'what\'s ${|
//@[041:046) StringLeftPiece |'up${|
//@[046:049) Identifier |doc|
//@[049:050) StringRightPiece |}|
//@[050:050) StringRightPiece ||
//@[050:051) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${doc}'
//@[000:005) Identifier |param|
//@[006:015) Identifier |wrongType|
//@[016:027) Identifier |fluffyBunny|
//@[028:029) Assignment |=|
//@[030:041) StringLeftPiece |'what\'s ${|
//@[041:046) StringLeftPiece |'up${|
//@[046:049) Identifier |doc|
//@[049:051) StringRightPiece |}'|
//@[051:051) StringRightPiece ||
//@[051:052) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${doc}'}?
//@[000:005) Identifier |param|
//@[006:015) Identifier |wrongType|
//@[016:027) Identifier |fluffyBunny|
//@[028:029) Assignment |=|
//@[030:041) StringLeftPiece |'what\'s ${|
//@[041:046) StringLeftPiece |'up${|
//@[046:049) Identifier |doc|
//@[049:051) StringRightPiece |}'|
//@[051:053) StringRightPiece |}?|
//@[053:055) NewLine |\n\n|

// object literal inside interpolated string
//@[044:045) NewLine |\n|
param wrongType fluffyBunny = '${{this: doesnt}.work}'
//@[000:005) Identifier |param|
//@[006:015) Identifier |wrongType|
//@[016:027) Identifier |fluffyBunny|
//@[028:029) Assignment |=|
//@[030:033) StringLeftPiece |'${|
//@[033:034) LeftBrace |{|
//@[034:038) Identifier |this|
//@[038:039) Colon |:|
//@[040:046) Identifier |doesnt|
//@[046:047) RightBrace |}|
//@[047:048) Dot |.|
//@[048:052) Identifier |work|
//@[052:053) RightBrace |}|
//@[053:054) StringComplete |'|
//@[054:054) StringRightPiece ||
//@[054:056) NewLine |\n\n|

// bad interpolated string format
//@[033:034) NewLine |\n|
param badInterpolatedString string = 'hello ${}!'
//@[000:005) Identifier |param|
//@[006:027) Identifier |badInterpolatedString|
//@[028:034) Identifier |string|
//@[035:036) Assignment |=|
//@[037:046) StringLeftPiece |'hello ${|
//@[046:049) StringRightPiece |}!'|
//@[049:050) NewLine |\n|
param badInterpolatedString2 string = 'hello ${a b c}!'
//@[000:005) Identifier |param|
//@[006:028) Identifier |badInterpolatedString2|
//@[029:035) Identifier |string|
//@[036:037) Assignment |=|
//@[038:047) StringLeftPiece |'hello ${|
//@[047:048) Identifier |a|
//@[049:050) Identifier |b|
//@[051:052) Identifier |c|
//@[052:055) StringRightPiece |}!'|
//@[055:057) NewLine |\n\n|

param wrongType fluffyBunny = 'what\'s up doc?'
//@[000:005) Identifier |param|
//@[006:015) Identifier |wrongType|
//@[016:027) Identifier |fluffyBunny|
//@[028:029) Assignment |=|
//@[030:047) StringComplete |'what\'s up doc?'|
//@[047:049) NewLine |\n\n|

// modifier on an invalid type
//@[030:031) NewLine |\n|
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
param someArray arra
//@[000:005) Identifier |param|
//@[006:015) Identifier |someArray|
//@[016:020) Identifier |arra|
//@[020:022) NewLine |\n\n|

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
@maxLength(123)
//@[000:001) At |@|
//@[001:010) Identifier |maxLength|
//@[010:011) LeftParen |(|
//@[011:014) Integer |123|
//@[014:015) RightParen |)|
//@[015:016) NewLine |\n|
param secureInt int
//@[000:005) Identifier |param|
//@[006:015) Identifier |secureInt|
//@[016:019) Identifier |int|
//@[019:021) NewLine |\n\n|

// wrong modifier value types
//@[029:030) NewLine |\n|
@allowed([
//@[000:001) At |@|
//@[001:008) Identifier |allowed|
//@[008:009) LeftParen |(|
//@[009:010) LeftSquare |[|
//@[010:011) NewLine |\n|
  'test'
//@[002:008) StringComplete |'test'|
//@[008:009) NewLine |\n|
  true
//@[002:006) TrueKeyword |true|
//@[006:007) NewLine |\n|
])
//@[000:001) RightSquare |]|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
@minValue({
//@[000:001) At |@|
//@[001:009) Identifier |minValue|
//@[009:010) LeftParen |(|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
})
//@[000:001) RightBrace |}|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
@maxValue([
//@[000:001) At |@|
//@[001:009) Identifier |maxValue|
//@[009:010) LeftParen |(|
//@[010:011) LeftSquare |[|
//@[011:012) NewLine |\n|
])
//@[000:001) RightSquare |]|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
@metadata('wrong')
//@[000:001) At |@|
//@[001:009) Identifier |metadata|
//@[009:010) LeftParen |(|
//@[010:017) StringComplete |'wrong'|
//@[017:018) RightParen |)|
//@[018:019) NewLine |\n|
param wrongIntModifier int = true
//@[000:005) Identifier |param|
//@[006:022) Identifier |wrongIntModifier|
//@[023:026) Identifier |int|
//@[027:028) Assignment |=|
//@[029:033) TrueKeyword |true|
//@[033:035) NewLine |\n\n|

@metadata(any([]))
//@[000:001) At |@|
//@[001:009) Identifier |metadata|
//@[009:010) LeftParen |(|
//@[010:013) Identifier |any|
//@[013:014) LeftParen |(|
//@[014:015) LeftSquare |[|
//@[015:016) RightSquare |]|
//@[016:017) RightParen |)|
//@[017:018) RightParen |)|
//@[018:019) NewLine |\n|
@allowed(any(2))
//@[000:001) At |@|
//@[001:008) Identifier |allowed|
//@[008:009) LeftParen |(|
//@[009:012) Identifier |any|
//@[012:013) LeftParen |(|
//@[013:014) Integer |2|
//@[014:015) RightParen |)|
//@[015:016) RightParen |)|
//@[016:017) NewLine |\n|
param fatalErrorInIssue1713
//@[000:005) Identifier |param|
//@[006:027) Identifier |fatalErrorInIssue1713|
//@[027:029) NewLine |\n\n|

// wrong metadata schema
//@[024:025) NewLine |\n|
@metadata({
//@[000:001) At |@|
//@[001:009) Identifier |metadata|
//@[009:010) LeftParen |(|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
  description: true
//@[002:013) Identifier |description|
//@[013:014) Colon |:|
//@[015:019) TrueKeyword |true|
//@[019:020) NewLine |\n|
})
//@[000:001) RightBrace |}|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
param wrongMetadataSchema string
//@[000:005) Identifier |param|
//@[006:025) Identifier |wrongMetadataSchema|
//@[026:032) Identifier |string|
//@[032:034) NewLine |\n\n|

// expression in modifier
//@[025:026) NewLine |\n|
@maxLength(a + 2)
//@[000:001) At |@|
//@[001:010) Identifier |maxLength|
//@[010:011) LeftParen |(|
//@[011:012) Identifier |a|
//@[013:014) Plus |+|
//@[015:016) Integer |2|
//@[016:017) RightParen |)|
//@[017:018) NewLine |\n|
@minLength(foo())
//@[000:001) At |@|
//@[001:010) Identifier |minLength|
//@[010:011) LeftParen |(|
//@[011:014) Identifier |foo|
//@[014:015) LeftParen |(|
//@[015:016) RightParen |)|
//@[016:017) RightParen |)|
//@[017:018) NewLine |\n|
@allowed([
//@[000:001) At |@|
//@[001:008) Identifier |allowed|
//@[008:009) LeftParen |(|
//@[009:010) LeftSquare |[|
//@[010:011) NewLine |\n|
  i
//@[002:003) Identifier |i|
//@[003:004) NewLine |\n|
])
//@[000:001) RightSquare |]|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
param expressionInModifier string = 2 + 3
//@[000:005) Identifier |param|
//@[006:026) Identifier |expressionInModifier|
//@[027:033) Identifier |string|
//@[034:035) Assignment |=|
//@[036:037) Integer |2|
//@[038:039) Plus |+|
//@[040:041) Integer |3|
//@[041:043) NewLine |\n\n|

@maxLength(2 + 3)
//@[000:001) At |@|
//@[001:010) Identifier |maxLength|
//@[010:011) LeftParen |(|
//@[011:012) Integer |2|
//@[013:014) Plus |+|
//@[015:016) Integer |3|
//@[016:017) RightParen |)|
//@[017:018) NewLine |\n|
@minLength(length([]))
//@[000:001) At |@|
//@[001:010) Identifier |minLength|
//@[010:011) LeftParen |(|
//@[011:017) Identifier |length|
//@[017:018) LeftParen |(|
//@[018:019) LeftSquare |[|
//@[019:020) RightSquare |]|
//@[020:021) RightParen |)|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
@allowed([
//@[000:001) At |@|
//@[001:008) Identifier |allowed|
//@[008:009) LeftParen |(|
//@[009:010) LeftSquare |[|
//@[010:011) NewLine |\n|
  resourceGroup().id
//@[002:015) Identifier |resourceGroup|
//@[015:016) LeftParen |(|
//@[016:017) RightParen |)|
//@[017:018) Dot |.|
//@[018:020) Identifier |id|
//@[020:021) NewLine |\n|
])
//@[000:001) RightSquare |]|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
param nonCompileTimeConstant string
//@[000:005) Identifier |param|
//@[006:028) Identifier |nonCompileTimeConstant|
//@[029:035) Identifier |string|
//@[035:038) NewLine |\n\n\n|


@allowed([])
//@[000:001) At |@|
//@[001:008) Identifier |allowed|
//@[008:009) LeftParen |(|
//@[009:010) LeftSquare |[|
//@[010:011) RightSquare |]|
//@[011:012) RightParen |)|
//@[012:013) NewLine |\n|
param emptyAllowedString string
//@[000:005) Identifier |param|
//@[006:024) Identifier |emptyAllowedString|
//@[025:031) Identifier |string|
//@[031:033) NewLine |\n\n|

@allowed([])
//@[000:001) At |@|
//@[001:008) Identifier |allowed|
//@[008:009) LeftParen |(|
//@[009:010) LeftSquare |[|
//@[010:011) RightSquare |]|
//@[011:012) RightParen |)|
//@[012:013) NewLine |\n|
param emptyAllowedInt int
//@[000:005) Identifier |param|
//@[006:021) Identifier |emptyAllowedInt|
//@[022:025) Identifier |int|
//@[025:027) NewLine |\n\n|

// 1-cycle in params
//@[020:021) NewLine |\n|
param paramDefaultOneCycle string = paramDefaultOneCycle
//@[000:005) Identifier |param|
//@[006:026) Identifier |paramDefaultOneCycle|
//@[027:033) Identifier |string|
//@[034:035) Assignment |=|
//@[036:056) Identifier |paramDefaultOneCycle|
//@[056:058) NewLine |\n\n|

// 2-cycle in params
//@[020:021) NewLine |\n|
param paramDefaultTwoCycle1 string = paramDefaultTwoCycle2
//@[000:005) Identifier |param|
//@[006:027) Identifier |paramDefaultTwoCycle1|
//@[028:034) Identifier |string|
//@[035:036) Assignment |=|
//@[037:058) Identifier |paramDefaultTwoCycle2|
//@[058:059) NewLine |\n|
param paramDefaultTwoCycle2 string = paramDefaultTwoCycle1
//@[000:005) Identifier |param|
//@[006:027) Identifier |paramDefaultTwoCycle2|
//@[028:034) Identifier |string|
//@[035:036) Assignment |=|
//@[037:058) Identifier |paramDefaultTwoCycle1|
//@[058:060) NewLine |\n\n|

@allowed([
//@[000:001) At |@|
//@[001:008) Identifier |allowed|
//@[008:009) LeftParen |(|
//@[009:010) LeftSquare |[|
//@[010:011) NewLine |\n|
  paramModifierSelfCycle
//@[002:024) Identifier |paramModifierSelfCycle|
//@[024:025) NewLine |\n|
])
//@[000:001) RightSquare |]|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
param paramModifierSelfCycle string
//@[000:005) Identifier |param|
//@[006:028) Identifier |paramModifierSelfCycle|
//@[029:035) Identifier |string|
//@[035:037) NewLine |\n\n|

// wrong types of "variable"/identifier access
//@[046:047) NewLine |\n|
var sampleVar = 'sample'
//@[000:003) Identifier |var|
//@[004:013) Identifier |sampleVar|
//@[014:015) Assignment |=|
//@[016:024) StringComplete |'sample'|
//@[024:025) NewLine |\n|
resource sampleResource 'Microsoft.Foo/foos@2020-02-02' = {
//@[000:008) Identifier |resource|
//@[009:023) Identifier |sampleResource|
//@[024:055) StringComplete |'Microsoft.Foo/foos@2020-02-02'|
//@[056:057) Assignment |=|
//@[058:059) LeftBrace |{|
//@[059:060) NewLine |\n|
  name: 'foo'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:013) StringComplete |'foo'|
//@[013:014) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:002) NewLine |\n|
output sampleOutput string = 'hello'
//@[000:006) Identifier |output|
//@[007:019) Identifier |sampleOutput|
//@[020:026) Identifier |string|
//@[027:028) Assignment |=|
//@[029:036) StringComplete |'hello'|
//@[036:038) NewLine |\n\n|

param paramAccessingVar string = concat(sampleVar, 's')
//@[000:005) Identifier |param|
//@[006:023) Identifier |paramAccessingVar|
//@[024:030) Identifier |string|
//@[031:032) Assignment |=|
//@[033:039) Identifier |concat|
//@[039:040) LeftParen |(|
//@[040:049) Identifier |sampleVar|
//@[049:050) Comma |,|
//@[051:054) StringComplete |'s'|
//@[054:055) RightParen |)|
//@[055:057) NewLine |\n\n|

param paramAccessingResource string = sampleResource
//@[000:005) Identifier |param|
//@[006:028) Identifier |paramAccessingResource|
//@[029:035) Identifier |string|
//@[036:037) Assignment |=|
//@[038:052) Identifier |sampleResource|
//@[052:054) NewLine |\n\n|

param paramAccessingOutput string = sampleOutput
//@[000:005) Identifier |param|
//@[006:026) Identifier |paramAccessingOutput|
//@[027:033) Identifier |string|
//@[034:035) Assignment |=|
//@[036:048) Identifier |sampleOutput|
//@[048:050) NewLine |\n\n|

// #completionTest(6) -> empty
//@[030:031) NewLine |\n|
param 
//@[000:005) Identifier |param|
//@[006:008) NewLine |\n\n|

// #completionTest(46,47) -> justSymbols
//@[040:041) NewLine |\n|
param defaultValueOneLinerCompletions string = 
//@[000:005) Identifier |param|
//@[006:037) Identifier |defaultValueOneLinerCompletions|
//@[038:044) Identifier |string|
//@[045:046) Assignment |=|
//@[047:049) NewLine |\n\n|

// invalid comma separator (array)
//@[034:035) NewLine |\n|
@metadata({
//@[000:001) At |@|
//@[001:009) Identifier |metadata|
//@[009:010) LeftParen |(|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
  description: 'Name of Virtual Machine'
//@[002:013) Identifier |description|
//@[013:014) Colon |:|
//@[015:040) StringComplete |'Name of Virtual Machine'|
//@[040:041) NewLine |\n|
})
//@[000:001) RightBrace |}|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
@allowed([
//@[000:001) At |@|
//@[001:008) Identifier |allowed|
//@[008:009) LeftParen |(|
//@[009:010) LeftSquare |[|
//@[010:011) NewLine |\n|
  'abc',
//@[002:007) StringComplete |'abc'|
//@[007:008) Comma |,|
//@[008:009) NewLine |\n|
  'def'
//@[002:007) StringComplete |'def'|
//@[007:008) NewLine |\n|
])
//@[000:001) RightSquare |]|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
param commaOne string
//@[000:005) Identifier |param|
//@[006:014) Identifier |commaOne|
//@[015:021) Identifier |string|
//@[021:023) NewLine |\n\n|

@secure
//@[000:001) At |@|
//@[001:007) Identifier |secure|
//@[007:008) NewLine |\n|
@
//@[000:001) At |@|
//@[001:002) NewLine |\n|
@&& xxx
//@[000:001) At |@|
//@[001:003) LogicalAnd |&&|
//@[004:007) Identifier |xxx|
//@[007:008) NewLine |\n|
@sys
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) NewLine |\n|
@paramAccessingVar
//@[000:001) At |@|
//@[001:018) Identifier |paramAccessingVar|
//@[018:019) NewLine |\n|
param incompleteDecorators string
//@[000:005) Identifier |param|
//@[006:026) Identifier |incompleteDecorators|
//@[027:033) Identifier |string|
//@[033:035) NewLine |\n\n|

@concat(1, 2)
//@[000:001) At |@|
//@[001:007) Identifier |concat|
//@[007:008) LeftParen |(|
//@[008:009) Integer |1|
//@[009:010) Comma |,|
//@[011:012) Integer |2|
//@[012:013) RightParen |)|
//@[013:014) NewLine |\n|
@sys.concat('a', 'b')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:011) Identifier |concat|
//@[011:012) LeftParen |(|
//@[012:015) StringComplete |'a'|
//@[015:016) Comma |,|
//@[017:020) StringComplete |'b'|
//@[020:021) RightParen |)|
//@[021:022) NewLine |\n|
@secure()
//@[000:001) At |@|
//@[001:007) Identifier |secure|
//@[007:008) LeftParen |(|
//@[008:009) RightParen |)|
//@[009:010) NewLine |\n|
// wrong target type
//@[020:021) NewLine |\n|
@minValue(20)
//@[000:001) At |@|
//@[001:009) Identifier |minValue|
//@[009:010) LeftParen |(|
//@[010:012) Integer |20|
//@[012:013) RightParen |)|
//@[013:014) NewLine |\n|
param someString string
//@[000:005) Identifier |param|
//@[006:016) Identifier |someString|
//@[017:023) Identifier |string|
//@[023:025) NewLine |\n\n|

@allowed([
//@[000:001) At |@|
//@[001:008) Identifier |allowed|
//@[008:009) LeftParen |(|
//@[009:010) LeftSquare |[|
//@[010:011) NewLine |\n|
    true
//@[004:008) TrueKeyword |true|
//@[008:009) NewLine |\n|
    10
//@[004:006) Integer |10|
//@[006:007) NewLine |\n|
    'foo'
//@[004:009) StringComplete |'foo'|
//@[009:010) NewLine |\n|
])
//@[000:001) RightSquare |]|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
@secure()
//@[000:001) At |@|
//@[001:007) Identifier |secure|
//@[007:008) LeftParen |(|
//@[008:009) RightParen |)|
//@[009:010) NewLine |\n|
// #completionTest(1, 2, 3) -> intParameterDecoratorsPlusNamespace
//@[066:067) NewLine |\n|
@  
//@[000:001) At |@|
//@[003:004) NewLine |\n|
// #completionTest(5, 6) -> intParameterDecorators
//@[050:051) NewLine |\n|
@sys.   
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[008:009) NewLine |\n|
param someInteger int = 20
//@[000:005) Identifier |param|
//@[006:017) Identifier |someInteger|
//@[018:021) Identifier |int|
//@[022:023) Assignment |=|
//@[024:026) Integer |20|
//@[026:028) NewLine |\n\n|

@allowed([], [], 2)
//@[000:001) At |@|
//@[001:008) Identifier |allowed|
//@[008:009) LeftParen |(|
//@[009:010) LeftSquare |[|
//@[010:011) RightSquare |]|
//@[011:012) Comma |,|
//@[013:014) LeftSquare |[|
//@[014:015) RightSquare |]|
//@[015:016) Comma |,|
//@[017:018) Integer |2|
//@[018:019) RightParen |)|
//@[019:020) NewLine |\n|
// #completionTest(4) -> empty
//@[030:031) NewLine |\n|
@az.
//@[000:001) At |@|
//@[001:003) Identifier |az|
//@[003:004) Dot |.|
//@[004:005) NewLine |\n|
param tooManyArguments1 int = 20
//@[000:005) Identifier |param|
//@[006:023) Identifier |tooManyArguments1|
//@[024:027) Identifier |int|
//@[028:029) Assignment |=|
//@[030:032) Integer |20|
//@[032:034) NewLine |\n\n|

@metadata({}, {}, true)
//@[000:001) At |@|
//@[001:009) Identifier |metadata|
//@[009:010) LeftParen |(|
//@[010:011) LeftBrace |{|
//@[011:012) RightBrace |}|
//@[012:013) Comma |,|
//@[014:015) LeftBrace |{|
//@[015:016) RightBrace |}|
//@[016:017) Comma |,|
//@[018:022) TrueKeyword |true|
//@[022:023) RightParen |)|
//@[023:024) NewLine |\n|
// #completionTest(2) -> stringParameterDecoratorsPlusNamespace
//@[063:064) NewLine |\n|
@m
//@[000:001) At |@|
//@[001:002) Identifier |m|
//@[002:003) NewLine |\n|
// #completionTest(1, 2, 3) -> stringParameterDecoratorsPlusNamespace
//@[069:070) NewLine |\n|
@   
//@[000:001) At |@|
//@[004:005) NewLine |\n|
// #completionTest(5) -> stringParameterDecorators
//@[050:051) NewLine |\n|
@sys.
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:006) NewLine |\n|
param tooManyArguments2 string
//@[000:005) Identifier |param|
//@[006:023) Identifier |tooManyArguments2|
//@[024:030) Identifier |string|
//@[030:032) NewLine |\n\n|

@description(sys.concat(2))
//@[000:001) At |@|
//@[001:012) Identifier |description|
//@[012:013) LeftParen |(|
//@[013:016) Identifier |sys|
//@[016:017) Dot |.|
//@[017:023) Identifier |concat|
//@[023:024) LeftParen |(|
//@[024:025) Integer |2|
//@[025:026) RightParen |)|
//@[026:027) RightParen |)|
//@[027:028) NewLine |\n|
@allowed([for thing in []: 's'])
//@[000:001) At |@|
//@[001:008) Identifier |allowed|
//@[008:009) LeftParen |(|
//@[009:010) LeftSquare |[|
//@[010:013) Identifier |for|
//@[014:019) Identifier |thing|
//@[020:022) Identifier |in|
//@[023:024) LeftSquare |[|
//@[024:025) RightSquare |]|
//@[025:026) Colon |:|
//@[027:030) StringComplete |'s'|
//@[030:031) RightSquare |]|
//@[031:032) RightParen |)|
//@[032:033) NewLine |\n|
param nonConstantInDecorator string
//@[000:005) Identifier |param|
//@[006:028) Identifier |nonConstantInDecorator|
//@[029:035) Identifier |string|
//@[035:037) NewLine |\n\n|

@minValue(-length('s'))
//@[000:001) At |@|
//@[001:009) Identifier |minValue|
//@[009:010) LeftParen |(|
//@[010:011) Minus |-|
//@[011:017) Identifier |length|
//@[017:018) LeftParen |(|
//@[018:021) StringComplete |'s'|
//@[021:022) RightParen |)|
//@[022:023) RightParen |)|
//@[023:024) NewLine |\n|
@metadata({
//@[000:001) At |@|
//@[001:009) Identifier |metadata|
//@[009:010) LeftParen |(|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
  bool: !true
//@[002:006) Identifier |bool|
//@[006:007) Colon |:|
//@[008:009) Exclamation |!|
//@[009:013) TrueKeyword |true|
//@[013:014) NewLine |\n|
})
//@[000:001) RightBrace |}|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
param unaryMinusOnFunction int
//@[000:005) Identifier |param|
//@[006:026) Identifier |unaryMinusOnFunction|
//@[027:030) Identifier |int|
//@[030:032) NewLine |\n\n|

@minLength(1)
//@[000:001) At |@|
//@[001:010) Identifier |minLength|
//@[010:011) LeftParen |(|
//@[011:012) Integer |1|
//@[012:013) RightParen |)|
//@[013:014) NewLine |\n|
@minLength(2)
//@[000:001) At |@|
//@[001:010) Identifier |minLength|
//@[010:011) LeftParen |(|
//@[011:012) Integer |2|
//@[012:013) RightParen |)|
//@[013:014) NewLine |\n|
@secure()
//@[000:001) At |@|
//@[001:007) Identifier |secure|
//@[007:008) LeftParen |(|
//@[008:009) RightParen |)|
//@[009:010) NewLine |\n|
@maxLength(3)
//@[000:001) At |@|
//@[001:010) Identifier |maxLength|
//@[010:011) LeftParen |(|
//@[011:012) Integer |3|
//@[012:013) RightParen |)|
//@[013:014) NewLine |\n|
@maxLength(4)
//@[000:001) At |@|
//@[001:010) Identifier |maxLength|
//@[010:011) LeftParen |(|
//@[011:012) Integer |4|
//@[012:013) RightParen |)|
//@[013:014) NewLine |\n|
param duplicateDecorators string
//@[000:005) Identifier |param|
//@[006:025) Identifier |duplicateDecorators|
//@[026:032) Identifier |string|
//@[032:034) NewLine |\n\n|

@maxLength(-1)
//@[000:001) At |@|
//@[001:010) Identifier |maxLength|
//@[010:011) LeftParen |(|
//@[011:012) Minus |-|
//@[012:013) Integer |1|
//@[013:014) RightParen |)|
//@[014:015) NewLine |\n|
@minLength(-100)
//@[000:001) At |@|
//@[001:010) Identifier |minLength|
//@[010:011) LeftParen |(|
//@[011:012) Minus |-|
//@[012:015) Integer |100|
//@[015:016) RightParen |)|
//@[016:017) NewLine |\n|
param invalidLength string
//@[000:005) Identifier |param|
//@[006:019) Identifier |invalidLength|
//@[020:026) Identifier |string|
//@[026:028) NewLine |\n\n|

@allowed([
//@[000:001) At |@|
//@[001:008) Identifier |allowed|
//@[008:009) LeftParen |(|
//@[009:010) LeftSquare |[|
//@[010:011) NewLine |\n|
	'Microsoft.AnalysisServices/servers'
//@[001:037) StringComplete |'Microsoft.AnalysisServices/servers'|
//@[037:038) NewLine |\n|
	'Microsoft.ApiManagement/service'
//@[001:034) StringComplete |'Microsoft.ApiManagement/service'|
//@[034:035) NewLine |\n|
	'Microsoft.Network/applicationGateways'
//@[001:040) StringComplete |'Microsoft.Network/applicationGateways'|
//@[040:041) NewLine |\n|
	'Microsoft.Automation/automationAccounts'
//@[001:042) StringComplete |'Microsoft.Automation/automationAccounts'|
//@[042:043) NewLine |\n|
	'Microsoft.ContainerInstance/containerGroups'
//@[001:046) StringComplete |'Microsoft.ContainerInstance/containerGroups'|
//@[046:047) NewLine |\n|
	'Microsoft.ContainerRegistry/registries'
//@[001:041) StringComplete |'Microsoft.ContainerRegistry/registries'|
//@[041:042) NewLine |\n|
	'Microsoft.ContainerService/managedClusters'
//@[001:045) StringComplete |'Microsoft.ContainerService/managedClusters'|
//@[045:046) NewLine |\n|
])
//@[000:001) RightSquare |]|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
param invalidPermutation array = [
//@[000:005) Identifier |param|
//@[006:024) Identifier |invalidPermutation|
//@[025:030) Identifier |array|
//@[031:032) Assignment |=|
//@[033:034) LeftSquare |[|
//@[034:035) NewLine |\n|
	'foobar'
//@[001:009) StringComplete |'foobar'|
//@[009:010) NewLine |\n|
	true
//@[001:005) TrueKeyword |true|
//@[005:006) NewLine |\n|
    100
//@[004:007) Integer |100|
//@[007:008) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\n\n|

@allowed([
//@[000:001) At |@|
//@[001:008) Identifier |allowed|
//@[008:009) LeftParen |(|
//@[009:010) LeftSquare |[|
//@[010:011) NewLine |\n|
	[
//@[001:002) LeftSquare |[|
//@[002:003) NewLine |\n|
		'Microsoft.AnalysisServices/servers'
//@[002:038) StringComplete |'Microsoft.AnalysisServices/servers'|
//@[038:039) NewLine |\n|
		'Microsoft.ApiManagement/service'
//@[002:035) StringComplete |'Microsoft.ApiManagement/service'|
//@[035:036) NewLine |\n|
	]
//@[001:002) RightSquare |]|
//@[002:003) NewLine |\n|
	[
//@[001:002) LeftSquare |[|
//@[002:003) NewLine |\n|
		'Microsoft.Network/applicationGateways'
//@[002:041) StringComplete |'Microsoft.Network/applicationGateways'|
//@[041:042) NewLine |\n|
		'Microsoft.Automation/automationAccounts'
//@[002:043) StringComplete |'Microsoft.Automation/automationAccounts'|
//@[043:044) NewLine |\n|
	]
//@[001:002) RightSquare |]|
//@[002:003) NewLine |\n|
])
//@[000:001) RightSquare |]|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
param invalidDefaultWithAllowedArrayDecorator array = true
//@[000:005) Identifier |param|
//@[006:045) Identifier |invalidDefaultWithAllowedArrayDecorator|
//@[046:051) Identifier |array|
//@[052:053) Assignment |=|
//@[054:058) TrueKeyword |true|
//@[058:060) NewLine |\n\n|

// unterminated multi-line comment
//@[034:035) NewLine |\n|
/*    

//@[000:000) EndOfFile ||
