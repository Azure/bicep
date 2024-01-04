using 'main.bicep'
//@[000:005) Identifier |using|
//@[006:018) StringComplete |'main.bicep'|
//@[018:019) NewLine |\n|
import * as bicepconfig from 'bicepconfig.bicep'
//@[000:006) Identifier |import|
//@[007:008) Asterisk |*|
//@[009:011) AsKeyword |as|
//@[012:023) Identifier |bicepconfig|
//@[024:028) Identifier |from|
//@[029:048) StringComplete |'bicepconfig.bicep'|
//@[048:049) NewLine |\n|
// ok
//@[005:006) NewLine |\n|
param one = bicepconfig.directExport
//@[000:005) Identifier |param|
//@[006:009) Identifier |one|
//@[010:011) Assignment |=|
//@[012:023) Identifier |bicepconfig|
//@[023:024) Dot |.|
//@[024:036) Identifier |directExport|
//@[036:037) NewLine |\n|
// Failed to evaluate parameter "two"
//@[037:038) NewLine |\n|
// Unhandled exception during evaluating template language function 'variables' is not handled.bicep(BCP338)
//@[108:109) NewLine |\n|
param two = bicepconfig.functionExport
//@[000:005) Identifier |param|
//@[006:009) Identifier |two|
//@[010:011) Assignment |=|
//@[012:023) Identifier |bicepconfig|
//@[023:024) Dot |.|
//@[024:038) Identifier |functionExport|
//@[038:039) NewLine |\n|

//@[000:000) EndOfFile ||
