// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Diagnostics;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using static System.Formats.Asn1.AsnWriter;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class NestedResourceTests
    {
        [TestMethod]
        public void NestedResources_symbols_are_bound()
        {
            var program = @"
resource parent 'My.RP/parentType@2020-01-01' = {
  name: 'parent'
  properties: {
    size: 'large'
  }

  resource child 'childType' = {
    name: 'child'
    properties: {
      style: 'very cool'
    }
  }

  resource sibling 'childType@2020-01-02' = {
    name: 'sibling'
    properties: {
      style: child.properties.style
      size: parent.properties.size
    }
  }
}
";

            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateFromText(program, BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);
            var model = compilation.GetEntrypointSemanticModel();

            model.GetAllDiagnostics().ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().BeEmpty();

            var expected = new[]
            {
                new { name = "child", type = "My.RP/parentType/childType@2020-01-01", },
                new { name = "parent", type = "My.RP/parentType@2020-01-01", },
                new { name = "sibling", type = "My.RP/parentType/childType@2020-01-02", },
            };

            model.AllResources.Select(x => x.Symbol)
              .Select(s => new { name = s.Name, type = (s.Type as ResourceType)?.TypeReference.FormatName(), })
              .OrderBy(n => n.name)
              .Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void NestedResources_resource_can_contain_property_called_resource()
        {
            var program = @"
resource parent 'My.RP/parentType@2020-01-01' = {
  name: 'parent'
  properties: {
    size: 'large'
  }
  resource: 'yes please'

  resource child 'childType' = {
    name: 'child'
    properties: {
      style: 'very cool'
    }
  }
}
";

            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateFromText(program, BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);
            var model = compilation.GetEntrypointSemanticModel();

            // The property "resource" is not allowed ...
            model.GetAllDiagnostics().ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().HaveCount(1);
            model.GetAllDiagnostics().ExcludingLinterDiagnostics().ExcludingMissingTypes().Single().Should().HaveCodeAndSeverity("BCP037", DiagnosticLevel.Error);

            var expected = new[]
            {
                new { name = "child", type = "My.RP/parentType/childType@2020-01-01", },
                new { name = "parent", type = "My.RP/parentType@2020-01-01", },
            };

            model.AllResources.Select(x => x.Symbol)
              .Select(s => new { name = s.Name, type = (s.Type as ResourceType)?.TypeReference.FormatName(), })
              .OrderBy(n => n.name)
              .Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void NestedResources_valid_resource_references()
        {
            var program = @"
resource parent 'My.RP/parentType@2020-01-01' = {
  name: 'parent'
  properties: {
    size: 'large'
  }

  resource child 'childType' = {
    name: 'child'
    properties: {
      style: 'very cool'
    }

    resource grandchild 'grandchildType' = {
      name: 'grandchild'
      properties: {
        temperature: 'ice-cold'
      }
    }
  }

  resource sibling 'childType@2020-01-02' = {
    name: 'sibling'
    properties: {
      style: parent::child.properties.style
      size: parent.properties.size
      temperatureC: child::grandchild.properties.temperature
      temperatureF: parent::child::grandchild.properties.temperature
    }
  }
}

output fromChild string = parent::child.properties.style
output fromGrandchild string = parent::child::grandchild.properties.style
";

            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateFromText(program, BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);
            var model = compilation.GetEntrypointSemanticModel();

            model.GetAllDiagnostics().ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().BeEmpty();

            var parent = model.AllResources.Select(x => x.Symbol).Single(r => r.Name == "parent");
            var references = model.FindReferences(parent);
            references.Should().HaveCount(6);

            var child = model.AllResources.Select(x => x.Symbol).Single(r => r.Name == "child");
            references = model.FindReferences(child);
            references.Should().HaveCount(6);

            var grandchild = model.AllResources.Select(x => x.Symbol).Single(r => r.Name == "grandchild");
            references = model.FindReferences(grandchild);
            references.Should().HaveCount(4);

            var sibling = model.AllResources.Select(x => x.Symbol).Single(r => r.Name == "sibling");
            references = model.FindReferences(sibling);
            references.Should().HaveCount(1);

            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel(), BicepTestConstants.EmitterSettings);
            using var outputStream = new MemoryStream();
            emitter.Emit(outputStream);

            outputStream.Seek(0L, SeekOrigin.Begin);
            var text = Encoding.UTF8.GetString(outputStream.GetBuffer());
        }

        [TestMethod]
        public void NestedResources_invalid_resource_references()
        {
            var program = @"
var notResource = 'hi'
resource parent 'My.RP/parentType@2020-01-01' = {
  name: 'parent'
  properties: {
    size: 'large'
  }

  resource child 'childType' = {
    name: 'child'
    properties: {
      style: 'very cool'
    }

    resource grandchild 'grandchildType' = {
      name: 'grandchild'
      properties: {
        temperature: 'ice-cold'
      }
    }
  }
}

output fromVariable string = notResource::child.properties.style
output fromChildInvalid string = parent::child2.properties.style
output fromGrandchildInvalid string = parent::child::cousin.properties.temperature
";

            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateFromText(program, BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);
            var model = compilation.GetEntrypointSemanticModel();

            model.GetAllDiagnostics().ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().HaveDiagnostics(new[]{
                ("BCP158", DiagnosticLevel.Error, "Cannot access nested resources of type \"'hi'\". A resource type is required."),
                ("BCP159", DiagnosticLevel.Error, "The resource \"parent\" does not contain a nested resource named \"child2\". Known nested resources are: \"child\"."),
                ("BCP159", DiagnosticLevel.Error, "The resource \"child\" does not contain a nested resource named \"cousin\". Known nested resources are: \"grandchild\"."),
            });
        }

        [TestMethod]
        public void NestedResources_child_cannot_be_referenced_outside_of_scope()
        {
            var program = @"
resource parent 'My.RP/parentType@2020-01-01' = {
  name: 'parent'
  properties: {
  }

  resource child 'childType' = {
    name: 'child'
    properties: {
      style: 'very cool'
    }
  }
}

resource other 'My.RP/parentType@2020-01-01' = {
  name: 'other'
  properties: {
    style: child.properties.style
  }
}
";

            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateFromText(program, BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);
            var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();
            diagnostics.ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().HaveDiagnostics(new[] {
                ("BCP057", DiagnosticLevel.Error, "The name \"child\" does not exist in the current context."),
            });
        }

        [TestMethod]
        public void NestedResources_child_cannot_specify_qualified_type()
        {
            var program = @"
resource parent 'My.RP/parentType@2020-01-01' = {
  name: 'parent'
  properties: {
  }

  resource child 'My.RP/parentType/childType@2020-01-01' = {
    name: 'child'
    properties: {
      style: 'very cool'
    }
  }
}
";

            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateFromText(program, BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);
            var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();
            diagnostics.ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().HaveDiagnostics(new[] {
                ("BCP156", DiagnosticLevel.Error, "The resource type segment \"My.RP/parentType/childType@2020-01-01\" is invalid. Nested resources must specify a single type segment, and optionally can specify an api version using the format \"<type>@<apiVersion>\"."),
            });
        }

        [TestMethod]
        public void NestedResources_error_in_base_type()
        {
            var program = @"
resource parent 'My.RP/parentType@invalid-version' = {
  name: 'parent'
  properties: {
  }

  resource child 'My.RP/parentType/childType@2020-01-01' = {
    name: 'child'
    properties: {
      style: 'very cool'
    }
  }
}
";

            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateFromText(program, BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);
            var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();
            diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP029", DiagnosticLevel.Error, "The resource type is not valid. Specify a valid resource type of format \"<types>@<apiVersion>\"."),
                ("BCP157", DiagnosticLevel.Error, "The resource type cannot be determined due to an error in the containing resource."),
            });
        }

        [TestMethod]
        public void NestedResources_error_in_parent_type()
        {
            var program = @"
resource parent 'My.RP/parentType@2020-01-01' = {
  name: 'parent'
  properties: {
  }

  // Error here
  resource child 'My.RP/parentType/childType@2020-01-01' = {
    name: 'child'
    properties: {
    }

    resource grandchild 'granchildType' = {
      name: 'grandchild'
      properties: {
      }
    }
  }
}
";

            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateFromText(program, BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);
            var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();
            diagnostics.ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().HaveDiagnostics(new[] {
                ("BCP156", DiagnosticLevel.Error, "The resource type segment \"My.RP/parentType/childType@2020-01-01\" is invalid. Nested resources must specify a single type segment, and optionally can specify an api version using the format \"<type>@<apiVersion>\"."),
                ("BCP157", DiagnosticLevel.Error, "The resource type cannot be determined due to an error in the containing resource."),
            });
        }

        [TestMethod]
        public void NestedResources_child_cycle_is_detected_correctly()
        {
            var program = @"
resource parent 'My.RP/parentType@2020-01-01' = {
  name: 'parent'
  properties: {
    style: child.properties.style
  }

  resource child 'childType' = {
    name: 'child'
    properties: {
      style: 'very cool'
    }
  }
}
";

            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateFromText(program, BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);
            compilation.GetEntrypointSemanticModel().GetAllDiagnostics().ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().HaveDiagnostics(new[] {
                ("BCP080", DiagnosticLevel.Error, "The expression is involved in a cycle (\"child\" -> \"parent\")."),
            });
        }

        [TestMethod] // With more than one level of nesting the name just isn't visible.
        public void NestedResources_grandchild_cycle_results_in_binding_failure()
        {
            var program = @"
resource parent 'My.RP/parentType@2020-01-01' = {
  name: 'parent'
  properties: {
    style: grandchild.properties.style
  }

  resource child 'childType' = {
    name: 'child'
    properties: {
    }

    resource grandchild 'grandchildType' = {
      name: 'grandchild'
      properties: {
        style: 'very cool'
      }
    }
  }
}
";

            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateFromText(program, BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);
            compilation.GetEntrypointSemanticModel().GetAllDiagnostics().ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().HaveDiagnostics(new[] {
                ("BCP057", DiagnosticLevel.Error, "The name \"grandchild\" does not exist in the current context."),
            });
        }

        [TestMethod]
        public void NestedResources_ancestors_are_detected()
        {
            var program = @"
resource parent 'My.RP/parentType@2020-01-01' = {
  name: 'parent'
  properties: {
  }

  resource child 'childType' = {
    name: 'child'
    properties: {
    }

    resource grandchild 'grandchildType' = {
      name: 'grandchild'
      properties: {
      }
    }
  }
}
";

            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateFromText(program, BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);
            var model = compilation.GetEntrypointSemanticModel();
            model.GetAllDiagnostics().ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().BeEmpty();

            var parent = model.ResourceMetadata.TryLookup(model.AllResources.Select(x => x.Symbol).Single(r => r.Name == "parent").DeclaringSyntax)!;
            model.ResourceAncestors.GetAncestors(parent).Should().BeEmpty();

            var child = model.ResourceMetadata.TryLookup(model.AllResources.Select(x => x.Symbol).Single(r => r.Name == "child").DeclaringSyntax)!;
            model.ResourceAncestors.GetAncestors(child).Select(x => x.Resource).Should().Equal(new[] { parent, });

            var grandchild = model.ResourceMetadata.TryLookup(model.AllResources.Select(x => x.Symbol).Single(r => r.Name == "grandchild").DeclaringSyntax)!;
            model.ResourceAncestors.GetAncestors(grandchild).Select(x => x.Resource).Should().Equal(new[] { parent, child, }); // order matters
        }

        [TestMethod]
        public void NestedResources_scopes_isolate_names()
        {
            var program = @"
resource parent 'My.RP/parentType@2020-01-01' = {
  name: 'parent'
  properties: {
  }

  resource child 'childType' = {
    name: 'child'
    properties: {
    }

    resource grandchild 'grandchildType' = {
      name: 'grandchild'
      properties: {
      }
    }
  }

  resource sibling 'childType' = {
    name: 'sibling'
    properties: {
    }

    resource grandchild 'grandchildType' = {
      name: 'grandchild'
      properties: {
      }
    }
  }
}
";

            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateFromText(program, BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);
            var model = compilation.GetEntrypointSemanticModel();
            model.GetAllDiagnostics().ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().BeEmpty();

            var parent = model.ResourceMetadata.TryLookup(model.AllResources.Select(x => x.Symbol).Single(r => r.Name == "parent").DeclaringSyntax)!;
            model.ResourceAncestors.GetAncestors(parent).Should().BeEmpty();

            var child = model.ResourceMetadata.TryLookup(model.AllResources.Select(x => x.Symbol).Single(r => r.Name == "child").DeclaringSyntax)!;
            model.ResourceAncestors.GetAncestors(child).Select(x => x.Resource).Should().Equal(new[] { parent, });

            var childGrandChild = model.ResourceMetadata.TryLookup(child.Symbol.DeclaringResource.GetBody().Resources.Single())!;
            model.ResourceAncestors.GetAncestors(childGrandChild).Select(x => x.Resource).Should().Equal(new[] { parent, child, });

            var sibling = model.ResourceMetadata.TryLookup(model.AllResources.Select(x => x.Symbol).Single(r => r.Name == "sibling").DeclaringSyntax)!;
            model.ResourceAncestors.GetAncestors(child).Select(x => x.Resource).Should().Equal(new[] { parent, });

            var siblingGrandChild = model.ResourceMetadata.TryLookup(sibling.Symbol.DeclaringResource.GetBody().Resources.Single())!;
            model.ResourceAncestors.GetAncestors(siblingGrandChild).Select(x => x.Resource).Should().Equal(new[] { parent, sibling, });
        }

        [TestMethod] // Should turn into positive test when support is added.
        public void NestedResources_cannot_appear_inside_loops()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
