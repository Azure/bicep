using './main.bicep'
//@[06:20) [BCP258 (Error)] The following parameters are declared in the Bicep file but are missing an assignment in the params file: "myString", "myInt", "myBool", "password", "secretObject", "storageSku", "storageName", "someArray", "emptyMetadata", "description", "description2", "additionalMetadata", "someParameter", "stringLiteral", "decoratedString". (CodeDescription: none) |'./main.bicep'|

param para1 = 'value
//@[00:20) [BCP259 (Error)] The parameter "para1" is assigned in the params file without being declared in the Bicep file. (CodeDescription: none) |param para1 = 'value|
//@[14:20) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (CodeDescription: none) |'value|

para
//@[00:04) [BCP337 (Error)] This declaration type is not valid for a Bicep Parameters file. Specify a "using" or "param" declaration. (CodeDescription: none) |para|

para2
//@[00:05) [BCP337 (Error)] This declaration type is not valid for a Bicep Parameters file. Specify a "using" or "param" declaration. (CodeDescription: none) |para2|

param expr = 1 + 2
//@[00:18) [BCP259 (Error)] The parameter "expr" is assigned in the params file without being declared in the Bicep file. (CodeDescription: none) |param expr = 1 + 2|
//@[13:18) [BCP338 (Error)] Complex expressions are not permitted in a Bicep Parameters file. (CodeDescription: none) |1 + 2|

param interp = 'abc${123}def'
//@[00:29) [BCP259 (Error)] The parameter "interp" is assigned in the params file without being declared in the Bicep file. (CodeDescription: none) |param interp = 'abc${123}def'|
//@[15:29) [BCP338 (Error)] Complex expressions are not permitted in a Bicep Parameters file. (CodeDescription: none) |'abc${123}def'|

param doubleinterp = 'abc${interp + 'blah'}def'
//@[00:47) [BCP259 (Error)] The parameter "doubleinterp" is assigned in the params file without being declared in the Bicep file. (CodeDescription: none) |param doubleinterp = 'abc${interp + 'blah'}def'|
//@[21:47) [BCP338 (Error)] Complex expressions are not permitted in a Bicep Parameters file. (CodeDescription: none) |'abc${interp + 'blah'}def'|

param objWithExpressions = {
//@[00:91) [BCP259 (Error)] The parameter "objWithExpressions" is assigned in the params file without being declared in the Bicep file. (CodeDescription: none) |param objWithExpressions = {\r\n  foo: 1 + 2\r\n  bar: {\r\n    baz: concat('abc', 'def')\r\n  }\r\n}|
  foo: 1 + 2
//@[07:12) [BCP338 (Error)] Complex expressions are not permitted in a Bicep Parameters file. (CodeDescription: none) |1 + 2|
  bar: {
    baz: concat('abc', 'def')
//@[09:29) [BCP338 (Error)] Complex expressions are not permitted in a Bicep Parameters file. (CodeDescription: none) |concat('abc', 'def')|
  }
}

param arrayWithExpressions = [1 + 1, 'ok']
//@[00:42) [BCP259 (Error)] The parameter "arrayWithExpressions" is assigned in the params file without being declared in the Bicep file. (CodeDescription: none) |param arrayWithExpressions = [1 + 1, 'ok']|
//@[30:35) [BCP338 (Error)] Complex expressions are not permitted in a Bicep Parameters file. (CodeDescription: none) |1 + 1|
