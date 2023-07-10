var doggos = [
//@[000:2847) ProgramExpression
//@[000:0054) ├─DeclaredVariableExpression { Name = doggos }
//@[013:0054) | └─ArrayExpression
  'Evie'
//@[002:0008) |   ├─StringLiteralExpression { Value = Evie }
  'Casper'
//@[002:0010) |   ├─StringLiteralExpression { Value = Casper }
  'Indy'
//@[002:0008) |   ├─StringLiteralExpression { Value = Indy }
  'Kira'
//@[002:0008) |   └─StringLiteralExpression { Value = Kira }
]

var numbers = range(0, 4)
//@[000:0025) ├─DeclaredVariableExpression { Name = numbers }
//@[014:0025) | └─FunctionCallExpression { Name = range }
//@[020:0021) |   ├─IntegerLiteralExpression { Value = 0 }
//@[023:0024) |   └─IntegerLiteralExpression { Value = 4 }

var sayHello = map(doggos, i => 'Hello ${i}!')
//@[000:0046) ├─DeclaredVariableExpression { Name = sayHello }
//@[015:0046) | └─FunctionCallExpression { Name = map }
//@[019:0025) |   ├─VariableReferenceExpression { Variable = doggos }
//@[027:0045) |   └─LambdaExpression
//@[032:0045) |     └─InterpolatedStringExpression
//@[041:0042) |       └─LambdaVariableReferenceExpression { Variable = i }

var isEven = filter(numbers, i => 0 == i % 2)
//@[000:0045) ├─DeclaredVariableExpression { Name = isEven }
//@[013:0045) | └─FunctionCallExpression { Name = filter }
//@[020:0027) |   ├─VariableReferenceExpression { Variable = numbers }
//@[029:0044) |   └─LambdaExpression
//@[034:0044) |     └─BinaryExpression { Operator = Equals }
//@[034:0035) |       ├─IntegerLiteralExpression { Value = 0 }
//@[039:0044) |       └─BinaryExpression { Operator = Modulo }
//@[039:0040) |         ├─LambdaVariableReferenceExpression { Variable = i }
//@[043:0044) |         └─IntegerLiteralExpression { Value = 2 }

var evenDoggosNestedLambdas = map(filter(numbers, i => contains(filter(numbers, j => 0 == j % 2), i)), x => doggos[x])
//@[000:0118) ├─DeclaredVariableExpression { Name = evenDoggosNestedLambdas }
//@[030:0118) | └─FunctionCallExpression { Name = map }
//@[034:0101) |   ├─FunctionCallExpression { Name = filter }
//@[041:0048) |   | ├─VariableReferenceExpression { Variable = numbers }
//@[050:0100) |   | └─LambdaExpression
//@[055:0100) |   |   └─FunctionCallExpression { Name = contains }
//@[064:0096) |   |     ├─FunctionCallExpression { Name = filter }
//@[071:0078) |   |     | ├─VariableReferenceExpression { Variable = numbers }
//@[080:0095) |   |     | └─LambdaExpression
//@[085:0095) |   |     |   └─BinaryExpression { Operator = Equals }
//@[085:0086) |   |     |     ├─IntegerLiteralExpression { Value = 0 }
//@[090:0095) |   |     |     └─BinaryExpression { Operator = Modulo }
//@[090:0091) |   |     |       ├─LambdaVariableReferenceExpression { Variable = j }
//@[094:0095) |   |     |       └─IntegerLiteralExpression { Value = 2 }
//@[098:0099) |   |     └─LambdaVariableReferenceExpression { Variable = i }
//@[103:0117) |   └─LambdaExpression
//@[108:0117) |     └─ArrayAccessExpression
//@[115:0116) |       ├─LambdaVariableReferenceExpression { Variable = x }
//@[108:0114) |       └─VariableReferenceExpression { Variable = doggos }

