//Multiline string samples
var list = 'a,b,c,d'
var arrayFromString = split(list, ',')

var find = 'findThisInString'
var found = contains(find, 'This')
var index = indexOf(find, 'This')
var indexNotFound = indexOf(find, 'NotFound')
var len = length(find)
var substr = substring(find, index, (len - index))
//var substrErr = substring( find, index,  15 )  //ERROR - substring cannot return more chars then the string has

output arrayFromString array = [for i in arrayFromString: {
  element: i
}]

output found string = found == true ? 'Found "this"' : 'Did not find "This"'
output index int = index
output indexNotFound int = indexNotFound

output substr string = substr