var items = [
  'a'
  'b'
]
resource parent 'My.RP/parentType@2020-01-01' = [for item in items: {
  name: 'parent'
  properties: {
  }

  resource child 'childType' = {
    name: 'child'
    properties: {
    }
  }
}]
");

            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diags.ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().HaveDiagnostics(new[]
                {
                    ("BCP179", DiagnosticLevel.Warning,"The loop item variable \"item\" must be referenced in at least one of the value expressions of the following properties: \"name\", \"scope\""),
                    ("BCP160", DiagnosticLevel.Error, "A nested resource cannot appear inside of a resource with a for-expression."),
                    ("BCP157", DiagnosticLevel.Error, "The resource type cannot be determined due to an error in the containing resource."),
                });
            }
        }

        [TestMethod]
        public void NestedResources_can_have_loops()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
var items = [
  'a'
  'b'
]
resource parent 'My.RP/parentType@2020-01-01' = {
  name: 'parent'
  properties: {
  }

  resource child 'childType' = [for item in items: {
    name: 'child'
    properties: {
    }
  }]
}

output loopy string = parent::child[0].name
");

            using (new AssertionScope())
            {
                diags.ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().HaveDiagnostics(new[]
                {
                    ("BCP179",DiagnosticLevel.Warning,"The loop item variable \"item\" must be referenced in at least one of the value expressions of the following properties: \"name\", \"scope\"")
                });
                template.Should().NotBeNull();
            }
        }

        [TestMethod]
        public void NestedResources_can_get_declared_type_for_property_completion()
        {
            var program = @"
resource parent 'My.RP/parentType@2020-01-01' = {
  name: 'parent'
  properties: {
  }

  resource child 'childType' = {
    name: 'child'
    properties: {
    }
  }
}

output hmmmm string = parent::child.properties
";

            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateFromText(program, BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);
            var model = compilation.GetEntrypointSemanticModel();

            var output = model.Root.OutputDeclarations.Single();
            var expression = output.DeclaringOutput.Value;
            var type = model.GetDeclaredType(expression);
            type.Should().BeOfType<ObjectType>();
        }

        [TestMethod]
        public void NestedResources_provides_correct_error_for_resource_access_with_broken_body()
        {
            var result = CompilationHelper.Compile(@"
resource broken 'Microsoft.Network/virtualNetworks@2020-06-01' =

output foo string = broken::fake
");

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP118", DiagnosticLevel.Error, "Expected the \"{\" character, the \"[\" character, or the \"if\" keyword at this location."),
                ("BCP159", DiagnosticLevel.Error, "The resource \"broken\" does not contain a nested resource named \"fake\". Known nested resources are: \"(none)\"."),
            });
        }

        [TestMethod]
        public void Nested_resource_formats_names_and_dependsOn_correctly()
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

  resource subnet1 'subnets' = {
    name: 'subnet1'
    properties: {
      addressPrefix: '10.0.0.0/24'
    }
  }

  resource subnet2 'subnets' = {
    name: 'subnet2'
    properties: {
      addressPrefix: '10.0.1.0/24'
    }
  }
}



