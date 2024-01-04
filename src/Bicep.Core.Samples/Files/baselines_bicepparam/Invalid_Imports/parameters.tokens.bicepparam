using 'main.bicep'
//@[00:05) Identifier |using|
//@[06:18) StringComplete |'main.bicep'|
//@[18:20) NewLine |\n\n|

import * as foo from 'foo.bicep'
//@[00:06) Identifier |import|
//@[07:08) Asterisk |*|
//@[09:11) AsKeyword |as|
//@[12:15) Identifier |foo|
//@[16:20) Identifier |from|
//@[21:32) StringComplete |'foo.bicep'|
//@[32:33) NewLine |\n|
import { bar } from 'foo.bicep'
//@[00:06) Identifier |import|
//@[07:08) LeftBrace |{|
//@[09:12) Identifier |bar|
//@[13:14) RightBrace |}|
//@[15:19) Identifier |from|
//@[20:31) StringComplete |'foo.bicep'|
//@[31:32) NewLine |\n|

//@[00:00) EndOfFile ||
