// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Linq;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ParentPropertyResourceTests
    {
        [TestMethod]
        public void Parent_property_formats_names_and_dependsOn_correctly()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
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

output subnet1prefix string = subnet1.properties.addressPrefix
output subnet1name string = subnet1.name
output subnet1type string = subnet1.type
output subnet1id string = subnet1.id
");

            using (new AssertionScope())
            {
                diags.Should().BeEmpty();

                template.Should().HaveValueAtPath("$.resources[0].name", "myVnet");
                template.Should().NotHaveValueAtPath("$.resources[0].dependsOn");

                template.Should().HaveValueAtPath("$.resources[1].name", "[format('{0}/{1}', 'myVnet', 'subnet1')]");
                template.Should().HaveValueAtPath("$.resources[1].dependsOn", new JArray { "[resourceId('Microsoft.Network/virtualNetworks', 'myVnet')]" });

                template.Should().HaveValueAtPath("$.resources[2].name", "[format('{0}/{1}', 'myVnet', 'subnet2')]");
                template.Should().HaveValueAtPath("$.resources[1].dependsOn", new JArray { "[resourceId('Microsoft.Network/virtualNetworks', 'myVnet')]" });

                template.Should().HaveValueAtPath("$.outputs['subnet1prefix'].value", "[reference(resourceId('Microsoft.Network/virtualNetworks/subnets', 'myVnet', 'subnet1')).addressPrefix]");
                template.Should().HaveValueAtPath("$.outputs['subnet1name'].value", "subnet1");
                template.Should().HaveValueAtPath("$.outputs['subnet1type'].value", "Microsoft.Network/virtualNetworks/subnets");
                template.Should().HaveValueAtPath("$.outputs['subnet1id'].value", "[resourceId('Microsoft.Network/virtualNetworks/subnets', 'myVnet', 'subnet1')]");
            }
        }
    }
}