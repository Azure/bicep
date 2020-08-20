
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
//@[7:10] Output str
output str string = false
//@[7:10] Output str
output str string = [
//@[7:10] Output str
]
output str string = {
//@[7:10] Output str
}
output str string = 52
//@[7:10] Output str

// wrong int output values
output i int = true
//@[7:8] Output i
output i int = false
//@[7:8] Output i
output i int = [
//@[7:8] Output i
]
output i int = }
}
output i int = 'test'
//@[7:8] Output i

// wrong bool output values
output b bool = [
//@[7:8] Output b
]
output b bool = {
//@[7:8] Output b
}
output b bool = 32
//@[7:8] Output b
output b bool = 'str'
//@[7:8] Output b

// wrong array output values
output arr array = 32
//@[7:10] Output arr
output arr array = true
//@[7:10] Output arr
output arr array = false
//@[7:10] Output arr
output arr array = {
//@[7:10] Output arr
}
output arr array = 'str'
//@[7:10] Output arr

// wrong object output values
output o object = 32
//@[7:8] Output o
output o object = true
//@[7:8] Output o
output o object = false
//@[7:8] Output o
output o object = [
//@[7:8] Output o
]
output o object = 'str'
//@[7:8] Output o

// a few expression cases
output exp string = 2 + 3
//@[7:10] Output exp
output union string = true ? 's' : 1
//@[7:12] Output union
output bad int = true && !4
//@[7:10] Output bad
output deeper bool = true ? -true : (14 && 's') + 10
//@[7:13] Output deeper