var flattenedArrayOfArrays = flatten([[0, 1], [2, 3], [4, 5]])
//@[000:0062) ├─DeclaredVariableExpression { Name = flattenedArrayOfArrays }
//@[029:0062) | └─FunctionCallExpression { Name = flatten }
//@[037:0061) |   └─ArrayExpression
//@[038:0044) |     ├─ArrayExpression
//@[039:0040) |     | ├─IntegerLiteralExpression { Value = 0 }
//@[042:0043) |     | └─IntegerLiteralExpression { Value = 1 }
//@[046:0052) |     ├─ArrayExpression
//@[047:0048) |     | ├─IntegerLiteralExpression { Value = 2 }
//@[050:0051) |     | └─IntegerLiteralExpression { Value = 3 }
//@[054:0060) |     └─ArrayExpression
//@[055:0056) |       ├─IntegerLiteralExpression { Value = 4 }
//@[058:0059) |       └─IntegerLiteralExpression { Value = 5 }
var flattenedEmptyArray = flatten([])
//@[000:0037) ├─DeclaredVariableExpression { Name = flattenedEmptyArray }
//@[026:0037) | └─FunctionCallExpression { Name = flatten }
//@[034:0036) |   └─ArrayExpression

var mapSayHi = map(['abc', 'def', 'ghi'], foo => 'Hi ${foo}!')
//@[000:0062) ├─DeclaredVariableExpression { Name = mapSayHi }
//@[015:0062) | └─FunctionCallExpression { Name = map }
//@[019:0040) |   ├─ArrayExpression
//@[020:0025) |   | ├─StringLiteralExpression { Value = abc }
//@[027:0032) |   | ├─StringLiteralExpression { Value = def }
//@[034:0039) |   | └─StringLiteralExpression { Value = ghi }
//@[042:0061) |   └─LambdaExpression
//@[049:0061) |     └─InterpolatedStringExpression
//@[055:0058) |       └─LambdaVariableReferenceExpression { Variable = foo }
var mapEmpty = map([], foo => 'Hi ${foo}!')
//@[000:0043) ├─DeclaredVariableExpression { Name = mapEmpty }
//@[015:0043) | └─FunctionCallExpression { Name = map }
//@[019:0021) |   ├─ArrayExpression
//@[023:0042) |   └─LambdaExpression
//@[030:0042) |     └─InterpolatedStringExpression
//@[036:0039) |       └─LambdaVariableReferenceExpression { Variable = foo }
var mapObject = map(range(0, length(doggos)), i => {
//@[000:0115) ├─DeclaredVariableExpression { Name = mapObject }
//@[016:0115) | └─FunctionCallExpression { Name = map }
//@[020:0044) |   ├─FunctionCallExpression { Name = range }
//@[026:0027) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[029:0043) |   | └─FunctionCallExpression { Name = length }
//@[036:0042) |   |   └─VariableReferenceExpression { Variable = doggos }
//@[046:0114) |   └─LambdaExpression
//@[051:0114) |     └─ObjectExpression
  i: i
//@[002:0006) |       ├─ObjectPropertyExpression
//@[002:0003) |       | ├─StringLiteralExpression { Value = i }
//@[005:0006) |       | └─LambdaVariableReferenceExpression { Variable = i }
  doggo: doggos[i]
//@[002:0018) |       ├─ObjectPropertyExpression
//@[002:0007) |       | ├─StringLiteralExpression { Value = doggo }
//@[009:0018) |       | └─ArrayAccessExpression
//@[016:0017) |       |   ├─LambdaVariableReferenceExpression { Variable = i }
//@[009:0015) |       |   └─VariableReferenceExpression { Variable = doggos }
  greeting: 'Ahoy, ${doggos[i]}!'
//@[002:0033) |       └─ObjectPropertyExpression
//@[002:0010) |         ├─StringLiteralExpression { Value = greeting }
//@[012:0033) |         └─InterpolatedStringExpression
//@[021:0030) |           └─ArrayAccessExpression
//@[028:0029) |             ├─LambdaVariableReferenceExpression { Variable = i }
//@[021:0027) |             └─VariableReferenceExpression { Variable = doggos }
})
var mapArray = flatten(map(range(1, 3), i => [i * 2, (i * 2) + 1]))
//@[000:0067) ├─DeclaredVariableExpression { Name = mapArray }
//@[015:0067) | └─FunctionCallExpression { Name = flatten }
//@[023:0066) |   └─FunctionCallExpression { Name = map }
//@[027:0038) |     ├─FunctionCallExpression { Name = range }
//@[033:0034) |     | ├─IntegerLiteralExpression { Value = 1 }
//@[036:0037) |     | └─IntegerLiteralExpression { Value = 3 }
//@[040:0065) |     └─LambdaExpression
//@[045:0065) |       └─ArrayExpression
//@[046:0051) |         ├─BinaryExpression { Operator = Multiply }
//@[046:0047) |         | ├─LambdaVariableReferenceExpression { Variable = i }
//@[050:0051) |         | └─IntegerLiteralExpression { Value = 2 }
//@[053:0064) |         └─BinaryExpression { Operator = Add }
//@[054:0059) |           ├─BinaryExpression { Operator = Multiply }
//@[054:0055) |           | ├─LambdaVariableReferenceExpression { Variable = i }
//@[058:0059) |           | └─IntegerLiteralExpression { Value = 2 }
//@[063:0064) |           └─IntegerLiteralExpression { Value = 1 }
var mapMultiLineArray = flatten(map(range(1, 3), i => [
//@[000:0095) ├─DeclaredVariableExpression { Name = mapMultiLineArray }
//@[024:0095) | └─FunctionCallExpression { Name = flatten }
//@[032:0094) |   └─FunctionCallExpression { Name = map }
//@[036:0047) |     ├─FunctionCallExpression { Name = range }
//@[042:0043) |     | ├─IntegerLiteralExpression { Value = 1 }
//@[045:0046) |     | └─IntegerLiteralExpression { Value = 3 }
//@[049:0093) |     └─LambdaExpression
//@[054:0093) |       └─ArrayExpression
  i * 3
//@[002:0007) |         ├─BinaryExpression { Operator = Multiply }
//@[002:0003) |         | ├─LambdaVariableReferenceExpression { Variable = i }
//@[006:0007) |         | └─IntegerLiteralExpression { Value = 3 }
  (i * 3) + 1
//@[002:0013) |         ├─BinaryExpression { Operator = Add }
//@[003:0008) |         | ├─BinaryExpression { Operator = Multiply }
//@[003:0004) |         | | ├─LambdaVariableReferenceExpression { Variable = i }
//@[007:0008) |         | | └─IntegerLiteralExpression { Value = 3 }
//@[012:0013) |         | └─IntegerLiteralExpression { Value = 1 }
  (i * 3) + 2
//@[002:0013) |         └─BinaryExpression { Operator = Add }
//@[003:0008) |           ├─BinaryExpression { Operator = Multiply }
//@[003:0004) |           | ├─LambdaVariableReferenceExpression { Variable = i }
//@[007:0008) |           | └─IntegerLiteralExpression { Value = 3 }
//@[012:0013) |           └─IntegerLiteralExpression { Value = 2 }
]))

