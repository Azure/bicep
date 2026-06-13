// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Semantics
{
    [TestClass]
    public class ResourceDeclarationDiagnosticTests
    {
        [TestMethod]
        public void Missing_resource_type_in_resource_declaration_emits_only_first_type_diagnostic()
        {
            var result = CompilationHelper.Compile(@"
resource trailingSpace
");

            result.Diagnostics.Should().HaveDiagnostics(new[]
            {
                ("BCP068", DiagnosticLevel.Error, "Expected a resource type string. Specify a valid resource type of format \"<type-name>@<apiVersion>\"."),
            });
        }
    }
}
