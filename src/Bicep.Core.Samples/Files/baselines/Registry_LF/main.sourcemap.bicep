targetScope = 'subscription'

resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
//@    {
//@      "type": "Microsoft.Resources/resourceGroups",
//@      "apiVersion": "2020-06-01",
//@      "name": "adotfrank-rg",
//@    },
  name: 'adotfrank-rg'
  location: deployment().location
//@      "location": "[deployment().location]"
}

module appPlanDeploy 'br:mock-registry-one.invalid/demo/plan:v2' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "resourceGroup": "adotfrank-rg",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "15019246960605065046"
//@            }
//@          },
//@          "parameters": {
//@            "namePrefix": {
//@              "type": "string"
//@            },
//@            "sku": {
//@              "type": "string",
//@              "defaultValue": "B1"
//@            }
//@          },
//@          "resources": [
//@            {
//@              "type": "Microsoft.Web/serverfarms",
//@              "apiVersion": "2020-06-01",
//@              "name": "[format('{0}appPlan', parameters('namePrefix'))]",
//@              "location": "[resourceGroup().location]",
//@              "kind": "linux",
//@              "sku": {
//@                "name": "[parameters('sku')]"
//@              },
//@              "properties": {
//@                "reserved": true
//@              }
//@            }
//@          ],
//@          "outputs": {
//@            "planId": {
//@              "type": "string",
//@              "value": "[resourceId('Microsoft.Web/serverfarms', format('{0}appPlan', parameters('namePrefix')))]"
//@            }
//@          }
//@        }
//@      },
//@      "dependsOn": [
//@        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@      ]
//@    },
  name: 'planDeploy'
//@      "name": "planDeploy",
  scope: rg
  params: {
//@        "parameters": {
//@        },
    namePrefix: 'hello'
//@          "namePrefix": {
//@            "value": "hello"
//@          }
  }
}

module appPlanDeploy2 'br/mock-registry-one:demo/plan:v2' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "resourceGroup": "adotfrank-rg",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "15019246960605065046"
//@            }
//@          },
//@          "parameters": {
//@            "namePrefix": {
//@              "type": "string"
//@            },
//@            "sku": {
//@              "type": "string",
//@              "defaultValue": "B1"
//@            }
//@          },
//@          "resources": [
//@            {
//@              "type": "Microsoft.Web/serverfarms",
//@              "apiVersion": "2020-06-01",
//@              "name": "[format('{0}appPlan', parameters('namePrefix'))]",
//@              "location": "[resourceGroup().location]",
//@              "kind": "linux",
//@              "sku": {
//@                "name": "[parameters('sku')]"
//@              },
//@              "properties": {
//@                "reserved": true
//@              }
//@            }
//@          ],
//@          "outputs": {
//@            "planId": {
//@              "type": "string",
//@              "value": "[resourceId('Microsoft.Web/serverfarms', format('{0}appPlan', parameters('namePrefix')))]"
//@            }
//@          }
//@        }
//@      },
//@      "dependsOn": [
//@        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@      ]
//@    },
  name: 'planDeploy2'
//@      "name": "planDeploy2",
  scope: rg
  params: {
//@        "parameters": {
//@        },
    namePrefix: 'hello'
//@          "namePrefix": {
//@            "value": "hello"
//@          }
  }
}

var websites = [
//@    "websites": [
//@    ],
  {
//@      {
//@      },
    name: 'fancy'
//@        "name": "fancy",
    tag: 'latest'
//@        "tag": "latest"
  }
  {
//@      {
//@      }
    name: 'plain'
//@        "name": "plain",
    tag: 'plain-text'
//@        "tag": "plain-text"
  }
]