output subnet1prefix string = vnet::subnet1.properties.addressPrefix
output subnet1name string = vnet::subnet1.name
output subnet1type string = vnet::subnet1.type
output subnet1id string = vnet::subnet1.id
");

            using (new AssertionScope())
            {
                diags.ExcludingLinterDiagnostics().Should().BeEmpty();

                template.Should().HaveValueAtPath("$.resources[0].name", "[format('{0}/{1}', 'myVnet', 'subnet1')]");
                template.Should().HaveValueAtPath("$.resources[0].dependsOn", new JArray { "[resourceId('Microsoft.Network/virtualNetworks', 'myVnet')]" });

                template.Should().HaveValueAtPath("$.resources[1].name", "[format('{0}/{1}', 'myVnet', 'subnet2')]");
                template.Should().HaveValueAtPath("$.resources[1].dependsOn", new JArray { "[resourceId('Microsoft.Network/virtualNetworks', 'myVnet')]" });

                template.Should().HaveValueAtPath("$.resources[2].name", "myVnet");
                template.Should().NotHaveValueAtPath("$.resources[2].dependsOn");

                template.Should().HaveValueAtPath("$.outputs['subnet1prefix'].value", "[reference(resourceId('Microsoft.Network/virtualNetworks/subnets', 'myVnet', 'subnet1')).addressPrefix]");
                template.Should().HaveValueAtPath("$.outputs['subnet1name'].value", "subnet1");
                template.Should().HaveValueAtPath("$.outputs['subnet1type'].value", "Microsoft.Network/virtualNetworks/subnets");
                template.Should().HaveValueAtPath("$.outputs['subnet1id'].value", "[resourceId('Microsoft.Network/virtualNetworks/subnets', 'myVnet', 'subnet1')]");
            }
        }

        [TestMethod]
        public void Nested_resource_works_with_extension_resources()
        {
            var result = CompilationHelper.Compile(@"
resource res1 'Microsoft.Rp1/resource1@2020-06-01' = {
  name: 'res1'

  resource child 'child1' = {
    name: 'child1'
  }
}

resource res2 'Microsoft.Rp2/resource2@2020-06-01' = {
  scope: res1::child
  name: 'res2'

  resource child 'child2' = {
    name: 'child2'
  }
}

output res2childprop string = res2::child.properties.someProp
output res2childname string = res2::child.name
output res2childtype string = res2::child.type
output res2childid string = res2::child.id
");

            result.Diagnostics.ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().BeEmpty();

            // res1
            result.Template.Should().HaveValueAtPath("$.resources[2].name", "res1");
            result.Template.Should().NotHaveValueAtPath("$.resources[2].dependsOn");

            // res1::child1
            result.Template.Should().HaveValueAtPath("$.resources[0].name", "[format('{0}/{1}', 'res1', 'child1')]");
            result.Template.Should().HaveValueAtPath("$.resources[0].dependsOn", new JArray { "[resourceId('Microsoft.Rp1/resource1', 'res1')]" });

            // res2
            result.Template.Should().HaveValueAtPath("$.resources[3].name", "res2");
            result.Template.Should().HaveValueAtPath("$.resources[3].dependsOn", new JArray { "[resourceId('Microsoft.Rp1/resource1/child1', 'res1', 'child1')]" });

            // res2::child2
            result.Template.Should().HaveValueAtPath("$.resources[1].name", "[format('{0}/{1}', 'res2', 'child2')]");
            result.Template.Should().HaveValueAtPath("$.resources[1].dependsOn", new JArray { "[extensionResourceId(resourceId('Microsoft.Rp1/resource1/child1', 'res1', 'child1'), 'Microsoft.Rp2/resource2', 'res2')]" });

            result.Template.Should().HaveValueAtPath("$.outputs['res2childprop'].value", "[reference(extensionResourceId(resourceId('Microsoft.Rp1/resource1/child1', 'res1', 'child1'), 'Microsoft.Rp2/resource2/child2', 'res2', 'child2')).someProp]");
            result.Template.Should().HaveValueAtPath("$.outputs['res2childname'].value", "child2");
            result.Template.Should().HaveValueAtPath("$.outputs['res2childtype'].value", "Microsoft.Rp2/resource2/child2");
            result.Template.Should().HaveValueAtPath("$.outputs['res2childid'].value", "[extensionResourceId(resourceId('Microsoft.Rp1/resource1/child1', 'res1', 'child1'), 'Microsoft.Rp2/resource2/child2', 'res2', 'child2')]");
        }

        [TestMethod]
        public void Nested_resource_works_with_existing_resources()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
resource res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
  name: 'res1'

  resource child 'child1' = {
    name: 'child1'
  }
}

