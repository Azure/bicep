# DeployIfNotExists Policy with Initiative and Assignment

### Deployment Summary
Resources Deployed | Bicep File
:----------|:-----
1x Resource Group | main.bicep
1x Action Group | actionGroup.bicep
1x Policy Definition with DeployIfNotExists effect for a Metric Alert v2 (Load Balancer - DipAvailability) | policyDefinition.bicep
1x Policy Initiative (policyset) | policyDefinition.bicep
1x Policy Assignment + 1x Role Assignment | policyAssignment.bicep
------------------------

### Input Summary
Parameter | Type | Default Value
:----------|:-----|:--------
resourceGroupName | string | 'BicepExampleRG'
resourceGrouplocation | string |'australiaeast'
actionGroupName | string |'BicepExampleAG'
actionGroupEnabled | bool |true
actionGroupShortName | string |'bicepag'
actionGroupEmailName | string |'jloudon'
actionGroupEmail | string |'jesse.loudon@lab3.com.au'
actionGroupAlertSchema | bool | true
metricAlertResourceNamespace | string | 'Microsoft.Network/loadBalancers'
metricAlertName | string | 'DipAvailability'
metricAlertDimension1 | string | 'ProtocolType'
metricAlertDimension2 | string | 'FrontendIPAddress'
metricAlertDimension3 | string | 'BackendIPAddress'
metricAlertDescription | string | 'Average Load Balancer health probe status per time duration'
metricAlertSeverity | string | '2'
metricAlertEnabled | string | 'true'
metricAlertEvaluationFrequency | string | 'PT15M'
metricAlertWindowSize | string |'PT1H'
metricAlertSensitivity | string | 'Medium'
metricAlertOperator | string | 'LessThan'
metricAlertTimeAggregation | string | 'Average'
metricAlertCriterionType | string | 'DynamicThresholdCriterion'
metricAlertAutoMitigate | string | 'true'
assignmentEnforcementMode | string | 'Default'
-----------------------------

Authored & Tested with:
* [azure-cli](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) version 2.20.0
* bicep cli version 0.3.1 (d0f5c9b164)
* bicep 0.3.1 vscode extension

Example Deployment steps
```
az login
az bicep build -f ./main.bicep
az deployment sub create -f ./main.bicep -l australiaeast
az policy state trigger-scan
```