module siteDeploy 'br:mock-registry-two.invalid/demo/site:v3' = [for site in websites: {
//@    {
//@      "copy": {
//@        "name": "siteDeploy",
//@        "count": "[length(variables('websites'))]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "resourceGroup": "adotfrank-rg",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "15188988612540889945"
//@            }
//@          },
//@          "parameters": {
//@            "namePrefix": {
//@              "type": "string"
//@            },
//@            "location": {
//@              "type": "string",
//@              "defaultValue": "[resourceGroup().location]"
//@            },
//@            "dockerImage": {
//@              "type": "string"
//@            },
//@            "dockerImageTag": {
//@              "type": "string"
//@            },
//@            "appPlanId": {
//@              "type": "string"
//@            }
//@          },
//@          "resources": [
//@            {
//@              "type": "Microsoft.Web/sites",
//@              "apiVersion": "2020-06-01",
//@              "name": "[format('{0}site', parameters('namePrefix'))]",
//@              "location": "[parameters('location')]",
//@              "properties": {
//@                "siteConfig": {
//@                  "appSettings": [
//@                    {
//@                      "name": "DOCKER_REGISTRY_SERVER_URL",
//@                      "value": "https://index.docker.io"
//@                    },
//@                    {
//@                      "name": "DOCKER_REGISTRY_SERVER_USERNAME",
//@                      "value": ""
//@                    },
//@                    {
//@                      "name": "DOCKER_REGISTRY_SERVER_PASSWORD",
//@                      "value": ""
//@                    },
//@                    {
//@                      "name": "WEBSITES_ENABLE_APP_SERVICE_STORAGE",
//@                      "value": "false"
//@                    }
//@                  ],
//@                  "linuxFxVersion": "[format('DOCKER|{0}:{1}', parameters('dockerImage'), parameters('dockerImageTag'))]"
//@                },
//@                "serverFarmId": "[parameters('appPlanId')]"
//@              }
//@            }
//@          ],
//@          "outputs": {
//@            "siteUrl": {
//@              "type": "string",
//@              "value": "[reference(resourceId('Microsoft.Web/sites', format('{0}site', parameters('namePrefix'))), '2020-06-01').hostNames[0]]"
//@            }
//@          }
//@        }
//@      },
//@      "dependsOn": [
//@        "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'adotfrank-rg'), 'Microsoft.Resources/deployments', 'planDeploy')]",
//@        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@      ]
//@    },
  name: '${site.name}siteDeploy'
//@      "name": "[format('{0}siteDeploy', variables('websites')[copyIndex()].name)]",
  scope: rg
  params: {
//@        "parameters": {
//@        },
    appPlanId: appPlanDeploy.outputs.planId
//@          "appPlanId": {
//@            "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'adotfrank-rg'), 'Microsoft.Resources/deployments', 'planDeploy'), '2025-04-01').outputs.planId.value]"
//@          },
    namePrefix: site.name
//@          "namePrefix": {
//@            "value": "[variables('websites')[copyIndex()].name]"
//@          },
    dockerImage: 'nginxdemos/hello'
//@          "dockerImage": {
//@            "value": "nginxdemos/hello"
//@          },
    dockerImageTag: site.tag
//@          "dockerImageTag": {
//@            "value": "[variables('websites')[copyIndex()].tag]"
//@          }
  }
}]

