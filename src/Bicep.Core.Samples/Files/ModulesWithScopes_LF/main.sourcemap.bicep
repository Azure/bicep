targetScope = 'tenant'

module myManagementGroupMod 'modules/managementgroup.bicep' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2022-09-01",
//@      "scope": "[format('Microsoft.Management/managementGroups/{0}', 'myManagementGroup')]",
//@      "location": "[deployment().location]",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-08-01/managementGroupDeploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "13668708128840728545"
//@            }
//@          },
//@          "resources": [
//@            {
//@              "type": "Microsoft.Resources/deployments",
//@              "apiVersion": "2022-09-01",
//@              "name": "myTenantMod",
//@              "scope": "/",
//@              "location": "[deployment().location]",
//@              "properties": {
//@                "expressionEvaluationOptions": {
//@                  "scope": "inner"
//@                },
//@                "mode": "Incremental",
//@                "template": {
//@                  "$schema": "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#",
//@                  "contentVersion": "1.0.0.0",
//@                  "metadata": {
//@                    "_generator": {
//@                      "name": "bicep",
//@                      "version": "dev",
//@                      "templateHash": "15729984543815100695"
//@                    }
//@                  },
//@                  "resources": [],
//@                  "outputs": {
//@                    "myOutput": {
//@                      "type": "string",
//@                      "value": "hello!"
//@                    }
//@                  }
//@                }
//@              }
//@            },
//@            {
//@              "type": "Microsoft.Resources/deployments",
//@              "apiVersion": "2022-09-01",
//@              "name": "myManagementGroupMod",
//@              "scope": "[format('Microsoft.Management/managementGroups/{0}', 'myManagementGroup2')]",
//@              "location": "[deployment().location]",
//@              "properties": {
//@                "expressionEvaluationOptions": {
//@                  "scope": "inner"
//@                },
//@                "mode": "Incremental",
//@                "template": {
//@                  "$schema": "https://schema.management.azure.com/schemas/2019-08-01/managementGroupDeploymentTemplate.json#",
//@                  "contentVersion": "1.0.0.0",
//@                  "metadata": {
//@                    "_generator": {
//@                      "name": "bicep",
//@                      "version": "dev",
//@                      "templateHash": "2139830058681583241"
//@                    }
//@                  },
//@                  "resources": []
//@                }
//@              }
//@            },
//@            {
//@              "type": "Microsoft.Resources/deployments",
//@              "apiVersion": "2022-09-01",
//@              "name": "mySubscriptionMod",
//@              "subscriptionId": "1ad827ac-2669-4c2f-9970-282b93c3c550",
//@              "location": "[deployment().location]",
//@              "properties": {
//@                "expressionEvaluationOptions": {
//@                  "scope": "inner"
//@                },
//@                "mode": "Incremental",
//@                "template": {
//@                  "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@                  "contentVersion": "1.0.0.0",
//@                  "metadata": {
//@                    "_generator": {
//@                      "name": "bicep",
//@                      "version": "dev",
//@                      "templateHash": "1395382333336833730"
//@                    }
//@                  },
//@                  "resources": []
//@                }
//@              }
//@            }
//@          ],
//@          "outputs": {
//@            "myOutput": {
//@              "type": "string",
//@              "value": "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myTenantMod'), '2022-09-01').outputs.myOutput.value]"
//@            }
//@          }
//@        }
//@      }
//@    },
  name: 'myManagementGroupMod'
//@      "name": "myManagementGroupMod",
  scope: managementGroup('myManagementGroup')
}
module myManagementGroupModWithDuplicatedNameButDifferentScope 'modules/managementgroup_empty.bicep' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2022-09-01",
//@      "scope": "[format('Microsoft.Management/managementGroups/{0}', 'myManagementGroup2')]",
//@      "location": "[deployment().location]",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-08-01/managementGroupDeploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "2139830058681583241"
//@            }
//@          },
//@          "resources": []
//@        }
//@      }
//@    },
  name: 'myManagementGroupMod'