var filterEqualityCheck = filter(['abc', 'def', 'ghi'], foo => 'def' == foo)
//@[000:0076) ├─DeclaredVariableExpression { Name = filterEqualityCheck }
//@[026:0076) | └─FunctionCallExpression { Name = filter }
//@[033:0054) |   ├─ArrayExpression
//@[034:0039) |   | ├─StringLiteralExpression { Value = abc }
//@[041:0046) |   | ├─StringLiteralExpression { Value = def }
//@[048:0053) |   | └─StringLiteralExpression { Value = ghi }
//@[056:0075) |   └─LambdaExpression
//@[063:0075) |     └─BinaryExpression { Operator = Equals }
//@[063:0068) |       ├─StringLiteralExpression { Value = def }
//@[072:0075) |       └─LambdaVariableReferenceExpression { Variable = foo }
var filterEmpty = filter([], foo => 'def' == foo)
//@[000:0049) ├─DeclaredVariableExpression { Name = filterEmpty }
//@[018:0049) | └─FunctionCallExpression { Name = filter }
//@[025:0027) |   ├─ArrayExpression
//@[029:0048) |   └─LambdaExpression
//@[036:0048) |     └─BinaryExpression { Operator = Equals }
//@[036:0041) |       ├─StringLiteralExpression { Value = def }
//@[045:0048) |       └─LambdaVariableReferenceExpression { Variable = foo }