output res1childprop string = res1::child.properties.someProp
output res1childname string = res1::child.name
output res1childtype string = res1::child.type
output res1childid string = res1::child.id
");

            using (new AssertionScope())
            {
                diags.ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().BeEmpty();

                // res1::child1
                template.Should().HaveValueAtPath("$.resources[0].name", "[format('{0}/{1}', 'res1', 'child1')]");
                template.Should().NotHaveValueAtPath("$.resources[0].dependsOn");

                template.Should().NotHaveValueAtPath("$.resources[1]");

                template.Should().HaveValueAtPath("$.outputs['res1childprop'].value", "[reference(resourceId('Microsoft.Rp1/resource1/child1', 'res1', 'child1')).someProp]");
                template.Should().HaveValueAtPath("$.outputs['res1childname'].value", "child1");
                template.Should().HaveValueAtPath("$.outputs['res1childtype'].value", "Microsoft.Rp1/resource1/child1");
                template.Should().HaveValueAtPath("$.outputs['res1childid'].value", "[resourceId('Microsoft.Rp1/resource1/child1', 'res1', 'child1')]");
            }
        }

        [TestMethod]
        public void Nested_resource_formats_references_correctly_for_existing_resources()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
resource res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
  scope: tenant()
  name: 'res1'

  resource child 'child1' existing = {
    name: 'child1'
  }
}