//@      "name": "myManagementGroupMod",
  scope: managementGroup('myManagementGroup2')
}
module mySubscriptionMod 'modules/subscription.bicep' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2022-09-01",
//@      "location": "[deployment().location]",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "15454708585237868866"
//@            }
//@          },
//@          "resources": [
//@            {
//@              "type": "Microsoft.Resources/deployments",
//@              "apiVersion": "2022-09-01",
//@              "name": "myResourceGroupMod",
//@              "resourceGroup": "myRg",
//@              "properties": {
//@                "expressionEvaluationOptions": {
//@                  "scope": "inner"
//@                },
//@                "mode": "Incremental",
//@                "template": {
//@                  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@                  "contentVersion": "1.0.0.0",
//@                  "metadata": {
//@                    "_generator": {
//@                      "name": "bicep",
//@                      "version": "dev",
//@                      "templateHash": "3707936864441531265"
//@                    }
//@                  },
//@                  "resources": [
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "myTenantMod",
//@                      "scope": "/",
//@                      "location": "[resourceGroup().location]",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "15729984543815100695"
//@                            }
//@                          },
//@                          "resources": [],
//@                          "outputs": {
//@                            "myOutput": {
//@                              "type": "string",
//@                              "value": "hello!"
//@                            }
//@                          }
//@                        }
//@                      }
//@                    },
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "myOtherResourceGroup",
//@                      "subscriptionId": "db90cfef-a146-4f67-b32f-b263518bd216",
//@                      "resourceGroup": "myOtherRg",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "7469434526073292388"
//@                            }
//@                          },
//@                          "resources": [],
//@                          "outputs": {
//@                            "myOutput": {
//@                              "type": "string",
//@                              "value": "hello!"
//@                            }
//@                          }
//@                        }
//@                      }
//@                    },
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "mySubscription",
//@                      "subscriptionId": "[subscription().subscriptionId]",
//@                      "location": "[resourceGroup().location]",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "1395382333336833730"
//@                            }
//@                          },
//@                          "resources": []
//@                        }
//@                      }
//@                    },
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "otherSubscription",
//@                      "subscriptionId": "cd780357-07f5-49cc-b945-a3fe15863860",
//@                      "location": "[resourceGroup().location]",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "1395382333336833730"
//@                            }
//@                          },
//@                          "resources": []
//@                        }
//@                      }
//@                    }
//@                  ],
//@                  "outputs": {
//@                    "myOutput": {
//@                      "type": "string",
//@                      "value": "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myTenantMod'), '2022-09-01').outputs.myOutput.value]"
//@                    },
//@                    "myOutputResourceGroup": {
//@                      "type": "string",
//@                      "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'db90cfef-a146-4f67-b32f-b263518bd216', 'myOtherRg'), 'Microsoft.Resources/deployments', 'myOtherResourceGroup'), '2022-09-01').outputs.myOutput.value]"
//@                    }
//@                  }
//@                }
//@              }
//@            },
//@            {
//@              "type": "Microsoft.Resources/deployments",
//@              "apiVersion": "2022-09-01",
//@              "name": "myResourceGroupMod2",
//@              "resourceGroup": "myRg",
//@              "properties": {
//@                "expressionEvaluationOptions": {
//@                  "scope": "inner"
//@                },
//@                "mode": "Incremental",
//@                "template": {
//@                  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@                  "contentVersion": "1.0.0.0",
//@                  "metadata": {
//@                    "_generator": {
//@                      "name": "bicep",
//@                      "version": "dev",
//@                      "templateHash": "3707936864441531265"
//@                    }
//@                  },
//@                  "resources": [
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "myTenantMod",
//@                      "scope": "/",
//@                      "location": "[resourceGroup().location]",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "15729984543815100695"
//@                            }
//@                          },
//@                          "resources": [],
//@                          "outputs": {
//@                            "myOutput": {
//@                              "type": "string",
//@                              "value": "hello!"
//@                            }
//@                          }
//@                        }
//@                      }
//@                    },
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "myOtherResourceGroup",
//@                      "subscriptionId": "db90cfef-a146-4f67-b32f-b263518bd216",
//@                      "resourceGroup": "myOtherRg",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "7469434526073292388"
//@                            }
//@                          },
//@                          "resources": [],
//@                          "outputs": {
//@                            "myOutput": {
//@                              "type": "string",
//@                              "value": "hello!"
//@                            }
//@                          }
//@                        }
//@                      }
//@                    },
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "mySubscription",
//@                      "subscriptionId": "[subscription().subscriptionId]",
//@                      "location": "[resourceGroup().location]",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "1395382333336833730"
//@                            }
//@                          },
//@                          "resources": []
//@                        }
//@                      }
//@                    },
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "otherSubscription",
//@                      "subscriptionId": "cd780357-07f5-49cc-b945-a3fe15863860",
//@                      "location": "[resourceGroup().location]",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "1395382333336833730"
//@                            }
//@                          },
//@                          "resources": []
//@                        }
//@                      }
//@                    }
//@                  ],
//@                  "outputs": {
//@                    "myOutput": {
//@                      "type": "string",
//@                      "value": "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myTenantMod'), '2022-09-01').outputs.myOutput.value]"
//@                    },
//@                    "myOutputResourceGroup": {
//@                      "type": "string",
//@                      "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'db90cfef-a146-4f67-b32f-b263518bd216', 'myOtherRg'), 'Microsoft.Resources/deployments', 'myOtherResourceGroup'), '2022-09-01').outputs.myOutput.value]"
//@                    }
//@                  }
//@                }
//@              }
//@            },
//@            {
//@              "type": "Microsoft.Resources/deployments",
//@              "apiVersion": "2022-09-01",
//@              "name": "myResourceGroupMod3",
//@              "subscriptionId": "subId",
//@              "resourceGroup": "myRg",
//@              "properties": {
//@                "expressionEvaluationOptions": {
//@                  "scope": "inner"
//@                },
//@                "mode": "Incremental",
//@                "template": {
//@                  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@                  "contentVersion": "1.0.0.0",
//@                  "metadata": {
//@                    "_generator": {
//@                      "name": "bicep",
//@                      "version": "dev",
//@                      "templateHash": "3707936864441531265"
//@                    }
//@                  },
//@                  "resources": [
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "myTenantMod",
//@                      "scope": "/",
//@                      "location": "[resourceGroup().location]",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "15729984543815100695"
//@                            }
//@                          },
//@                          "resources": [],
//@                          "outputs": {
//@                            "myOutput": {
//@                              "type": "string",
//@                              "value": "hello!"
//@                            }
//@                          }
//@                        }
//@                      }
//@                    },
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "myOtherResourceGroup",
//@                      "subscriptionId": "db90cfef-a146-4f67-b32f-b263518bd216",
//@                      "resourceGroup": "myOtherRg",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "7469434526073292388"
//@                            }
//@                          },
//@                          "resources": [],
//@                          "outputs": {
//@                            "myOutput": {
//@                              "type": "string",
//@                              "value": "hello!"
//@                            }
//@                          }
//@                        }
//@                      }
//@                    },
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "mySubscription",
//@                      "subscriptionId": "[subscription().subscriptionId]",
//@                      "location": "[resourceGroup().location]",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "1395382333336833730"
//@                            }
//@                          },
//@                          "resources": []
//@                        }
//@                      }
//@                    },
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "otherSubscription",
//@                      "subscriptionId": "cd780357-07f5-49cc-b945-a3fe15863860",
//@                      "location": "[resourceGroup().location]",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "1395382333336833730"
//@                            }
//@                          },
//@                          "resources": []
//@                        }
//@                      }
//@                    }
//@                  ],
//@                  "outputs": {
//@                    "myOutput": {
//@                      "type": "string",
//@                      "value": "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myTenantMod'), '2022-09-01').outputs.myOutput.value]"
//@                    },
//@                    "myOutputResourceGroup": {
//@                      "type": "string",
//@                      "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'db90cfef-a146-4f67-b32f-b263518bd216', 'myOtherRg'), 'Microsoft.Resources/deployments', 'myOtherResourceGroup'), '2022-09-01').outputs.myOutput.value]"
//@                    }
//@                  }
//@                }
//@              }
//@            },
//@            {
//@              "type": "Microsoft.Resources/deployments",
//@              "apiVersion": "2022-09-01",
//@              "name": "myTenantMod",
//@              "scope": "/",
//@              "location": "[deployment().location]",
//@              "properties": {
//@                "expressionEvaluationOptions": {
//@                  "scope": "inner"
//@                },
//@                "mode": "Incremental",
//@                "template": {
//@                  "$schema": "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#",
//@                  "contentVersion": "1.0.0.0",
//@                  "metadata": {
//@                    "_generator": {
//@                      "name": "bicep",
//@                      "version": "dev",
//@                      "templateHash": "15729984543815100695"
//@                    }
//@                  },
//@                  "resources": [],
//@                  "outputs": {
//@                    "myOutput": {
//@                      "type": "string",
//@                      "value": "hello!"
//@                    }
//@                  }
//@                }
//@              }
//@            }
//@          ],
//@          "outputs": {
//@            "myOutput": {
//@              "type": "string",
//@              "value": "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myTenantMod'), '2022-09-01').outputs.myOutput.value]"
//@            },
//@            "myOutputRgMod": {
//@              "type": "string",
//@              "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'myRg'), 'Microsoft.Resources/deployments', 'myResourceGroupMod'), '2022-09-01').outputs.myOutput.value]"
//@            },
//@            "myOutputRgMod2": {
//@              "type": "string",
//@              "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'myRg'), 'Microsoft.Resources/deployments', 'myResourceGroupMod2'), '2022-09-01').outputs.myOutput.value]"
//@            }
//@          }
//@        }
//@      }
//@    },
  name: 'mySubscriptionMod'