var sortNumeric = sort([8, 3, 10, -13, 5], (x, y) => x < y)
//@[000:0059) ├─DeclaredVariableExpression { Name = sortNumeric }
//@[018:0059) | └─FunctionCallExpression { Name = sort }
//@[023:0041) |   ├─ArrayExpression
//@[024:0025) |   | ├─IntegerLiteralExpression { Value = 8 }
//@[027:0028) |   | ├─IntegerLiteralExpression { Value = 3 }
//@[030:0032) |   | ├─IntegerLiteralExpression { Value = 10 }
//@[034:0037) |   | ├─IntegerLiteralExpression { Value = -13 }
//@[039:0040) |   | └─IntegerLiteralExpression { Value = 5 }
//@[043:0058) |   └─LambdaExpression
//@[053:0058) |     └─BinaryExpression { Operator = LessThan }
//@[053:0054) |       ├─LambdaVariableReferenceExpression { Variable = x }
//@[057:0058) |       └─LambdaVariableReferenceExpression { Variable = y }
var sortAlpha = sort(['ghi', 'abc', 'def'], (x, y) => x < y)
//@[000:0060) ├─DeclaredVariableExpression { Name = sortAlpha }
//@[016:0060) | └─FunctionCallExpression { Name = sort }
//@[021:0042) |   ├─ArrayExpression
//@[022:0027) |   | ├─StringLiteralExpression { Value = ghi }
//@[029:0034) |   | ├─StringLiteralExpression { Value = abc }
//@[036:0041) |   | └─StringLiteralExpression { Value = def }
//@[044:0059) |   └─LambdaExpression
//@[054:0059) |     └─BinaryExpression { Operator = LessThan }
//@[054:0055) |       ├─LambdaVariableReferenceExpression { Variable = x }
//@[058:0059) |       └─LambdaVariableReferenceExpression { Variable = y }
var sortAlphaReverse = sort(['ghi', 'abc', 'def'], (x, y) => x > y)
//@[000:0067) ├─DeclaredVariableExpression { Name = sortAlphaReverse }
//@[023:0067) | └─FunctionCallExpression { Name = sort }
//@[028:0049) |   ├─ArrayExpression
//@[029:0034) |   | ├─StringLiteralExpression { Value = ghi }
//@[036:0041) |   | ├─StringLiteralExpression { Value = abc }
//@[043:0048) |   | └─StringLiteralExpression { Value = def }
//@[051:0066) |   └─LambdaExpression
//@[061:0066) |     └─BinaryExpression { Operator = GreaterThan }
//@[061:0062) |       ├─LambdaVariableReferenceExpression { Variable = x }
//@[065:0066) |       └─LambdaVariableReferenceExpression { Variable = y }
var sortByObjectKey = sort([
//@[000:0188) ├─DeclaredVariableExpression { Name = sortByObjectKey }
//@[022:0188) | └─FunctionCallExpression { Name = sort }
//@[027:0152) |   ├─ArrayExpression
  { key: 124, name: 'Second' }
//@[002:0030) |   | ├─ObjectExpression
//@[004:0012) |   | | ├─ObjectPropertyExpression
//@[004:0007) |   | | | ├─StringLiteralExpression { Value = key }
//@[009:0012) |   | | | └─IntegerLiteralExpression { Value = 124 }
//@[014:0028) |   | | └─ObjectPropertyExpression
//@[014:0018) |   | |   ├─StringLiteralExpression { Value = name }
//@[020:0028) |   | |   └─StringLiteralExpression { Value = Second }
  { key: 298, name: 'Third' }
//@[002:0029) |   | ├─ObjectExpression
//@[004:0012) |   | | ├─ObjectPropertyExpression
//@[004:0007) |   | | | ├─StringLiteralExpression { Value = key }
//@[009:0012) |   | | | └─IntegerLiteralExpression { Value = 298 }
//@[014:0027) |   | | └─ObjectPropertyExpression
//@[014:0018) |   | |   ├─StringLiteralExpression { Value = name }
//@[020:0027) |   | |   └─StringLiteralExpression { Value = Third }
  { key: 24, name: 'First' }
//@[002:0028) |   | ├─ObjectExpression
//@[004:0011) |   | | ├─ObjectPropertyExpression
//@[004:0007) |   | | | ├─StringLiteralExpression { Value = key }
//@[009:0011) |   | | | └─IntegerLiteralExpression { Value = 24 }
//@[013:0026) |   | | └─ObjectPropertyExpression
//@[013:0017) |   | |   ├─StringLiteralExpression { Value = name }
//@[019:0026) |   | |   └─StringLiteralExpression { Value = First }
  { key: 1232, name: 'Fourth' }
//@[002:0031) |   | └─ObjectExpression
//@[004:0013) |   |   ├─ObjectPropertyExpression
//@[004:0007) |   |   | ├─StringLiteralExpression { Value = key }
//@[009:0013) |   |   | └─IntegerLiteralExpression { Value = 1232 }
//@[015:0029) |   |   └─ObjectPropertyExpression
//@[015:0019) |   |     ├─StringLiteralExpression { Value = name }
//@[021:0029) |   |     └─StringLiteralExpression { Value = Fourth }
], (x, y) => int(x.key) < int(y.key))
//@[003:0036) |   └─LambdaExpression
//@[013:0036) |     └─BinaryExpression { Operator = LessThan }
//@[013:0023) |       ├─FunctionCallExpression { Name = int }
//@[017:0022) |       | └─PropertyAccessExpression { PropertyName = key }
//@[017:0018) |       |   └─LambdaVariableReferenceExpression { Variable = x }
//@[026:0036) |       └─FunctionCallExpression { Name = int }
//@[030:0035) |         └─PropertyAccessExpression { PropertyName = key }
//@[030:0031) |           └─LambdaVariableReferenceExpression { Variable = y }
var sortEmpty = sort([], (x, y) => int(x) < int(y))
//@[000:0051) ├─DeclaredVariableExpression { Name = sortEmpty }
//@[016:0051) | └─FunctionCallExpression { Name = sort }
//@[021:0023) |   ├─ArrayExpression
//@[025:0050) |   └─LambdaExpression
//@[035:0050) |     └─BinaryExpression { Operator = LessThan }
//@[035:0041) |       ├─FunctionCallExpression { Name = int }
//@[039:0040) |       | └─LambdaVariableReferenceExpression { Variable = x }
//@[044:0050) |       └─FunctionCallExpression { Name = int }
//@[048:0049) |         └─LambdaVariableReferenceExpression { Variable = y }

