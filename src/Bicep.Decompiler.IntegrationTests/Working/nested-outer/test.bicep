var outer = 'hello!'

module testnested './nested_testnested.bicep' = {
//@[18:45) [BCP104 (Error)] The referenced module has errors. (CodeDescription: none) |'./nested_testnested.bicep'|
  name: 'testnested'
  params: {
    variables_outer: outer
  }
}
