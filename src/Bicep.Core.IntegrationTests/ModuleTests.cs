// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Resources;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ModuleTests
    {
        [TestMethod]
        public void Modules_can_be_compiled_successfully()
        {
            var files = new Dictionary<string, string>
            {
                ["/main.bicep"] = @"
param inputa string
param inputb string

module modulea 'modulea.bicep' = {
  name: 'modulea'
  params: {
    inputa: inputa
    inputb: inputb
  }
}

module moduleb 'moduleb.bicep' = {
  name: 'moduleb'
  params: {
    inputa: inputa
    inputb: inputb
  }
}

output outputa string = modulea.outputs.outputa
output outputb string = moduleb.outputs.outputb
",
                ["/modulea.bicep"] = @"
param inputa string
param inputb string

output outputa string = '${inputa}-${inputb}'
",
                ["/moduleb.bicep"] = @"
param inputa string
param inputb string

output outputb string = '${inputa}-${inputb}'
",
            };


            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateForFiles(files, "/main.bicep"));

            var (success, diagnosticsByFile) = GetSuccessAndDiagnosticsByFile(compilation);
            diagnosticsByFile.Values.SelectMany(x => x).Should().BeEmpty();
            success.Should().BeTrue();
            GetTemplate(compilation).Should().NotBeEmpty();
        }

        [TestMethod]
        public void Module_self_cycle_is_detected_correctly()
        {
            var files = new Dictionary<string, string>
            {
                ["/main.bicep"] = @"
param inputa string
param inputb string

module mainRecursive 'main.bicep' = {
  name: 'mainRecursive'
  params: {
    inputa: inputa
    inputb: inputb
  }
}
",
            };


            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateForFiles(files, "/main.bicep"));

            var (success, diagnosticsByFile) = GetSuccessAndDiagnosticsByFile(compilation);
            diagnosticsByFile["/main.bicep"].Should().HaveDiagnostics(new[] {
                ("BCP094", DiagnosticLevel.Error, "This module references itself, which is not allowed."),
            });

            success.Should().BeFalse();
        }

        [TestMethod]
        public void Module_cycles_are_detected_correctly()
        {
            var files = new Dictionary<string, string>
            {
                ["/main.bicep"] = @"
param inputa string
param inputb string

module modulea 'modulea.bicep' = {
  name: 'modulea'
  params: {
    inputa: inputa
    inputb: inputb
  }
}
",
                ["/modulea.bicep"] = @"
param inputa string
param inputb string

module moduleb 'moduleb.bicep' = {
  name: 'moduleb'
  params: {
    inputa: inputa
    inputb: inputb
  }
}
",
                ["/moduleb.bicep"] = @"
param inputa string
param inputb string

module main 'main.bicep' = {
  inputa: inputa
  inputb: inputb
}
",
            };


            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateForFiles(files, "/main.bicep"));

            var (success, diagnosticsByFile) = GetSuccessAndDiagnosticsByFile(compilation);
            diagnosticsByFile["/main.bicep"].Should().HaveDiagnostics(new[] {
                ("BCP095", DiagnosticLevel.Error, "The module is involved in a cycle (\"/modulea.bicep\" -> \"/moduleb.bicep\" -> \"/main.bicep\")."),
            });
            diagnosticsByFile["/modulea.bicep"].Should().HaveDiagnostics(new[] {
                ("BCP095", DiagnosticLevel.Error, "The module is involved in a cycle (\"/moduleb.bicep\" -> \"/main.bicep\" -> \"/modulea.bicep\")."),
            });
            diagnosticsByFile["/moduleb.bicep"].Should().HaveDiagnostics(new[] {
                ("BCP095", DiagnosticLevel.Error, "The module is involved in a cycle (\"/main.bicep\" -> \"/modulea.bicep\" -> \"/moduleb.bicep\")."),
            });
            success.Should().BeFalse();
        }

        private delegate string? TryReadDelegate(string fileName, out string? failureMessage);

        [TestMethod]
        public void SyntaxTreeGroupingBuilder_build_should_throw_diagnostic_exception_if_entrypoint_file_read_fails()
        {
            var mockFileResolver = new Mock<IFileResolver>();
            mockFileResolver.Setup(x => x.GetNormalizedFileName("main.bicep")).Returns("/path/to/main.bicep");
            mockFileResolver.Setup(x => x.GetNormalizedFileName("/path/to/main.bicep")).Returns("/path/to/main.bicep");
            string? tryReadOutput;
            mockFileResolver.Setup(x => x.TryRead("/path/to/main.bicep", out tryReadOutput)).Returns(new TryReadDelegate((string fileName, out string? failureMessage) => { failureMessage = "Mock read failure!"; return null; }));

            Action buildAction = () => SyntaxTreeGroupingBuilder.Build(mockFileResolver.Object, new Workspace(), "main.bicep");
            buildAction.Should().Throw<ErrorDiagnosticException>()
                .And.Diagnostic.Should().HaveCodeAndSeverity("BCP091", DiagnosticLevel.Error).And.HaveMessage("An error occurred loading the module. Mock read failure!");
        }

        [TestMethod]
        public void Module_should_include_diagnostic_if_module_file_cannot_be_resolved()
        {
            var mainFileContents = @"
param inputa string
param inputb string

module modulea 'modulea.bicep' = {
  name: 'modulea'
  params: {
    inputa: inputa
    inputb: inputb
  }
}
";

            var mockFileResolver = new Mock<IFileResolver>();
            mockFileResolver.Setup(x => x.GetNormalizedFileName("main.bicep")).Returns("/path/to/main.bicep");
            mockFileResolver.Setup(x => x.GetNormalizedFileName("/path/to/main.bicep")).Returns("/path/to/main.bicep");
            mockFileResolver.Setup(x => x.GetNormalizedFileName("modulea.bicep")).Returns("/path/to/modulea.bicep");
            string? tryReadOutput;
            mockFileResolver.Setup(x => x.TryRead("/path/to/main.bicep", out tryReadOutput)).Returns(new TryReadDelegate((string fileName, out string? failureMessage) => { failureMessage = null; return mainFileContents; }));
            mockFileResolver.Setup(x => x.TryResolveModulePath("/path/to/main.bicep", "modulea.bicep")).Returns((string?)null);

            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingBuilder.Build(mockFileResolver.Object, new Workspace(), "main.bicep"));

            var (success, diagnosticsByFile) = GetSuccessAndDiagnosticsByFile(compilation);
            diagnosticsByFile["/path/to/main.bicep"].Should().HaveDiagnostics(new[] {
                ("BCP093", DiagnosticLevel.Error, "Module path \"modulea.bicep\" could not be resolved relative to \"/path/to/main.bicep\"."),
            });
        }

        [TestMethod]
        public void Module_should_include_diagnostic_if_module_file_cannot_be_loaded()
        {
            var mainFileContents = @"
param inputa string
param inputb string

module modulea 'modulea.bicep' = {
  name: 'modulea'
  params: {
    inputa: inputa
    inputb: inputb
  }
}
";

            var mockFileResolver = new Mock<IFileResolver>();
            mockFileResolver.Setup(x => x.GetNormalizedFileName("main.bicep")).Returns("/path/to/main.bicep");
            mockFileResolver.Setup(x => x.GetNormalizedFileName("/path/to/main.bicep")).Returns("/path/to/main.bicep");
            mockFileResolver.Setup(x => x.GetNormalizedFileName("modulea.bicep")).Returns("/path/to/modulea.bicep");
            mockFileResolver.Setup(x => x.GetNormalizedFileName("/path/to/modulea.bicep")).Returns("/path/to/modulea.bicep");
            string? tryReadOutput;
            mockFileResolver.Setup(x => x.TryRead("/path/to/main.bicep", out tryReadOutput)).Returns(new TryReadDelegate((string fileName, out string? failureMessage) => { failureMessage = null; return mainFileContents; }));
            mockFileResolver.Setup(x => x.TryRead("/path/to/modulea.bicep", out tryReadOutput)).Returns(new TryReadDelegate((string fileName, out string? failureMessage) => { failureMessage = "Mock read failure!"; return null; }));
            mockFileResolver.Setup(x => x.TryResolveModulePath("/path/to/main.bicep", "modulea.bicep")).Returns("/path/to/modulea.bicep");

            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingBuilder.Build(mockFileResolver.Object, new Workspace(), "main.bicep"));

            var (success, diagnosticsByFile) = GetSuccessAndDiagnosticsByFile(compilation);
            diagnosticsByFile["/path/to/main.bicep"].Should().HaveDiagnostics(new[] {
                ("BCP091", DiagnosticLevel.Error, "An error occurred loading the module. Mock read failure!"),
            });
        }

        private static string GetTemplate(Compilation compilation)
        {
            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);

            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());
            emitter.Emit(stringWriter);

            return stringBuilder.ToString();
        }

        private static IEnumerable<(SyntaxTree file, Diagnostic diagnostic)> GetDiagnosticsByFile(IDictionary<SyntaxTree, List<Diagnostic>> diagnosticsByFile)
        {
            foreach (var kvp in diagnosticsByFile)
            {
                foreach (var diagnostic in kvp.Value)
                {
                    yield return (kvp.Key, diagnostic);
                }
            }
        }

        private static (bool success, IDictionary<string, IEnumerable<Diagnostic>> diagnosticsByFile) GetSuccessAndDiagnosticsByFile(Compilation compilation)
        {
            var diagnosticsByFile = compilation.GetAllDiagnosticsBySyntaxTree().ToDictionary(kvp => kvp.Key.FilePath, kvp => kvp.Value);
            var success = diagnosticsByFile.Values.SelectMany(x => x).All(d => d.Level != DiagnosticLevel.Error);

            return (success, diagnosticsByFile);
        }
    }
}