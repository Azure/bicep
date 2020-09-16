
// wrong declaration
bad

// incomplete
output 

// missing type
output foo

// wrong type + missing value
output foo fluffy

// missing value
output foo string

// missing value
output foo string =

// wrong string output values
output str string = true
//@[7:10) Output str. Type: string. Declaration start char: 0, length: 26
output str string = false
//@[7:10) Output str. Type: string. Declaration start char: 0, length: 27
output str string = [
//@[7:10) Output str. Type: string. Declaration start char: 0, length: 26
]
output str string = {
//@[7:10) Output str. Type: string. Declaration start char: 0, length: 26
}
output str string = 52
//@[7:10) Output str. Type: string. Declaration start char: 0, length: 26

// wrong int output values
output i int = true
//@[7:8) Output i. Type: int. Declaration start char: 0, length: 21
output i int = false
//@[7:8) Output i. Type: int. Declaration start char: 0, length: 22
output i int = [
//@[7:8) Output i. Type: int. Declaration start char: 0, length: 21
]
output i int = }
}
output i int = 'test'
//@[7:8) Output i. Type: int. Declaration start char: 0, length: 25

// wrong bool output values
output b bool = [
//@[7:8) Output b. Type: bool. Declaration start char: 0, length: 22
]
output b bool = {
//@[7:8) Output b. Type: bool. Declaration start char: 0, length: 22
}
output b bool = 32
//@[7:8) Output b. Type: bool. Declaration start char: 0, length: 20
output b bool = 'str'
//@[7:8) Output b. Type: bool. Declaration start char: 0, length: 25

// wrong array output values
output arr array = 32
//@[7:10) Output arr. Type: array. Declaration start char: 0, length: 23
output arr array = true
//@[7:10) Output arr. Type: array. Declaration start char: 0, length: 25
output arr array = false
//@[7:10) Output arr. Type: array. Declaration start char: 0, length: 26
output arr array = {
//@[7:10) Output arr. Type: array. Declaration start char: 0, length: 25
}
output arr array = 'str'
//@[7:10) Output arr. Type: array. Declaration start char: 0, length: 28

// wrong object output values
output o object = 32
//@[7:8) Output o. Type: object. Declaration start char: 0, length: 22
output o object = true
//@[7:8) Output o. Type: object. Declaration start char: 0, length: 24
output o object = false
//@[7:8) Output o. Type: object. Declaration start char: 0, length: 25
output o object = [
//@[7:8) Output o. Type: object. Declaration start char: 0, length: 24
]
output o object = 'str'
//@[7:8) Output o. Type: object. Declaration start char: 0, length: 27

// a few expression cases
output exp string = 2 + 3
//@[7:10) Output exp. Type: string. Declaration start char: 0, length: 27
output union string = true ? 's' : 1
//@[7:12) Output union. Type: string. Declaration start char: 0, length: 38
output bad int = true && !4
//@[7:10) Output bad. Type: int. Declaration start char: 0, length: 29
output deeper bool = true ? -true : (14 && 's') + 10
//@[7:13) Output deeper. Type: bool. Declaration start char: 0, length: 54