var reduceStringConcat = reduce(['abc', 'def', 'ghi'], '', (cur, next) => concat(cur, next))
//@[000:0092) ├─DeclaredVariableExpression { Name = reduceStringConcat }
//@[025:0092) | └─FunctionCallExpression { Name = reduce }
//@[032:0053) |   ├─ArrayExpression
//@[033:0038) |   | ├─StringLiteralExpression { Value = abc }
//@[040:0045) |   | ├─StringLiteralExpression { Value = def }
//@[047:0052) |   | └─StringLiteralExpression { Value = ghi }
//@[055:0057) |   ├─StringLiteralExpression { Value =  }
//@[059:0091) |   └─LambdaExpression
//@[074:0091) |     └─FunctionCallExpression { Name = concat }
//@[081:0084) |       ├─LambdaVariableReferenceExpression { Variable = cur }
//@[086:0090) |       └─LambdaVariableReferenceExpression { Variable = next }
var reduceFactorial = reduce(range(1, 5), 1, (cur, next) => cur * next)
//@[000:0071) ├─DeclaredVariableExpression { Name = reduceFactorial }
//@[022:0071) | └─FunctionCallExpression { Name = reduce }
//@[029:0040) |   ├─FunctionCallExpression { Name = range }
//@[035:0036) |   | ├─IntegerLiteralExpression { Value = 1 }
//@[038:0039) |   | └─IntegerLiteralExpression { Value = 5 }
//@[042:0043) |   ├─IntegerLiteralExpression { Value = 1 }
//@[045:0070) |   └─LambdaExpression
//@[060:0070) |     └─BinaryExpression { Operator = Multiply }
//@[060:0063) |       ├─LambdaVariableReferenceExpression { Variable = cur }
//@[066:0070) |       └─LambdaVariableReferenceExpression { Variable = next }
var reduceObjectUnion = reduce([
//@[000:0117) ├─DeclaredVariableExpression { Name = reduceObjectUnion }
//@[024:0117) | └─FunctionCallExpression { Name = reduce }
//@[031:0079) |   ├─ArrayExpression
  { foo: 123 }
//@[002:0014) |   | ├─ObjectExpression
//@[004:0012) |   | | └─ObjectPropertyExpression
//@[004:0007) |   | |   ├─StringLiteralExpression { Value = foo }
//@[009:0012) |   | |   └─IntegerLiteralExpression { Value = 123 }
  { bar: 456 }
//@[002:0014) |   | ├─ObjectExpression
//@[004:0012) |   | | └─ObjectPropertyExpression
//@[004:0007) |   | |   ├─StringLiteralExpression { Value = bar }
//@[009:0012) |   | |   └─IntegerLiteralExpression { Value = 456 }
  { baz: 789 }
//@[002:0014) |   | └─ObjectExpression
//@[004:0012) |   |   └─ObjectPropertyExpression
//@[004:0007) |   |     ├─StringLiteralExpression { Value = baz }
//@[009:0012) |   |     └─IntegerLiteralExpression { Value = 789 }
], {}, (cur, next) => union(cur, next))
//@[003:0005) |   ├─ObjectExpression
//@[007:0038) |   └─LambdaExpression
//@[022:0038) |     └─FunctionCallExpression { Name = union }
//@[028:0031) |       ├─LambdaVariableReferenceExpression { Variable = cur }
//@[033:0037) |       └─LambdaVariableReferenceExpression { Variable = next }
var reduceEmpty = reduce([], 0, (cur, next) => cur)
//@[000:0051) ├─DeclaredVariableExpression { Name = reduceEmpty }
//@[018:0051) | └─FunctionCallExpression { Name = reduce }
//@[025:0027) |   ├─ArrayExpression
//@[029:0030) |   ├─IntegerLiteralExpression { Value = 0 }
//@[032:0050) |   └─LambdaExpression
//@[047:0050) |     └─LambdaVariableReferenceExpression { Variable = cur }

