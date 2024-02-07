metadata other = 'other metadata'

@description('eventHubNameSpace description')
param eventHubNameSpace string
param eventHubName string
param eventHubLocation string

output output1 string = concat(
//@[24:89) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(\n  eventHubLocation,\n  eventHubName,\n  eventHubNameSpace\n)|
  eventHubLocation,
  eventHubName,
  eventHubNameSpace
)

