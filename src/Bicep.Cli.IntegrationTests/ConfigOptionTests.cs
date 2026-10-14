// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.UnitTests;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Bicep.Testing;
using Bicep.Testing.IO;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Cli.IntegrationTests;

[TestClass]
public class ConfigOptionTests : TestBase
{
    private const string UnusedParamErrorConfig = """
        {
          "analyzers": { "core": { "rules": { "no-unused-params": { "level": "error" } } } },
          "formatting": { "indentSize": 4 }
        }
        """;

    private static MockFileSystemTestFileSet CreateProject() => MockFileSystemTestFileSet.Create(
        ("main.bicep", """
            param foo string
            module mod 'modules/mod.bicep' = {
            name: 'mod'
            }
            """),
        ("main.bicepparam", """
            using 'main.bicep'

            param foo = 'foo'
            """),
        ("modules/mod.bicep", "param bar string = 'bar'"),
        // A nested config that the explicit configuration must override.
        ("modules/bicepconfig.json", """{ "analyzers": { "core": { "rules": { "no-unused-params": { "level": "off" } } } } }"""),
        ("eng/bicepconfig.ci.json", UnusedParamErrorConfig));

    private static string PathOf(TestFileSet fileSet, string path) => fileSet.GetUri(path).GetFilePath();

    private static Task<CliResult> Bicep(MockFileSystemTestFileSet fileSet, params string[] args)
        => Bicep(services => services.WithFileSystem(fileSet.FileSystem), args);

    [TestMethod]
    public async Task Build_WithoutConfig_DiscoversConfiguration()
    {
        var fileSet = CreateProject();

        var (_, error, result) = await Bicep(fileSet, "build", PathOf(fileSet, "main.bicep"), "--stdout");

        using (new AssertionScope())
        {
            result.Should().Be(0);
            error.Should().Contain("main.bicep(1,7) : Warning no-unused-params");
            error.Should().NotContain("mod.bicep");
        }
    }

    [TestMethod]
    public async Task Build_WithConfig_UsesExplicitConfigurationForAllSourceFiles()
    {
        var fileSet = CreateProject();

        var (output, error, result) = await Bicep(fileSet, "build", PathOf(fileSet, "main.bicep"), "--stdout", "--config", PathOf(fileSet, "eng/bicepconfig.ci.json"));

        using (new AssertionScope())
        {
            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().Contain("main.bicep(1,7) : Error no-unused-params");
            error.Should().Contain("mod.bicep(1,7) : Error no-unused-params");
        }
    }

    [TestMethod]
    public async Task Build_WithConfigThatExtends_ResolvesBaseRelativeToConfigFile()
    {
        var fileSet = CreateProject();
        fileSet.AddFile("eng/shared/base.json", UnusedParamErrorConfig);
        fileSet.AddFile("eng/bicepconfig.prod.json", """{ "extends": "./shared/base.json" }""");

        var (_, error, result) = await Bicep(fileSet, "build", PathOf(fileSet, "main.bicep"), "--stdout", "--config", PathOf(fileSet, "eng/bicepconfig.prod.json"));

        using (new AssertionScope())
        {
            result.Should().Be(1);
            error.Should().Contain("main.bicep(1,7) : Error no-unused-params");
        }
    }

    [TestMethod]
    public async Task Build_WithPatternAndConfig_AppliesExplicitConfigurationToEveryInput()
    {
        // --pattern globbing (Microsoft.Extensions.FileSystemGlobbing) reads the real disk rather than IFileSystem,
        // so this test cannot use a mock file system.
        var root = FileHelper.GetUniqueTestOutputPath(TestContext);
        FileHelper.SaveResultFile(TestContext, "main.bicep", "param foo string", root);
        FileHelper.SaveResultFile(TestContext, "modules/mod.bicep", "param bar string = 'bar'", root);
        var configPath = FileHelper.SaveResultFile(TestContext, "eng/bicepconfig.ci.json", UnusedParamErrorConfig, root);

        var (_, error, result) = await Bicep("build", "--pattern", $"{root}/**/*.bicep", "--config", configPath);

        using (new AssertionScope())
        {
            result.Should().Be(1);
            error.Should().Contain("main.bicep(1,7) : Error no-unused-params");
            error.Should().Contain("mod.bicep(1,7) : Error no-unused-params");
        }
    }

    [TestMethod]
    public async Task BuildParams_WithConfig_AppliesExplicitConfigurationToUsingTarget()
    {
        var fileSet = CreateProject();

        var (_, error, result) = await Bicep(fileSet, "build-params", PathOf(fileSet, "main.bicepparam"), "--stdout", "--config", PathOf(fileSet, "eng/bicepconfig.ci.json"));

        using (new AssertionScope())
        {
            result.Should().Be(1);
            error.Should().Contain("main.bicep(1,7) : Error no-unused-params");
        }
    }

