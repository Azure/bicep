
// wrong declaration
bad

// incomplete
output 
//@[7:7) Output <missing>. Type: any. Declaration start char: 0, length: 7

output foo
//@[7:10) Output foo. Type: any. Declaration start char: 0, length: 10

// space after identifier #completionTest(20) -> outputTypes
output spaceAfterId 
//@[7:19) Output spaceAfterId. Type: any. Declaration start char: 0, length: 20

// partial type #completionTest(19, 20, 21, 22) -> outputTypes
output partialType obj
//@[7:18) Output partialType. Type: error. Declaration start char: 0, length: 22

// malformed identifier
output 2
//@[7:8) Output <error>. Type: any. Declaration start char: 0, length: 8

// malformed type
output malformedType 3
//@[7:20) Output malformedType. Type: any. Declaration start char: 0, length: 22

// malformed type but type check should still happen
output malformedType2 3 = 2 + null
//@[7:21) Output malformedType2. Type: any. Declaration start char: 0, length: 34

// malformed type assignment
output malformedAssignment 2 = 2
//@[7:26) Output malformedAssignment. Type: any. Declaration start char: 0, length: 32

// malformed type before assignment
output lol 2 = true
//@[7:10) Output lol. Type: any. Declaration start char: 0, length: 19

// wrong type + missing value
output foo fluffy
//@[7:10) Output foo. Type: error. Declaration start char: 0, length: 17

// missing value
output foo string
//@[7:10) Output foo. Type: string. Declaration start char: 0, length: 17

// missing value
output foo string =
//@[7:10) Output foo. Type: string. Declaration start char: 0, length: 19

// wrong string output values
output str string = true
//@[7:10) Output str. Type: string. Declaration start char: 0, length: 24
output str string = false
//@[7:10) Output str. Type: string. Declaration start char: 0, length: 25
output str string = [
//@[7:10) Output str. Type: string. Declaration start char: 0, length: 24
]
output str string = {
//@[7:10) Output str. Type: string. Declaration start char: 0, length: 24
}
output str string = 52
//@[7:10) Output str. Type: string. Declaration start char: 0, length: 22

// wrong int output values
output i int = true
//@[7:8) Output i. Type: int. Declaration start char: 0, length: 19
output i int = false
//@[7:8) Output i. Type: int. Declaration start char: 0, length: 20
output i int = [
//@[7:8) Output i. Type: int. Declaration start char: 0, length: 19
]
output i int = }
//@[7:8) Output i. Type: int. Declaration start char: 0, length: 16
}
output i int = 'test'
//@[7:8) Output i. Type: int. Declaration start char: 0, length: 21

// wrong bool output values
output b bool = [
//@[7:8) Output b. Type: bool. Declaration start char: 0, length: 20
]
output b bool = {
//@[7:8) Output b. Type: bool. Declaration start char: 0, length: 20
}
output b bool = 32
//@[7:8) Output b. Type: bool. Declaration start char: 0, length: 18
output b bool = 'str'
//@[7:8) Output b. Type: bool. Declaration start char: 0, length: 21

// wrong array output values
output arr array = 32
//@[7:10) Output arr. Type: array. Declaration start char: 0, length: 21
output arr array = true
//@[7:10) Output arr. Type: array. Declaration start char: 0, length: 23
output arr array = false
//@[7:10) Output arr. Type: array. Declaration start char: 0, length: 24
output arr array = {
//@[7:10) Output arr. Type: array. Declaration start char: 0, length: 23
}
output arr array = 'str'
//@[7:10) Output arr. Type: array. Declaration start char: 0, length: 24

// wrong object output values
output o object = 32
//@[7:8) Output o. Type: object. Declaration start char: 0, length: 20
output o object = true
//@[7:8) Output o. Type: object. Declaration start char: 0, length: 22
output o object = false
//@[7:8) Output o. Type: object. Declaration start char: 0, length: 23
output o object = [
//@[7:8) Output o. Type: object. Declaration start char: 0, length: 22
]
output o object = 'str'
//@[7:8) Output o. Type: object. Declaration start char: 0, length: 23

// a few expression cases
output exp string = 2 + 3
//@[7:10) Output exp. Type: string. Declaration start char: 0, length: 25
output union string = true ? 's' : 1
//@[7:12) Output union. Type: string. Declaration start char: 0, length: 36
output bad int = true && !4
//@[7:10) Output bad. Type: int. Declaration start char: 0, length: 27
output deeper bool = true ? -true : (14 && 's') + 10
//@[7:13) Output deeper. Type: bool. Declaration start char: 0, length: 52