//@      "name": "mySubscriptionMod",
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
//@      "subscriptionId": "ee44cd78-68c6-43d9-874e-e684ec8d1191",
}

module mySubscriptionModWithCondition 'modules/subscription.bicep' = if (length('foo') == 3) {
//@    {
//@      "condition": "[equals(length('foo'), 3)]",
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2022-09-01",
//@      "location": "[deployment().location]",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "15454708585237868866"
//@            }
//@          },
//@          "resources": [
//@            {
//@              "type": "Microsoft.Resources/deployments",
//@              "apiVersion": "2022-09-01",
//@              "name": "myResourceGroupMod",
//@              "resourceGroup": "myRg",
//@              "properties": {
//@                "expressionEvaluationOptions": {
//@                  "scope": "inner"
//@                },
//@                "mode": "Incremental",
//@                "template": {
//@                  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@                  "contentVersion": "1.0.0.0",
//@                  "metadata": {
//@                    "_generator": {
//@                      "name": "bicep",
//@                      "version": "dev",
//@                      "templateHash": "3707936864441531265"
//@                    }
//@                  },
//@                  "resources": [
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "myTenantMod",
//@                      "scope": "/",
//@                      "location": "[resourceGroup().location]",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "15729984543815100695"
//@                            }
//@                          },
//@                          "resources": [],
//@                          "outputs": {
//@                            "myOutput": {
//@                              "type": "string",
//@                              "value": "hello!"
//@                            }
//@                          }
//@                        }
//@                      }
//@                    },
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "myOtherResourceGroup",
//@                      "subscriptionId": "db90cfef-a146-4f67-b32f-b263518bd216",
//@                      "resourceGroup": "myOtherRg",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "7469434526073292388"
//@                            }
//@                          },
//@                          "resources": [],
//@                          "outputs": {
//@                            "myOutput": {
//@                              "type": "string",
//@                              "value": "hello!"
//@                            }
//@                          }
//@                        }
//@                      }
//@                    },
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "mySubscription",
//@                      "subscriptionId": "[subscription().subscriptionId]",
//@                      "location": "[resourceGroup().location]",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "1395382333336833730"
//@                            }
//@                          },
//@                          "resources": []
//@                        }
//@                      }
//@                    },
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "otherSubscription",
//@                      "subscriptionId": "cd780357-07f5-49cc-b945-a3fe15863860",
//@                      "location": "[resourceGroup().location]",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "1395382333336833730"
//@                            }
//@                          },
//@                          "resources": []
//@                        }
//@                      }
//@                    }
//@                  ],
//@                  "outputs": {
//@                    "myOutput": {
//@                      "type": "string",
//@                      "value": "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myTenantMod'), '2022-09-01').outputs.myOutput.value]"
//@                    },
//@                    "myOutputResourceGroup": {
//@                      "type": "string",
//@                      "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'db90cfef-a146-4f67-b32f-b263518bd216', 'myOtherRg'), 'Microsoft.Resources/deployments', 'myOtherResourceGroup'), '2022-09-01').outputs.myOutput.value]"
//@                    }
//@                  }
//@                }
//@              }
//@            },
//@            {
//@              "type": "Microsoft.Resources/deployments",
//@              "apiVersion": "2022-09-01",
//@              "name": "myResourceGroupMod2",
//@              "resourceGroup": "myRg",
//@              "properties": {
//@                "expressionEvaluationOptions": {
//@                  "scope": "inner"
//@                },
//@                "mode": "Incremental",
//@                "template": {
//@                  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@                  "contentVersion": "1.0.0.0",
//@                  "metadata": {
//@                    "_generator": {
//@                      "name": "bicep",
//@                      "version": "dev",
//@                      "templateHash": "3707936864441531265"
//@                    }
//@                  },
//@                  "resources": [
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "myTenantMod",
//@                      "scope": "/",
//@                      "location": "[resourceGroup().location]",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "15729984543815100695"
//@                            }
//@                          },
//@                          "resources": [],
//@                          "outputs": {
//@                            "myOutput": {
//@                              "type": "string",
//@                              "value": "hello!"
//@                            }
//@                          }
//@                        }
//@                      }
//@                    },
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "myOtherResourceGroup",
//@                      "subscriptionId": "db90cfef-a146-4f67-b32f-b263518bd216",
//@                      "resourceGroup": "myOtherRg",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "7469434526073292388"
//@                            }
//@                          },
//@                          "resources": [],
//@                          "outputs": {
//@                            "myOutput": {
//@                              "type": "string",
//@                              "value": "hello!"
//@                            }
//@                          }
//@                        }
//@                      }
//@                    },
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "mySubscription",
//@                      "subscriptionId": "[subscription().subscriptionId]",
//@                      "location": "[resourceGroup().location]",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "1395382333336833730"
//@                            }
//@                          },
//@                          "resources": []
//@                        }
//@                      }
//@                    },
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "otherSubscription",
//@                      "subscriptionId": "cd780357-07f5-49cc-b945-a3fe15863860",
//@                      "location": "[resourceGroup().location]",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "1395382333336833730"
//@                            }
//@                          },
//@                          "resources": []
//@                        }
//@                      }
//@                    }
//@                  ],
//@                  "outputs": {
//@                    "myOutput": {
//@                      "type": "string",
//@                      "value": "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myTenantMod'), '2022-09-01').outputs.myOutput.value]"
//@                    },
//@                    "myOutputResourceGroup": {
//@                      "type": "string",
//@                      "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'db90cfef-a146-4f67-b32f-b263518bd216', 'myOtherRg'), 'Microsoft.Resources/deployments', 'myOtherResourceGroup'), '2022-09-01').outputs.myOutput.value]"
//@                    }
//@                  }
//@                }
//@              }
//@            },
//@            {
//@              "type": "Microsoft.Resources/deployments",
//@              "apiVersion": "2022-09-01",
//@              "name": "myResourceGroupMod3",
//@              "subscriptionId": "subId",
//@              "resourceGroup": "myRg",
//@              "properties": {
//@                "expressionEvaluationOptions": {
//@                  "scope": "inner"
//@                },
//@                "mode": "Incremental",
//@                "template": {
//@                  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@                  "contentVersion": "1.0.0.0",
//@                  "metadata": {
//@                    "_generator": {
//@                      "name": "bicep",
//@                      "version": "dev",
//@                      "templateHash": "3707936864441531265"
//@                    }
//@                  },
//@                  "resources": [
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "myTenantMod",
//@                      "scope": "/",
//@                      "location": "[resourceGroup().location]",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "15729984543815100695"
//@                            }
//@                          },
//@                          "resources": [],
//@                          "outputs": {
//@                            "myOutput": {
//@                              "type": "string",
//@                              "value": "hello!"
//@                            }
//@                          }
//@                        }
//@                      }
//@                    },
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "myOtherResourceGroup",
//@                      "subscriptionId": "db90cfef-a146-4f67-b32f-b263518bd216",
//@                      "resourceGroup": "myOtherRg",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "7469434526073292388"
//@                            }
//@                          },
//@                          "resources": [],
//@                          "outputs": {
//@                            "myOutput": {
//@                              "type": "string",
//@                              "value": "hello!"
//@                            }
//@                          }
//@                        }
//@                      }
//@                    },
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "mySubscription",
//@                      "subscriptionId": "[subscription().subscriptionId]",
//@                      "location": "[resourceGroup().location]",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "1395382333336833730"
//@                            }
//@                          },
//@                          "resources": []
//@                        }
//@                      }
//@                    },
//@                    {
//@                      "type": "Microsoft.Resources/deployments",
//@                      "apiVersion": "2022-09-01",
//@                      "name": "otherSubscription",
//@                      "subscriptionId": "cd780357-07f5-49cc-b945-a3fe15863860",
//@                      "location": "[resourceGroup().location]",
//@                      "properties": {
//@                        "expressionEvaluationOptions": {
//@                          "scope": "inner"
//@                        },
//@                        "mode": "Incremental",
//@                        "template": {
//@                          "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@                          "contentVersion": "1.0.0.0",
//@                          "metadata": {
//@                            "_generator": {
//@                              "name": "bicep",
//@                              "version": "dev",
//@                              "templateHash": "1395382333336833730"
//@                            }
//@                          },
//@                          "resources": []
//@                        }
//@                      }
//@                    }
//@                  ],
//@                  "outputs": {
//@                    "myOutput": {
//@                      "type": "string",
//@                      "value": "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myTenantMod'), '2022-09-01').outputs.myOutput.value]"
//@                    },
//@                    "myOutputResourceGroup": {
//@                      "type": "string",
//@                      "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'db90cfef-a146-4f67-b32f-b263518bd216', 'myOtherRg'), 'Microsoft.Resources/deployments', 'myOtherResourceGroup'), '2022-09-01').outputs.myOutput.value]"
//@                    }
//@                  }
//@                }
//@              }
//@            },
//@            {
//@              "type": "Microsoft.Resources/deployments",
//@              "apiVersion": "2022-09-01",
//@              "name": "myTenantMod",
//@              "scope": "/",
//@              "location": "[deployment().location]",
//@              "properties": {
//@                "expressionEvaluationOptions": {
//@                  "scope": "inner"
//@                },
//@                "mode": "Incremental",
//@                "template": {
//@                  "$schema": "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#",
//@                  "contentVersion": "1.0.0.0",
//@                  "metadata": {
//@                    "_generator": {
//@                      "name": "bicep",
//@                      "version": "dev",
//@                      "templateHash": "15729984543815100695"
//@                    }
//@                  },
//@                  "resources": [],
//@                  "outputs": {
//@                    "myOutput": {
//@                      "type": "string",
//@                      "value": "hello!"
//@                    }
//@                  }
//@                }
//@              }
//@            }
//@          ],
//@          "outputs": {
//@            "myOutput": {
//@              "type": "string",
//@              "value": "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myTenantMod'), '2022-09-01').outputs.myOutput.value]"
//@            },
//@            "myOutputRgMod": {
//@              "type": "string",
//@              "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'myRg'), 'Microsoft.Resources/deployments', 'myResourceGroupMod'), '2022-09-01').outputs.myOutput.value]"
//@            },
//@            "myOutputRgMod2": {
//@              "type": "string",
//@              "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'myRg'), 'Microsoft.Resources/deployments', 'myResourceGroupMod2'), '2022-09-01').outputs.myOutput.value]"
//@            }
//@          }
//@        }
//@      }
//@    },
  name: 'mySubscriptionModWithCondition'
//@      "name": "mySubscriptionModWithCondition",
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
//@      "subscriptionId": "ee44cd78-68c6-43d9-874e-e684ec8d1191",
}

