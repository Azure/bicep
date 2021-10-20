// $1 = policyDefinition
// $2 = 'name'
// $3 = 'displayName'
// $4 = 'Custom'
// $5 = 'All'
// $6 = 'description'
// $7 = '0.1.0'
// $8 = 'category'
// $9 = 'source'
// $10 = parameterName
// $11 = 'String'
// $12 = 'defaultValue'
// $13 = 'displayName'
// $14 = 'description'
// $15 = allOf
// $16 = 'field'
// $17 = equals
// $18 = 'conditionValue'
// $19 = 'deployIfNotExists'
// $20 = '/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c'
// $21 = 'type'
// $22 = 'field'
// $23 = equals
// $24 = 'conditionValue'
// $25 = 'incremental'
// $26 = 'https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
// $27 = '1.0.0.0'
// $28 = parameterName
// $29 = 'String'
// $30 = 'displayName'
// $31 = 'description'
// $32 = 'name'
// $33 = 'type'
// $34 = 'apiVersion'
// $35 = parameterName
// $36 = 'value'

targetScope = 'subscription'
param location string
//@[6:14) [no-unused-params (Warning)] Parameter "location" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |location|

resource policyDefinition policyDefinition 'Microsoft.Authorization/policyDefinitions@2020-09-01' = {
//@[26:42) [BCP068 (Error)] Expected a resource type string. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |policyDefinition|
//@[26:101) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |policyDefinition 'Microsoft.Authorization/policyDefinitions@2020-09-01' = {|
//@[101:101) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||
  name: 'name' 'name'
//@[2:6) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |name|
  properties: {
//@[2:12) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |properties|
    displayName: 'displayName' 'displayName'
//@[4:15) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |displayName|
    policyType: 'Custom' 'Custom'
//@[4:14) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |policyType|
    mode: 'All' 'All'
//@[4:8) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |mode|
    description: 'description' 'description'
//@[4:15) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |description|
    metadata: {
//@[4:12) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |metadata|
      version: '0.1.0' '0.1.0'
//@[6:13) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |version|
      category: 'category' 'category'
//@[6:14) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |category|
      source: 'source' 'source'
//@[6:12) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |source|
    }
//@[4:5) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
    parameters: {
//@[4:14) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |parameters|
      parameterName: {
//@[6:19) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |parameterName|
        type: 'String' 'String'
//@[8:12) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |type|
        defaultValue: 'defaultValue' 'defaultValue'
//@[8:20) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |defaultValue|
        metadata: {
//@[8:16) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |metadata|
          displayName: 'displayName' 'displayName'
//@[10:21) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |displayName|
          description: 'description' 'description'
//@[10:21) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |description|
        }
//@[8:9) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
      }
//@[6:7) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
    }
//@[4:5) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
    policyRule: {
//@[4:14) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |policyRule|
      if: {
//@[6:8) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |if|
        allOf: [
//@[8:13) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |allOf|
          {
//@[10:11) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |{|
            field: 'field' 'field'
//@[12:17) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |field|
            equals: 'conditionValue' 'conditionValue'
//@[12:18) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |equals|
          }
//@[10:11) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
        ]
//@[8:9) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |]|
      }
//@[6:7) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
      then: {
//@[6:10) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |then|
        effect: 'deployIfNotExists' 'Audit'
//@[8:14) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |effect|
        details: {
//@[8:15) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |details|
          roleDefinitionIds: [
//@[10:27) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |roleDefinitionIds|
            '/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c'
//@[12:101) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |'/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c'|
          ]
//@[10:11) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |]|
          type: 'type' 'type'
//@[10:14) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |type|
          existenceCondition: {
//@[10:28) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |existenceCondition|
            allOf: [
//@[12:17) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |allOf|
              {
//@[14:15) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |{|
                field: 'field' 'field'
//@[16:21) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |field|
                equals: 'conditionValue' 'conditionValue'
//@[16:22) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |equals|
              }
//@[14:15) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
            ]
//@[12:13) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |]|
          }
//@[10:11) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
          deployment: {
//@[10:20) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |deployment|
            properties: {
//@[12:22) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |properties|
              mode: 'incremental' 'incremental'
//@[14:18) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |mode|
              template: {
//@[14:22) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |template|
                '$schema': 'https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#' 'schema'
//@[16:25) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |'$schema'|
                contentVersion: '1.0.0.0' '1.0.0.0'
//@[16:30) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |contentVersion|
                parameters: {
//@[16:26) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |parameters|
                  parameterName: {
//@[18:31) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |parameterName|
                    type: 'String' 'String'
//@[20:24) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |type|
                    metadata: {
//@[20:28) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |metadata|
                      displayName: 'displayName' 'displayName'
//@[22:33) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |displayName|
                      description: 'description' 'description'
//@[22:33) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |description|
                    }
//@[20:21) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
                  }
//@[18:19) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
                }
//@[16:17) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
                variables: {}
//@[16:25) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |variables|
                resources: [
//@[16:25) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |resources|
                  {
//@[18:19) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |{|
                    name: 'name' 'name'
//@[20:24) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |name|
                    type: 'type' 'type'
//@[20:24) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |type|
                    apiVersion: 'apiVersion' 'apiVersion'
//@[20:30) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |apiVersion|
                    location: parameterName 'location'
//@[20:28) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |location|
                    properties: {}
//@[20:30) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |properties|
                  }
//@[18:19) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
                ]
//@[16:17) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |]|
              }
//@[14:15) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
              parameters: {
//@[14:24) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |parameters|
                'value': {
//@[16:23) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |'value'|
                  value:  'value'
//@[18:23) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |value|
                }
//@[16:17) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
              }
//@[14:15) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
            }
//@[12:13) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
          }
//@[10:11) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
        }
//@[8:9) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
      }
//@[6:7) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
    }
//@[4:5) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
  }
//@[2:3) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
}
//@[0:1) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
// Insert snippet here

