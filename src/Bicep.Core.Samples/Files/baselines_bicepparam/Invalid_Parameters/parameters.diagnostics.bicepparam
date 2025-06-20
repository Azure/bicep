using './main.bicep'
//@[06:20) [BCP258 (Error)] The following parameters are declared in the Bicep file but are missing an assignment in the params file: "additionalMetadata", "decoratedString", "description", "description2", "emptyMetadata", "myBool", "myInt", "myString", "password", "secretObject", "someArray", "someParameter", "storageName", "storageSku", "stringLiteral". (bicep https://aka.ms/bicep/core-diagnostics#BCP258) |'./main.bicep'|
//@[06:20) [BCP104 (Error)] The referenced module has errors. (bicep https://aka.ms/bicep/core-diagnostics#BCP104) |'./main.bicep'|

param para1 = 'value
//@[00:20) [BCP259 (Error)] The parameter "para1" is assigned in the params file without being declared in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP259) |param para1 = 'value|
//@[14:20) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (bicep https://aka.ms/bicep/core-diagnostics#BCP004) |'value|

para
//@[00:04) [BCP337 (Error)] This declaration type is not valid for a Bicep Parameters file. Supported declarations: "using", "extends", "param", "var", "type". (bicep https://aka.ms/bicep/core-diagnostics#BCP337) |para|

para2
//@[00:05) [BCP337 (Error)] This declaration type is not valid for a Bicep Parameters file. Supported declarations: "using", "extends", "param", "var", "type". (bicep https://aka.ms/bicep/core-diagnostics#BCP337) |para2|

param expr = 1 + 2
//@[00:18) [BCP259 (Error)] The parameter "expr" is assigned in the params file without being declared in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP259) |param expr = 1 + 2|

param interp = 'abc${123}def'
//@[00:29) [BCP259 (Error)] The parameter "interp" is assigned in the params file without being declared in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP259) |param interp = 'abc${123}def'|

param doubleinterp = 'abc${interp + 'blah'}def'
//@[00:47) [BCP259 (Error)] The parameter "doubleinterp" is assigned in the params file without being declared in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP259) |param doubleinterp = 'abc${interp + 'blah'}def'|
//@[27:42) [BCP045 (Error)] Cannot apply operator "+" to operands of type "'abc123def'" and "'blah'". Use string interpolation instead. (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |interp + 'blah'|

param objWithExpressions = {
//@[00:91) [BCP259 (Error)] The parameter "objWithExpressions" is assigned in the params file without being declared in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP259) |param objWithExpressions = {\r\n  foo: 1 + 2\r\n  bar: {\r\n    baz: concat('abc', 'def')\r\n  }\r\n}|
  foo: 1 + 2
  bar: {
    baz: concat('abc', 'def')
//@[09:29) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat('abc', 'def')|
  }
}

param arrayWithExpressions = [1 + 1, 'ok']
//@[00:42) [BCP259 (Error)] The parameter "arrayWithExpressions" is assigned in the params file without being declared in the Bicep file. (bicep https://aka.ms/bicep/core-diagnostics#BCP259) |param arrayWithExpressions = [1 + 1, 'ok']|