module mySubscriptionModWithDuplicatedNameButDifferentScope 'modules/subscription_empty.bicep' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2022-09-01",
//@      "location": "[deployment().location]",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "1395382333336833730"
//@            }
//@          },
//@          "resources": []
//@        }
//@      }
//@    }
  name: 'mySubscriptionMod'
//@      "name": "mySubscriptionMod",
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
//@      "subscriptionId": "1ad827ac-2669-4c2f-9970-282b93c3c550",
}


output myManagementGroupOutput string = myManagementGroupMod.outputs.myOutput
//@    "myManagementGroupOutput": {
//@      "type": "string",
//@      "value": "[reference(extensionResourceId(tenantResourceId('Microsoft.Management/managementGroups', 'myManagementGroup'), 'Microsoft.Resources/deployments', 'myManagementGroupMod'), '2022-09-01').outputs.myOutput.value]"
//@    },
output mySubscriptionOutput string = mySubscriptionMod.outputs.myOutput
//@    "mySubscriptionOutput": {
//@      "type": "string",
//@      "value": "[reference(subscriptionResourceId('ee44cd78-68c6-43d9-874e-e684ec8d1191', 'Microsoft.Resources/deployments', 'mySubscriptionMod'), '2022-09-01').outputs.myOutput.value]"
//@    }