output res1childprop string = res1::child.properties.someProp
output res1childname string = res1::child.name
output res1childtype string = res1::child.type
output res1childid string = res1::child.id
");

            using (new AssertionScope())
            {
                diags.ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().BeEmpty();

                template.Should().NotHaveValueAtPath("$.resources[0]");

                template.Should().HaveValueAtPath("$.outputs['res1childprop'].value", "[reference(tenantResourceId('Microsoft.Rp1/resource1/child1', 'res1', 'child1'), '2020-06-01').someProp]");
                template.Should().HaveValueAtPath("$.outputs['res1childname'].value", "child1");
                template.Should().HaveValueAtPath("$.outputs['res1childtype'].value", "Microsoft.Rp1/resource1/child1");
                template.Should().HaveValueAtPath("$.outputs['res1childid'].value", "[tenantResourceId('Microsoft.Rp1/resource1/child1', 'res1', 'child1')]");
            }
        }

        [DataTestMethod]
        [DataRow("resourceGroup('other')")]
        [DataRow("subscription()")]
        [DataRow("managementGroup('abcdef')")]
        public void Nested_resource_blocks_existing_parents_at_different_scopes(string parentScope)
        {
            var result = CompilationHelper.Compile(@"
resource res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
  scope: " + parentScope + @"
  name: 'res1'

  resource child 'child1' = {
    name: 'child1'
  }
}
");

            result.Diagnostics.ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().HaveDiagnostics(new[] {
                ("BCP165", DiagnosticLevel.Error, "Cannot deploy a resource with ancestor under a different scope. Resource \"res1\" has the \"scope\" property set."),
            });
        }

        [TestMethod]
        public void Nested_resource_allows_existing_parents_at_tenant_scope()
        {
            var result = CompilationHelper.Compile(@"
resource res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
  scope: tenant()
  name: 'res1'

  resource child 'child1' = {
    name: 'child1'
  }
}
");

            result.Diagnostics.ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().BeEmpty();
            result.Template.Should().HaveValueAtPath("$.resources[0].scope", "/");
            result.Template.Should().HaveValueAtPath("$.resources[0].name", "[format('{0}/{1}', 'res1', 'child1')]");
        }

        [TestMethod]
        public void Nested_resource_blocks_scope_on_child_resources()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
