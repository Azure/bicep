// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.IO;
using System.Linq;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ParentPropertyResourceTests
    {
        [TestMethod]
        public void NestedResources_symbols_are_bound()
        {
            var program = @"
resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  location: resourceGroup().location
  name: 'myVnet'
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/20'
      ]
    }
  }
}

resource subnet1 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
  parent: vnet
  name: 'subnet1'
  properties: {
    addressPrefix: '10.0.0.0/24'
  }
}

resource subnet2 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
  parent: vnet
  name: 'subnet2'
  properties: {
    addressPrefix: '10.0.1.0/24'
  }
}
";

            var (template, diags, _) = CompilationHelper.Compile(program);

            using (new AssertionScope())
            {
                template!.Should().NotBeNull();
                diags.Should().BeEmpty();

                template!.SelectToken("$.resources[0].name")!.Should().DeepEqual("myVnet");
                template!.SelectToken("$.resources[1].name")!.Should().DeepEqual("[format('{0}/{1}', 'myVnet', 'subnet1')]");
                template!.SelectToken("$.resources[2].name")!.Should().DeepEqual("[format('{0}/{1}', 'myVnet', 'subnet2')]");
            }
        }
    }
}