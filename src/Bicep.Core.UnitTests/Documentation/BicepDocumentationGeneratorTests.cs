// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Bicep.Core.Documentation;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.IO.Abstraction;
using Bicep.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Core.UnitTests.Documentation;

[TestClass]
public class BicepDocumentationGeneratorTests
{
    private const string ComprehensiveModule = """
        metadata name = 'Storage Module'
        metadata description = 'Creates a storage account with example telemetry and diagnostics settings.'

        @description('Name of the storage account.')
        @minLength(3)
        @maxLength(24)
        param storageAccountName string

        @description('Azure region for the resources.')
        param location string = 'westus'

        @description('Storage account SKU name.')
        @allowed([
          'Standard_LRS'
          'Standard_GRS'
        ])
        param skuName string = 'Standard_LRS'

        @description('Number of days to retain diagnostic logs.')
        @minValue(1)
        @maxValue(365)
        param retentionInDays int = 30

        @description('Administrator password for the jumpbox.')
        @secure()
        param adminPassword string

        @description('Network rule configuration for the storage account.')
        param networkRule networkRuleUnion = {
          type: 'allowAll'
        }

        @description('Enables usage telemetry for this module.')
        param enableTelemetry bool = true

        @export()
        @description('An allow-all network rule.')
        type allowAllNetworkRule = {
          type: 'allowAll'
        }

        @export()
        @description('An IP-restricted network rule.')
        type ipRestrictedNetworkRule = {
          type: 'ipRestricted'
          @description('Allowed IP ranges in CIDR notation.')
          allowedIpRanges: string[]
        }

        @export()
        @discriminator('type')
        type networkRuleUnion = allowAllNetworkRule | ipRestrictedNetworkRule

        resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
          name: storageAccountName
          location: location
          sku: {
            name: skuName
          }
          kind: 'StorageV2'
          tags: {
            retentionInDays: string(retentionInDays)
            hasAdminPassword: string(length(adminPassword) > 0)
            networkRuleType: networkRule.type
          }
        }

        resource existingVnet 'Microsoft.Network/virtualNetworks@2023-09-01' existing = {
          name: 'existing-vnet'
        }

        module logging 'modules/logging.bicep' = {
          name: 'loggingDeployment'
          params: {
            location: location
          }
        }

        @export()
        @description('Builds a resource tag object from an environment name.')
        func buildTags(environmentName string) object => {
          environment: environmentName
        }

        @description('The resource ID of the storage account.')
        output storageAccountId string = storageAccount.id
        """;

    private const string LoggingModule = """
        @description('Azure region for the resources.')
        param location string

        output workspaceId string = 'workspace-id'
        """;

    [TestMethod]
    public async Task BuildModel_ComprehensiveModule_ProjectsDeterministicMetadataAndConstraints()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile(
            ("main.bicep", ComprehensiveModule),
            ("modules/logging.bicep", LoggingModule));

