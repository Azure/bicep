{
  "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "16411227777067905793"
    }
  },
  "variables": {
    "websites": [
      {
        "name": "fancy",
        "tag": "latest"
      },
      {
        "name": "plain",
        "tag": "plain-text"
      }
    ],
    "vnets": [
      {
        "name": "vnet1",
        "subnetName": "subnet1.1"
      },
      {
        "name": "vnet2",
        "subnetName": "subnet2.1"
      }
    ]
  },
  "resources": [
    {
      "type": "Microsoft.Resources/resourceGroups",
      "apiVersion": "2020-06-01",
      "name": "adotfrank-rg",
      "location": "[deployment().location]"
    },
    {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "planDeploy",
      "resourceGroup": "adotfrank-rg",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "namePrefix": {
            "value": "hello"
          }
        },
        "templateLink": {
          "relativePath": "../restored/br/mock-registry-one.invalid/demo$plan/v2$/main.json"
        }
      },
      "dependsOn": [
        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
      ]
    },
    {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "planDeploy2",
      "resourceGroup": "adotfrank-rg",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "namePrefix": {
            "value": "hello"
          }
        },
        "templateLink": {
          "relativePath": "../restored/br/mock-registry-one.invalid/demo$plan/v2$/main.json"
        }
      },
      "dependsOn": [
        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
      ]
    },
    {
      "copy": {
        "name": "siteDeploy",
        "count": "[length(variables('websites'))]"
      },
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "[format('{0}siteDeploy', variables('websites')[copyIndex()].name)]",
      "resourceGroup": "adotfrank-rg",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "appPlanId": {
            "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'adotfrank-rg'), 'Microsoft.Resources/deployments', 'planDeploy'), '2025-04-01').outputs.planId.value]"
          },
          "namePrefix": {
            "value": "[variables('websites')[copyIndex()].name]"
          },
          "dockerImage": {
            "value": "nginxdemos/hello"
          },
          "dockerImageTag": {
            "value": "[variables('websites')[copyIndex()].tag]"
          }
        },
        "templateLink": {
          "relativePath": "../restored/br/mock-registry-two.invalid/demo$site/v3$/main.json"
        }
      },
      "dependsOn": [
        "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'adotfrank-rg'), 'Microsoft.Resources/deployments', 'planDeploy')]",
        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
      ]
    },
    {
      "copy": {
        "name": "siteDeploy2",
        "count": "[length(variables('websites'))]"
      },
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "[format('{0}siteDeploy2', variables('websites')[copyIndex()].name)]",
      "resourceGroup": "adotfrank-rg",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "appPlanId": {
            "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'adotfrank-rg'), 'Microsoft.Resources/deployments', 'planDeploy'), '2025-04-01').outputs.planId.value]"
          },
          "namePrefix": {
            "value": "[variables('websites')[copyIndex()].name]"
          },
          "dockerImage": {
            "value": "nginxdemos/hello"
          },
          "dockerImageTag": {
            "value": "[variables('websites')[copyIndex()].tag]"
          }
        },
        "templateLink": {
          "relativePath": "../restored/br/mock-registry-two.invalid/demo$site/v3$/main.json"
        }
      },
      "dependsOn": [
        "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'adotfrank-rg'), 'Microsoft.Resources/deployments', 'planDeploy')]",
        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
      ]
    },
    {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "storageDeploy",
      "resourceGroup": "adotfrank-rg",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "location": {
            "value": "eastus"
          }
        },
        "templateLink": {
          "id": "/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/test-rg/providers/Microsoft.Resources/templateSpecs/storage-spec/versions/1.0"
        }
      },
      "dependsOn": [
        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
      ]
    },
    {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "storageDeploy2",
      "resourceGroup": "adotfrank-rg",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "location": {
            "value": "eastus"
          }
        },
        "templateLink": {
          "id": "/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/test-rg/providers/Microsoft.Resources/templateSpecs/storage-spec/versions/1.0"
        }
      },
      "dependsOn": [
        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
      ]
    },
    {
      "copy": {
        "name": "vnetDeploy",
        "count": "[length(variables('vnets'))]"
      },
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "[format('{0}Deploy', variables('vnets')[copyIndex()].name)]",
      "resourceGroup": "adotfrank-rg",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "vnetName": {
            "value": "[variables('vnets')[copyIndex()].name]"
          },
          "subnetName": {
            "value": "[variables('vnets')[copyIndex()].subnetName]"
          }
        },
        "templateLink": {
          "id": "/subscriptions/11111111-1111-1111-1111-111111111111/resourceGroups/prod-rg/providers/Microsoft.Resources/templateSpecs/vnet-spec/versions/v2"
        }
      },
      "dependsOn": [
        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
      ]
    },
    {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "port",
      "resourceGroup": "adotfrank-rg",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "port": {
            "value": "test"
          }
        },
        "templateLink": {
          "relativePath": "../restored/br/localhost$5000/passthrough$port/v1$/main.json"
        }
      },
      "dependsOn": [
        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
      ]
    },
    {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "ipv4",
      "resourceGroup": "adotfrank-rg",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "ipv4": {
            "value": "test"
          }
        },
        "templateLink": {
          "relativePath": "../restored/br/127.0.0.1/passthrough$ipv4/v1$/main.json"
        }
      },
      "dependsOn": [
        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
      ]
    },
    {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "ipv4port",
      "resourceGroup": "adotfrank-rg",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "ipv4port": {
            "value": "test"
          }
        },
        "templateLink": {
          "relativePath": "../restored/br/127.0.0.1$5000/passthrough$ipv4port/v1$/main.json"
        }
      },
      "dependsOn": [
        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
      ]
    },
    {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "ipv6",
      "resourceGroup": "adotfrank-rg",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "ipv6": {
            "value": "test"
          }
        },
        "templateLink": {
          "relativePath": "../restored/br/[$$1]/passthrough$ipv6/v1$/main.json"
        }
      },
      "dependsOn": [
        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
      ]
    },
    {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "ipv6port",
      "resourceGroup": "adotfrank-rg",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "ipv6port": {
            "value": "test"
          }
        },
        "templateLink": {
          "relativePath": "../restored/br/[$$1]$5000/passthrough$ipv6port/v1$/main.json"
        }
      },
      "dependsOn": [
        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
      ]
    }
  ],
  "outputs": {
    "siteUrls": {
      "type": "array",
      "copy": {
        "count": "[length(variables('websites'))]",
        "input": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'adotfrank-rg'), 'Microsoft.Resources/deployments', format('{0}siteDeploy', variables('websites')[copyIndex()].name)), '2025-04-01').outputs.siteUrl.value]"
      }
    }
  }
}