// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class OrchestrationTests
{
    private static ServiceBuilder Services => new ServiceBuilder()
        .WithEmptyAzResources()
        .WithFeatureOverrides(new(OrchestrationEnabled: true));

    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    [DataRow("{ region: 'westus' }", true)]
    [DataRow("{ region: 1 }", false)]
    [DataRow("{ region: true }", false)]
    [DataRow("{ region: ['westus'] }", false)]
    [DataRow("{ region: { name: 'westus' } }", false)]
    [DataRow("{ region: null }", false)]
    [DataRow("'westus'", false)]
    public void Stack_labels_require_string_values(string labels, bool valid)
    {
        var result = CompilationHelper.Compile(Services,
            ("main.bicep", $$"""
                targetScope = 'orchestrator'
                stack example './stack.bicepparam' = {
                  labels: {{labels}}
                  inputs: {}
                }
                """),
            ("stack.bicepparam", "using './stack.bicep'"),
            ("stack.bicep", "output value string = 'example'"));
        if (valid)
        {
            result.Should().NotHaveAnyDiagnostics();
        }
        else
        {
            result.Diagnostics.Should().Contain(diagnostic => diagnostic.Level == DiagnosticLevel.Error);
        }
    }

    [TestMethod]
    [EmbeddedFilesTestData(@"orchestration_samples/basic/main\.bicepparam")]
    [TestCategory(BaselineHelper.BaselineTestCategory)]
    public async Task Basic_example_can_be_compiled_successfully(EmbeddedFile file)
    {
        var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, file);

        var compiler = Services.Build().GetCompiler();
        var compilation = await compiler.CreateCompilation(IOUri.FromFilePath(baselineFolder.EntryFile.OutputFilePath));

        var result = CompilationHelper.CompileParams(compilation);

        result.Should().NotHaveAnyDiagnostics();

        // TODO needs some work to support baseline paths that differ from the source file paths
        // var paramsResult = result.Compilation.Emitter.Parameters();
        // var template = paramsResult.Template?.Template;
        // var parameters = paramsResult.Parameters;

        // var outputBaseUri = IOUri.FromFilePath(baselineFolder.OutputFolderPath).ToUriString();

        // var paramsFile = baselineFolder.GetFileOrEnsureCheckedIn("main.parameters.json");
        // var templateFile = baselineFolder.GetFileOrEnsureCheckedIn("main.json");

        // paramsFile.WriteToOutputFolder(parameters!);
        // templateFile.WriteToOutputFolder(template!);

        // paramsFile.ShouldHaveExpectedJsonValue();
        // templateFile.ShouldHaveExpectedJsonValue();
    }
}