module siteDeploy2 'br/demo-two:site:v3' = [for site in websites: {
//@    {
//@      "copy": {
//@        "name": "siteDeploy2",
//@        "count": "[length(variables('websites'))]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "resourceGroup": "adotfrank-rg",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "15188988612540889945"
//@            }
//@          },
//@          "parameters": {
//@            "namePrefix": {
//@              "type": "string"
//@            },
//@            "location": {
//@              "type": "string",
//@              "defaultValue": "[resourceGroup().location]"
//@            },
//@            "dockerImage": {
//@              "type": "string"
//@            },
//@            "dockerImageTag": {
//@              "type": "string"
//@            },
//@            "appPlanId": {
//@              "type": "string"
//@            }
//@          },
//@          "resources": [
//@            {
//@              "type": "Microsoft.Web/sites",
//@              "apiVersion": "2020-06-01",
//@              "name": "[format('{0}site', parameters('namePrefix'))]",
//@              "location": "[parameters('location')]",
//@              "properties": {
//@                "siteConfig": {
//@                  "appSettings": [
//@                    {
//@                      "name": "DOCKER_REGISTRY_SERVER_URL",
//@                      "value": "https://index.docker.io"
//@                    },
//@                    {
//@                      "name": "DOCKER_REGISTRY_SERVER_USERNAME",
//@                      "value": ""
//@                    },
//@                    {
//@                      "name": "DOCKER_REGISTRY_SERVER_PASSWORD",
//@                      "value": ""
//@                    },
//@                    {
//@                      "name": "WEBSITES_ENABLE_APP_SERVICE_STORAGE",
//@                      "value": "false"
//@                    }
//@                  ],
//@                  "linuxFxVersion": "[format('DOCKER|{0}:{1}', parameters('dockerImage'), parameters('dockerImageTag'))]"
//@                },
//@                "serverFarmId": "[parameters('appPlanId')]"
//@              }
//@            }
//@          ],
//@          "outputs": {
//@            "siteUrl": {
//@              "type": "string",
//@              "value": "[reference(resourceId('Microsoft.Web/sites', format('{0}site', parameters('namePrefix'))), '2020-06-01').hostNames[0]]"
//@            }
//@          }
//@        }
//@      },
//@      "dependsOn": [
//@        "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'adotfrank-rg'), 'Microsoft.Resources/deployments', 'planDeploy')]",
//@        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@      ]
//@    },
  name: '${site.name}siteDeploy2'
//@      "name": "[format('{0}siteDeploy2', variables('websites')[copyIndex()].name)]",
  scope: rg
  params: {
//@        "parameters": {
//@        },
    appPlanId: appPlanDeploy.outputs.planId
//@          "appPlanId": {
//@            "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'adotfrank-rg'), 'Microsoft.Resources/deployments', 'planDeploy'), '2025-04-01').outputs.planId.value]"
//@          },
    namePrefix: site.name
//@          "namePrefix": {
//@            "value": "[variables('websites')[copyIndex()].name]"
//@          },
    dockerImage: 'nginxdemos/hello'
//@          "dockerImage": {
//@            "value": "nginxdemos/hello"
//@          },
    dockerImageTag: site.tag
//@          "dockerImageTag": {
//@            "value": "[variables('websites')[copyIndex()].tag]"
//@          }
  }
}]

module storageDeploy 'ts:00000000-0000-0000-0000-000000000000/test-rg/storage-spec:1.0' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "resourceGroup": "adotfrank-rg",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "templateLink": {
//@          "id": "/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/test-rg/providers/Microsoft.Resources/templateSpecs/storage-spec/versions/1.0"
//@        }
//@      },
//@      "dependsOn": [
//@        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@      ]
//@    },
  name: 'storageDeploy'
//@      "name": "storageDeploy",
  scope: rg
  params: {
//@        "parameters": {
//@        },
    location: 'eastus'
//@          "location": {
//@            "value": "eastus"
//@          }
  }
}

module storageDeploy2 'ts/mySpecRG:storage-spec:1.0' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "resourceGroup": "adotfrank-rg",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "templateLink": {
//@          "id": "/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/test-rg/providers/Microsoft.Resources/templateSpecs/storage-spec/versions/1.0"
//@        }
//@      },
//@      "dependsOn": [
//@        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@      ]
//@    },
  name: 'storageDeploy2'
//@      "name": "storageDeploy2",
  scope: rg
  params: {
//@        "parameters": {
//@        },
    location: 'eastus'
//@          "location": {
//@            "value": "eastus"
//@          }
  }
}

var vnets = [
//@    "vnets": [
//@    ]
  {
//@      {
//@      },
    name: 'vnet1'
//@        "name": "vnet1",
    subnetName: 'subnet1.1'
//@        "subnetName": "subnet1.1"
  }
  {
//@      {
//@      }
    name: 'vnet2'
//@        "name": "vnet2",
    subnetName: 'subnet2.1'
//@        "subnetName": "subnet2.1"
  }
]

module vnetDeploy 'ts:11111111-1111-1111-1111-111111111111/prod-rg/vnet-spec:v2' = [for vnet in vnets: {
//@    {
//@      "copy": {
//@        "name": "vnetDeploy",
//@        "count": "[length(variables('vnets'))]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "resourceGroup": "adotfrank-rg",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "templateLink": {
//@          "id": "/subscriptions/11111111-1111-1111-1111-111111111111/resourceGroups/prod-rg/providers/Microsoft.Resources/templateSpecs/vnet-spec/versions/v2"
//@        }
//@      },
//@      "dependsOn": [
//@        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@      ]
//@    },
  name: '${vnet.name}Deploy'
//@      "name": "[format('{0}Deploy', variables('vnets')[copyIndex()].name)]",
  scope: rg
  params: {
//@        "parameters": {
//@        },
    vnetName: vnet.name
//@          "vnetName": {
//@            "value": "[variables('vnets')[copyIndex()].name]"
//@          },
    subnetName: vnet.subnetName
//@          "subnetName": {
//@            "value": "[variables('vnets')[copyIndex()].subnetName]"
//@          }
  }
}]

