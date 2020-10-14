// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
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
                ["main.bicep"] = @"
param inputa string
param inputb string

module modulea 'modulea.bicep' = {
  inputa: inputa
  inputb: inputb
}

module moduleb 'moduleb.bicep' = {
  inputa: inputa
  inputb: inputb
}

output outputa string = modulea.outputa
output outputb string = moduleb.outputb
",
                ["modulea.bicep"] = @"
param inputa string
param inputb string

output outputa string = '${inputa}-${inputb}'
",
                ["moduleb.bicep"] = @"
param inputa string
param inputb string

output outputb string = '${inputa}-${inputb}'
",
            };


            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateForFiles(files, "main.bicep"));

            var (success, diagnosticsByFile) = GetSuccessAndDiagnosticsByFile(compilation);
            diagnosticsByFile.Should().BeEmpty();
            success.Should().BeTrue();
            GetTemplate(compilation).Should().NotBeEmpty();
        }

        [TestMethod]
        public void Module_self_cycle_is_detected_correctly()
        {
            var files = new Dictionary<string, string>
            {
                ["main.bicep"] = @"
param inputa string
param inputb string

module modulea 'main.bicep' = {
  inputa: inputa
  inputb: inputb
}
",
            };


            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateForFiles(files, "main.bicep"));

            var (success, diagnosticsByFile) = GetSuccessAndDiagnosticsByFile(compilation);
            diagnosticsByFile.Should().SatisfyRespectively(
                x => x.diagnostic.Should().HaveCodeAndSeverity("BCP092", DiagnosticLevel.Error)
            );
            success.Should().BeFalse();
        }

        [TestMethod]
        public void Module_cycles_are_detected_correctly()
        {
            var files = new Dictionary<string, string>
            {
                ["main.bicep"] = @"
param inputa string
param inputb string

module modulea 'modulea.bicep' = {
  inputa: inputa
  inputb: inputb
}
",
                ["modulea.bicep"] = @"
param inputa string
param inputb string

module modulea 'main.bicep' = {
  inputa: inputa
  inputb: inputb
}
",
            };


            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateForFiles(files, "main.bicep"));

            var (success, diagnosticsByFile) = GetSuccessAndDiagnosticsByFile(compilation);
            diagnosticsByFile.Should().SatisfyRespectively(
                x => x.diagnostic.Should().HaveCodeAndSeverity("BCP093", DiagnosticLevel.Error),
                x => x.diagnostic.Should().HaveCodeAndSeverity("BCP093", DiagnosticLevel.Error)
            );
            success.Should().BeFalse();
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

        private static (bool success, IEnumerable<(SyntaxTree file, Diagnostic diagnostic)> diagnosticsByFile) GetSuccessAndDiagnosticsByFile(Compilation compilation)
        {
            var diagnosticsByFile = compilation.SyntaxTreeGrouping.SyntaxTrees.ToDictionary(x => x, x => new List<Diagnostic>());
            var success = compilation.EmitDiagnosticsAndCheckSuccess(
                (syntaxTree, diagnostic) => diagnosticsByFile[syntaxTree].Add(diagnostic));

            return (success, GetDiagnosticsByFile(diagnosticsByFile));
        }
    }
}