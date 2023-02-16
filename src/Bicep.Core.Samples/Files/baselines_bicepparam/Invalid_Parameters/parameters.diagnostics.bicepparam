using './main.bicep'
//@[06:20) [BCP258 (Error)] The following parameters are declared in the Bicep file but are missing an assignment in the params file: "myString", "myInt", "myBool", "password", "secretObject", "storageSku", "storageName", "someArray", "emptyMetadata", "description", "description2", "additionalMetadata", "someParameter", "stringLiteral", "decoratedString". (CodeDescription: none) |'./main.bicep'|

param para1 = 'value
//@[00:20) [BCP259 (Error)] The parameter "para1" is assigned in the params file without being declared in the Bicep file. (CodeDescription: none) |param para1 = 'value|
//@[14:20) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (CodeDescription: none) |'value|

para
//@[00:04) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |para|

para2
//@[00:05) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |para2|
