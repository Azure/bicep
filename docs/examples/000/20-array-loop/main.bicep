//Array look samples
var bicepVarArray = [
  'Michael'
  'Dwight'
  'Jim'
  'Pam'
]

//output the iteration item
output out1 array = [for i in bicepVarArray: {
  element: i
}]

//output the renamed iteration item
output out2 array = [for (name, i) in bicepVarArray: {
  element: name
}]

//output the  original array element by index
output out3 array = [for (name, i) in bicepVarArray: {
  element: bicepVarArray[i]
}]
