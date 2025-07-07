metadata other = 'other metadata'

@description('eventHubNameSpace description')
param eventHubNameSpace string
param eventHubName string
param eventHubLocation string

output output1 string = concat(eventHubLocation, eventHubName, eventHubNameSpace)
//@[24:81) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(eventHubLocation, eventHubName, eventHubNameSpace)|

