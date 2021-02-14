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
using Bicep.Core.Semantics;
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
            var mainUri = new Uri("file:///main.bicep");
            var moduleAUri = new Uri("file:///modulea.bicep");
            var moduleBUri = new Uri("file:///moduleb.bicep");

            var files = new Dictionary<Uri, string>
            {
                [mainUri] = @"
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
                [moduleAUri] = @"
param inputa string
param inputb string

output outputa string = '${inputa}-${inputb}'
",
                [moduleBUri] = @"
param inputa string
param inputb string

output outputb string = '${inputa}-${inputb}'
",
            };


            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingFactory.CreateForFiles(files, mainUri));

            var (success, diagnosticsByFile) = GetSuccessAndDiagnosticsByFile(compilation);
            diagnosticsByFile.Values.SelectMany(x => x).Should().BeEmpty();
            success.Should().BeTrue();
            GetTemplate(compilation).Should().NotBeEmpty();
        }

        [TestMethod]
        public void Module_self_cycle_is_detected_correctly()
        {
            var mainUri = new Uri("file:///main.bicep");

            var files = new Dictionary<Uri, string>
            {
                [mainUri] = @"
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


            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingFactory.CreateForFiles(files, mainUri));

            var (success, diagnosticsByFile) = GetSuccessAndDiagnosticsByFile(compilation);
            diagnosticsByFile[mainUri].Should().HaveDiagnostics(new[] {
                ("BCP094", DiagnosticLevel.Error, "This module references itself, which is not allowed."),
            });

            success.Should().BeFalse();
        }

        [TestMethod]
        public void Module_cycles_are_detected_correctly()
        {
            var mainUri = new Uri("file:///main.bicep");
            var moduleAUri = new Uri("file:///modulea.bicep");
            var moduleBUri = new Uri("file:///moduleb.bicep");

            var files = new Dictionary<Uri, string>
            {
                [mainUri] = @"
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
                [moduleAUri] = @"
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
                [moduleBUri] = @"
param inputa string
param inputb string

module main 'main.bicep' = {
  inputa: inputa
  inputb: inputb
}
",
            };


            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingFactory.CreateForFiles(files, mainUri));

            var (success, diagnosticsByFile) = GetSuccessAndDiagnosticsByFile(compilation);
            diagnosticsByFile[mainUri].Should().HaveDiagnostics(new[] {
                ("BCP095", DiagnosticLevel.Error, "The module is involved in a cycle (\"/modulea.bicep\" -> \"/moduleb.bicep\" -> \"/main.bicep\")."),
            });
            diagnosticsByFile[moduleAUri].Should().HaveDiagnostics(new[] {
                ("BCP095", DiagnosticLevel.Error, "The module is involved in a cycle (\"/moduleb.bicep\" -> \"/main.bicep\" -> \"/modulea.bicep\")."),
            });
            diagnosticsByFile[moduleBUri].Should().HaveDiagnostics(new[] {
                ("BCP095", DiagnosticLevel.Error, "The module is involved in a cycle (\"/main.bicep\" -> \"/modulea.bicep\" -> \"/moduleb.bicep\")."),
            });
            success.Should().BeFalse();
        }

        private delegate bool TryReadDelegate(Uri fileUri, out string? fileContents, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        private static void SetupFileReaderMock(Mock<IFileResolver> mockFileResolver, Uri fileUri, string? fileContents, DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            string? outFileContents;
            DiagnosticBuilder.ErrorBuilderDelegate? outFailureBuilder;
            mockFileResolver.Setup(x => x.TryRead(fileUri, out outFileContents, out outFailureBuilder))
                .Returns(new TryReadDelegate((Uri filePath, out string? outFileContents, out DiagnosticBuilder.ErrorBuilderDelegate? outFailureBuilder) => {
                    outFailureBuilder = failureBuilder;
                    outFileContents = fileContents;
                    return fileContents != null;
                }));
        }

        [TestMethod]
        public void SyntaxTreeGroupingBuilder_build_should_throw_diagnostic_exception_if_entrypoint_file_read_fails()
        {
            var fileUri = new Uri("file:///path/to/main.bicep");

            var mockFileResolver = new Mock<IFileResolver>();
            SetupFileReaderMock(mockFileResolver, fileUri, null, x => x.ErrorOccurredReadingFile("Mock read failure!"));

            Action buildAction = () => SyntaxTreeGroupingBuilder.Build(mockFileResolver.Object, new Workspace(), fileUri);
            buildAction.Should().Throw<ErrorDiagnosticException>()
                .And.Diagnostic.Should().HaveCodeAndSeverity("BCP091", DiagnosticLevel.Error).And.HaveMessage("An error occurred reading file. Mock read failure!");
        }

        [TestMethod]
        public void Module_should_include_diagnostic_if_module_file_cannot_be_resolved()
        {
            var mainFileUri = new Uri("file:///path/to/main.bicep");
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
            SetupFileReaderMock(mockFileResolver, mainFileUri, mainFileContents, null);
            mockFileResolver.Setup(x => x.TryResolveModulePath(mainFileUri, "modulea.bicep")).Returns((Uri?)null);

            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingBuilder.Build(mockFileResolver.Object, new Workspace(), mainFileUri));

            var (success, diagnosticsByFile) = GetSuccessAndDiagnosticsByFile(compilation);
            diagnosticsByFile[mainFileUri].Should().HaveDiagnostics(new[] {
                ("BCP093", DiagnosticLevel.Error, "Module path \"modulea.bicep\" could not be resolved relative to \"/path/to/main.bicep\"."),
            });
        }

        [TestMethod]
        public void Module_should_include_diagnostic_if_module_file_cannot_be_loaded()
        {
            var mainUri = new Uri("file:///path/to/main.bicep");
            var moduleAUri = new Uri("file:///path/to/modulea.bicep");

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
            SetupFileReaderMock(mockFileResolver, mainUri, mainFileContents, null);
            SetupFileReaderMock(mockFileResolver, moduleAUri, null, x => x.ErrorOccurredReadingFile("Mock read failure!"));
            mockFileResolver.Setup(x => x.TryResolveModulePath(mainUri, "modulea.bicep")).Returns(moduleAUri);

            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingBuilder.Build(mockFileResolver.Object, new Workspace(), mainUri));

            var (success, diagnosticsByFile) = GetSuccessAndDiagnosticsByFile(compilation);
            diagnosticsByFile[mainUri].Should().HaveDiagnostics(new[] {
                ("BCP091", DiagnosticLevel.Error, "An error occurred reading file. Mock read failure!"),
            });
        }

        private static string GetTemplate(Compilation compilation)
        {
            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);

            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());
            emitter.Emit(stringWriter, "dev");

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

        private static (bool success, IDictionary<Uri, IEnumerable<Diagnostic>> diagnosticsByFile) GetSuccessAndDiagnosticsByFile(Compilation compilation)
        {
            var diagnosticsByFile = compilation.GetAllDiagnosticsBySyntaxTree().ToDictionary(kvp => kvp.Key.FileUri, kvp => kvp.Value);
            var success = diagnosticsByFile.Values.SelectMany(x => x).All(d => d.Level != DiagnosticLevel.Error);

            return (success, diagnosticsByFile);
        }
    }
}