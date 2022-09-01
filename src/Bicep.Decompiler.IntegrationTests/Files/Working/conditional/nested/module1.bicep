param foo string
//@[6:9) [no-unused-params (Warning)] Parameter "foo" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |foo|
param bar object
//@[6:9) [no-unused-params (Warning)] Parameter "bar" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |bar|
param baz array
//@[6:9) [no-unused-params (Warning)] Parameter "baz" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |baz|
param qux string = 'xyzzy'
//@[6:9) [no-unused-params (Warning)] Parameter "qux" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |qux|