output siteUrls array = [for (site, i) in websites: siteDeploy[i].outputs.siteUrl]
//@    "siteUrls": {
//@      "type": "array",
//@      "copy": {
//@        "count": "[length(variables('websites'))]",
//@        "input": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'adotfrank-rg'), 'Microsoft.Resources/deployments', format('{0}siteDeploy', variables('websites')[copyIndex()].name)), '2025-04-01').outputs.siteUrl.value]"
//@      }
//@    }

module passthroughPort 'br:localhost:5000/passthrough/port:v1' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "resourceGroup": "adotfrank-rg",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "14234608298281212658"
//@            },
//@            "description": "passthrough port description"
//@          },
//@          "parameters": {
//@            "port": {
//@              "type": "string"
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "port": {
//@              "type": "string",
//@              "value": "[parameters('port')]"
//@            }
//@          }
//@        }
//@      },
//@      "dependsOn": [
//@        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@      ]
//@    },
  scope: rg
  name: 'port'
//@      "name": "port",
  params: {
//@        "parameters": {
//@        },
    port: 'test'
//@          "port": {
//@            "value": "test"
//@          }
  }
}

module ipv4 'br:127.0.0.1/passthrough/ipv4:v1' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "resourceGroup": "adotfrank-rg",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "12121705673191614943"
//@            }
//@          },
//@          "parameters": {
//@            "ipv4": {
//@              "type": "string"
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "ipv4": {
//@              "type": "string",
//@              "value": "[parameters('ipv4')]"
//@            }
//@          }
//@        }
//@      },
//@      "dependsOn": [
//@        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@      ]
//@    },
  scope: rg
  name: 'ipv4'
//@      "name": "ipv4",
  params: {
//@        "parameters": {
//@        },
    ipv4: 'test'
//@          "ipv4": {
//@            "value": "test"
//@          }
  }
}

module ipv4port 'br:127.0.0.1:5000/passthrough/ipv4port:v1' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "resourceGroup": "adotfrank-rg",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "7823936554124086035"
//@            }
//@          },
//@          "parameters": {
//@            "ipv4port": {
//@              "type": "string"
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "ipv4port": {
//@              "type": "string",
//@              "value": "[parameters('ipv4port')]"
//@            }
//@          }
//@        }
//@      },
//@      "dependsOn": [
//@        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@      ]
//@    },
  scope: rg
  name: 'ipv4port'
//@      "name": "ipv4port",
  params: {
//@        "parameters": {
//@        },
    ipv4port: 'test'
//@          "ipv4port": {
//@            "value": "test"
//@          }
  }
}

module ipv6 'br:[::1]/passthrough/ipv6:v1' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "resourceGroup": "adotfrank-rg",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "5093140793351809054"
//@            }
//@          },
//@          "parameters": {
//@            "ipv6": {
//@              "type": "string"
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "ipv6": {
//@              "type": "string",
//@              "value": "[parameters('ipv6')]"
//@            }
//@          }
//@        }
//@      },
//@      "dependsOn": [
//@        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@      ]
//@    },
  scope: rg
  name: 'ipv6'
//@      "name": "ipv6",
  params: {
//@        "parameters": {
//@        },
    ipv6: 'test'
//@          "ipv6": {
//@            "value": "test"
//@          }
  }
}

module ipv6port 'br:[::1]:5000/passthrough/ipv6port:v1' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "resourceGroup": "adotfrank-rg",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "969477156678070948"
//@            }
//@          },
//@          "parameters": {
//@            "ipv6port": {
//@              "type": "string"
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "ipv6port": {
//@              "type": "string",
//@              "value": "[parameters('ipv6port')]"
//@            }
//@          }
//@        }
//@      },
//@      "dependsOn": [
//@        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@      ]
//@    }
  scope: rg
  name: 'ipv6port'
//@      "name": "ipv6port",
  params: {
//@        "parameters": {
//@        },
    ipv6port: 'test'
//@          "ipv6port": {
//@            "value": "test"
//@          }
  }
}
