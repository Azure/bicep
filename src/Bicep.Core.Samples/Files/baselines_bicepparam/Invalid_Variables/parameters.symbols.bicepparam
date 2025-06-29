using 'main.bicep'

var foo
//@[4:7) Variable foo. Type: error. Declaration start char: 0, length: 7

var foo2 =
//@[4:8) Variable foo2. Type: error. Declaration start char: 0, length: 10

var foo3 = asdf
//@[4:8) Variable foo3. Type: error. Declaration start char: 0, length: 15

var foo4 = utcNow()
//@[4:8) Variable foo4. Type: error. Declaration start char: 0, length: 19

