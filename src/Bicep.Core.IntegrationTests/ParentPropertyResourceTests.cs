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
                template.Should().HaveValueAtPath("$.resources[2].dependsOn", new JArray { "[resourceId('Microsoft.Network/virtualNetworks', 'myVnet')]" });

                template.Should().HaveValueAtPath("$.outputs['subnet1prefix'].value", "[reference(resourceId('Microsoft.Network/virtualNetworks/subnets', 'myVnet', 'subnet1')).addressPrefix]");
                template.Should().HaveValueAtPath("$.outputs['subnet1name'].value", "subnet1");
                template.Should().HaveValueAtPath("$.outputs['subnet1type'].value", "Microsoft.Network/virtualNetworks/subnets");
                template.Should().HaveValueAtPath("$.outputs['subnet1id'].value", "[resourceId('Microsoft.Network/virtualNetworks/subnets', 'myVnet', 'subnet1')]");
            }
        }

        [TestMethod]
        public void Parent_property_works_with_extension_resources()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
resource res1 'Microsoft.Rp1/resource1@2020-06-01' = {
  name: 'res1'
}

resource res1child 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
  parent: res1
  name: 'child1'
}

resource res2 'Microsoft.Rp2/resource2@2020-06-01' = {
  scope: res1child
  name: 'res2'
}

resource res2child 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
  parent: res2
  name: 'child2'
}

output res2childprop string = res2child.properties.someProp
output res2childname string = res2child.name
output res2childtype string = res2child.type
output res2childid string = res2child.id
");

            using (new AssertionScope())
            {
                diags.Where(x => x.Code != "BCP081").Should().BeEmpty();

                template.Should().HaveValueAtPath("$.resources[0].name", "res1");
                template.Should().NotHaveValueAtPath("$.resources[0].dependsOn");

                template.Should().HaveValueAtPath("$.resources[1].name", "[format('{0}/{1}', 'res1', 'child1')]");
                template.Should().HaveValueAtPath("$.resources[1].dependsOn", new JArray { "[resourceId('Microsoft.Rp1/resource1', 'res1')]" });

                template.Should().HaveValueAtPath("$.resources[2].name", "res2");
                template.Should().HaveValueAtPath("$.resources[2].dependsOn", new JArray { "[resourceId('Microsoft.Rp1/resource1/child1', 'res1', 'child1')]" });

                template.Should().HaveValueAtPath("$.resources[3].name", "[format('{0}/{1}', 'res2', 'child2')]");
                template.Should().HaveValueAtPath("$.resources[3].dependsOn", new JArray { "[extensionResourceId(resourceId('Microsoft.Rp1/resource1/child1', 'res1', 'child1'), 'Microsoft.Rp2/resource2', 'res2')]" });

                template.Should().HaveValueAtPath("$.outputs['res2childprop'].value", "[reference(resourceId('Microsoft.Rp2/resource2/child2', 'res2', 'child2')).someProp]");
                template.Should().HaveValueAtPath("$.outputs['res2childname'].value", "child2");
                template.Should().HaveValueAtPath("$.outputs['res2childtype'].value", "Microsoft.Rp2/resource2/child2");
                template.Should().HaveValueAtPath("$.outputs['res2childid'].value", "[resourceId('Microsoft.Rp2/resource2/child2', 'res2', 'child2')]");
            }
        }

        [TestMethod]
        public void Parent_property_works_with_existing_resources()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
resource res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
  name: 'res1'
}

resource child1 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
  parent: res1
  name: 'child1'
}

output res1childprop string = child1.properties.someProp
output res1childname string = child1.name
output res1childtype string = child1.type
output res1childid string = child1.id
");

            using (new AssertionScope())
            {
                diags.Where(x => x.Code != "BCP081").Should().BeEmpty();

                // child1
                template.Should().HaveValueAtPath("$.resources[0].name", "[format('{0}/{1}', 'res1', 'child1')]");
                template.Should().HaveValueAtPath("$.resources[0].dependsOn", new JArray());

                template.Should().NotHaveValueAtPath("$.resources[1]");

                template.Should().HaveValueAtPath("$.outputs['res1childprop'].value", "[reference(resourceId('Microsoft.Rp1/resource1/child1', 'res1', 'child1')).someProp]");
                template.Should().HaveValueAtPath("$.outputs['res1childname'].value", "child1");
                template.Should().HaveValueAtPath("$.outputs['res1childtype'].value", "Microsoft.Rp1/resource1/child1");
                template.Should().HaveValueAtPath("$.outputs['res1childid'].value", "[resourceId('Microsoft.Rp1/resource1/child1', 'res1', 'child1')]");
            }
        }

        [TestMethod]
        public void Parent_property_formats_references_correctly_for_existing_resources()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
resource res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
  scope: tenant()
  name: 'res1'
}

resource child1 'Microsoft.Rp1/resource1/child1@2020-06-01' existing = {
  parent: res1
  name: 'child1'
}

output res1childprop string = child1.properties.someProp
output res1childname string = child1.name
output res1childtype string = child1.type
output res1childid string = child1.id
");

            using (new AssertionScope())
            {
                diags.Where(x => x.Code != "BCP081").Should().BeEmpty();

                template.Should().NotHaveValueAtPath("$.resources[0]");

                // TODO: this should be a tenant resource reference
                template.Should().HaveValueAtPath("$.outputs['res1childprop'].value", "[reference(resourceId('Microsoft.Rp1/resource1/child1', 'res1', 'child1'), '2020-06-01').someProp]");
                template.Should().HaveValueAtPath("$.outputs['res1childname'].value", "child1");
                template.Should().HaveValueAtPath("$.outputs['res1childtype'].value", "Microsoft.Rp1/resource1/child1");
                // TODO: this should be a tenant resourceId
                template.Should().HaveValueAtPath("$.outputs['res1childid'].value", "[resourceId('Microsoft.Rp1/resource1/child1', 'res1', 'child1')]");
            }
        }

        [TestMethod]
        public void Parent_property_blocks_existing_parents_at_different_scopes()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
resource res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
  scope: tenant()
  name: 'res1'
}

resource child1 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
  parent: res1
  name: 'child1'
}
");

            using (new AssertionScope())
            {
                // TODO: this should raise an error as cross-scope deployment should be blocked
                template.Should().NotHaveValue();
                diags.Where(x => x.Code != "BCP081").Should().NotBeEmpty();
            }
        }

        [TestMethod]
        public void Parent_property_blocks_scope_on_child_resources()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
resource res1 'Microsoft.Rp1/resource1@2020-06-01' = {
  name: 'res1'
}

resource res2 'Microsoft.Rp2/resource2@2020-06-01' = {
  name: 'res2'
}

resource res2child 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
  scope: res1
  parent: res2
  name: 'child2'
}
");

            using (new AssertionScope())
            {
                // TODO: this should raise an error as setting scope + parent should be blocked
                template.Should().NotHaveValue();
                diags.Where(x => x.Code != "BCP081").Should().NotBeEmpty();
            }
        }
    }
}