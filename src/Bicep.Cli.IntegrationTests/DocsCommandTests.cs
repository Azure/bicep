// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using System.Text.Json;
using Bicep.Cli.Arguments;
using Bicep.Cli.Services;
using Bicep.Core.Documentation;
using Bicep.Core.Exceptions;
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
    private const string FixturePrefix = "Files/DocsCommandTests/Comprehensive/";

    private static InvocationSettings DocsEnabledSettings() => InvocationSettings.Default;

    private string SaveComprehensiveFixture() =>
        FileHelper.SaveEmbeddedResourcesWithPathPrefix(TestContext, Assembly.GetExecutingAssembly(), FixturePrefix);

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

        var outputResult = await Bicep("docs", "output", mainFile);

        outputResult.ExitCode.Should().Be(0);
        outputResult.Stdout.Should().Be(File.ReadAllText(Path.Combine(moduleRoot, "README.expected.md")));
        File.Exists(outputFile).Should().BeFalse();
    }

    [TestMethod]
    public async Task Output_CustomTemplate_SupportsIncludesTemplateRootAndCustomValues()
    {
        var moduleRoot = SaveComprehensiveFixture();
        var mainFile = Path.Combine(moduleRoot, "main.bicep");
        var templateFile = Path.Combine(moduleRoot, "templates", "custom.scriban");

        var result = await Bicep(
            "docs",
            "output",
            mainFile,
            "--template-file",
            templateFile,
            "--template-root",
            moduleRoot,
            "--custom-template-value",
            "owner=Platform Team",
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

        var trailingSeparatorResult = await Bicep(
            "docs",
            "output",
            mainFile,
            "--template-file",
            templateFile,
            "--template-root",
            moduleRoot + Path.DirectorySeparatorChar,
            "--custom-template-value",
            "owner=Platform Team");
        trailingSeparatorResult.ExitCode.Should().Be(0);
    }

    [TestMethod]
    public async Task Output_CustomTemplateValues_MergeFilesAndIndividualValuesInCommandLineOrder()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("main.bicep", "metadata name = 'Custom values'"),
                new("template.scriban", "{{ custom.owner }}|{{ custom.fromFirst }}|{{ custom.fromSecond }}|{{ custom.inlineOnly }}"),
                new("first.json", """{ "owner": "first file", "fromFirst": "one" }"""),
                new("second.json", """{ "owner": "second file", "fromSecond": "two" }"""),
            ]);
        var mainFile = Path.Combine(root, "main.bicep");
        var templateFile = Path.Combine(root, "template.scriban");
        var firstFile = Path.Combine(root, "first.json");
        var secondFile = Path.Combine(root, "second.json");

        var fileLast = await Bicep(
            "docs",
            "output",
            mainFile,
            "--template-file",
            templateFile,
            "--custom-template-value-file-path",
            firstFile,
            "--custom-template-value",
            "owner=first inline",
            "--custom-template-value",
            "owner=second inline",
            "--custom-template-value-file-path",
            secondFile,
            "--custom-template-value",
            "inlineOnly=three");
        var inlineLast = await Bicep(
            "docs",
            "output",
            mainFile,
            "--template-file",
            templateFile,
            "--custom-template-value-file-path",
            secondFile,
            "--custom-template-value",
            "owner=last inline");

        fileLast.ExitCode.Should().Be(0);
        fileLast.Stdout.Should().Be("second file|one|two|three\n");
        inlineLast.ExitCode.Should().Be(0);
        inlineLast.Stdout.Should().Be("last inline||two|\n");
    }

    [DataTestMethod]
    [DataRow("[]", "must contain a JSON object")]
    [DataRow("""{ "count": 1 }""", "value for \"count\" must be a string")]
    [DataRow("""{ "value": null }""", "value for \"value\" must be a string")]
    [DataRow("""{ "": "value" }""", "contains an empty key")]
    [DataRow("""{ "value": "first", "value": "second" }""", "contains the duplicate key \"value\"")]
    [DataRow("{ invalid", "is not valid JSON")]
    public async Task Output_CustomTemplateValueFile_RejectsInvalidContent(string contents, string expectedError)
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("main.bicep", "metadata name = 'Invalid values'"),
                new("values.json", contents),
            ]);

        var result = await Bicep(
            "docs",
            "output",
            Path.Combine(root, "main.bicep"),
            "--custom-template-value-file-path",
            Path.Combine(root, "values.json"));

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().Contain(expectedError);
    }

    [TestMethod]
    public async Task Output_CustomTemplateValueFile_RejectsMissingFile()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [new("main.bicep", "metadata name = 'Missing values'")]);

        var result = await Bicep(
            "docs",
            "output",
            Path.Combine(root, "main.bicep"),
            "--custom-template-value-file-path",
            Path.Combine(root, "missing.json"));

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().Contain("does not exist");
    }

    [TestMethod]
    public async Task Output_CustomTemplateValueFile_RejectsEmptyPath()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [new("main.bicep", "metadata name = 'Empty path'")]);

        var result = await Bicep(
            "docs",
            "output",
            Path.Combine(root, "main.bicep"),
            "--custom-template-value-file-path",
            "");

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().Contain("expects a nonempty path");
    }

    [TestMethod]
    public async Task Output_CustomTemplateValueFile_WrapsInvalidPath()
    {
        var fileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
        var path = new Mock<IPath>(MockBehavior.Strict);
        fileSystem.SetupGet(system => system.Path).Returns(path.Object);
        path.Setup(systemPath => systemPath.GetFullPath("invalid")).Throws(new ArgumentException("invalid path"));

        var result = await Bicep(
            DocsEnabledSettings(),
            services => services.AddSingleton(fileSystem.Object),
            TestContext.CancellationTokenSource.Token,
            "docs",
            "output",
            "main.bicep",
            "--custom-template-value-file-path",
            "invalid");

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().ContainAll("is invalid", "invalid path");
    }

    [TestMethod]
    public async Task Output_CustomTemplateValueFile_WrapsReadFailures()
    {
        var fileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
        var path = new Mock<IPath>(MockBehavior.Strict);
        var file = new Mock<IFile>(MockBehavior.Strict);
        fileSystem.SetupGet(system => system.Path).Returns(path.Object);
        fileSystem.SetupGet(system => system.File).Returns(file.Object);
        path.Setup(systemPath => systemPath.GetFullPath("values.json")).Returns("C:\\values.json");
        file.Setup(systemFile => systemFile.Exists("C:\\values.json")).Returns(true);
        file.Setup(systemFile => systemFile.ReadAllText("C:\\values.json")).Throws(new IOException("read failed"));
        var result = await Bicep(
            DocsEnabledSettings(),
            services => services.AddSingleton(fileSystem.Object),
            TestContext.CancellationTokenSource.Token,
            "docs",
            "output",
            "main.bicep",
            "--custom-template-value-file-path",
            "values.json");

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().ContainAll("Unable to read", "read failed");
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
            ]);

        var result = await Bicep(
            DocsEnabledSettings(),
            "docs",
            "generate",
            Path.Combine(root, "main.bicep"),
            "--template-file",
            Path.Combine(root, "invalid.scriban"));

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
            ]);

        var result = await Bicep(
            DocsEnabledSettings(),
            "docs",
            "generate",
            Path.Combine(root, "main.bicep"),
            "--template-file",
            Path.Combine(root, "invalid.scriban"),
            "--diagnostics-format",
            "sarif");

        result.ExitCode.Should().Be(1);
        using var document = JsonDocument.Parse(result.Stderr);
        document.RootElement.ToString().Should().ContainAll("DOCS003", "Failed to parse");
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
    public async Task Generate_WriteFailureWithSarif_EmitsOneValidLog()
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
        using var document = JsonDocument.Parse(result.Stderr);
        document.RootElement.ToString().Should().ContainAll("DOCS002", "write failed");
    }

    [TestMethod]
    public async Task Generate_OutputFile_ChangesOnlyTheDestinationName()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [new("main.bicep", "metadata name = 'Example'\nparam value string = 'ok'")]);

        var defaultResult = await Bicep(DocsEnabledSettings(), "docs", "output", Path.Combine(root, "main.bicep"));
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
            "output",
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
            "output",
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
            ]);

        var result = await Bicep(
            DocsEnabledSettings(),
            "docs",
            "output",
            Path.Combine(root, "main.bicep"),
            "--template-file",
            Path.Combine(root, "invalid.scriban"),
            "--diagnostics-format",
            "sarif");

        result.ExitCode.Should().Be(1);
        result.Stdout.Should().BeEmpty();
        using var document = JsonDocument.Parse(result.Stderr);
        document.RootElement.ToString().Should().ContainAll("DOCS003", "Failed to parse");
    }

    [TestMethod]
    public async Task Generate_PatternSarifDiagnostics_EmitsOneValidLog()
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
        using var document = JsonDocument.Parse(result.Stderr);
        document.RootElement.GetProperty("runs").GetArrayLength().Should().Be(1);
        result.Stderr.Should().Contain("invalidType");
        result.Stderr.Should().NotContain("WARNING:");
        File.Exists(Path.Combine(root, "valid", "README.md")).Should().BeTrue();
        File.Exists(Path.Combine(root, "invalid", "README.md")).Should().BeFalse();
    }

    [TestMethod]
    public async Task CommandRunner_ObservesCancellationBeforeCompilation()
    {
        var runner = new DocsCommandRunner(null!, null!, null!, null!, null!);
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        await FluentActions.Invoking(() => runner.RenderAsync(
                IOUri.FromFilePath(Path.GetFullPath("main.bicep")),
                null,
                null,
                new Dictionary<string, string>(),
                noRestore: false,
                diagnosticsFormat: null,
                cancellationToken: cancellation.Token))
            .Should().ThrowAsync<OperationCanceledException>();
    }

    [TestMethod]
    public void DocsRenderFailure_BehavesAsAValueRecord()
    {
        var sourceUri = IOUri.FromFilePath(Path.GetFullPath("main.bicep"));
        var result = new DocsRenderResult.Failed(sourceUri);
        var clone = result with { };

        result.Compilation.Should().BeNull();
        result.Should().Be(clone);
    }

    [TestMethod]
    public async Task Generate_CompilationSetupFailure_UsesTheSelectedDiagnosticsFormat()
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
            "output",
            "/main.bicep",
            "--diagnostics-format",
            "sarif");

        defaultResult.ExitCode.Should().Be(1);
        defaultResult.Stderr.Should().Contain("compilation setup failed");
        sarifResult.ExitCode.Should().Be(1);
        using var document = JsonDocument.Parse(sarifResult.Stderr);
        document.RootElement.ToString().Should().ContainAll("DOCS001", "compilation setup failed");
        outputSarifResult.ExitCode.Should().Be(1);
        outputSarifResult.Stdout.Should().BeEmpty();
        using var outputDocument = JsonDocument.Parse(outputSarifResult.Stderr);
        outputDocument.RootElement.ToString().Should().ContainAll("DOCS001", "compilation setup failed");
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
            "output",
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
            "output",
            "invalid");

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().Contain("invalid path");
    }

    [TestMethod]
    public async Task Output_WrapsTemplateFilePathExceptions()
    {
        var fileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
        var path = new Mock<IPath>(MockBehavior.Strict);
        fileSystem.SetupGet(system => system.Path).Returns(path.Object);
        path.Setup(systemPath => systemPath.GetFullPath("main.bicep")).Returns("C:\\main.bicep");
        path.Setup(systemPath => systemPath.GetFullPath("invalid")).Throws(new IOException("invalid template path"));

        var result = await Bicep(
            DocsEnabledSettings(),
            services => services.AddSingleton(fileSystem.Object),
            TestContext.CancellationTokenSource.Token,
            "docs",
            "output",
            "main.bicep",
            "--template-file",
            "invalid");

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().Contain("invalid template path");
    }

    [DataTestMethod]
    [DataRow(["docs", "generate", "main.bicep", "--custom-template-value"])]
    [DataRow(["docs", "generate", "main.bicep", "--custom-template-value", "invalid"])]
    [DataRow(["docs", "generate", "main.bicep", "--custom-template-value-file-path"])]
    [DataRow(["docs", "generate", "main.bicep", "--pattern", "**/main.bicep", "--outfile", "README.md"])]
    [DataRow(["docs", "output", "main.bicep", "--pattern", "**/main.bicep"])]
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
    public async Task Output_RejectsMissingTemplateRoot()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [new("main.bicep", "param value string = 'ok'")]);

        var result = await Bicep(
            DocsEnabledSettings(),
            "docs",
            "output",
            Path.Combine(root, "main.bicep"),
            "--template-root",
            Path.Combine(root, "missing"));

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
            null,
            null,
            System.Collections.Immutable.ImmutableSortedDictionary<string, string>.Empty,
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

        Path.GetFileName(defaultOutput.Single().OutputUri.GetFilePath()).Should().Be("README.md");
        explicitOutput.Single().OutputUri.GetFilePath().Should().Be(fileSystem.Path.GetFullPath("/docs/custom.md"));
        fixedOutDir.Single().OutputUri.GetFilePath().Should().Be(fileSystem.Path.GetFullPath("/docs/README.md"));
        Path.GetFileName(extensionOutput.Single().OutputUri.GetFilePath()).Should().Be("main.md");
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
            null,
            null,
            System.Collections.Immutable.ImmutableSortedDictionary<string, string>.Empty,
            null,
            null,
            false,
            null);

        var output = resolver.ResolveFilePatternInputOutputArguments(arguments);

        output.Should().ContainSingle();
        output.Single().OutputUri.GetFilePath().Should().Be(Path.Combine(root, "nested", "main.md"));
    }

    [TestMethod]
    public async Task OutputWriter_AtomicWrite_ReportsUnauthorizedAccess()
    {
        var fileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
        var file = new Mock<IFile>(MockBehavior.Strict);
        var path = new Mock<IPath>(MockBehavior.Strict);
        var fileExplorer = new Mock<IFileExplorer>(MockBehavior.Strict);
        var temporaryFile = new Mock<IFileHandle>(MockBehavior.Strict);
        fileSystem.SetupGet(system => system.File).Returns(file.Object);
        fileSystem.SetupGet(system => system.Path).Returns(path.Object);
        path.Setup(systemPath => systemPath.Combine(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string left, string right) => Path.Combine(left, right));
        path.Setup(systemPath => systemPath.GetFileName(It.IsAny<string>()))
            .Returns((string value) => Path.GetFileName(value));
        fileExplorer
            .Setup(explorer => explorer.GetFile(It.IsAny<IOUri>()))
            .Returns(temporaryFile.Object);
        temporaryFile
            .Setup(handle => handle.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("denied"));
        file.Setup(systemFile => systemFile.Delete(It.IsAny<string>()));
        var writer = new OutputWriter(
            new(
                new(new StringReader(string.Empty), false),
                new(new StringWriter(), false),
                new(new StringWriter(), false)),
            fileSystem.Object,
            fileExplorer.Object);
        var outputUri = IOUri.FromFilePath(Path.GetFullPath("README.md"));

        await FluentActions.Invoking(() => writer.WriteToFileAtomicallyAsync(outputUri, "contents"))
            .Should().ThrowAsync<BicepException>()
            .WithMessage("denied");
    }

    [TestMethod]
    public async Task OutputWriter_AtomicWrite_CleansUpTemporaryFileWhenMoveFails()
    {
        var root = FileHelper.GetUniqueTestOutputPath(TestContext);
        var outputDirectory = Path.Combine(root, "output");
        Directory.CreateDirectory(outputDirectory);
        var fileSystem = new System.IO.Abstractions.FileSystem();
        var writer = new OutputWriter(
            new(
                new(new StringReader(string.Empty), false),
                new(new StringWriter(), false),
                new(new StringWriter(), false)),
            fileSystem,
            new FileSystemFileExplorer(fileSystem));

        await FluentActions.Invoking(() => writer.WriteToFileAtomicallyAsync(
                IOUri.FromFilePath(outputDirectory),
                "contents"))
            .Should().ThrowAsync<BicepException>();

        Directory.EnumerateFiles(root, "*.tmp").Should().BeEmpty();
    }

    [TestMethod]
    public async Task OutputWriter_AtomicWrite_PreservesPrimaryFailureWhenCleanupFails()
    {
        var fileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
        var file = new Mock<IFile>(MockBehavior.Strict);
        var path = new Mock<IPath>(MockBehavior.Strict);
        var fileExplorer = new Mock<IFileExplorer>(MockBehavior.Strict);
        var temporaryFile = new Mock<IFileHandle>(MockBehavior.Strict);
        fileSystem.SetupGet(system => system.File).Returns(file.Object);
        fileSystem.SetupGet(system => system.Path).Returns(path.Object);
        path.Setup(systemPath => systemPath.Combine(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string left, string right) => Path.Combine(left, right));
        path.Setup(systemPath => systemPath.GetFileName(It.IsAny<string>()))
            .Returns((string value) => Path.GetFileName(value));
        fileExplorer.Setup(explorer => explorer.GetFile(It.IsAny<IOUri>())).Returns(temporaryFile.Object);
        temporaryFile
            .Setup(handle => handle.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new IOException("write failed"));
        file.Setup(systemFile => systemFile.Delete(It.IsAny<string>())).Throws(new IOException("cleanup failed"));
        var writer = new OutputWriter(
            new(
                new(new StringReader(string.Empty), false),
                new(new StringWriter(), false),
                new(new StringWriter(), false)),
            fileSystem.Object,
            fileExplorer.Object);

        var exception = await FluentActions.Invoking(() => writer.WriteToFileAtomicallyAsync(
                IOUri.FromFilePath(Path.GetFullPath("README.md")),
                "contents"))
            .Should().ThrowAsync<BicepException>()
            .WithMessage("write failed");

        exception.Which.InnerException.Should().BeOfType<IOException>()
            .Which.Message.Should().Be("write failed");
    }

    [TestMethod]
    public async Task OutputWriter_AtomicWrite_IgnoresCleanupFailureAfterSuccessfulWrite()
    {
        var fileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
        var file = new Mock<IFile>(MockBehavior.Strict);
        var path = new Mock<IPath>(MockBehavior.Strict);
        var fileExplorer = new Mock<IFileExplorer>(MockBehavior.Strict);
        var temporaryFile = new Mock<IFileHandle>(MockBehavior.Strict);
        fileSystem.SetupGet(system => system.File).Returns(file.Object);
        fileSystem.SetupGet(system => system.Path).Returns(path.Object);
        path.Setup(systemPath => systemPath.Combine(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string left, string right) => Path.Combine(left, right));
        path.Setup(systemPath => systemPath.GetFileName(It.IsAny<string>()))
            .Returns((string value) => Path.GetFileName(value));
        fileExplorer.Setup(explorer => explorer.GetFile(It.IsAny<IOUri>())).Returns(temporaryFile.Object);
        temporaryFile
            .Setup(handle => handle.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        file.Setup(systemFile => systemFile.Move(It.IsAny<string>(), It.IsAny<string>(), true));
        file.Setup(systemFile => systemFile.Delete(It.IsAny<string>())).Throws(new IOException("cleanup failed"));
        var writer = new OutputWriter(
            new(
                new(new StringReader(string.Empty), false),
                new(new StringWriter(), false),
                new(new StringWriter(), false)),
            fileSystem.Object,
            fileExplorer.Object);

        await writer.WriteToFileAtomicallyAsync(
            IOUri.FromFilePath(Path.GetFullPath("README.md")),
            "contents");
    }

    [TestMethod]
    public async Task OutputWriter_AtomicWrite_CleansUpWithoutMaskingCancellation()
    {
        var fileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
        var file = new Mock<IFile>(MockBehavior.Strict);
        var path = new Mock<IPath>(MockBehavior.Strict);
        var fileExplorer = new Mock<IFileExplorer>(MockBehavior.Strict);
        var temporaryFile = new Mock<IFileHandle>(MockBehavior.Strict);
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();
        var canceled = new OperationCanceledException(cancellation.Token);
        fileSystem.SetupGet(system => system.File).Returns(file.Object);
        fileSystem.SetupGet(system => system.Path).Returns(path.Object);
        path.Setup(systemPath => systemPath.Combine(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string left, string right) => Path.Combine(left, right));
        path.Setup(systemPath => systemPath.GetFileName(It.IsAny<string>()))
            .Returns((string value) => Path.GetFileName(value));
        fileExplorer.Setup(explorer => explorer.GetFile(It.IsAny<IOUri>())).Returns(temporaryFile.Object);
        temporaryFile
            .Setup(handle => handle.WriteAllTextAsync(It.IsAny<string>(), cancellation.Token))
            .ThrowsAsync(canceled);
        file.Setup(systemFile => systemFile.Delete(It.IsAny<string>())).Throws(new IOException("cleanup failed"));
        var writer = new OutputWriter(
            new(
                new(new StringReader(string.Empty), false),
                new(new StringWriter(), false),
                new(new StringWriter(), false)),
            fileSystem.Object,
            fileExplorer.Object);

        await FluentActions.Invoking(() => writer.WriteToFileAtomicallyAsync(
                IOUri.FromFilePath(Path.GetFullPath("README.md")),
                "contents",
                cancellation.Token))
            .Should().ThrowAsync<OperationCanceledException>();
    }
}
