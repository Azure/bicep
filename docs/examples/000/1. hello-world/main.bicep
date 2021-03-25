//Sample hello world
//Deploy with:
//PS:
//    New-AzResourceGroupDeployment -ResourceGroupName myRG -TemplateFile .\main.bicep
//AZ CLI:
//    az deployment group create --resource-group myRG --template-file ./main.bicep
//
param yourName string
var hello = 'Hello World! - Hi'

output helloWorld string = '${hello} ${yourName}'
