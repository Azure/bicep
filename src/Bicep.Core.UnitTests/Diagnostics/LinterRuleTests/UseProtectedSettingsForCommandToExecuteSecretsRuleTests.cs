// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class ProtectCommandToExecuteSecretsRuleTests : LinterRuleTestsBase
    {
        private void CompileAndTest(string text, string[] expectedMessages)
        {
            AssertLinterRuleDiagnostics(ProtectCommandToExecuteSecretsRule.Code, text, expectedMessages);
        }

        // Modified from https://docs.microsoft.com/en-us/azure/azure-arc/servers/manage-vm-extensions-template#template-file-for-windows-1
        // CustomScriptExtension
        [TestMethod]
        public void If_HybridCompute_AndTypeIsCustomScriptExtension_AndContainsSecureParam_AndNotUsingProtectedSettings_ShouldFail()
        {
            CompileAndTest(@"
param vmName string
param location string
param fileUris string

@secure()
param arguments string = ''

var UriFileNamePieces = split(fileUris, '/')
var firstFileNameString = UriFileNamePieces[(length(UriFileNamePieces) - 1)]
var firstFileNameBreakString = split(firstFileNameString, '?')
var firstFileName = firstFileNameBreakString[0]

resource customScriptExtension 'Microsoft.HybridCompute/machines/extensions@2019-08-02-preview' = {
  name: '${vmName}/CustomScriptExtension'
  location: location
  properties: {
    publisher: 'Microsoft.Compute'
    type: 'CustomScriptExtension'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: split(fileUris, ' ')
      commandToExecute: 'powershell -ExecutionPolicy Unrestricted -File ${firstFileName} ${arguments}' // MODIFIED
    }
  }
}
            ",
              new string[] {
                // TTK error message:
                //  [-] CommandToExecute Must Use ProtectedSettings For Secrets (62 ms)
                //    CommandToExecute references parameter 'arguments' of type 'secureString', but is not in .protectedSettings
                "[23] Use protectedSettings for commandToExecute secrets. Found possible secret: secure parameter 'arguments'"
              }
            );
        }

        // Modified from https://docs.microsoft.com/en-us/azure/azure-arc/servers/manage-vm-extensions-template#template-file-for-windows-1
        // CustomScriptExtension
        [TestMethod]
        public void If_VirtualMachines_AndTypeIsCustomScriptExtension_AndContainsSecureParam_AndNotUsingProtectedSettings_ShouldFail()
        {
            CompileAndTest(@"
param vmName string
param location string
param fileUris string

@secure()
param arguments string = ''

var UriFileNamePieces = split(fileUris, '/')
var firstFileNameString = UriFileNamePieces[(length(UriFileNamePieces) - 1)]
var firstFileNameBreakString = split(firstFileNameString, '?')
var firstFileName = firstFileNameBreakString[0]

resource customScriptExtension 'Microsoft.Compute/virtualMachines/extensions@2021-04-01' = {
  name: '${vmName}/CustomScriptExtension'
  location: location
  properties: {
    publisher: 'Microsoft.Compute'
    type: 'CustomScriptExtension'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: split(fileUris, ' ')
      commandToExecute: 'powershell -ExecutionPolicy Unrestricted -File ${firstFileName} ${arguments}' // MODIFIED
    }
  }
}
            ",
              new string[] {
                "[23] Use protectedSettings for commandToExecute secrets. Found possible secret: secure parameter 'arguments'"
               }
            );
        }

        // Modified from https://docs.microsoft.com/en-us/azure/azure-arc/servers/manage-vm-extensions-template#template-file-for-windows-1
        // CustomScriptExtension
        [TestMethod]
        public void If_VirtualMachines_AndTypeIsCustomScriptForLinux_AndContainsSecureParam_AndNotUsingProtectedSettings_ShouldFail()
        {
            CompileAndTest(@"
param vmNamePrefix string = 'php'
param artifactsLocation string = deployment().properties.templateLink.uri
@secure()
param artifactsLocationSasToken string = ''
param numberOfDataDisks int = 8
param location string = resourceGroup().location
@secure()
param adminPasswordOrKey string
var vmNames_var = [
  '${vmNamePrefix}-frontend'
  '${vmNamePrefix}-php1'
  '${vmNamePrefix}-php2'
]
var nicConfig = [
  {
    name: '${vmNames_var[0]}nic1'
    subnetRef: subnet1Ref
  }
  {
    name: '${vmNames_var[0]}nic2'
    subnetRef: subnet2Ref
  }
  {
    name: '${vmNames_var[1]}nic1'
    subnetRef: subnet2Ref
  }
  {
    name: '${vmNames_var[1]}nic2'
    subnetRef: subnet3Ref
  }
  {
    name: '${vmNames_var[2]}nic1'
    subnetRef: subnet2Ref
  }
  {
    name: '${vmNames_var[2]}nic2'
    subnetRef: subnet3Ref
  }
]
var subnet1Name = 'Subnet-1'
var subnet2Name = 'Subnet-2'
var subnet3Name = 'Subnet-3'
var virtualNetworkName_var = '${vmNamePrefix}-vnet'
var subnet1Ref = resourceId('Microsoft.Network/virtualNetworks/subnets', virtualNetworkName_var, subnet1Name)
var subnet2Ref = resourceId('Microsoft.Network/virtualNetworks/subnets', virtualNetworkName_var, subnet2Name)
var subnet3Ref = resourceId('Microsoft.Network/virtualNetworks/subnets', virtualNetworkName_var, subnet3Name)

resource vmNames_installnginx 'Microsoft.Compute/virtualMachines/extensions@2019-07-01' = [for i in range(0, 3): {
  name: '${vmNames_var[i]}/installnginx'
  location: location
  properties: {
    publisher: 'Microsoft.OSTCExtensions'
    type: 'CustomScriptForLinux'
    typeHandlerVersion: '1.4'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: [
        uri(artifactsLocation, 'scripts/install_reverse_nginx.sh${artifactsLocationSasToken}')
        uri(artifactsLocation, 'scripts/install_nginx_php.sh${artifactsLocationSasToken}')
        uri(artifactsLocation, 'conf/frontend_nginx.conf${artifactsLocationSasToken}')
        uri(artifactsLocation, 'conf/frontend_proxy.conf${artifactsLocationSasToken}')
        uri(artifactsLocation, 'conf/backend_nginx.conf${artifactsLocationSasToken}')
      ]
      commandToExecute: ((i == 0) ? 'sh install_reverse_nginx.sh ${reference(nicConfig[0].name).ipConfigurations[0].properties.privateIPAddress} ${reference(nicConfig[2].name).ipConfigurations[0].properties.privateIPAddress} ${reference(nicConfig[4].name).ipConfigurations[0].properties.privateIPAddress}' : 'sh install_nginx_php.sh')
    }
  }
}]

resource vmNameSql_installpostgresql 'Microsoft.Compute/virtualMachines/extensions@2019-07-01' = {
  name: 'abc/installpostgresql'
  location: location
  properties: {
    publisher: 'Microsoft.OSTCExtensions'
    type: 'CustomScriptForLinux'
    typeHandlerVersion: '1.4'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: [
        uri(artifactsLocation, 'conf/postgresql.conf${artifactsLocationSasToken}')
        uri(artifactsLocation, 'scripts/install_postgresql.sh${artifactsLocationSasToken}')
        uri(artifactsLocation, 'conf/pgbouncer.ini${artifactsLocationSasToken}')
      ]
      commandToExecute: 'sh install_postgresql.sh ${numberOfDataDisks}; ${adminPasswordOrKey}' // MODIFIED
    }
  }
}
            ",
              new string[] {
                "[84] Use protectedSettings for commandToExecute secrets. Found possible secret: secure parameter 'adminPasswordOrKey'"
               }
            );
        }

        // Modified from https://docs.microsoft.com/en-us/azure/azure-arc/servers/manage-vm-extensions-template#template-file-for-linux-1
        // CustomScript
        [TestMethod]
        public void If_TypeIsCustomScript_AndContainsSecureParam_AndUsingSettings_ShouldFail()
        {
            CompileAndTest(@"
param vmName string
param location string
param fileUris array

@secure()
param commandToExecute string

resource vmName_CustomScript 'Microsoft.HybridCompute/machines/extensions@2019-08-02-preview' = {
  name: '${vmName}/CustomScript'
  location: location
  properties: {
    publisher: 'Microsoft.Azure.Extensions'
    type: 'CustomScript'
    autoUpgradeMinorVersion: true
    settings: {
      commandToExecute: commandToExecute
      fileUris: fileUris
    }
  }
}
            ",
              new string[] {
                "[17] Use protectedSettings for commandToExecute secrets. Found possible secret: secure parameter 'commandToExecute'"
              }
            );
        }

        // https://docs.microsoft.com/en-us/azure/azure-arc/servers/manage-vm-extensions-template#template-file-for-linux-1
        [TestMethod]
        public void If_UsingProtectedSettings_ShouldPass()
        {
            CompileAndTest(@"
param vmName string
param location string
param fileUris array

@secure()
param commandToExecute string

resource vmName_CustomScript 'Microsoft.HybridCompute/machines/extensions@2019-08-02-preview' = {
  name: '${vmName}/CustomScript'
  location: location
  properties: {
    publisher: 'Microsoft.Azure.Extensions'
    type: 'CustomScript'
    autoUpgradeMinorVersion: true
    settings: {}
    protectedSettings: {
      commandToExecute: commandToExecute
      fileUris: fileUris
    }
  }
}
            ",
              new string[] { }
            );
        }

        // Modified from https://docs.microsoft.com/en-us/azure/azure-arc/servers/manage-vm-extensions-template#template-file-for-windows-1
        [TestMethod]
        public void If_DoesNotContainSecureParam_ShouldPass()
        {
            CompileAndTest(@"
param vmName string
param location string
param fileUris string

// @secure() - CHANGED
param arguments string = ''

var UriFileNamePieces = split(fileUris, '/')
var firstFileNameString = UriFileNamePieces[(length(UriFileNamePieces) - 1)]
var firstFileNameBreakString = split(firstFileNameString, '?')
var firstFileName = firstFileNameBreakString[0]

resource customScriptExtension 'Microsoft.HybridCompute/machines/extensions@2019-08-02-preview' = {
  name: '${vmName}/CustomScriptExtension'
  location: location
  properties: {
    publisher: 'Microsoft.Compute'
    type: 'CustomScriptExtension'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: split(fileUris, ' ')
      commandToExecute: 'powershell -ExecutionPolicy Unrestricted -File ${firstFileName} ${arguments}' // CHANGED
    }
  }
}
            ",
              new string[] { }
            );
        }

        // Modified from https://docs.microsoft.com/en-us/azure/azure-arc/servers/manage-vm-extensions-template#template-file-for-windows-1
        [TestMethod]
        public void If_TypeIsNotCustomScriptExtension_ShouldPass()
        {
            CompileAndTest(@"
param vmName string
param location string
param fileUris string

@secure()
param arguments string = ''

var UriFileNamePieces = split(fileUris, '/')
var firstFileNameString = UriFileNamePieces[(length(UriFileNamePieces) - 1)]
var firstFileNameBreakString = split(firstFileNameString, '?')
var firstFileName = firstFileNameBreakString[0]

resource customScriptExtension 'Microsoft.HybridCompute/machines/extensions@2019-08-02-preview' = {
  name: '${vmName}/CustomScriptExtension'
  location: location
  properties: {
    publisher: 'Microsoft.Compute'
    type: 'SomeOtherExtensionType'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: split(fileUris, ' ')
      commandToExecute: 'powershell -ExecutionPolicy Unrestricted -File ${firstFileName} ${arguments}'
    }
  }
}
            ",
              new string[] { }
            );
        }

        // Modified from https://docs.microsoft.com/en-us/azure/azure-arc/servers/manage-vm-extensions-template#template-file-for-windows-1
        // CustomScriptExtension
        [TestMethod]
        public void If_DoesNotHaveCommandToExecute_ShouldPass()
        {
            CompileAndTest(@"
param vmName string
param location string
param fileUris string

@secure()
param arguments string = ''

var UriFileNamePieces = split(fileUris, '/')
var firstFileNameString = UriFileNamePieces[(length(UriFileNamePieces) - 1)]
var firstFileNameBreakString = split(firstFileNameString, '?')
var firstFileName = firstFileNameBreakString[0]

resource customScriptExtension 'Microsoft.HybridCompute/machines/extensions@2019-08-02-preview' = {
  name: '${vmName}/CustomScriptExtension'
  location: location
  properties: {
    publisher: 'Microsoft.Compute'
    type: 'CustomScriptExtension'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: split(fileUris, ' ')
      // commandToExecute: 'powershell -ExecutionPolicy Unrestricted -File ${firstFileName} ${arguments}' // MODIFIED
    }
  }
}
            ",
              new string[] { }
            );
        }

        [TestMethod]
        public void If_UsesListFunctionDirectly_ShouldFail()
        {
            CompileAndTest(@"
param vmName string
param location string
param fileUris string
param maharaCommon object
param storageAccountId string
param siteFQDN string

resource customScriptExtension 'Microsoft.HybridCompute/machines/extensions@2019-08-02-preview' = {
  name: '${vmName}/CustomScriptExtension'
  location: location
  properties: {
    publisher: 'Microsoft.Compute'
    type: 'CustomScriptExtension'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: split(fileUris, ' ')
      commandToExecute: 'bash ${maharaCommon.webServerSetupScriptFilename} ${maharaCommon.gfsNameRoot}0 data ${maharaCommon.siteURL} ${maharaCommon.httpsTermination} controller-vm-${maharaCommon.resourcesPrefix} ${maharaCommon.webServerType} ${maharaCommon.fileServerType} ${maharaCommon.storageAccountName} ${listKeys(storageAccountId, '2021-02-01').keys[0].value} ${maharaCommon.ctlrVmName} ${maharaCommon.htmlLocalCopySwitch} ${siteFQDN}'
    }
  }
}
            ",
              new string[] {
                "[18] Use protectedSettings for commandToExecute secrets. Found possible secret: function 'listKeys'"
              }
            );
        }

        [TestMethod]
        public void If_UsesListFunctionMethod_ShouldFail()
        {
            CompileAndTest(@"
param vmName string
param location string
param fileUris string

resource stg 'Microsoft.Storage/storageAccounts@2021-06-01' existing = {
  name: 'name'
}

resource customScriptExtension 'Microsoft.HybridCompute/machines/extensions@2019-08-02-preview' = {
  name: '${vmName}/CustomScriptExtension'
  location: location
  properties: {
    publisher: 'Microsoft.Compute'
    type: 'CustomScriptExtension'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: split(fileUris, ' ')
      commandToExecute: 'bash ${stg.listKeys().keys[0].value}'
    }
  }
}
            ",
              new string[] {
                "[19] Use protectedSettings for commandToExecute secrets. Found possible secret: function 'listKeys'"
              }
            );
        }

    }
}