var itemForLoop = [for item in range(0, 10): item]
//@[000:0050) ├─DeclaredVariableExpression { Name = itemForLoop }
//@[018:0050) | └─ForLoopExpression
//@[031:0043) |   ├─FunctionCallExpression { Name = range }
//@[037:0038) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[040:0042) |   | └─IntegerLiteralExpression { Value = 10 }
//@[045:0049) |   └─ArrayAccessExpression
//@[045:0049) |     ├─CopyIndexExpression
//@[031:0043) |     └─FunctionCallExpression { Name = range }
//@[037:0038) |       ├─IntegerLiteralExpression { Value = 0 }
//@[040:0042) |       └─IntegerLiteralExpression { Value = 10 }
var filteredLoop = filter(itemForLoop, i => i > 5)
//@[000:0050) ├─DeclaredVariableExpression { Name = filteredLoop }
//@[019:0050) | └─FunctionCallExpression { Name = filter }
//@[026:0037) |   ├─VariableReferenceExpression { Variable = itemForLoop }
//@[039:0049) |   └─LambdaExpression
//@[044:0049) |     └─BinaryExpression { Operator = GreaterThan }
//@[044:0045) |       ├─LambdaVariableReferenceExpression { Variable = i }
//@[048:0049) |       └─IntegerLiteralExpression { Value = 5 }

output doggoGreetings array = [for item in mapObject: item.greeting]
//@[000:0068) └─DeclaredOutputExpression { Name = doggoGreetings }
//@[030:0068)   └─ForLoopExpression
//@[043:0052)     ├─VariableReferenceExpression { Variable = mapObject }
//@[054:0067)     └─PropertyAccessExpression { PropertyName = greeting }
//@[054:0058)       └─ArrayAccessExpression
//@[054:0058)         ├─CopyIndexExpression
//@[043:0052)         └─VariableReferenceExpression { Variable = mapObject }

