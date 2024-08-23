var outer = 'hello!'

module testnested './nested_testnested.bicep' = {
//@[18:45) [BCP104 (Error)] The referenced module has errors. (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP104)) |'./nested_testnested.bicep'|
  name: 'testnested'
  params: {
    variables_outer: outer
  }
}

