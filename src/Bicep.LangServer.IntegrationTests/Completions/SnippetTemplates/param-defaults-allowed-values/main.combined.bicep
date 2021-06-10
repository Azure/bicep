// $1 = testParam
// $2 = int
// $3 = 1
// $4 = 2

@allowed([
  1
])
param testParam int = 2// Insert snippet here
//@[6:15) [no-unused-params (Warning)] Parameter "testParam" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |testParam|

