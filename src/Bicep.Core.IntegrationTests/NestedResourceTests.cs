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
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingFactory.CreateFromText(program));
            var model = compilation.GetEntrypointSemanticModel();

            model.GetAllDiagnostics().Should().BeEmpty();

            var expected = new []
            {
                new { name = "child", type = "My.RP/parentType/childType@2020-01-01", },
                new { name = "parent", type = "My.RP/parentType@2020-01-01", },
                new { name = "sibling", type = "My.RP/parentType/childType@2020-01-02", },
            };

            model.Root.GetAllResourceDeclarations()
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

            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingFactory.CreateFromText(program));
            var model = compilation.GetEntrypointSemanticModel();

            // The property "resource" is not allowed ...
            model.GetAllDiagnostics().Should().HaveCount(1);
            model.GetAllDiagnostics().Single().Should().HaveCodeAndSeverity("BCP038", DiagnosticLevel.Error);

            var expected = new []
            {
                new { name = "child", type = "My.RP/parentType/childType@2020-01-01", },
                new { name = "parent", type = "My.RP/parentType@2020-01-01", },
            };

            model.Root.GetAllResourceDeclarations()
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
      style: parent:child.properties.style
      size: parent.properties.size
      temperatureC: child:grandchild.properties.temperature
      temperatureF: parent:child:grandchild.properties.temperature
    }
  }
}

output fromChild string = parent:child.properties.style
output fromGrandchild string = parent:child:grandchild.properties.style
";

            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingFactory.CreateFromText(program));
            var model = compilation.GetEntrypointSemanticModel();

            model.GetAllDiagnostics().Should().BeEmpty();

            var parent = model.Root.GetAllResourceDeclarations().Single(r => r.Name == "parent");
            var references = model.FindReferences(parent);
            references.Should().HaveCount(6);

            var child = model.Root.GetAllResourceDeclarations().Single(r => r.Name == "child");
            references = model.FindReferences(child);
            references.Should().HaveCount(6);

            var grandchild = model.Root.GetAllResourceDeclarations().Single(r => r.Name == "grandchild");
            references = model.FindReferences(grandchild);
            references.Should().HaveCount(4);

            var sibling = model.Root.GetAllResourceDeclarations().Single(r => r.Name == "sibling");
            references = model.FindReferences(sibling);
            references.Should().HaveCount(1);

            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel(), BicepTestConstants.DevAssemblyFileVersion);
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