resource storageAcc 'Microsoft.Storage/storageAccounts@2021-09-01' existing = {
//@[000:0100) ├─DeclaredResourceExpression
//@[078:0100) | └─ObjectExpression
  name: 'asdfsadf'
}
var mappedResProps = map(items(storageAcc.properties.secondaryEndpoints), item => item.value)

module myMod './test.bicep' = {
//@[000:0117) ├─DeclaredModuleExpression
//@[030:0117) | ├─ObjectExpression
  name: 'asdfsadf'
//@[002:0018) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0018) | |   └─StringLiteralExpression { Value = asdfsadf }
  params: {
//@[010:0064) | └─ObjectExpression
    outputThis: map(mapObject, obj => obj.doggo)
//@[004:0048) |   └─ObjectPropertyExpression
//@[004:0014) |     ├─StringLiteralExpression { Value = outputThis }
//@[016:0048) |     └─FunctionCallExpression { Name = map }
//@[020:0029) |       ├─VariableReferenceExpression { Variable = mapObject }
//@[031:0047) |       └─LambdaExpression
//@[038:0047) |         └─PropertyAccessExpression { PropertyName = doggo }
//@[038:0041) |           └─LambdaVariableReferenceExpression { Variable = obj }
  }
}
var mappedModOutputProps = map(myMod.outputs.outputThis, doggo => '${doggo} says bork')

var parentheses = map([123], (i => '${i}'))
//@[000:0043) ├─DeclaredVariableExpression { Name = parentheses }
//@[018:0043) | └─FunctionCallExpression { Name = map }
//@[022:0027) |   ├─ArrayExpression
//@[023:0026) |   | └─IntegerLiteralExpression { Value = 123 }
//@[030:0041) |   └─LambdaExpression
//@[035:0041) |     └─InterpolatedStringExpression
//@[038:0039) |       └─LambdaVariableReferenceExpression { Variable = i }

var objectMap = toObject([123, 456, 789], i => '${i / 100}')
//@[000:0060) ├─DeclaredVariableExpression { Name = objectMap }
//@[016:0060) | └─FunctionCallExpression { Name = toObject }
//@[025:0040) |   ├─ArrayExpression
//@[026:0029) |   | ├─IntegerLiteralExpression { Value = 123 }
//@[031:0034) |   | ├─IntegerLiteralExpression { Value = 456 }
//@[036:0039) |   | └─IntegerLiteralExpression { Value = 789 }
//@[042:0059) |   └─LambdaExpression
//@[047:0059) |     └─InterpolatedStringExpression
//@[050:0057) |       └─BinaryExpression { Operator = Divide }
//@[050:0051) |         ├─LambdaVariableReferenceExpression { Variable = i }
//@[054:0057) |         └─IntegerLiteralExpression { Value = 100 }
var objectMap2 = toObject(range(0, 10), i => '${i}', i => {
//@[000:0111) ├─DeclaredVariableExpression { Name = objectMap2 }
//@[017:0111) | └─FunctionCallExpression { Name = toObject }
//@[026:0038) |   ├─FunctionCallExpression { Name = range }
//@[032:0033) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[035:0037) |   | └─IntegerLiteralExpression { Value = 10 }
//@[040:0051) |   ├─LambdaExpression
//@[045:0051) |   | └─InterpolatedStringExpression
//@[048:0049) |   |   └─LambdaVariableReferenceExpression { Variable = i }
//@[053:0110) |   └─LambdaExpression
//@[058:0110) |     └─ObjectExpression
  isEven: (i % 2) == 0
//@[002:0022) |       ├─ObjectPropertyExpression
//@[002:0008) |       | ├─StringLiteralExpression { Value = isEven }
//@[010:0022) |       | └─BinaryExpression { Operator = Equals }
//@[011:0016) |       |   ├─BinaryExpression { Operator = Modulo }
//@[011:0012) |       |   | ├─LambdaVariableReferenceExpression { Variable = i }
//@[015:0016) |       |   | └─IntegerLiteralExpression { Value = 2 }
//@[021:0022) |       |   └─IntegerLiteralExpression { Value = 0 }
  isGreaterThan4: (i > 4)
//@[002:0025) |       └─ObjectPropertyExpression
//@[002:0016) |         ├─StringLiteralExpression { Value = isGreaterThan4 }
//@[019:0024) |         └─BinaryExpression { Operator = GreaterThan }
//@[019:0020) |           ├─LambdaVariableReferenceExpression { Variable = i }
//@[023:0024) |           └─IntegerLiteralExpression { Value = 4 }
})
var objectMap3 = toObject(sortByObjectKey, x => x.name)
//@[000:0055) ├─DeclaredVariableExpression { Name = objectMap3 }
//@[017:0055) | └─FunctionCallExpression { Name = toObject }
//@[026:0041) |   ├─VariableReferenceExpression { Variable = sortByObjectKey }
//@[043:0054) |   └─LambdaExpression
//@[048:0054) |     └─PropertyAccessExpression { PropertyName = name }
//@[048:0049) |       └─LambdaVariableReferenceExpression { Variable = x }
var objectMap4 = toObject(sortByObjectKey, x =>
//@[000:0060) ├─DeclaredVariableExpression { Name = objectMap4 }
//@[017:0060) | └─FunctionCallExpression { Name = toObject }
//@[026:0041) |   ├─VariableReferenceExpression { Variable = sortByObjectKey }
//@[043:0059) |   └─LambdaExpression
  
  x.name)
