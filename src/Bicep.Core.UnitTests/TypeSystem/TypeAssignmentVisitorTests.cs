// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Az;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.TypeSystem
{
    [TestClass]
    public class TypeAssignmentVisitorTests
    {
        [TestMethod]
        public void Foo()
        {
            var program = SyntaxFactory.CreateFromText(@"
//param pass string {
//  metadata: {
//    description: 'test'
//  }
//}

//resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//  name: 'v'
//  location: 'eastus'
//  properties: {
//    subnets: [
//      {
        
//      }
//    ]
//  }
//}

//resource discriminatorKeyMissing 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//  // #completionTest(0,1,2) -> discriminatorProperty
  
//}

resource discriminatorKeySetOne 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  kind: 'AzureCLI'
  // #completionTest(0,1,2) -> deploymentScriptCliProperties
  
}


//resource foo 'Microsoft.Storage/storageAccounts@2019-07-01' = {
//  name: 'test'
//}
");

            var compilation = new Compilation(new AzResourceTypeProvider(), program);
            var model = compilation.GetSemanticModel();

            var diagnostics = model.GetAllDiagnostics();
        }
    }
}