output fromVariable string = notResource:child.properties.style
output fromChildInvalid string = parent:child2.properties.style
output fromGrandchildInvalid string = parent:child:cousin.properties.temperature
";

            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingFactory.CreateFromText(program));
            var model = compilation.GetEntrypointSemanticModel();

            model.GetAllDiagnostics().Should().HaveDiagnostics(new []{
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

            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingFactory.CreateFromText(program));
            var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();
            diagnostics.Should().HaveDiagnostics(new[] {
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

            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingFactory.CreateFromText(program));
            var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();
            diagnostics.Should().HaveDiagnostics(new[] {
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

            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingFactory.CreateFromText(program));
            var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();
            diagnostics.Should().HaveDiagnostics(new[] {
                ("BCP029", DiagnosticLevel.Error, "The resource type is not valid. Specify a valid resource type of format \"<provider>/<types>@<apiVersion>\"."),
                ("BCP157", DiagnosticLevel.Error, "The resource type cannot be determined due to an error in containing resource \"parent\"."),
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

            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingFactory.CreateFromText(program));
            var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();
            diagnostics.Should().HaveDiagnostics(new[] {
                ("BCP156", DiagnosticLevel.Error, "The resource type segment \"My.RP/parentType/childType@2020-01-01\" is invalid. Nested resources must specify a single type segment, and optionally can specify an api version using the format \"<type>@<apiVersion>\"."),
                ("BCP157", DiagnosticLevel.Error, "The resource type cannot be determined due to an error in containing resource \"child\"."),
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

            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingFactory.CreateFromText(program));
            compilation.GetEntrypointSemanticModel().GetAllDiagnostics().Should().HaveDiagnostics(new[] {
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

            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingFactory.CreateFromText(program));
            compilation.GetEntrypointSemanticModel().GetAllDiagnostics().Should().HaveDiagnostics(new[] {
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

            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingFactory.CreateFromText(program));
            var model = compilation.GetEntrypointSemanticModel();
            model.GetAllDiagnostics().Should().BeEmpty();

            var parent = model.Root.GetAllResourceDeclarations().Single(r => r.Name == "parent");
            model.ResourceAncestors.GetAncestors(parent).Should().BeEmpty();

            var child = model.Root.GetAllResourceDeclarations().Single(r => r.Name == "child");
            model.ResourceAncestors.GetAncestors(child).Should().Equal(new []{ parent, });

            var grandchild = model.Root.GetAllResourceDeclarations().Single(r => r.Name == "grandchild");
            model.ResourceAncestors.GetAncestors(grandchild).Should().Equal(new []{ parent, child, }); // order matters
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

            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingFactory.CreateFromText(program));
            var model = compilation.GetEntrypointSemanticModel();
            model.GetAllDiagnostics().Should().BeEmpty();

            var parent = model.Root.GetAllResourceDeclarations().Single(r => r.Name == "parent");
            model.ResourceAncestors.GetAncestors(parent).Should().BeEmpty();

            var child = model.Root.GetAllResourceDeclarations().Single(r => r.Name == "child");
            model.ResourceAncestors.GetAncestors(child).Should().Equal(new []{ parent, });

            var childGrandChild = (ResourceSymbol)model.GetSymbolInfo(child.DeclaringResource.GetBody().Resources.Single())!;
            model.ResourceAncestors.GetAncestors(childGrandChild).Should().Equal(new []{ parent, child, });

            var sibling = model.Root.GetAllResourceDeclarations().Single(r => r.Name == "sibling");
            model.ResourceAncestors.GetAncestors(child).Should().Equal(new []{ parent, });

            var siblingGrandChild = (ResourceSymbol)model.GetSymbolInfo(sibling.DeclaringResource.GetBody().Resources.Single())!;
            model.ResourceAncestors.GetAncestors(siblingGrandChild).Should().Equal(new []{ parent, sibling, });
        }

        [TestMethod] // Should turn into positive test when support is added.
        public void NestedResources_cannot_appear_inside_loops()
        {
            var program = @"
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
";

            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingFactory.CreateFromText(program));
            var model = compilation.GetEntrypointSemanticModel();
            compilation.GetEntrypointSemanticModel().GetAllDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP160", DiagnosticLevel.Error, "A nested resource cannot appear inside of a resource with a for-expression."),
            });
        }

        [TestMethod]
        public void NestedResources_can_have_loops()
        {
            var program = @"
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

output loopy string = parent:child[0].name
";

            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingFactory.CreateFromText(program));
            var model = compilation.GetEntrypointSemanticModel();
            compilation.GetEntrypointSemanticModel().GetAllDiagnostics().Should().BeEmpty();

            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel(), BicepTestConstants.DevAssemblyFileVersion);
            using var outputStream = new MemoryStream();
            emitter.Emit(outputStream);

            outputStream.Seek(0L, SeekOrigin.Begin);
            var text = Encoding.UTF8.GetString(outputStream.GetBuffer());
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

output hmmmm string = parent:child.properties
";

            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingFactory.CreateFromText(program));
            var model = compilation.GetEntrypointSemanticModel();

            var output = model.Root.OutputDeclarations.Single();
            var expression = output.DeclaringOutput.Value;
            var type = model.GetDeclaredType(expression);
            type.Should().BeOfType<ObjectType>();
        }

        [TestMethod]
        public void NestedResources_provides_correct_error_for_resource_access_with_broken_body()
        {
            var program = @"
resource broken 'Microsoft.Network/virtualNetworks@2020-06-01' = 

output foo string = broken:fake
";

            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingFactory.CreateFromText(program));
            var model = compilation.GetEntrypointSemanticModel();
            model.GetAllDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP118", DiagnosticLevel.Error, "Expected the \"{\" character, the \"[\" character, or the \"if\" keyword at this location."),
                ("BCP159", DiagnosticLevel.Error, "The resource \"broken\" does not contain a nested resource named \"fake\". Known nested resources are: \"(none)\"."),
            });
        }
    }
}