        result.Diagnostics.Should().NotContain(d => d.Level == Bicep.Core.Diagnostics.DiagnosticLevel.Error);

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation);

        model.Name.Should().Be("Storage Module");
        model.Description.Should().Be("Creates a storage account with example telemetry and diagnostics settings.");
        model.TargetScope.Should().Be("resourceGroup");
        model.Custom.Should().BeEmpty();

        model.ResourceTypes.Select(r => (r.Type, r.IsExisting)).Should().BeEquivalentTo(
        [
            ("Microsoft.Network/virtualNetworks@2023-09-01", true),
            ("Microsoft.Storage/storageAccounts@2023-01-01", false),
        ], options => options.WithStrictOrdering());

        // Deterministic ordering: case-insensitive, ordinal tie-break.
        model.Parameters.Select(p => p.Name).Should().Equal(
            "adminPassword",
            "enableTelemetry",
            "location",
            "networkRule",
            "retentionInDays",
            "skuName",
            "storageAccountName");

        var storageAccountName = model.Parameters.Single(p => p.Name == "storageAccountName");
        storageAccountName.TypeName.Should().Be("string");
        storageAccountName.IsRequired.Should().BeTrue();
        storageAccountName.IsSecure.Should().BeFalse();
        storageAccountName.MinLength.Should().Be(3);
        storageAccountName.MaxLength.Should().Be(24);
        storageAccountName.DefaultValue.Should().BeNull();

        var location = model.Parameters.Single(p => p.Name == "location");
        location.IsRequired.Should().BeFalse();
        location.DefaultValue.Should().Be("'westus'");

        var skuName = model.Parameters.Single(p => p.Name == "skuName");
        skuName.TypeName.Should().Be("string");
        skuName.AllowedValues.Should().Equal("Standard_GRS", "Standard_LRS");

        var retentionInDays = model.Parameters.Single(p => p.Name == "retentionInDays");
        retentionInDays.TypeName.Should().Be("int");
        retentionInDays.MinValue.Should().Be(1);
        retentionInDays.MaxValue.Should().Be(365);

        var adminPassword = model.Parameters.Single(p => p.Name == "adminPassword");
        adminPassword.IsSecure.Should().BeTrue();

        var networkRule = model.Parameters.Single(p => p.Name == "networkRule");
        networkRule.Discriminator.Should().NotBeNull();
        networkRule.Discriminator!.PropertyName.Should().Be("type");
        networkRule.Discriminator.Cases.Select(c => c.Value).Should().Equal("allowAll", "ipRestricted");

        var ipRestrictedCase = networkRule.Discriminator.Cases.Single(c => c.Value == "ipRestricted");
        ipRestrictedCase.Properties.Select(p => p.Name).Should().Contain("allowedIpRanges");
        var allowedIpRanges = ipRestrictedCase.Properties.Single(p => p.Name == "allowedIpRanges");
        allowedIpRanges.TypeName.Should().Be("array");
        allowedIpRanges.Description.Should().Be("Allowed IP ranges in CIDR notation.");

        model.Outputs.Select(o => o.Name).Should().Equal("storageAccountId");
        model.Outputs.Single().TypeName.Should().Be("string");

        model.ExportedFunctions.Select(f => f.Name).Should().Equal("buildTags");
        var buildTags = model.ExportedFunctions.Single();
        buildTags.Parameters.Select(p => p.Name).Should().Equal("environmentName");
        buildTags.ReturnTypeName.Should().Be("object");
        buildTags.Description.Should().Be("Builds a resource tag object from an environment name.");

        model.References.Should().ContainSingle();
        var reference = model.References.Single();
        reference.SymbolicName.Should().Be("logging");
        reference.Path.Should().Be("modules/logging.bicep");
        reference.Description.Should().BeNull();

    }

    [TestMethod]
    public async Task BuildModel_ModuleWithoutMetadataName_FallsBackToDirectoryName()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("param foo string = 'bar'");

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation);

        model.Name.Should().Be("to");
        model.Description.Should().BeNull();
    }

    [TestMethod]
    public void GetFallbackModuleName_RootModule_UsesEntryFileName()
    {
        var root = new Bicep.IO.Abstraction.IOUri("file", null, "/");
        var entryFile = new Bicep.IO.Abstraction.IOUri("file", null, "/main.bicep");

        BicepDocumentationGenerator.GetFallbackModuleName(root, entryFile).Should().Be("main");
    }

    [TestMethod]
    public async Task BuildModel_CompilationWithErrors_ThrowsBicepDocumentationException()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("param foo invalidType");

        result.Diagnostics.Should().NotBeEmpty();

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var act = () => generator.BuildModel(result.Compilation);

        act.Should().Throw<BicepDocumentationException>();
    }

    [TestMethod]
    public async Task Generate_CompilationWithErrors_ThrowsBicepDocumentationException()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("param foo invalidType");

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var act = () => generator.Generate(result.Compilation);

        act.Should().Throw<BicepDocumentationException>();
    }

    [TestMethod]
    public async Task Generate_WithExplicitOptions_UsesProvidedOptions()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("param foo string = 'bar'");

        compiler.FileSet.AddFile("readme.scriban", "# {{ module.name }}");

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var options = new BicepDocumentationGenerationOptions(
            TemplateFile: compiler.FileSet.GetUri("readme.scriban"),
            TemplateRoot: null,
            CustomValues: null);

        var rendered = generator.Generate(result.Compilation, options);

        rendered.Should().Be("# to\n");
    }

    [TestMethod]
    public async Task Generate_WithConfiguredExampleSources_UsesOptionsDuringModelConstruction()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile(
            ("main.bicep", "metadata name = 'Configured examples'"),
            ("examples/default/main.bicep", "metadata name = 'default'"),
            ("samples/custom/example.bicep", "metadata name = 'custom'"));
        compiler.FileSet.AddFile(
            "readme.scriban",
            "{{ for example in module.usageExamples }}{{ example.name }}{{ end }}");
        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var options = new BicepDocumentationGenerationOptions(
            TemplateFile: compiler.FileSet.GetUri("readme.scriban"),
            TemplateRoot: null,
            CustomValues: null)
        {
            Examples = new()
            {
                Sources =
                [
                    new()
                    {
                        Path = "samples",
                        Include = ["**/*.bicep"],
                    },
                ],
            },
        };

        var rendered = generator.Generate(result.Compilation, options);

        rendered.Should().Be("custom\n");
    }

    [TestMethod]
    public void GenerationOptions_EqualityAndWith_BehaveAsValueRecord()
    {
        var options = BicepDocumentationGenerationOptions.Default;
        var clone = options with { };
        var different = options with { TemplateFile = IOUri.FromFilePath(Path.GetFullPath("readme.scriban")) };
        var differentExamples = options with
        {
            Examples = new() { Sources = [] },
        };

        options.Should().Be(clone);
        (options == clone).Should().BeTrue();
        options.Should().NotBe(different);
        options.Should().NotBe(differentExamples);
        options.GetHashCode().Should().Be(clone.GetHashCode());
        options.ToString().Should().Contain("TemplateFile");
    }

    [TestMethod]
    public void DocumentationConfiguration_DefaultsAreCompleteAndReplaceable()
    {
        var configuration = new BicepDocumentationConfiguration();
        var clone = configuration with { };
        var withoutExamples = configuration with
        {
            Examples = configuration.Examples with { Sources = [] },
        };
        var custom = new BicepDocumentationConfiguration
        {
            Schema = "https://example.com/bicepdocsconfig.schema.json",
            Input = new()
            {
                Include = ["**/*.module.bicep"],
                Exclude = ["**/*.test.bicep"],
            },
            Output = new() { File = "DOCS.md" },
            Template = new()
            {
                File = "readme.scriban",
                IncludeRoot = "templates",
                Values = ImmutableSortedDictionary<string, string>.Empty.Add("owner", "Platform"),
            },
            Examples = new()
            {
                Sources =
                [
                    new()
                    {
                        Path = "samples",
                        Include = ["**/*.demo"],
                        Exclude = ["**/ignored/**"],
                    },
                ],
                Reassignments =
                [
                    new()
                    {
                        From = new()
                        {
                            Include = ["**/parent/**"],
                            Exclude = ["**/ignored/**"],
                        },
                        To = "child",
                    },
                ],
            },
        };

        configuration.Schema.Should().BeNull();
        configuration.Input.Include.Should().Equal("main.bicep");
        configuration.Input.Exclude.Should().BeEmpty();
        configuration.Output.File.Should().Be("README.md");
        configuration.Template.File.Should().BeNull();
        configuration.Template.IncludeRoot.Should().BeNull();
        configuration.Template.Values.Should().BeEmpty();
        configuration.Examples.Sources.Should().HaveCount(2);
        configuration.Examples.Reassignments.Should().BeEmpty();
        configuration.Should().Be(clone);
        configuration.Should().NotBe(withoutExamples);
        custom.Schema.Should().Be("https://example.com/bicepdocsconfig.schema.json");
        custom.Input.Include.Should().Equal("**/*.module.bicep");
        custom.Input.Exclude.Should().Equal("**/*.test.bicep");
        custom.Output.File.Should().Be("DOCS.md");
        custom.Template.File.Should().Be("readme.scriban");
        custom.Template.IncludeRoot.Should().Be("templates");
        custom.Template.Values.Should().ContainKey("owner");
        custom.Examples.Sources.Single().Path.Should().Be("samples");
        custom.Examples.Sources.Single().Include.Should().ContainSingle();
        custom.Examples.Sources.Single().Exclude.Should().ContainSingle();
        custom.Examples.Reassignments.Single().From.Include.Should().ContainSingle();
        custom.Examples.Reassignments.Single().From.Exclude.Should().ContainSingle();
        custom.Examples.Reassignments.Single().To.Should().Be("child");
        custom.Should().Be(custom with { });
        (custom.Output == (custom.Output with { })).Should().BeTrue();
        (custom.Template == (custom.Template with { })).Should().BeTrue();
        (custom.Examples.Sources.Single() == (custom.Examples.Sources.Single() with { })).Should().BeTrue();
        (custom.Examples.Reassignments.Single() == (custom.Examples.Reassignments.Single() with { })).Should().BeTrue();
        (custom.Examples.Reassignments.Single().From == (custom.Examples.Reassignments.Single().From with { })).Should().BeTrue();
        custom.Output.GetHashCode().Should().Be((custom.Output with { }).GetHashCode());
        custom.Template.ToString().Should().Contain("readme.scriban");
    }

    [TestMethod]
    public async Task BuildModel_ExamplesAndTestsFolders_DiscoversUsageExamplesDeterministically()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile(
            ("main.bicep", "param foo string = 'bar'"),
            ("examples/default/main.bicep", "// Deploys with default settings.\nmodule example '../../main.bicep' = { name: 'example' }"),
            ("examples/other.bicep", "module example '../main.bicep' = { name: 'example' }"),
            ("tests/e2e/defaults/main.test.bicep", "module test '../../../main.bicep' = { name: 'test' }"),
            ("notes.md", "This file must not be treated as a usage example."));

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation);

        model.UsageExamples.Select(e => e.RelativePath).Should().Equal(
            "examples/default/main.bicep",
            "examples/other.bicep",
            "tests/e2e/defaults/main.test.bicep");

        var defaultExample = model.UsageExamples.Single(e => e.RelativePath == "examples/default/main.bicep");
        defaultExample.Name.Should().Be("default");
        defaultExample.Description.Should().Be("Deploys with default settings.");
        defaultExample.Contents.Should().Contain("module example");

        var otherExample = model.UsageExamples.Single(e => e.RelativePath == "examples/other.bicep");
        otherExample.Name.Should().Be("other");
    }

    [TestMethod]
    public async Task Render_CustomTemplate_SupportsIncludesAndCustomValues()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("param foo string = 'bar'");

        compiler.FileSet.AddFile("readme.scriban", "{{ include \"_header.md\" }}\n# {{ module.name }}\nOwner: {{ custom.ownerDisplayName }} / {{ module.custom.ownerDisplayName }}\n");
        compiler.FileSet.AddFile("_header.md", "> Header content.");

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation, new Dictionary<string, string> { ["ownerDisplayName"] = "Old Team" });

        var options = new BicepDocumentationGenerationOptions(
            TemplateFile: compiler.FileSet.GetUri("readme.scriban"),
            TemplateRoot: null,
            CustomValues: new Dictionary<string, string> { ["ownerDisplayName"] = "Platform Team" });

        var rendered = generator.Render(model, options);

        rendered.Should().Be("> Header content.\n# to\nOwner: Platform Team / Platform Team\n");
    }

    [TestMethod]
    public async Task Render_CustomTemplate_SupportsLargeRepeatedIncludesWithoutTruncation()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("param foo string = 'bar'");
        var included = new string('x', 600_000);

        compiler.FileSet.AddFile("readme.scriban", "{{ include \"_large.md\" }}{{ include \"_large.md\" }}");
        compiler.FileSet.AddFile("_large.md", included);

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var options = new BicepDocumentationGenerationOptions(
            TemplateFile: compiler.FileSet.GetUri("readme.scriban"),
            TemplateRoot: null,
            CustomValues: null);

        var rendered = generator.Generate(result.Compilation, options);

        rendered.Should().Be(included + included + "\n");
    }

    [TestMethod]
    public async Task Render_CustomTemplate_AllowsMoreThanTheScribanDefaultLoopLimit()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("param foo string = 'bar'");

        compiler.FileSet.AddFile("readme.scriban", "{{ for i in 0..1001 }}x{{ end }}");

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var options = new BicepDocumentationGenerationOptions(
            TemplateFile: compiler.FileSet.GetUri("readme.scriban"),
            TemplateRoot: null,
            CustomValues: null);

        var rendered = generator.Generate(result.Compilation, options);

        rendered.Should().Be(new string('x', 1002) + "\n");
    }

    [TestMethod]
    public async Task Render_BuiltInTemplate_UsesFenceLongerThanEmbeddedExampleFence()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile(
            ("main.bicep", "metadata name = 'Fence example'"),
            ("examples/default/main.bicep", "var markdown = '''\n````\ncontent\n````\n'''"));

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var rendered = generator.Generate(result.Compilation);

        rendered.Should().Contain("`````bicep\nvar markdown");
        rendered.Should().Contain("\n````\ncontent\n````\n");
        rendered.Should().Contain("\n`````\n");
    }

    [TestMethod]
    public async Task Render_BuiltInTemplate_NumbersDuplicateExampleNames()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile(
            ("main.bicep", "metadata name = 'Duplicate examples'"),
            ("examples/first/main.bicep", "metadata name = 'same'"),
            ("examples/second/main.bicep", "metadata name = 'Same'"));

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var rendered = generator.Generate(result.Compilation);

        rendered.Should().Contain("### Example 1: _same_");
        rendered.Should().Contain("### Example 2: _Same_");
    }

    [TestMethod]
    public async Task BuildModel_ReparsePointExample_IsNotRead()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile(
            ("main.bicep", "metadata name = 'Safe'"),
            ("examples/leak/main.bicep", "sensitive contents"));
        var fileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
        var file = new Mock<IFile>(MockBehavior.Strict);
        fileSystem.SetupGet(system => system.File).Returns(file.Object);
        file.Setup(systemFile => systemFile.GetAttributes(It.IsAny<string>()))
            .Returns((string path) => path.EndsWith("main.bicep", StringComparison.OrdinalIgnoreCase)
                ? FileAttributes.ReparsePoint
                : FileAttributes.Normal);
        var generator = new BicepDocumentationGenerator(compiler.FileSet.FileExplorer, fileSystem.Object);

        var model = generator.BuildModel(result.Compilation);

        model.UsageExamples.Should().BeEmpty();
    }

    [TestMethod]
    public async Task BuildModel_WithoutFileSystem_StillDiscoversExamples()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile(
            ("main.bicep", "metadata name = 'Example'"),
            ("examples/default/main.bicep", "metadata name = 'Default'"));
        var generator = new BicepDocumentationGenerator(compiler.FileSet.FileExplorer);

        var model = generator.BuildModel(result.Compilation);

        model.UsageExamples.Should().ContainSingle();
    }

    [TestMethod]
    public async Task BuildModel_ObservesCancellation()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("param value string");
        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        var action = () => generator.BuildModel(result.Compilation, cancellationToken: cancellation.Token);

        action.Should().Throw<OperationCanceledException>();
    }

    [TestMethod]
    public async Task Render_CustomTemplateWithMissingInclude_ThrowsActionableBicepDocumentationException()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("param foo string = 'bar'");

        compiler.FileSet.AddFile("readme.scriban", "{{ include \"_missing.md\" }}");

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation);

        var options = new BicepDocumentationGenerationOptions(
            TemplateFile: compiler.FileSet.GetUri("readme.scriban"),
            TemplateRoot: null,
            CustomValues: null);

        var act = () => generator.Render(model, options);

        act.Should().Throw<BicepDocumentationException>().WithMessage("*_missing.md*");
    }

    [TestMethod]
    public async Task Render_InvalidCustomTemplate_ThrowsActionableBicepDocumentationException()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("param foo string = 'bar'");

        compiler.FileSet.AddFile("readme.scriban", "{{ if module.name }}");

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation);

        var options = new BicepDocumentationGenerationOptions(
            TemplateFile: compiler.FileSet.GetUri("readme.scriban"),
            TemplateRoot: null,
            CustomValues: null);

        var act = () => generator.Render(model, options);

        act.Should().Throw<BicepDocumentationException>();
    }

    [TestMethod]
    public async Task Render_BuiltInTemplate_ProducesExpectedMarkdown()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile(
            ("main.bicep", ComprehensiveModule),
            ("modules/logging.bicep", LoggingModule),
            ("examples/default/main.bicep", "// Deploys the module with default settings.\nmodule example '../../main.bicep' = {\n  name: 'example'\n}\n"));

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var rendered = generator.Generate(result.Compilation);

        var expected = GetEmbeddedFixture("ExpectedMarkdown.md");
        rendered.Should().EqualWithLineByLineDiff(expected);
    }

    [TestMethod]
    public void LoadTemplateSource_MissingResource_Throws()
    {
        FluentActions.Invoking(() =>
                BicepDocumentationGenerator.LoadTemplateSource(
                    Assembly.GetExecutingAssembly(),
                    "missing.template"))
            .Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public async Task Render_MissingCustomTemplateFile_ThrowsBicepDocumentationException()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("param foo string = 'bar'");

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation);

        var options = new BicepDocumentationGenerationOptions(
            TemplateFile: compiler.FileSet.GetUri("missing.scriban"),
            TemplateRoot: null,
            CustomValues: null);

        var act = () => generator.Render(model, options);

        act.Should().Throw<BicepDocumentationException>().WithMessage("*missing.scriban*does not exist*");
    }

    [TestMethod]
    public async Task Render_CustomTemplateFileReadError_ThrowsBicepDocumentationException()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("param foo string = 'bar'");

        compiler.FileSet.AddFile("readme.scriban", "# {{ module.name }}");

        var innerGenerator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = innerGenerator.BuildModel(result.Compilation);

        var throwingGenerator = new BicepDocumentationGenerator(new ThrowingFileExplorer(new IOException("disk error")));
        var options = new BicepDocumentationGenerationOptions(
            TemplateFile: compiler.FileSet.GetUri("readme.scriban"),
            TemplateRoot: null,
            CustomValues: null);

        var act = () => throwingGenerator.Render(model, options);

        act.Should().Throw<BicepDocumentationException>().WithMessage("*disk error*");
    }

    [TestMethod]
    public async Task Render_CustomTemplateIncludeReadError_ThrowsActionableBicepDocumentationException()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("param foo string = 'bar'");

        compiler.FileSet.AddFile("readme.scriban", "{{ include \"_header.md\" }}");
        compiler.FileSet.AddFile("_header.md", "> Header content.");

        var innerGenerator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = innerGenerator.BuildModel(result.Compilation);

        // The template file itself reads fine, but the included file does not, exercising the template
        // loader's own narrow I/O catch rather than the top-level template-file read catch.
        var throwingExplorer = new SelectivelyThrowingFileExplorer(compiler.FileSet.FileExplorer, compiler.FileSet.GetUri("_header.md"), new IOException("disk error"));
        var throwingGenerator = new BicepDocumentationGenerator(throwingExplorer);
        var options = new BicepDocumentationGenerationOptions(
            TemplateFile: compiler.FileSet.GetUri("readme.scriban"),
            TemplateRoot: null,
            CustomValues: null);

        var act = () => throwingGenerator.Render(model, options);

        act.Should().Throw<BicepDocumentationException>().WithMessage("*disk error*");
    }

    [TestMethod]
    public async Task Render_CustomTemplate_WithTemplateRootOverride_ResolvesIncludesRelativeToOverrideRoot()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("param foo string = 'bar'");

        compiler.FileSet.AddFile("readme.scriban", "{{ include \"_header.md\" }}\n# {{ module.name }}\n");
        compiler.FileSet.AddFile("overrideRoot/_header.md", "> Overridden header.");

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation);

        var options = new BicepDocumentationGenerationOptions(
            TemplateFile: compiler.FileSet.GetUri("readme.scriban"),
            TemplateRoot: compiler.FileSet.GetUri("overrideRoot"),
            CustomValues: null);

        var rendered = generator.Render(model, options);

        rendered.Should().Be("> Overridden header.\n# to\n");
    }

    [TestMethod]
    public async Task Render_InvalidModulePathWithoutTemplateRootOverride_ThrowsActionableBicepDocumentationException()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("param foo string = 'bar'");

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation) with { Path = "https://example.com/not-a-file-path/main.bicep" };

        var act = () => generator.Render(model);

        act.Should().Throw<BicepDocumentationException>().WithMessage("*Unable to resolve an include root*");
    }

    [TestMethod]
    public async Task Render_InvalidModulePathWithTemplateRootOverride_SucceedsUsingOverride()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("param foo string = 'bar'");

        compiler.FileSet.AddFile("overrideRoot/_header.md", "> Header.");

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation) with { Path = "https://example.com/not-a-file-path/main.bicep" };

        compiler.FileSet.AddFile("readme.scriban", "{{ include \"_header.md\" }}");
        var options = new BicepDocumentationGenerationOptions(
            TemplateFile: compiler.FileSet.GetUri("readme.scriban"),
            TemplateRoot: compiler.FileSet.FileExplorer.GetDirectory(compiler.FileSet.GetUri("overrideRoot")).Uri,
            CustomValues: null);

        var rendered = generator.Render(model, options);

        rendered.Should().Be("> Header.\n");
    }

    [TestMethod]
    public void Render_TemplateWithTrailingBlankLinesAndCrlf_NormalizesLineEndingsAndTrailingWhitespace()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        compiler.FileSet.AddFile("readme.scriban", "Line one\r\nLine two\r\n\r\n\r\n");

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = MinimalModel();

        var options = new BicepDocumentationGenerationOptions(
            TemplateFile: compiler.FileSet.GetUri("readme.scriban"),
            TemplateRoot: compiler.FileSet.FileExplorer.GetDirectory(compiler.FileSet.GetUri("")).Uri,
            CustomValues: null);

        var rendered = generator.Render(model, options);

        rendered.Should().Be("Line one\nLine two\n");
    }

    [TestMethod]
    public async Task Render_EmptyModule_ShowsFallbackTextAndOmitsOptionalNavigationLinks()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("// This module intentionally declares nothing.\n");

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation);

        model.ResourceTypes.Should().BeEmpty();
        model.Parameters.Should().BeEmpty();
        model.Outputs.Should().BeEmpty();
        model.ExportedFunctions.Should().BeEmpty();
        model.References.Should().BeEmpty();
        model.UsageExamples.Should().BeEmpty();

        var rendered = generator.Render(model);

        rendered.Should().Contain("_No resources are declared in this module._");
        rendered.Should().Contain("_No parameters are declared in this module._");
        rendered.Should().Contain("_No outputs are declared in this module._");
        rendered.Should().NotContain("Usage Examples");
        rendered.Should().NotContain("Exported Functions");
        rendered.Should().NotContain("Cross-referenced Modules");
        rendered.Should().NotContain("Data Collection");
    }

    [TestMethod]
    public async Task BuildModel_TargetScopeTenant_ProjectsTenantScopeName()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("targetScope = 'tenant'\n");

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation);

        model.TargetScope.Should().Be("tenant");
    }

    [TestMethod]
    public async Task BuildModel_TargetScopeManagementGroup_ProjectsManagementGroupScopeName()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("targetScope = 'managementGroup'\n");

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation);

        model.TargetScope.Should().Be("managementGroup");
    }

    [TestMethod]
    public async Task BuildModel_TargetScopeSubscription_ProjectsSubscriptionScopeName()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("targetScope = 'subscription'\n");

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation);

        model.TargetScope.Should().Be("subscription");
    }

    [TestMethod]
    public async Task BuildModel_TargetScopeLocal_ProjectsLocalScopeName()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation()
            .WithFeatureOverrides<FeatureProviderOverrides, OverriddenFeatureProviderFactory>(new FeatureProviderOverrides(LocalDeployEnabled: true));
        var result = await compiler.Compile("targetScope = 'local'\n\nparam foo string = 'bar'\n");

        result.Diagnostics.Should().NotContain(d => d.Level == Bicep.Core.Diagnostics.DiagnosticLevel.Error);

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation);

        model.TargetScope.Should().Be("local");
    }

    [TestMethod]
    public async Task BuildModel_MetadataNameWithNonStringValue_FallsBackToDirectoryName()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("metadata name = 123\nparam foo string = 'bar'\n");

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation);

        model.Name.Should().Be("to");
    }

    [TestMethod]
    public async Task BuildModel_MetadataNameWithDifferentCase_FallsBackToDirectoryName()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("metadata Name = 'Not the well-known name'\nparam foo string = 'bar'\n");

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation);

        model.Name.Should().Be("to");
    }

    [TestMethod]
    public async Task BuildModel_ModuleReferenceWithDescription_ProjectsDescription()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile(
            ("main.bicep", "module logging 'modules/logging.bicep' = {\n  name: 'loggingDeployment'\n}\n"),
            ("modules/logging.bicep", "metadata description = 'Deploys centralized logging.'\noutput workspaceId string = 'workspace-id'\n"));

        result.Diagnostics.Should().NotContain(d => d.Level == Bicep.Core.Diagnostics.DiagnosticLevel.Error);

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation);

        var reference = model.References.Single();
        reference.SymbolicName.Should().Be("logging");
        reference.Description.Should().Be("Deploys centralized logging.");
    }

    [TestMethod]
    public async Task BuildModel_CustomValues_AreDeterministicallyOrderedByOrdinalKeyWithExactSpelling()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("param foo string = 'bar'");

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation, new Dictionary<string, string>
        {
            ["zeta"] = "last",
            ["Alpha"] = "first-capitalized",
            ["beta"] = "middle",
        });

        // Ordinal ordering: uppercase 'A' (65) sorts before lowercase 'b' (98) and 'z' (122).
        model.Custom.Keys.Should().Equal("Alpha", "beta", "zeta");
        model.Custom["Alpha"].Should().Be("first-capitalized");
    }

    [TestMethod]
    public async Task BuildModel_NoCustomValues_ProjectsEmptyCustomDictionary()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("param foo string = 'bar'");

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation);

        model.Custom.Should().BeEmpty();
    }

    [TestMethod]
    public void Generate_LegacyInterfaceImplementationUsesCompatibilityModelBuilder()
    {
        IBicepDocumentationGenerator generator = new LegacyDocumentationGenerator();
        var options = new BicepDocumentationGenerationOptions(
            TemplateFile: null,
            TemplateRoot: null,
            CustomValues: new Dictionary<string, string> { ["value"] = "configured" });

        var rendered = generator.Generate(null!, options);

        rendered.Should().Be("configured");
    }

    private static BicepDocumentationModel MinimalModel() => new(
        Name: "minimal",
        Description: null,
        Path: "C:\\path\\to\\main.bicep",
        TargetScope: "resourceGroup",
        Custom: System.Collections.Immutable.ImmutableSortedDictionary<string, string>.Empty,
        ResourceTypes: [],
        Parameters: [],
        Outputs: [],
        ExportedFunctions: [],
        References: [],
        UsageExamples: []);

    private static string GetEmbeddedFixture(string name)
    {
        var resourceName = $"Files/Documentation/{name}";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Could not find embedded fixture '{resourceName}'.");
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }

    private sealed class LegacyDocumentationGenerator : IBicepDocumentationGenerator
    {
        public BicepDocumentationModel BuildModel(
            Bicep.Core.Semantics.Compilation compilation,
            IReadOnlyDictionary<string, string>? customValues = null,
            CancellationToken cancellationToken = default) =>
            MinimalModel() with
            {
                Custom = customValues is null
                    ? ImmutableSortedDictionary<string, string>.Empty
                    : customValues.ToImmutableSortedDictionary(StringComparer.Ordinal),
            };

        public string Render(
            BicepDocumentationModel model,
            BicepDocumentationGenerationOptions? options = null,
            CancellationToken cancellationToken = default) =>
            model.Custom["value"];
    }
}
