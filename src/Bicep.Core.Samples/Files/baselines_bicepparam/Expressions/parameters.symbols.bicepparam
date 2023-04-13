using 'main.bicep'

param myObject = {
//@[6:14) ParameterAssignment myObject. Type: object. Declaration start char: 0, length: 1926
  any: any('foo')
  array: array([])
  base64ToString: base64ToString(base64('abc'))
  base64ToJson: base64ToJson(base64('{"hi": "there"}')).hi
  bool: bool(true)
  concat: concat(['abc'], ['def'])
  contains: contains('foo/bar', '/')
  dataUriToString: dataUriToString(dataUri('abc'))
  dateTimeAdd: dateTimeAdd(dateTimeFromEpoch(1680224438), 'P1D')  
  dateTimeToEpoch: dateTimeToEpoch(dateTimeFromEpoch(1680224438))
  empty: empty([])
  endsWith: endsWith('foo', 'o')
  filter: filter([1, 2], i => i < 2)
  first: first([124, 25])
  flatten: flatten([['abc'], ['def']])
  format: format('->{0}<-', 123)
  guid: guid('asdf')
  indexOf: indexOf('abc', 'b')
  int: int(123)
  intersection: intersection([1, 2, 3], [2, 3, 4])
  items: items({ foo: 'abc', bar: 123 })
  join: join(['abc', 'def', 'ghi'], '/')
  last: last([1, 2])
  lastIndexOf: lastIndexOf('abcba', 'b')
  length: length([0, 1, 2])
  loadFileAsBase64: loadFileAsBase64('test.txt')
  loadJsonContent: loadJsonContent('test.json')
  loadTextContent: loadTextContent('test.txt')
  map: map(range(0, 3), i => 'Hi ${i}!')
  max: max(1, 2, 3)
  min: min(1, 2, 3)
  padLeft: padLeft(13, 5)
  range: range(0, 3)
  reduce: reduce(['a', 'b', 'c'], '', (a, b) => '${a}-${b}')
  replace: replace('abc', 'b', '/')
  skip: skip([1, 2, 3], 1)
  sort: sort(['c', 'd', 'a'], (a, b) => a < b)
  split: split('a/b/c', '/')
  startsWith: startsWith('abc', 'a')
  string: string('asdf')
  substring: substring('asdfasf', 3)
  take: take([1, 2, 3], 2)
  toLower: toLower('AiKInIniIN')
  toObject: toObject(['a', 'b', 'c'], x => x, x => 'Hi ${x}!')
  toUpper: toUpper('AiKInIniIN')
  trim: trim('  adf asdf  ')
  union: union({ abc: 'def' }, { def: 'ghi' })
  uniqueString: uniqueString('asd', 'asdf', 'asdf')
  uri: uri('https://github.com', 'Azure/bicep')
  uriComponent: uriComponent('UB*8h 0+=_)9h9n')
  uriComponentToString: uriComponentToString('%20%25%20')
}

param myBool = true
//@[6:12) ParameterAssignment myBool. Type: true. Declaration start char: 0, length: 19
param myInt = sys.int(myBool ? 123 : 456)
//@[6:11) ParameterAssignment myInt. Type: int. Declaration start char: 0, length: 41

param myArray = [
//@[6:13) ParameterAssignment myArray. Type: ['a' | 'b', false, 579, 333, 6, 20, true, false, false, true]. Declaration start char: 0, length: 123
  (true ? 'a' : 'b')
  !true
  123 + 456
  456 - 123
  2 * 3
  10 / 2
  1 < 2
  1 > 2
  1 >= 2
  1 <= 2
]
param myString = '''
//@[6:14) ParameterAssignment myString. Type: 'THis\n  is\n    a\n      multiline\n        string!\n'. Declaration start char: 0, length: 72
THis
  is
    a
      multiline
        string!
'''

