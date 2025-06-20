param foo string
//@[6:9) [no-unused-params (Warning)] Parameter "foo" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |foo|
param bar object
//@[6:9) [no-unused-params (Warning)] Parameter "bar" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |bar|
param baz array
//@[6:9) [no-unused-params (Warning)] Parameter "baz" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |baz|
param qux string = 'xyzzy'
//@[6:9) [no-unused-params (Warning)] Parameter "qux" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |qux|