    [TestMethod]
    public async Task Lint_WithConfig_UsesExplicitConfiguration()
    {
        var fileSet = CreateProject();

        var (_, error, result) = await Bicep(fileSet, "lint", PathOf(fileSet, "main.bicep"), "--config", PathOf(fileSet, "eng/bicepconfig.ci.json"));

        using (new AssertionScope())
        {
            result.Should().Be(1);
            error.Should().Contain("main.bicep(1,7) : Error no-unused-params");
        }
    }

    [TestMethod]
    public async Task Format_WithConfig_UsesExplicitFormattingOptions()
    {
        var fileSet = CreateProject();

        var (output, error, result) = await Bicep(fileSet, "format", PathOf(fileSet, "main.bicep"), "--stdout", "--config", PathOf(fileSet, "eng/bicepconfig.ci.json"));

        using (new AssertionScope())
        {
            result.Should().Be(0);
            error.Should().BeEmpty();
            output.Should().Contain("\n    name: 'mod'\n");
        }
    }

    [TestMethod]
    public async Task Test_WithConfigEnablingTestFramework_RunsTests()
    {
        var fileSet = MockFileSystemTestFileSet.Create(
            ("tests/main.tests.bicep", "test valid 'target.bicep' = {}"),
            ("tests/target.bicep", "// Valid test target."),
            ("eng/bicepconfig.test.json", """{ "experimentalFeaturesEnabled": { "testFramework": true, "assertions": true } }"""));
        var testsPath = PathOf(fileSet, "tests/main.tests.bicep");

        var (discoveredOutput, discoveredError, discoveredResult) = await Bicep(fileSet, "test", testsPath);
        var (output, _, result) = await Bicep(fileSet, "test", testsPath, "--config", PathOf(fileSet, "eng/bicepconfig.test.json"));

        using (new AssertionScope())
        {
            discoveredResult.Should().Be(1);
            discoveredOutput.Should().BeEmpty();
            discoveredError.Should().Contain("TestFrameWork not enabled");

            result.Should().Be(0);
            output.Should().Contain("Evaluation valid Passed!");
        }
    }

    [TestMethod]
    public async Task Build_WithMissingConfig_FailsWithoutCompiling()
    {
        var fileSet = CreateProject();

        var (output, error, result) = await Bicep(fileSet, "build", PathOf(fileSet, "main.bicep"), "--config", "./missing.json");

        using (new AssertionScope())
        {
            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().Contain("Could not find configuration file \"./missing.json\".");
            fileSet.FileExplorer.GetFile(fileSet.GetUri("main.json")).Exists().Should().BeFalse();
        }
    }

    [DataTestMethod]
    [DataRow("build", "{bicep}")]
    [DataRow("build-params", "{params}")]
    [DataRow("lint", "{bicep}")]
    [DataRow("format", "{bicep}")]
    [DataRow("publish", "{bicep}", "--target", "br:example.com/hello/there:v1")]
    [DataRow("publish-extension", "{index}", "--target", "br:example.com/hello/there:v1")]
    [DataRow("restore", "{bicep}")]
    [DataRow("generate-params", "{bicep}")]
    [DataRow("snapshot", "{params}")]
    [DataRow("test", "{bicep}")]
    [DataRow("deploy", "{params}")]
    [DataRow("what-if", "{params}")]
    [DataRow("teardown", "{params}")]
    [DataRow("local-deploy", "{params}")]
    [DataRow("console")]
    [DataRow("docs", "generate", "{bicep}")]
    public async Task Command_WithMissingConfig_FailsBeforeRunning(params string[] commandArgs)
    {
        var fileSet = CreateProject();
        fileSet.AddFile("types/index.json", "{}");
        var paths = new Dictionary<string, string>
        {
            ["{bicep}"] = PathOf(fileSet, "main.bicep"),
            ["{params}"] = PathOf(fileSet, "main.bicepparam"),
            ["{index}"] = PathOf(fileSet, "types/index.json"),
        };
        string[] args = [.. commandArgs.Select(x => paths.GetValueOrDefault(x, x)), "--config", "./missing.json"];

        var (output, error, result) = await Bicep(fileSet, args);

        using (new AssertionScope())
        {
            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().Be("Could not find configuration file \"./missing.json\"." + Environment.NewLine);
        }
    }

    [TestMethod]
    public async Task Build_WithConfigSpecifiedTwice_Fails()
    {
        var fileSet = CreateProject();
        var configPath = PathOf(fileSet, "eng/bicepconfig.ci.json");

        var (_, error, result) = await Bicep(fileSet, "build", PathOf(fileSet, "main.bicep"), "--config", configPath, "--config", configPath);

        using (new AssertionScope())
        {
            result.Should().Be(1);
            error.Should().Contain("--config");
        }
    }
}