//@[002:0008) |     └─PropertyAccessExpression { PropertyName = name }
//@[002:0003) |       └─LambdaVariableReferenceExpression { Variable = x }
var objectMap5 = toObject(sortByObjectKey, xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx => xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.name)
//@[000:0129) ├─DeclaredVariableExpression { Name = objectMap5 }
//@[017:0129) | └─FunctionCallExpression { Name = toObject }
//@[026:0041) |   ├─VariableReferenceExpression { Variable = sortByObjectKey }
//@[043:0128) |   └─LambdaExpression
//@[085:0128) |     └─PropertyAccessExpression { PropertyName = name }
//@[085:0123) |       └─LambdaVariableReferenceExpression { Variable = xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx }
var objectMap6 = toObject(range(0, 10), i => '${i}', i => // comment
//@[000:0122) ├─DeclaredVariableExpression { Name = objectMap6 }
//@[017:0122) | └─FunctionCallExpression { Name = toObject }
//@[026:0038) |   ├─FunctionCallExpression { Name = range }
//@[032:0033) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[035:0037) |   | └─IntegerLiteralExpression { Value = 10 }
//@[040:0051) |   ├─LambdaExpression
//@[045:0051) |   | └─InterpolatedStringExpression
//@[048:0049) |   |   └─LambdaVariableReferenceExpression { Variable = i }
//@[053:0121) |   └─LambdaExpression
{
//@[000:0052) |     └─ObjectExpression
  isEven: (i % 2) == 0
//@[002:0022) |       ├─ObjectPropertyExpression
//@[002:0008) |       | ├─StringLiteralExpression { Value = isEven }
//@[010:0022) |       | └─BinaryExpression { Operator = Equals }
//@[011:0016) |       |   ├─BinaryExpression { Operator = Modulo }
//@[011:0012) |       |   | ├─LambdaVariableReferenceExpression { Variable = i }
//@[015:0016) |       |   | └─IntegerLiteralExpression { Value = 2 }
//@[021:0022) |       |   └─IntegerLiteralExpression { Value = 0 }
  isGreaterThan4: (i > 4)
//@[002:0025) |       └─ObjectPropertyExpression
//@[002:0016) |         ├─StringLiteralExpression { Value = isGreaterThan4 }
//@[019:0024) |         └─BinaryExpression { Operator = GreaterThan }
//@[019:0020) |           ├─LambdaVariableReferenceExpression { Variable = i }
//@[023:0024) |           └─IntegerLiteralExpression { Value = 4 }
})

