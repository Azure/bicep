var outer = 'hello!'

module testnested './nested_testnested.bicep' = {
//@[18:45) [BCP104 (Error)] The referenced module has errors. |'./nested_testnested.bicep'|
  name: 'testnested'
  params: {
    variables_outer: outer
  }
}
