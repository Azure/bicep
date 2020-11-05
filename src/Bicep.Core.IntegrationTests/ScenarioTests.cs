// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.Core.UnitTests.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions.Execution;
using FluentAssertions;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ScenarioTests
    {
        private static (string? jsonOutput, IEnumerable<Diagnostic> diagnostics) Compile(string bicepContents)
        {
            var syntaxTreeGrouping = SyntaxFactory.CreateFromText(bicepContents);
            var compilation = new Compilation(new AzResourceTypeProvider(), syntaxTreeGrouping);
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());
            
            var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

            string? jsonOutput = null;
            if (!compilation.GetEntrypointSemanticModel().HasErrors())
            {
                using var stream = new MemoryStream();
                var emitResult = emitter.Emit(stream);

                if (emitResult.Status != EmitStatus.Failed)
                {
                    stream.Position = 0;
                    jsonOutput = new StreamReader(stream).ReadToEnd();
                }
            }

            return (jsonOutput, diagnostics);
        }

        private static void AssertFailureWithDiagnostics(string bicepContents, IEnumerable<(string code, DiagnosticLevel level, string message)> expectedDiagnostics)
        {
            var (jsonOutput, diagnostics) = Compile(bicepContents);
            using (new AssertionScope())
            {
                jsonOutput.Should().BeNull();
                diagnostics.Should().HaveDiagnostics(expectedDiagnostics);
            }
        }

        [TestMethod]
        public void Test_Issue746()
        {
            var bicepContents = @"
var l = l
param l
";

            AssertFailureWithDiagnostics(
                bicepContents,
                new [] {
                    ("BCP028", DiagnosticLevel.Error, "Identifier \"l\" is declared multiple times. Remove or rename the duplicates."),
                    ("BCP079", DiagnosticLevel.Error, "This expression is referencing its own declaration, which is not allowed."),
                    ("BCP028", DiagnosticLevel.Error, "Identifier \"l\" is declared multiple times. Remove or rename the duplicates."),
                    ("BCP014", DiagnosticLevel.Error, "Expected a parameter type at this location. Please specify one of the following types: \"array\", \"bool\", \"int\", \"object\", \"string\"."),
                });
        }
    }
}