resource res1 'Microsoft.Rp1/resource1@2020-06-01' = {
  name: 'res1'
}

resource res2 'Microsoft.Rp2/resource2@2020-06-01' = {
  name: 'res2'

  resource child 'child2' = {
    scope: res1
    name: 'child2'
  }
}
");

            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diags.ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().HaveDiagnostics(new[] {
                  ("BCP164", DiagnosticLevel.Error, "The \"scope\" property is unsupported for a resource with a parent resource. This resource has \"res2\" declared as its parent."),
                });
            }
        }

        [TestMethod]
        public void Nested_resource_detects_invalid_child_resource_literal_names()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
resource res1 'Microsoft.Rp1/resource1@2020-06-01' = {
  name: 'res1'

  resource res2 'child' = {
    name: 'res1/res2'
  }

  resource res3 'child' = {
    name: '${res1.name}/res2'
  }
}
");

            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diags.ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().HaveDiagnostics(new[] {
                  ("BCP170", DiagnosticLevel.Error, "Expected resource name to not contain any \"/\" characters. Child resources with a parent resource reference (via the parent property or via nesting) must not contain a fully-qualified name."),
                  ("BCP170", DiagnosticLevel.Error, "Expected resource name to not contain any \"/\" characters. Child resources with a parent resource reference (via the parent property or via nesting) must not contain a fully-qualified name."),
                });
            }
        }
    }
}
