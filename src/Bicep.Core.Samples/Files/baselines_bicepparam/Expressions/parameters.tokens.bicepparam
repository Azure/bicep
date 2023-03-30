using 'main.bicep'
//@[00:05) Identifier |using|
//@[06:18) StringComplete |'main.bicep'|
//@[18:20) NewLine |\n\n|

param myObject = {
//@[00:05) Identifier |param|
//@[06:14) Identifier |myObject|
//@[15:16) Assignment |=|
//@[17:18) LeftBrace |{|
//@[18:19) NewLine |\n|
  any: any('foo')
//@[02:05) Identifier |any|
//@[05:06) Colon |:|
//@[07:10) Identifier |any|
//@[10:11) LeftParen |(|
//@[11:16) StringComplete |'foo'|
//@[16:17) RightParen |)|
//@[17:18) NewLine |\n|
  array: array([])
//@[02:07) Identifier |array|
//@[07:08) Colon |:|
//@[09:14) Identifier |array|
//@[14:15) LeftParen |(|
//@[15:16) LeftSquare |[|
//@[16:17) RightSquare |]|
//@[17:18) RightParen |)|
//@[18:19) NewLine |\n|
  base64ToString: base64ToString(base64('abc'))
//@[02:16) Identifier |base64ToString|
//@[16:17) Colon |:|
//@[18:32) Identifier |base64ToString|
//@[32:33) LeftParen |(|
//@[33:39) Identifier |base64|
//@[39:40) LeftParen |(|
//@[40:45) StringComplete |'abc'|
//@[45:46) RightParen |)|
//@[46:47) RightParen |)|
//@[47:48) NewLine |\n|
  base64ToJson: base64ToJson(base64('{"hi": "there"}')).hi
//@[02:14) Identifier |base64ToJson|
//@[14:15) Colon |:|
//@[16:28) Identifier |base64ToJson|
//@[28:29) LeftParen |(|
//@[29:35) Identifier |base64|
//@[35:36) LeftParen |(|
//@[36:53) StringComplete |'{"hi": "there"}'|
//@[53:54) RightParen |)|
//@[54:55) RightParen |)|
//@[55:56) Dot |.|
//@[56:58) Identifier |hi|
//@[58:59) NewLine |\n|
  bool: bool(true)
//@[02:06) Identifier |bool|
//@[06:07) Colon |:|
//@[08:12) Identifier |bool|
//@[12:13) LeftParen |(|
//@[13:17) TrueKeyword |true|
//@[17:18) RightParen |)|
//@[18:19) NewLine |\n|
  concat: concat(['abc'], ['def'])
//@[02:08) Identifier |concat|
//@[08:09) Colon |:|
//@[10:16) Identifier |concat|
//@[16:17) LeftParen |(|
//@[17:18) LeftSquare |[|
//@[18:23) StringComplete |'abc'|
//@[23:24) RightSquare |]|
//@[24:25) Comma |,|
//@[26:27) LeftSquare |[|
//@[27:32) StringComplete |'def'|
//@[32:33) RightSquare |]|
//@[33:34) RightParen |)|
//@[34:35) NewLine |\n|
  contains: contains('foo/bar', '/')
//@[02:10) Identifier |contains|
//@[10:11) Colon |:|
//@[12:20) Identifier |contains|
//@[20:21) LeftParen |(|
//@[21:30) StringComplete |'foo/bar'|
//@[30:31) Comma |,|
//@[32:35) StringComplete |'/'|
//@[35:36) RightParen |)|
//@[36:37) NewLine |\n|
  dataUriToString: dataUriToString(dataUri('abc'))
//@[02:17) Identifier |dataUriToString|
//@[17:18) Colon |:|
//@[19:34) Identifier |dataUriToString|
//@[34:35) LeftParen |(|
//@[35:42) Identifier |dataUri|
//@[42:43) LeftParen |(|
//@[43:48) StringComplete |'abc'|
//@[48:49) RightParen |)|
//@[49:50) RightParen |)|
//@[50:51) NewLine |\n|
  dateTimeAdd: dateTimeAdd(dateTimeFromEpoch(1680224438), 'P1D')  
//@[02:13) Identifier |dateTimeAdd|
//@[13:14) Colon |:|
//@[15:26) Identifier |dateTimeAdd|
//@[26:27) LeftParen |(|
//@[27:44) Identifier |dateTimeFromEpoch|
//@[44:45) LeftParen |(|
//@[45:55) Integer |1680224438|
//@[55:56) RightParen |)|
//@[56:57) Comma |,|
//@[58:63) StringComplete |'P1D'|
//@[63:64) RightParen |)|
//@[66:67) NewLine |\n|
  dateTimeToEpoch: dateTimeToEpoch(dateTimeFromEpoch(1680224438))
//@[02:17) Identifier |dateTimeToEpoch|
//@[17:18) Colon |:|
//@[19:34) Identifier |dateTimeToEpoch|
//@[34:35) LeftParen |(|
//@[35:52) Identifier |dateTimeFromEpoch|
//@[52:53) LeftParen |(|
//@[53:63) Integer |1680224438|
//@[63:64) RightParen |)|
//@[64:65) RightParen |)|
//@[65:66) NewLine |\n|
  empty: empty([])
//@[02:07) Identifier |empty|
//@[07:08) Colon |:|
//@[09:14) Identifier |empty|
//@[14:15) LeftParen |(|
//@[15:16) LeftSquare |[|
//@[16:17) RightSquare |]|
//@[17:18) RightParen |)|
//@[18:19) NewLine |\n|
  endsWith: endsWith('foo', 'o')
//@[02:10) Identifier |endsWith|
//@[10:11) Colon |:|
//@[12:20) Identifier |endsWith|
//@[20:21) LeftParen |(|
//@[21:26) StringComplete |'foo'|
//@[26:27) Comma |,|
//@[28:31) StringComplete |'o'|
//@[31:32) RightParen |)|
//@[32:33) NewLine |\n|
  filter: filter([1, 2], i => i < 2)
//@[02:08) Identifier |filter|
//@[08:09) Colon |:|
//@[10:16) Identifier |filter|
//@[16:17) LeftParen |(|
//@[17:18) LeftSquare |[|
//@[18:19) Integer |1|
//@[19:20) Comma |,|
//@[21:22) Integer |2|
//@[22:23) RightSquare |]|
//@[23:24) Comma |,|
//@[25:26) Identifier |i|
//@[27:29) Arrow |=>|
//@[30:31) Identifier |i|
//@[32:33) LessThan |<|
//@[34:35) Integer |2|
//@[35:36) RightParen |)|
//@[36:37) NewLine |\n|
  first: first([124, 25])
//@[02:07) Identifier |first|
//@[07:08) Colon |:|
//@[09:14) Identifier |first|
//@[14:15) LeftParen |(|
//@[15:16) LeftSquare |[|
//@[16:19) Integer |124|
//@[19:20) Comma |,|
//@[21:23) Integer |25|
//@[23:24) RightSquare |]|
//@[24:25) RightParen |)|
//@[25:26) NewLine |\n|
  flatten: flatten([['abc'], ['def']])
//@[02:09) Identifier |flatten|
//@[09:10) Colon |:|
//@[11:18) Identifier |flatten|
//@[18:19) LeftParen |(|
//@[19:20) LeftSquare |[|
//@[20:21) LeftSquare |[|
//@[21:26) StringComplete |'abc'|
//@[26:27) RightSquare |]|
//@[27:28) Comma |,|
//@[29:30) LeftSquare |[|
//@[30:35) StringComplete |'def'|
//@[35:36) RightSquare |]|
//@[36:37) RightSquare |]|
//@[37:38) RightParen |)|
//@[38:39) NewLine |\n|
  format: format('->{0}<-', 123)
//@[02:08) Identifier |format|
//@[08:09) Colon |:|
//@[10:16) Identifier |format|
//@[16:17) LeftParen |(|
//@[17:26) StringComplete |'->{0}<-'|
//@[26:27) Comma |,|
//@[28:31) Integer |123|
//@[31:32) RightParen |)|
//@[32:33) NewLine |\n|
  guid: guid('asdf')
//@[02:06) Identifier |guid|
//@[06:07) Colon |:|
//@[08:12) Identifier |guid|
//@[12:13) LeftParen |(|
//@[13:19) StringComplete |'asdf'|
//@[19:20) RightParen |)|
//@[20:21) NewLine |\n|
  indexOf: indexOf('abc', 'b')
//@[02:09) Identifier |indexOf|
//@[09:10) Colon |:|
//@[11:18) Identifier |indexOf|
//@[18:19) LeftParen |(|
//@[19:24) StringComplete |'abc'|
//@[24:25) Comma |,|
//@[26:29) StringComplete |'b'|
//@[29:30) RightParen |)|
//@[30:31) NewLine |\n|
  int: int(123)
//@[02:05) Identifier |int|
//@[05:06) Colon |:|
//@[07:10) Identifier |int|
//@[10:11) LeftParen |(|
//@[11:14) Integer |123|
//@[14:15) RightParen |)|
//@[15:16) NewLine |\n|
  intersection: intersection([1, 2, 3], [2, 3, 4])
//@[02:14) Identifier |intersection|
//@[14:15) Colon |:|
//@[16:28) Identifier |intersection|
//@[28:29) LeftParen |(|
//@[29:30) LeftSquare |[|
//@[30:31) Integer |1|
//@[31:32) Comma |,|
//@[33:34) Integer |2|
//@[34:35) Comma |,|
//@[36:37) Integer |3|
//@[37:38) RightSquare |]|
//@[38:39) Comma |,|
//@[40:41) LeftSquare |[|
//@[41:42) Integer |2|
//@[42:43) Comma |,|
//@[44:45) Integer |3|
//@[45:46) Comma |,|
//@[47:48) Integer |4|
//@[48:49) RightSquare |]|
//@[49:50) RightParen |)|
//@[50:51) NewLine |\n|
  items: items({ foo: 'abc', bar: 123 })
//@[02:07) Identifier |items|
//@[07:08) Colon |:|
//@[09:14) Identifier |items|
//@[14:15) LeftParen |(|
//@[15:16) LeftBrace |{|
//@[17:20) Identifier |foo|
//@[20:21) Colon |:|
//@[22:27) StringComplete |'abc'|
//@[27:28) Comma |,|
//@[29:32) Identifier |bar|
//@[32:33) Colon |:|
//@[34:37) Integer |123|
//@[38:39) RightBrace |}|
//@[39:40) RightParen |)|
//@[40:41) NewLine |\n|
  join: join(['abc', 'def', 'ghi'], '/')
//@[02:06) Identifier |join|
//@[06:07) Colon |:|
//@[08:12) Identifier |join|
//@[12:13) LeftParen |(|
//@[13:14) LeftSquare |[|
//@[14:19) StringComplete |'abc'|
//@[19:20) Comma |,|
//@[21:26) StringComplete |'def'|
//@[26:27) Comma |,|
//@[28:33) StringComplete |'ghi'|
//@[33:34) RightSquare |]|
//@[34:35) Comma |,|
//@[36:39) StringComplete |'/'|
//@[39:40) RightParen |)|
//@[40:41) NewLine |\n|
  last: last([1, 2])
//@[02:06) Identifier |last|
//@[06:07) Colon |:|
//@[08:12) Identifier |last|
//@[12:13) LeftParen |(|
//@[13:14) LeftSquare |[|
//@[14:15) Integer |1|
//@[15:16) Comma |,|
//@[17:18) Integer |2|
//@[18:19) RightSquare |]|
//@[19:20) RightParen |)|
//@[20:21) NewLine |\n|
  lastIndexOf: lastIndexOf('abcba', 'b')
//@[02:13) Identifier |lastIndexOf|
//@[13:14) Colon |:|
//@[15:26) Identifier |lastIndexOf|
//@[26:27) LeftParen |(|
//@[27:34) StringComplete |'abcba'|
//@[34:35) Comma |,|
//@[36:39) StringComplete |'b'|
//@[39:40) RightParen |)|
//@[40:41) NewLine |\n|
  length: length([0, 1, 2])
//@[02:08) Identifier |length|
//@[08:09) Colon |:|
//@[10:16) Identifier |length|
//@[16:17) LeftParen |(|
//@[17:18) LeftSquare |[|
//@[18:19) Integer |0|
//@[19:20) Comma |,|
//@[21:22) Integer |1|
//@[22:23) Comma |,|
//@[24:25) Integer |2|
//@[25:26) RightSquare |]|
//@[26:27) RightParen |)|
//@[27:28) NewLine |\n|
  loadFileAsBase64: loadFileAsBase64('test.txt')
//@[02:18) Identifier |loadFileAsBase64|
//@[18:19) Colon |:|
//@[20:36) Identifier |loadFileAsBase64|
//@[36:37) LeftParen |(|
//@[37:47) StringComplete |'test.txt'|
//@[47:48) RightParen |)|
//@[48:49) NewLine |\n|
  loadJsonContent: loadJsonContent('test.json')
//@[02:17) Identifier |loadJsonContent|
//@[17:18) Colon |:|
//@[19:34) Identifier |loadJsonContent|
//@[34:35) LeftParen |(|
//@[35:46) StringComplete |'test.json'|
//@[46:47) RightParen |)|
//@[47:48) NewLine |\n|
  loadTextContent: loadTextContent('test.txt')
//@[02:17) Identifier |loadTextContent|
//@[17:18) Colon |:|
//@[19:34) Identifier |loadTextContent|
//@[34:35) LeftParen |(|
//@[35:45) StringComplete |'test.txt'|
//@[45:46) RightParen |)|
//@[46:47) NewLine |\n|
  map: map(range(0, 3), i => 'Hi ${i}!')
//@[02:05) Identifier |map|
//@[05:06) Colon |:|
//@[07:10) Identifier |map|
//@[10:11) LeftParen |(|
//@[11:16) Identifier |range|
//@[16:17) LeftParen |(|
//@[17:18) Integer |0|
//@[18:19) Comma |,|
//@[20:21) Integer |3|
//@[21:22) RightParen |)|
//@[22:23) Comma |,|
//@[24:25) Identifier |i|
//@[26:28) Arrow |=>|
//@[29:35) StringLeftPiece |'Hi ${|
//@[35:36) Identifier |i|
//@[36:39) StringRightPiece |}!'|
//@[39:40) RightParen |)|
//@[40:41) NewLine |\n|
  max: max(1, 2, 3)
//@[02:05) Identifier |max|
//@[05:06) Colon |:|
//@[07:10) Identifier |max|
//@[10:11) LeftParen |(|
//@[11:12) Integer |1|
//@[12:13) Comma |,|
//@[14:15) Integer |2|
//@[15:16) Comma |,|
//@[17:18) Integer |3|
//@[18:19) RightParen |)|
//@[19:20) NewLine |\n|
  min: min(1, 2, 3)
//@[02:05) Identifier |min|
//@[05:06) Colon |:|
//@[07:10) Identifier |min|
//@[10:11) LeftParen |(|
//@[11:12) Integer |1|
//@[12:13) Comma |,|
//@[14:15) Integer |2|
//@[15:16) Comma |,|
//@[17:18) Integer |3|
//@[18:19) RightParen |)|
//@[19:20) NewLine |\n|
  padLeft: padLeft(13, 5)
//@[02:09) Identifier |padLeft|
//@[09:10) Colon |:|
//@[11:18) Identifier |padLeft|
//@[18:19) LeftParen |(|
//@[19:21) Integer |13|
//@[21:22) Comma |,|
//@[23:24) Integer |5|
//@[24:25) RightParen |)|
//@[25:26) NewLine |\n|
  range: range(0, 3)
//@[02:07) Identifier |range|
//@[07:08) Colon |:|
//@[09:14) Identifier |range|
//@[14:15) LeftParen |(|
//@[15:16) Integer |0|
//@[16:17) Comma |,|
//@[18:19) Integer |3|
//@[19:20) RightParen |)|
//@[20:21) NewLine |\n|
  reduce: reduce(['a', 'b', 'c'], '', (a, b) => '${a}-${b}')
//@[02:08) Identifier |reduce|
//@[08:09) Colon |:|
//@[10:16) Identifier |reduce|
//@[16:17) LeftParen |(|
//@[17:18) LeftSquare |[|
//@[18:21) StringComplete |'a'|
//@[21:22) Comma |,|
//@[23:26) StringComplete |'b'|
//@[26:27) Comma |,|
//@[28:31) StringComplete |'c'|
//@[31:32) RightSquare |]|
//@[32:33) Comma |,|
//@[34:36) StringComplete |''|
//@[36:37) Comma |,|
//@[38:39) LeftParen |(|
//@[39:40) Identifier |a|
//@[40:41) Comma |,|
//@[42:43) Identifier |b|
//@[43:44) RightParen |)|
//@[45:47) Arrow |=>|
//@[48:51) StringLeftPiece |'${|
//@[51:52) Identifier |a|
//@[52:56) StringMiddlePiece |}-${|
//@[56:57) Identifier |b|
//@[57:59) StringRightPiece |}'|
//@[59:60) RightParen |)|
//@[60:61) NewLine |\n|
  replace: replace('abc', 'b', '/')
//@[02:09) Identifier |replace|
//@[09:10) Colon |:|
//@[11:18) Identifier |replace|
//@[18:19) LeftParen |(|
//@[19:24) StringComplete |'abc'|
//@[24:25) Comma |,|
//@[26:29) StringComplete |'b'|
//@[29:30) Comma |,|
//@[31:34) StringComplete |'/'|
//@[34:35) RightParen |)|
//@[35:36) NewLine |\n|
  skip: skip([1, 2, 3], 1)
//@[02:06) Identifier |skip|
//@[06:07) Colon |:|
//@[08:12) Identifier |skip|
//@[12:13) LeftParen |(|
//@[13:14) LeftSquare |[|
//@[14:15) Integer |1|
//@[15:16) Comma |,|
//@[17:18) Integer |2|
//@[18:19) Comma |,|
//@[20:21) Integer |3|
//@[21:22) RightSquare |]|
//@[22:23) Comma |,|
//@[24:25) Integer |1|
//@[25:26) RightParen |)|
//@[26:27) NewLine |\n|
  sort: sort(['c', 'd', 'a'], (a, b) => a < b)
//@[02:06) Identifier |sort|
//@[06:07) Colon |:|
//@[08:12) Identifier |sort|
//@[12:13) LeftParen |(|
//@[13:14) LeftSquare |[|
//@[14:17) StringComplete |'c'|
//@[17:18) Comma |,|
//@[19:22) StringComplete |'d'|
//@[22:23) Comma |,|
//@[24:27) StringComplete |'a'|
//@[27:28) RightSquare |]|
//@[28:29) Comma |,|
//@[30:31) LeftParen |(|
//@[31:32) Identifier |a|
//@[32:33) Comma |,|
//@[34:35) Identifier |b|
//@[35:36) RightParen |)|
//@[37:39) Arrow |=>|
//@[40:41) Identifier |a|
//@[42:43) LessThan |<|
//@[44:45) Identifier |b|
//@[45:46) RightParen |)|
//@[46:47) NewLine |\n|
  split: split('a/b/c', '/')
//@[02:07) Identifier |split|
//@[07:08) Colon |:|
//@[09:14) Identifier |split|
//@[14:15) LeftParen |(|
//@[15:22) StringComplete |'a/b/c'|
//@[22:23) Comma |,|
//@[24:27) StringComplete |'/'|
//@[27:28) RightParen |)|
//@[28:29) NewLine |\n|
  startsWith: startsWith('abc', 'a')
//@[02:12) Identifier |startsWith|
//@[12:13) Colon |:|
//@[14:24) Identifier |startsWith|
//@[24:25) LeftParen |(|
//@[25:30) StringComplete |'abc'|
//@[30:31) Comma |,|
//@[32:35) StringComplete |'a'|
//@[35:36) RightParen |)|
//@[36:37) NewLine |\n|
  string: string('asdf')
//@[02:08) Identifier |string|
//@[08:09) Colon |:|
//@[10:16) Identifier |string|
//@[16:17) LeftParen |(|
//@[17:23) StringComplete |'asdf'|
//@[23:24) RightParen |)|
//@[24:25) NewLine |\n|
  substring: substring('asdfasf', 3)
//@[02:11) Identifier |substring|
//@[11:12) Colon |:|
//@[13:22) Identifier |substring|
//@[22:23) LeftParen |(|
//@[23:32) StringComplete |'asdfasf'|
//@[32:33) Comma |,|
//@[34:35) Integer |3|
//@[35:36) RightParen |)|
//@[36:37) NewLine |\n|
  take: take([1, 2, 3], 2)
//@[02:06) Identifier |take|
//@[06:07) Colon |:|
//@[08:12) Identifier |take|
//@[12:13) LeftParen |(|
//@[13:14) LeftSquare |[|
//@[14:15) Integer |1|
//@[15:16) Comma |,|
//@[17:18) Integer |2|
//@[18:19) Comma |,|
//@[20:21) Integer |3|
//@[21:22) RightSquare |]|
//@[22:23) Comma |,|
//@[24:25) Integer |2|
//@[25:26) RightParen |)|
//@[26:27) NewLine |\n|
  toLower: toLower('AiKInIniIN')
//@[02:09) Identifier |toLower|
//@[09:10) Colon |:|
//@[11:18) Identifier |toLower|
//@[18:19) LeftParen |(|
//@[19:31) StringComplete |'AiKInIniIN'|
//@[31:32) RightParen |)|
//@[32:33) NewLine |\n|
  toObject: toObject(['a', 'b', 'c'], x => x, x => 'Hi ${x}!')
//@[02:10) Identifier |toObject|
//@[10:11) Colon |:|
//@[12:20) Identifier |toObject|
//@[20:21) LeftParen |(|
//@[21:22) LeftSquare |[|
//@[22:25) StringComplete |'a'|
//@[25:26) Comma |,|
//@[27:30) StringComplete |'b'|
//@[30:31) Comma |,|
//@[32:35) StringComplete |'c'|
//@[35:36) RightSquare |]|
//@[36:37) Comma |,|
//@[38:39) Identifier |x|
//@[40:42) Arrow |=>|
//@[43:44) Identifier |x|
//@[44:45) Comma |,|
//@[46:47) Identifier |x|
//@[48:50) Arrow |=>|
//@[51:57) StringLeftPiece |'Hi ${|
//@[57:58) Identifier |x|
//@[58:61) StringRightPiece |}!'|
//@[61:62) RightParen |)|
//@[62:63) NewLine |\n|
  toUpper: toUpper('AiKInIniIN')
//@[02:09) Identifier |toUpper|
//@[09:10) Colon |:|
//@[11:18) Identifier |toUpper|
//@[18:19) LeftParen |(|
//@[19:31) StringComplete |'AiKInIniIN'|
//@[31:32) RightParen |)|
//@[32:33) NewLine |\n|
  trim: trim('  adf asdf  ')
//@[02:06) Identifier |trim|
//@[06:07) Colon |:|
//@[08:12) Identifier |trim|
//@[12:13) LeftParen |(|
//@[13:27) StringComplete |'  adf asdf  '|
//@[27:28) RightParen |)|
//@[28:29) NewLine |\n|
  union: union({ abc: 'def' }, { def: 'ghi' })
//@[02:07) Identifier |union|
//@[07:08) Colon |:|
//@[09:14) Identifier |union|
//@[14:15) LeftParen |(|
//@[15:16) LeftBrace |{|
//@[17:20) Identifier |abc|
//@[20:21) Colon |:|
//@[22:27) StringComplete |'def'|
//@[28:29) RightBrace |}|
//@[29:30) Comma |,|
//@[31:32) LeftBrace |{|
//@[33:36) Identifier |def|
//@[36:37) Colon |:|
//@[38:43) StringComplete |'ghi'|
//@[44:45) RightBrace |}|
//@[45:46) RightParen |)|
//@[46:47) NewLine |\n|
  uniqueString: uniqueString('asd', 'asdf', 'asdf')
//@[02:14) Identifier |uniqueString|
//@[14:15) Colon |:|
//@[16:28) Identifier |uniqueString|
//@[28:29) LeftParen |(|
//@[29:34) StringComplete |'asd'|
//@[34:35) Comma |,|
//@[36:42) StringComplete |'asdf'|
//@[42:43) Comma |,|
//@[44:50) StringComplete |'asdf'|
//@[50:51) RightParen |)|
//@[51:52) NewLine |\n|
  uri: uri('https://github.com', 'Azure/bicep')
//@[02:05) Identifier |uri|
//@[05:06) Colon |:|
//@[07:10) Identifier |uri|
//@[10:11) LeftParen |(|
//@[11:31) StringComplete |'https://github.com'|
//@[31:32) Comma |,|
//@[33:46) StringComplete |'Azure/bicep'|
//@[46:47) RightParen |)|
//@[47:48) NewLine |\n|
  uriComponent: uriComponent('UB*8h 0+=_)9h9n')
//@[02:14) Identifier |uriComponent|
//@[14:15) Colon |:|
//@[16:28) Identifier |uriComponent|
//@[28:29) LeftParen |(|
//@[29:46) StringComplete |'UB*8h 0+=_)9h9n'|
//@[46:47) RightParen |)|
//@[47:48) NewLine |\n|
  uriComponentToString: uriComponentToString('%20%25%20')
//@[02:22) Identifier |uriComponentToString|
//@[22:23) Colon |:|
//@[24:44) Identifier |uriComponentToString|
//@[44:45) LeftParen |(|
//@[45:56) StringComplete |'%20%25%20'|
//@[56:57) RightParen |)|
//@[57:58) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

param myBool = true
//@[00:05) Identifier |param|
//@[06:12) Identifier |myBool|
//@[13:14) Assignment |=|
//@[15:19) TrueKeyword |true|
//@[19:20) NewLine |\n|
param myInt = myBool ? 123 : 456
//@[00:05) Identifier |param|
//@[06:11) Identifier |myInt|
//@[12:13) Assignment |=|
//@[14:20) Identifier |myBool|
//@[21:22) Question |?|
//@[23:26) Integer |123|
//@[27:28) Colon |:|
//@[29:32) Integer |456|
//@[32:34) NewLine |\n\n|

param myArray = [
//@[00:05) Identifier |param|
//@[06:13) Identifier |myArray|
//@[14:15) Assignment |=|
//@[16:17) LeftSquare |[|
//@[17:18) NewLine |\n|
  (true ? 'a' : 'b')
//@[02:03) LeftParen |(|
//@[03:07) TrueKeyword |true|
//@[08:09) Question |?|
//@[10:13) StringComplete |'a'|
//@[14:15) Colon |:|
//@[16:19) StringComplete |'b'|
//@[19:20) RightParen |)|
//@[20:21) NewLine |\n|
  !true
//@[02:03) Exclamation |!|
//@[03:07) TrueKeyword |true|
//@[07:08) NewLine |\n|
  123 + 456
//@[02:05) Integer |123|
//@[06:07) Plus |+|
//@[08:11) Integer |456|
//@[11:12) NewLine |\n|
  456 - 123
//@[02:05) Integer |456|
//@[06:07) Minus |-|
//@[08:11) Integer |123|
//@[11:12) NewLine |\n|
  2 * 3
//@[02:03) Integer |2|
//@[04:05) Asterisk |*|
//@[06:07) Integer |3|
//@[07:08) NewLine |\n|
  10 / 2
//@[02:04) Integer |10|
//@[05:06) Slash |/|
//@[07:08) Integer |2|
//@[08:09) NewLine |\n|
  1 < 2
//@[02:03) Integer |1|
//@[04:05) LessThan |<|
//@[06:07) Integer |2|
//@[07:08) NewLine |\n|
  1 > 2
//@[02:03) Integer |1|
//@[04:05) GreaterThan |>|
//@[06:07) Integer |2|
//@[07:08) NewLine |\n|
  1 >= 2
//@[02:03) Integer |1|
//@[04:06) GreaterThanOrEqual |>=|
//@[07:08) Integer |2|
//@[08:09) NewLine |\n|
  1 <= 2
//@[02:03) Integer |1|
//@[04:06) LessThanOrEqual |<=|
//@[07:08) Integer |2|
//@[08:09) NewLine |\n|
]
//@[00:01) RightSquare |]|
//@[01:02) NewLine |\n|
param myString = '''
//@[00:05) Identifier |param|
//@[06:14) Identifier |myString|
//@[15:16) Assignment |=|
//@[17:72) MultilineString |'''\nTHis\n  is\n    a\n      multiline\n        string!\n'''|
THis
  is
    a
      multiline
        string!
'''
//@[03:04) NewLine |\n|

//@[00:00) EndOfFile ||
