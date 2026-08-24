// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using System.Text.Json;
using Bicep.Cli.Arguments;
using Bicep.Cli.Services;
using Bicep.Core.Configuration;
using Bicep.Core.Documentation;
using Bicep.Core.Exceptions;
using Bicep.Core.Json;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Cli.IntegrationTests;

[TestClass]
public class DocsCommandTests : TestBase
{
    private const string ConventionalConfigFileName = "bicepconfig.json";
    private const string FixturePrefix = "Files/DocsCommandTests/Comprehensive/";

    private static InvocationSettings DocsEnabledSettings() => InvocationSettings.Default;

    private string SaveComprehensiveFixture() =>
        FileHelper.SaveEmbeddedResourcesWithPathPrefix(TestContext, Assembly.GetExecutingAssembly(), FixturePrefix);

    [TestMethod]
    public async Task DocsCommand_IsAvailableWithoutBicepConfigFeatureFlag()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("main.bicep", "metadata name = 'No feature flag'"),
                new("bicepconfig.json", "{}"),
            ]);

        var result = await Bicep(
            "docs",
            "generate",
            "--stdout",
            Path.Combine(root, "main.bicep"));

        result.ExitCode.Should().Be(0);
        result.Stdout.Should().Contain("# No feature flag");
        result.Stderr.Should().Contain(
            "following experimental Bicep features have been enabled: docs");
    }

    [TestMethod]
    public async Task Generate_ComprehensiveFixture_PerformsRealIoAndMatchesGoldenFile()
    {
        var moduleRoot = SaveComprehensiveFixture();
        var mainFile = Path.Combine(moduleRoot, "main.bicep");
        var outputFile = Path.Combine(moduleRoot, "README.md");
        var expectedFile = Path.Combine(moduleRoot, "README.expected.md");
        File.WriteAllText(outputFile, "stale content");

        var result = await Bicep("docs", "generate", mainFile);

        using (new AssertionScope())
        {
            result.ExitCode.Should().Be(0);
            result.Stdout.Should().BeEmpty();
            result.Stderr.Should().Contain("docs");
            File.ReadAllText(outputFile).Should().Be(File.ReadAllText(expectedFile));
            File.ReadAllText(outputFile).Should().NotContain("stale content");
        }
    }

    [TestMethod]
    public async Task Output_ComprehensiveFixture_MatchesGeneratedContentWithoutWriting()
    {
        var moduleRoot = SaveComprehensiveFixture();
        var mainFile = Path.Combine(moduleRoot, "main.bicep");
        var outputFile = Path.Combine(moduleRoot, "README.md");
        File.Delete(outputFile);

        var outputResult = await Bicep("docs", "generate", "--stdout", mainFile);

        outputResult.ExitCode.Should().Be(0);
        outputResult.Stdout.Should().Be(File.ReadAllText(Path.Combine(moduleRoot, "README.expected.md")));
        File.Exists(outputFile).Should().BeFalse();
    }

    [TestMethod]
    public async Task Output_CustomTemplate_SupportsIncludesTemplateRootAndCustomValues()
    {
        var moduleRoot = SaveComprehensiveFixture();
        var mainFile = Path.Combine(moduleRoot, "main.bicep");
        File.WriteAllText(Path.Combine(moduleRoot, "bicepconfig.json"), """
            {
              "documentation": {
                "template": {
                  "file": "templates/custom.scriban",
                  "includeRoot": ".",
                  "values": {
                    "owner": "Platform Team"
                  }
                }
              }
            }
            """);

        var result = await Bicep(
            "docs",
            "generate",
            "--stdout",
            mainFile,
            "--no-restore");

        result.ExitCode.Should().Be(0);
        result.Stdout.Should().Be("""
            > Generated module documentation.

            # Comprehensive Module

            Owner: Platform Team
            Scope: subscription
            Parameters: 9
            Documentation footer.
            """.ReplaceLineEndings("\n") + "\n");

    }

    [TestMethod]
    public async Task BicepConfig_AppliesOutputTemplateValuesAndSources()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("module.bicep", "metadata name = 'Configured module'"),
                new("templates/readme.scriban", "{{ include \"_header.md\" }}\n{{ module.name }}|{{ custom.owner }}|{{ custom.configOnly }}|{{ for example in module.usageExamples }}{{ example.name }}{{ end }}"),
                new("templates/_header.md", "Header"),
                new("samples/kept/example.demo", "metadata name = 'sample'"),
                new("samples/ignored/example.demo", "metadata name = 'ignored'"),
                new("bicepconfig.json", """
                    {
                      "documentation": {
                        "output": {
                          "file": "GENERATED.md"
                        },
                        "template": {
                          "file": "templates/readme.scriban",
                          "includeRoot": "templates",
                          "values": {
                            "owner": "Config",
                            "configOnly": "retained"
                          }
                        },
                        "examples": {
                          "sources": [
                            {
                              "path": "samples",
                              "include": ["**/*.demo"],
                              "exclude": ["**/ignored/**"]
                            }
                          ]
                        }
                      }
                    }
                    """),
            ]);
        var generateResult = await Bicep(
            "docs",
            "generate",
            Path.Combine(root, "module.bicep"));
        var outputResult = await Bicep(
            "docs",
            "generate",
            "--stdout",
            Path.Combine(root, "module.bicep"));

        generateResult.ExitCode.Should().Be(0);
        outputResult.ExitCode.Should().Be(0);
        var expected = "Header\nConfigured module|Config|retained|sample\n";
        File.ReadAllText(Path.Combine(root, "GENERATED.md")).Should().Be(expected);
        outputResult.Stdout.Should().Be(expected);
        File.Exists(Path.Combine(root, "README.md")).Should().BeFalse();

        var overrideResult = await Bicep(
            "docs",
            "generate",
            Path.Combine(root, "module.bicep"),
            "--outfile",
            Path.Combine(root, "OVERRIDE.md"));
        overrideResult.ExitCode.Should().Be(0);
        File.ReadAllText(Path.Combine(root, "OVERRIDE.md")).Should().Be(expected);
    }

    [TestMethod]
    public async Task Config_ReassignsParentExamplesToChildrenAndIsNoOpForOrdinaryModules()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("main.bicep", "metadata name = 'Parent'"),
                new("mg-scope/main.bicep", "metadata name = 'Child'"),
                new("tests/e2e/mg-scope.defaults/main.test.bicep", "metadata name = 'mapped'"),
                new("tests/e2e/unmapped/main.test.bicep", "metadata name = 'unmapped'"),
                new("ordinary/main.bicep", "metadata name = 'Ordinary'"),
                new("ordinary/tests/e2e/default/main.test.bicep", "metadata name = 'ordinary'"),
                new("examples.scriban", "{{ for example in module.usageExamples }}{{ example.name }}|{{ example.path }}\n{{ end }}"),
                new("bicepconfig.json", """
                    {
                      "documentation": {
                        "template": {
                          "file": "examples.scriban"
                        },
                        "examples": {
                          "reassignments": [
                            {
                              "from": {
                                "include": ["**/mg-scope.*/**"],
                                "exclude": ["**/*.skip/**"]
                              },
                              "to": "mg-scope"
                            }
                          ]
                        }
                      }
                    }
                    """),
            ]);
        var parentResult = await Bicep(
            "docs",
            "generate",
            "--stdout",
            Path.Combine(root, "main.bicep"));
        var childResult = await Bicep(
            "docs",
            "generate",
            "--stdout",
            Path.Combine(root, "mg-scope", "main.bicep"));
        var ordinaryResult = await Bicep(
            "docs",
            "generate",
            "--stdout",
            Path.Combine(root, "ordinary", "main.bicep"));

        parentResult.ExitCode.Should().Be(0, parentResult.Stderr);
        parentResult.Stdout.Should().Be("unmapped|tests/e2e/unmapped/main.test.bicep\n");
        childResult.ExitCode.Should().Be(0);
        childResult.Stdout.Should().Be("mapped|../tests/e2e/mg-scope.defaults/main.test.bicep\n");
        ordinaryResult.ExitCode.Should().Be(0);
        ordinaryResult.Stdout.Should().Be("ordinary|tests/e2e/default/main.test.bicep\n");
    }

    [DataTestMethod]
    [DataRow("""{ "output": { "file": "nested/README.md" } }""", "cannot traverse")]
    [DataRow("""{ "output": { "file": "CON.md" } }""", "portable file name")]
    [DataRow("""{ "output": { "file": "README.md." } }""", "portable file name")]
    [DataRow("""{ "template": { "file": "" } }""", "cannot be empty")]
    [DataRow("""{ "template": { "includeRoot": "" } }""", "cannot be empty")]
    [DataRow("""{ "template": { "values": { "": "value" } } }""", "cannot be empty")]
    [DataRow("""{ "examples": { "sources": [null] } }""", "cannot contain null values")]
    [DataRow("""{ "examples": { "reassignments": [null] } }""", "cannot contain null values")]
    [DataRow("""{ "examples": { "reassignments": [{ "from": null, "to": "child" }] } }""", "cannot contain null values")]
    [DataRow("""{ "examples": { "sources": [{ "path": "../samples" }] } }""", "cannot traverse")]
    [DataRow("""{ "examples": { "sources": [{ "path": "samples", "include": [""] }] } }""", "cannot be empty")]
    [DataRow("""{ "examples": { "reassignments": [{ "from": {}, "to": "child" }] } }""", "must contain")]
    [DataRow("""{ "examples": { "reassignments": [{ "from": { "include": ["**/*"] }, "to": "nested/child" }] } }""", "cannot traverse")]
    public async Task Config_InvalidValuesReturnActionableErrors(string contents, string expected)
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("main.bicep", "metadata name = 'Example'"),
                new("bicepconfig.json", $$"""{ "documentation": {{contents}} }"""),
            ]);

        var result = await Bicep(
            "docs",
            "generate",
            "--stdout",
            Path.Combine(root, "main.bicep"));

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().Contain(expected);
        result.Stderr.Should().NotContain("Unhandled exception");
    }

    [TestMethod]
    public async Task Generate_MissingInputAndPatternReturnsActionableError()
    {
        var result = await Bicep("docs", "generate");

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().Contain("Either the input file path or the --pattern parameter must be specified");
    }

    [TestMethod]
    public async Task Generate_DirectoryInputIsRejected()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [new("main.bicep", "metadata name = 'Example'")]);

        var result = await Bicep("docs", "generate", root);

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().Contain("not recognized as a Bicep file");
    }

    [TestMethod]
    public async Task Config_AbsoluteTemplatePathIsSupported()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("main.bicep", "metadata name = 'Absolute template'"),
                new("readme.scriban", "{{ module.name }}"),
            ]);
        var configPath = Path.Combine(root, "bicepconfig.json");
        File.WriteAllText(
            configPath,
            JsonSerializer.Serialize(new
            {
                documentation = new
                {
                    template = new
                    {
                        file = Path.Combine(root, "readme.scriban"),
                    },
                },
            }));

        var result = await Bicep(
            "docs",
            "generate",
            "--stdout",
            Path.Combine(root, "main.bicep"));

        result.ExitCode.Should().Be(0);
        result.Stdout.Should().Be("Absolute template\n");
    }

    [TestMethod]
    public async Task Config_EmptyObjectUsesBuiltInDefaults()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("main.bicep", "metadata name = 'Defaults'"),
                new("bicepconfig.json", "{}"),
            ]);

        var result = await Bicep(
            "docs",
            "generate",
            Path.Combine(root, "main.bicep"));

        result.ExitCode.Should().Be(0);
        File.Exists(Path.Combine(root, "README.md")).Should().BeTrue();
    }

    [TestMethod]
    public async Task Config_EmptyNestedSettingsUseTheirDefaults()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("main.bicep", "metadata name = 'Nested defaults'"),
                new("bicepconfig.json", """
                    {
                      "documentation": {
                        "output": {},
                        "template": {},
                        "examples": {
                          "sources": [
                            {
                              "path": "missing"
                            }
                          ]
                        }
                      }
                    }
                    """),
            ]);

        var result = await Bicep(
            "docs",
            "generate",
            Path.Combine(root, "main.bicep"));

        result.ExitCode.Should().Be(0);
        File.Exists(Path.Combine(root, "README.md")).Should().BeTrue();
    }

    [TestMethod]
    public async Task Config_FileInputUsesNearestBicepConfig()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("bicepconfig.json", """
                    {
                      "documentation": {
                        "output": { "file": "ROOT.md" }
                      }
                    }
                    """),
                new("module/main.bicep", "metadata name = 'Module'"),
                new("module/bicepconfig.json", """
                    {
                      "documentation": {
                        "output": { "file": "MODULE.md" }
                      }
                    }
                    """),
            ]);

        var result = await Bicep(
            "docs",
            "generate",
            Path.Combine(root, "module", "main.bicep"));

        result.ExitCode.Should().Be(0);
        File.Exists(Path.Combine(root, "module", "MODULE.md")).Should().BeTrue();
        File.Exists(Path.Combine(root, "module", "ROOT.md")).Should().BeFalse();
    }

    [TestMethod]
    public async Task Config_PatternUsesPerModuleNearestBicepConfig()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("modules/a/main.bicep", "metadata name = 'A'"),
                new("modules/b/main.bicep", "metadata name = 'B'"),
                new("modules/c/main.bicep", "metadata name = 'C'"),
                new("bicepconfig.json", """
                    {
                      "documentation": {
                        "output": { "file": "ROOT.md" }
                      }
                    }
                    """),
                new("modules/a/bicepconfig.json", """
                    {
                      "documentation": {
                        "output": { "file": "MODULE.md" }
                      }
                    }
                    """),
                new("modules/b/bicepconfig.json", """{ "experimentalFeaturesWarning": false }"""),
            ]);

        var result = await Bicep(
            "docs",
            "generate",
            "--pattern",
            Path.Combine(root, "modules", "*", "main.bicep"));

        result.ExitCode.Should().Be(0);
        File.Exists(Path.Combine(root, "modules", "a", "MODULE.md")).Should().BeTrue();
        File.Exists(Path.Combine(root, "modules", "b", "README.md")).Should().BeTrue();
        File.Exists(Path.Combine(root, "modules", "c", "ROOT.md")).Should().BeTrue();
    }

    [TestMethod]
    public async Task Config_SearchesParentDirectories()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("bicepconfig.json", """
                    {
                      "documentation": {
                        "output": { "file": "PARENT.md" }
                      }
                    }
                    """),
                new("target/main.bicep", "metadata name = 'Target'"),
            ]);

        var result = await Bicep(
            "docs",
            "generate",
            Path.Combine(root, "target", "main.bicep"));

        result.ExitCode.Should().Be(0);
        File.Exists(Path.Combine(root, "target", "PARENT.md")).Should().BeTrue();
    }

    [TestMethod]
    public async Task Config_MissingBicepConfigUsesBuiltInDefaults()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [new("main.bicep", "metadata name = 'Defaults'")]);

        var result = await Bicep(
            "docs",
            "generate",
            Path.Combine(root, "main.bicep"));

        result.ExitCode.Should().Be(0);
        result.Stderr.Should().NotContain(ConventionalConfigFileName);
        File.Exists(Path.Combine(root, "README.md")).Should().BeTrue();
    }

    [DataTestMethod]
    [DataRow("{ invalid", "invalid")]
    [DataRow("""{ "documentation": { "output": { "file": "../README.md" } } }""", "cannot traverse")]
    public async Task Config_InvalidBicepConfigReturnsNamedError(string contents, string expectedError)
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("main.bicep", "metadata name = 'Invalid config'"),
                new("bicepconfig.json", contents),
            ]);
        var configPath = Path.Combine(root, ConventionalConfigFileName);

        var result = await Bicep(
            "docs",
            "generate",
            "--stdout",
            Path.Combine(root, "main.bicep"));

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().Contain(configPath);
        result.Stderr.Should().Contain(expectedError);
    }

    [TestMethod]
    public async Task Generate_PatternSelectsMultipleInputsWithoutConfigInputSettings()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("modules/a/module.bicep", "metadata name = 'A'"),
                new("modules/a/MODULE.TEST.BICEP", "metadata name = 'Excluded'"),
                new("modules/b/module.bicep", "metadata name = 'B'"),
                new("modules/c/module.bicep", "metadata name = 'C'"),
                new("bicepconfig.json", """
                    {
                      "documentation": {
                        "output": {
                          "file": "DOCS.md"
                        }
                      }
                    }
                    """),
            ]);

        var generateResult = await Bicep(
            "docs",
            "generate",
            "--pattern",
            Path.Combine(root, "modules", "*", "module.bicep"));

        generateResult.ExitCode.Should().Be(0);
        File.Exists(Path.Combine(root, "modules", "a", "DOCS.md")).Should().BeTrue();
        File.Exists(Path.Combine(root, "modules", "b", "DOCS.md")).Should().BeTrue();
        File.Exists(Path.Combine(root, "modules", "c", "DOCS.md")).Should().BeTrue();
        File.Exists(Path.Combine(root, "modules", "a", "README.md")).Should().BeFalse();
    }

    [TestMethod]
    public async Task Generate_UnmatchedPatternWritesNothing()
    {
        var result = await Bicep(
            "docs",
            "generate",
            "--pattern",
            Path.Combine(FileHelper.GetUniqueTestOutputPath(TestContext), "**", "main.bicep"));

        result.ExitCode.Should().Be(0);
        result.Stdout.Should().BeEmpty();
        result.Stderr.Should().NotContain("Unhandled exception");
    }

    [TestMethod]
    public async Task Config_PatternGenerationUsesConfiguredOutputForEveryModule()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("modules/a/main.bicep", "metadata name = 'A'"),
                new("modules/b/main.bicep", "metadata name = 'B'"),
                new("bicepconfig.json", """
                    {
                      "documentation": {
                        "output": { "file": "DOCS.md" }
                      }
                    }
                    """),
            ]);

        var result = await Bicep(
            "docs",
            "generate",
            "--pattern",
            Path.Combine(root, "modules", "*", "main.bicep"));

        result.ExitCode.Should().Be(0);
        File.Exists(Path.Combine(root, "modules", "a", "DOCS.md")).Should().BeTrue();
        File.Exists(Path.Combine(root, "modules", "b", "DOCS.md")).Should().BeTrue();
        Directory.EnumerateFiles(root, "README.md", SearchOption.AllDirectories).Should().BeEmpty();
    }

    [TestMethod]
    public async Task Generate_Pattern_ContinuesAfterCompilationFailure()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("valid/main.bicep", "metadata name = 'Valid'\nparam value string = 'ok'"),
                new("invalid/main.bicep", "param value invalidType"),
            ]);
        var pattern = Path.Combine(root, "**", "main.bicep");

        var result = await Bicep(DocsEnabledSettings(), "docs", "generate", "--pattern", pattern);

        result.ExitCode.Should().Be(1);
        File.Exists(Path.Combine(root, "valid", "README.md")).Should().BeTrue();
        File.Exists(Path.Combine(root, "invalid", "README.md")).Should().BeFalse();
        result.Stderr.Should().Contain("invalidType");
    }

    [TestMethod]
    public async Task Generate_PatternWithOutDir_PreservesRelativeDirectories()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("modules/a/main.bicep", "metadata name = 'A'"),
                new("modules/b/main.bicep", "metadata name = 'B'"),
            ]);
        var outputRoot = Path.Combine(root, "generated");

        var result = await Bicep(
            "docs",
            "generate",
            "--pattern",
            Path.Combine(root, "modules", "**", "main.bicep"),
            "--outdir",
            outputRoot);

        result.ExitCode.Should().Be(0);
        File.Exists(Path.Combine(outputRoot, "a", "README.md")).Should().BeTrue();
        File.Exists(Path.Combine(outputRoot, "b", "README.md")).Should().BeTrue();
    }

    [TestMethod]
    public async Task Generate_RootPatternWithOutDir_WritesReadmeAtOutputRoot()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [new("main.bicep", "metadata name = 'Root'")]);
        var outputRoot = Path.Combine(root, "generated");

        var result = await Bicep(
            "docs",
            "generate",
            "--pattern",
            Path.Combine(root, "*.bicep"),
            "--outdir",
            outputRoot);

        result.ExitCode.Should().Be(0);
        File.Exists(Path.Combine(outputRoot, "README.md")).Should().BeTrue();
    }

    [TestMethod]
    public async Task Generate_CompilationFailure_DoesNotOverwriteExistingOutput()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("main.bicep", "param value invalidType"),
                new("README.md", "preserve me"),
            ]);

        var result = await Bicep(DocsEnabledSettings(), "docs", "generate", Path.Combine(root, "main.bicep"));

        result.ExitCode.Should().Be(1);
        File.ReadAllText(Path.Combine(root, "README.md")).Should().Be("preserve me");
    }

    [TestMethod]
    public async Task Generate_TemplateFailure_DoesNotOverwriteExistingOutput()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("main.bicep", "param value string = 'ok'"),
                new("invalid.scriban", "{{ if module.name }}"),
                new("README.md", "preserve me"),
                new("bicepconfig.json", """{ "documentation": { "template": { "file": "invalid.scriban" } } }"""),
            ]);

        var result = await Bicep(
            DocsEnabledSettings(),
            "docs",
            "generate",
            Path.Combine(root, "main.bicep"));

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().Contain("Failed to parse");
        File.ReadAllText(Path.Combine(root, "README.md")).Should().Be("preserve me");
    }

    [TestMethod]
    public async Task Generate_TemplateFailureWithSarif_EmitsOneValidLog()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("main.bicep", "metadata name = 'Example'"),
                new("invalid.scriban", "{{ if module.name }}"),
                new("bicepconfig.json", """{ "documentation": { "template": { "file": "invalid.scriban" } } }"""),
            ]);

        var result = await Bicep(
            DocsEnabledSettings(),
            "docs",
            "generate",
            Path.Combine(root, "main.bicep"),
            "--diagnostics-format",
            "sarif");

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().Contain("Failed to parse");
    }

    [TestMethod]
    public async Task Generate_WriteFailure_ReturnsNonZero()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("main.bicep", "metadata name = 'Example'"),
                new("README.md", "preserve me"),
            ]);
        var fileSystem = new System.IO.Abstractions.FileSystem();
        var fileExplorer = new WriteFailingFileExplorer(
            new FileSystemFileExplorer(fileSystem),
            "README.md",
            new IOException("write failed"));

        var result = await Bicep(
            DocsEnabledSettings(),
            services => services
                .AddSingleton<IFileSystem>(fileSystem)
                .AddSingleton<IFileExplorer>(fileExplorer),
            TestContext.CancellationTokenSource.Token,
            "docs",
            "generate",
            Path.Combine(root, "main.bicep"));

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().Contain("write failed");
        File.ReadAllText(Path.Combine(root, "README.md")).Should().Be("preserve me");
    }

    [TestMethod]
    public async Task Generate_WriteFailureWithSarif_ReturnsTheWriteError()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [new("main.bicep", "metadata name = 'Example'")]);
        var fileSystem = new System.IO.Abstractions.FileSystem();
        var fileExplorer = new WriteFailingFileExplorer(
            new FileSystemFileExplorer(fileSystem),
            "README.md",
            new IOException("write failed"));

        var result = await Bicep(
            DocsEnabledSettings(),
            services => services
                .AddSingleton<IFileSystem>(fileSystem)
                .AddSingleton<IFileExplorer>(fileExplorer),
            TestContext.CancellationTokenSource.Token,
            "docs",
            "generate",
            Path.Combine(root, "main.bicep"),
            "--diagnostics-format",
            "sarif");

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().Contain("write failed");
    }

    [TestMethod]
    public async Task Generate_OutputFile_ChangesOnlyTheDestinationName()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [new("main.bicep", "metadata name = 'Example'\nparam value string = 'ok'")]);

        var defaultResult = await Bicep(DocsEnabledSettings(), "docs", "generate", "--stdout", Path.Combine(root, "main.bicep"));
        var generateResult = await Bicep(
            DocsEnabledSettings(),
            "docs",
            "generate",
            Path.Combine(root, "main.bicep"),
            "--outfile",
            Path.Combine(root, "MODULE.md"));

        generateResult.ExitCode.Should().Be(0);
        File.ReadAllText(Path.Combine(root, "MODULE.md")).Should().Be(defaultResult.Stdout);
        File.Exists(Path.Combine(root, "README.md")).Should().BeFalse();
    }

    [TestMethod]
    public async Task Generate_RejectsBicepOutputExtension()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [new("main.bicep", "metadata name = 'Example'")]);

        var result = await Bicep(
            "docs",
            "generate",
            Path.Combine(root, "main.bicep"),
            "--outfile",
            Path.Combine(root, "output.bicep"));

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().Contain("cannot use a Bicep source file extension");
        File.Exists(Path.Combine(root, "output.bicep")).Should().BeFalse();
    }

    [TestMethod]
    public async Task Generate_RejectsInputOverwrite()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [new("main.bicep", "metadata name = 'Example'")]);
        var mainFile = Path.Combine(root, "main.bicep");

        var result = await Bicep(
            "docs",
            "generate",
            mainFile,
            "--outfile",
            mainFile);

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().Contain("cannot overwrite the input");
        File.ReadAllText(mainFile).Should().Contain("metadata name");

        if (OperatingSystem.IsWindows())
        {
            var aliasedResult = await Bicep(
                "docs",
                "generate",
                mainFile,
                "--outfile",
                $"{mainFile}.");

            aliasedResult.ExitCode.Should().Be(1);
            aliasedResult.Stderr.Should().Contain("cannot overwrite the input");
            File.ReadAllText(mainFile).Should().Contain("metadata name");
        }
    }

    [TestMethod]
    public async Task Commands_RejectReservedWindowsPathsWithoutCrashing()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        var root = FileHelper.SaveResultFiles(
            TestContext,
            [new("main.bicep", "metadata name = 'Example'")]);

        var outputResult = await Bicep(
            "docs",
            "generate",
            "--stdout",
            Path.Combine(root, "CON"));
        var generateResult = await Bicep(
            "docs",
            "generate",
            Path.Combine(root, "main.bicep"),
            "--outfile",
            Path.Combine(root, "CON.md"));

        outputResult.ExitCode.Should().Be(1);
        outputResult.Stderr.Should().Contain("reserved file name");
        outputResult.Stderr.Should().NotContain("Unhandled exception");
        generateResult.ExitCode.Should().Be(1);
        generateResult.Stderr.Should().Contain("reserved file name");
        generateResult.Stderr.Should().NotContain("Unhandled exception");
    }

    [TestMethod]
    public async Task Generate_PatternRejectsCollidingOutputFilesBeforeWriting()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("a.bicep", "metadata name = 'A'"),
                new("b.bicep", "metadata name = 'B'"),
            ]);

        var result = await Bicep(
            "docs",
            "generate",
            "--pattern",
            Path.Combine(root, "*.bicep"));

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().Contain("resolve to the output file");
        File.Exists(Path.Combine(root, "README.md")).Should().BeFalse();
    }

    [TestMethod]
    public async Task Output_SarifDiagnostics_KeepStdoutEmptyOnFailure()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [new("main.bicep", "param value invalidType")]);

        var result = await Bicep(
            DocsEnabledSettings(),
            "docs",
            "generate",
            "--stdout",
            Path.Combine(root, "main.bicep"),
            "--diagnostics-format",
            "sarif");

        result.ExitCode.Should().Be(1);
        result.Stdout.Should().BeEmpty();
        result.Stderr.Should().ContainAll("\"runs\"", "invalidType");
    }

    [TestMethod]
    public async Task Output_TemplateFailureWithSarif_EmitsOneValidLog()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("main.bicep", "metadata name = 'Example'"),
                new("invalid.scriban", "{{ if module.name }}"),
                new("bicepconfig.json", """{ "documentation": { "template": { "file": "invalid.scriban" } } }"""),
            ]);

        var result = await Bicep(
            DocsEnabledSettings(),
            "docs",
            "generate",
            "--stdout",
            Path.Combine(root, "main.bicep"),
            "--diagnostics-format",
            "sarif");

        result.ExitCode.Should().Be(1);
        result.Stdout.Should().BeEmpty();
        result.Stderr.Should().Contain("Failed to parse");
    }

    [TestMethod]
    public async Task Generate_PatternSarifDiagnostics_EmitsOneLogPerInput()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("valid/main.bicep", "metadata name = 'Valid'"),
                new("invalid/main.bicep", "param value invalidType"),
            ]);

        var result = await Bicep(
            DocsEnabledSettings(),
            "docs",
            "generate",
            "--pattern",
            Path.Combine(root, "*", "main.bicep"),
            "--diagnostics-format",
            "sarif");

        result.ExitCode.Should().Be(1);
        result.Stdout.Should().BeEmpty();
        result.Stderr.Split("\"runs\"", StringSplitOptions.None).Should().HaveCount(3);
        result.Stderr.Should().Contain("invalidType");
        result.Stderr.Should().NotContain("WARNING:");
        File.Exists(Path.Combine(root, "valid", "README.md")).Should().BeTrue();
        File.Exists(Path.Combine(root, "invalid", "README.md")).Should().BeFalse();
    }

    [TestMethod]
    public async Task Generate_PatternCompilationFailureThenSuccess_LogsExperimentalDisclaimerOnce()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("a-invalid/main.bicep", "param value invalidType"),
                new("b-valid/main.bicep", "metadata name = 'Valid'"),
            ]);

        var result = await Bicep(
            "docs",
            "generate",
            "--pattern",
            Path.Combine(root, "*", "main.bicep"));

        result.ExitCode.Should().Be(1);
        result.Stderr.Split(
                "following experimental Bicep features have been enabled: docs",
                StringSplitOptions.None)
            .Should().HaveCount(2);
        File.Exists(Path.Combine(root, "a-invalid", "README.md")).Should().BeFalse();
        File.Exists(Path.Combine(root, "b-valid", "README.md")).Should().BeTrue();
    }

    [TestMethod]
    public async Task CommandRunner_ObservesCancellationBeforeCompilation()
    {
        var runner = new DocsCommandRunner(null!, null!, null!, null!, null!);
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        await FluentActions.Invoking(() => runner.RenderAsync(
                IOUri.FromFilePath(Path.GetFullPath("main.bicep")),
                noRestore: false,
                diagnosticsFormat: null,
                workspace: null!,
                cancellationToken: cancellation.Token))
            .Should().ThrowAsync<OperationCanceledException>();
    }

    [TestMethod]
    public async Task Generate_CompilationSetupFailure_ReturnsTheCompilerError()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["/main.bicep"] = "metadata name = 'Example'",
        });
        var mainFile = IOUri.FromFilePath(fileSystem.Path.GetFullPath("/main.bicep"));
        var innerExplorer = new FileSystemFileExplorer(fileSystem);
        var fileExplorer = new Mock<IFileExplorer>(MockBehavior.Strict);
        fileExplorer
            .Setup(explorer => explorer.GetDirectory(It.IsAny<IOUri>()))
            .Returns((IOUri uri) => innerExplorer.GetDirectory(uri));
        fileExplorer
            .Setup(explorer => explorer.GetFile(It.IsAny<IOUri>()))
            .Returns((IOUri uri) => uri.Equals(mainFile)
                ? throw new BicepException("compilation setup failed")
                : innerExplorer.GetFile(uri));
        Action<IServiceCollection> registerServices = services => services
            .AddSingleton<System.IO.Abstractions.IFileSystem>(fileSystem)
            .AddSingleton(fileExplorer.Object);

        var defaultResult = await Bicep(
            DocsEnabledSettings(),
            registerServices,
            TestContext.CancellationTokenSource.Token,
            "docs",
            "generate",
            "/main.bicep");
        var sarifResult = await Bicep(
            DocsEnabledSettings(),
            registerServices,
            TestContext.CancellationTokenSource.Token,
            "docs",
            "generate",
            "/main.bicep",
            "--diagnostics-format",
            "sarif");
        var outputSarifResult = await Bicep(
            DocsEnabledSettings(),
            registerServices,
            TestContext.CancellationTokenSource.Token,
            "docs",
            "generate",
            "--stdout",
            "/main.bicep",
            "--diagnostics-format",
            "sarif");

        defaultResult.ExitCode.Should().Be(1);
        defaultResult.Stderr.Should().Contain("compilation setup failed");
        sarifResult.ExitCode.Should().Be(1);
        sarifResult.Stderr.Should().Contain("compilation setup failed");
        outputSarifResult.ExitCode.Should().Be(1);
        outputSarifResult.Stdout.Should().BeEmpty();
        outputSarifResult.Stderr.Should().Contain("compilation setup failed");
    }

    [DataTestMethod]
    [DataRow("missing.bicep")]
    [DataRow("module.txt")]
    public async Task Output_InvalidInput_ReturnsNonZero(string fileName)
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [new("module.txt", "not bicep")]);

        var result = await Bicep(
            DocsEnabledSettings(),
            "docs",
            "generate",
            "--stdout",
            Path.Combine(root, fileName));

        result.ExitCode.Should().Be(1);
        result.Stdout.Should().BeEmpty();
        result.Stderr.Should().NotBeEmpty();
    }

    [DataTestMethod]
    [DataRow(typeof(IOException))]
    [DataRow(typeof(UnauthorizedAccessException))]
    [DataRow(typeof(ArgumentException))]
    [DataRow(typeof(NotSupportedException))]
    public async Task Output_WrapsInputPathExceptions(Type exceptionType)
    {
        var exception = (Exception)Activator.CreateInstance(exceptionType, "invalid path")!;
        var fileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
        var path = new Mock<IPath>(MockBehavior.Strict);
        fileSystem.SetupGet(system => system.Path).Returns(path.Object);
        path.Setup(systemPath => systemPath.GetFullPath("invalid")).Throws(exception);

        var result = await Bicep(
            DocsEnabledSettings(),
            services => services.AddSingleton(fileSystem.Object),
            TestContext.CancellationTokenSource.Token,
            "docs",
            "generate",
            "--stdout",
            "invalid");

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().Contain("invalid path");
    }

    [TestMethod]
    public async Task Generate_RejectsMissingTemplateFile()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("main.bicep", "metadata name = 'Example'"),
                new("bicepconfig.json", """{ "documentation": { "template": { "file": "missing.scriban" } } }"""),
            ]);

        var result = await Bicep(
            "docs",
            "generate",
            "--stdout",
            Path.Combine(root, "main.bicep"));

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().Contain("does not exist");
    }

    [DataTestMethod]
    [DataRow(["docs", "generate", "main.bicep", "--custom-template-value"])]
    [DataRow(["docs", "generate", "main.bicep", "--custom-template-value", "invalid"])]
    [DataRow(["docs", "generate", "main.bicep", "--custom-template-value-file-path"])]
    [DataRow(["docs", "generate", "main.bicep", "--template-file", "template.scriban"])]
    [DataRow(["docs", "generate", "main.bicep", "--template-root", "templates"])]
    [DataRow(["docs", "generate", "main.bicep", "--pattern", "**/main.bicep", "--outfile", "README.md"])]
    [DataRow(["docs", "generate", "main.bicep", "--stdout", "--pattern", "**/main.bicep"])]
    [DataRow(["docs", "generate", "main.bicep", "--stdout", "--outdir", "docs"])]
    [DataRow(["docs", "generate", "main.bicep", "--stdout", "--outfile", "README.md"])]
    public async Task InvalidArguments_ReturnNonZero(string[] arguments)
    {
        var result = await Bicep(DocsEnabledSettings(), arguments);

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().NotBeEmpty();
    }

    [TestMethod]
    public async Task Generate_RejectsInputPathWithPattern()
    {
        var result = await Bicep(
            DocsEnabledSettings(),
            "docs",
            "generate",
            "main.bicep",
            "--pattern",
            "**/main.bicep");

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().Contain("cannot both be specified");
    }

    [TestMethod]
    public async Task Generate_RejectsMissingTemplateRoot()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("main.bicep", "param value string = 'ok'"),
                new("bicepconfig.json", """{ "documentation": { "template": { "includeRoot": "missing" } } }"""),
            ]);

        var result = await Bicep(
            DocsEnabledSettings(),
            "docs",
            "generate",
            "--stdout",
            Path.Combine(root, "main.bicep"));

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().Contain("does not exist");
    }

    [TestMethod]
    public async Task Generate_MissingTemplateRootWithSarifReturnsTheConfigurationError()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("main.bicep", "param value string = 'ok'"),
                new("bicepconfig.json", """{ "documentation": { "template": { "includeRoot": "missing" } } }"""),
            ]);

        var result = await Bicep(
            "docs",
            "generate",
            "--stdout",
            Path.Combine(root, "main.bicep"),
            "--diagnostics-format",
            "sarif");

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().Contain("does not exist");
    }

    [TestMethod]
    public void InputOutputResolver_UsesFixedReadmeNameForDocs()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["/module/main.bicep"] = "metadata name = 'Example'",
        });
        var resolver = new InputOutputArgumentsResolver(fileSystem);
        var arguments = new DocsGenerateArguments(
            "/module/main.bicep",
            null,
            false,
            null,
            null,
            false,
            null);

        var defaultOutput = resolver.ResolveFilePatternInputOutputArguments(arguments, (_, _) => "README.md");
        var explicitOutput = resolver.ResolveFilePatternInputOutputArguments(
            arguments with { OutputFile = "/docs/custom.md" },
            (_, _) => "README.md");
        var fixedOutDir = resolver.ResolveFilePatternInputOutputArguments(
            arguments with { OutputDir = "/docs" },
            (_, _) => "README.md");
        var extensionOutput = resolver.ResolveFilePatternInputOutputArguments(arguments);
        FluentActions.Invoking(() => resolver.ResolveFilePatternInputOutputArguments(
            arguments with { InputFile = null }))
            .Should().Throw<CommandLineException>();

        Path.GetFileName(defaultOutput.Single().OutputUri.GetFilePath()).Should().Be("README.md");
        explicitOutput.Single().OutputUri.GetFilePath().Should().Be(fileSystem.Path.GetFullPath("/docs/custom.md"));
        fixedOutDir.Single().OutputUri.GetFilePath().Should().Be(fileSystem.Path.GetFullPath("/docs/README.md"));
        Path.GetFileName(extensionOutput.Single().OutputUri.GetFilePath()).Should().Be("main.md");

        var physicalRoot = FileHelper.SaveResultFiles(
            TestContext,
            [new("main.bicep", "metadata name = 'Pattern'")]);
        var physicalFileSystem = new FileSystem();
        var physicalResolver = new InputOutputArgumentsResolver(physicalFileSystem);
        var patternArguments = arguments with
        {
            InputFile = null,
            FilePattern = Path.Combine(physicalRoot, "*.bicep"),
        };
        var patternExplicitOutput = physicalResolver.ResolveFilePatternInputOutputArguments(
            patternArguments with { OutputFile = Path.Combine(physicalRoot, "pattern.md") });
        var patternExtensionOutput = physicalResolver.ResolveFilePatternInputOutputArguments(
            patternArguments with { OutputDir = Path.Combine(physicalRoot, "docs") });

        patternExplicitOutput.Single().OutputUri.GetFilePath().Should().Be(Path.Combine(physicalRoot, "pattern.md"));
        patternExtensionOutput.Single().OutputUri.GetFilePath().Should().Be(Path.Combine(physicalRoot, "docs", "main.md"));
    }

    [TestMethod]
    public void InputOutputResolver_PreservesExtensionBehaviorWithoutFixedName()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [new("nested/main.bicep", "metadata name = 'Nested'")]);
        var fileSystem = new System.IO.Abstractions.FileSystem();
        var resolver = new InputOutputArgumentsResolver(fileSystem);
        var arguments = new DocsGenerateArguments(
            null,
            Path.Combine(root, "*", "main.bicep"),
            false,
            null,
            null,
            false,
            null);

        var output = resolver.ResolveFilePatternInputOutputArguments(arguments);

        output.Should().ContainSingle();
        output.Single().OutputUri.GetFilePath().Should().Be(Path.Combine(root, "nested", "main.md"));
    }

    [TestMethod]
    public void OptionsResolver_AnchorsConfiguredPathsAndUsesConfiguredValues()
    {
        var fileSystem = new MockFileSystem();
        var configPath = fileSystem.Path.GetFullPath("/repo/bicepconfig.json");
        var templateRoot = fileSystem.Path.GetFullPath("/repo/templates");
        fileSystem.AddDirectory(templateRoot);
        var configuration = BicepTestConstants.BuiltInConfiguration.With(
            documentation: DocumentationConfiguration.Bind(JsonElementFactory.CreateElement("""
                {
                  "template": {
                    "file": "templates/readme.scriban",
                    "includeRoot": "templates",
                    "values": {
                      "owner": "Config",
                      "retained": "yes"
                    }
                  }
                }
                """)),
            configFileIdentifier: IOUri.FromFilePath(configPath));
        var resolver = new DocsGenerationOptionsResolver(
            new InputOutputArgumentsResolver(fileSystem),
            fileSystem);

        var options = resolver.Resolve(configuration);

        options.TemplateFile!.GetFilePath().Should().Be(fileSystem.Path.Combine(templateRoot, "readme.scriban"));
        options.TemplateRoot!.GetFilePath().TrimEnd(fileSystem.Path.DirectorySeparatorChar)
            .Should().Be(templateRoot.TrimEnd(fileSystem.Path.DirectorySeparatorChar));
        options.CustomValues.Should().Contain("owner", "Config").And.Contain("retained", "yes");
    }

    [TestMethod]
    public void OptionsResolver_RejectsRelativeConfiguredPathWithoutConfigFile()
    {
        var fileSystem = new MockFileSystem();
        var configuration = BicepTestConstants.BuiltInConfiguration.With(
            documentation: DocumentationConfiguration.Bind(JsonElementFactory.CreateElement("""
                {
                  "template": {
                    "file": "templates/readme.scriban"
                  }
                }
                """)));
        var resolver = new DocsGenerationOptionsResolver(
            new InputOutputArgumentsResolver(fileSystem),
            fileSystem);

        FluentActions.Invoking(() => resolver.Resolve(configuration))
            .Should().Throw<CommandLineException>()
            .WithMessage("*no bicepconfig.json file was resolved*");
    }

}
