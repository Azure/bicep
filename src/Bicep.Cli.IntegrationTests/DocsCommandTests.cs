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
            "--set",
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
    public async Task Generate_CompilationFailure_DoesNotOverwriteExistingOutput()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("main.bicep", "param value invalidType"),
                new("README.md", "preserve me"),
            ]);

        var result = await Bicep(DocsEnabledSettings(), "docs", "generate", root);

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
            root,
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
            root,
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
        var writer = new Mock<IDocsFileWriter>(MockBehavior.Strict);
        writer
            .Setup(fileWriter => fileWriter.WriteAsync(
                It.IsAny<IOUri>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BicepException("write failed"));

        var result = await Bicep(
            DocsEnabledSettings(),
            services => services.AddSingleton(writer.Object),
            TestContext.CancellationTokenSource.Token,
            "docs",
            "generate",
            root);

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
        var writer = new Mock<IDocsFileWriter>(MockBehavior.Strict);
        writer
            .Setup(fileWriter => fileWriter.WriteAsync(
                It.IsAny<IOUri>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BicepException("write failed"));

        var result = await Bicep(
            DocsEnabledSettings(),
            services => services.AddSingleton(writer.Object),
            TestContext.CancellationTokenSource.Token,
            "docs",
            "generate",
            root,
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

        var defaultResult = await Bicep(DocsEnabledSettings(), "docs", "output", root);
        var generateResult = await Bicep(
            DocsEnabledSettings(),
            "docs",
            "generate",
            root,
            "--output-file",
            "MODULE.md");

        generateResult.ExitCode.Should().Be(0);
        File.ReadAllText(Path.Combine(root, "MODULE.md")).Should().Be(defaultResult.Stdout);
        File.Exists(Path.Combine(root, "README.md")).Should().BeFalse();
    }

    [TestMethod]
    public async Task Generate_OutputFileCannotOverwriteAnUnselectedBicepDependency()
    {
        const string childSource = "metadata name = 'Child'";
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("main.bicep", "module child 'child.bicep' = { name: 'child' }"),
                new("child.bicep", childSource),
            ]);

        var result = await Bicep(
            DocsEnabledSettings(),
            "docs",
            "generate",
            root,
            "--output-file",
            "child.bicep");

        result.ExitCode.Should().Be(1);
        File.ReadAllText(Path.Combine(root, "child.bicep")).Should().Be(childSource);
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
            DocsEnabledSettings(),
            "docs",
            "generate",
            "--pattern",
            Path.Combine(root, "*.bicep"));

        result.ExitCode.Should().Be(1);
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
            root,
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
            root,
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

        defaultResult.ExitCode.Should().Be(1);
        defaultResult.Stderr.Should().Contain("compilation setup failed");
        sarifResult.ExitCode.Should().Be(1);
        using var document = JsonDocument.Parse(sarifResult.Stderr);
        document.RootElement.ToString().Should().ContainAll("DOCS001", "compilation setup failed");
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
    [DataRow(["docs", "generate", "main.bicep", "--set"])]
    [DataRow(["docs", "generate", "main.bicep", "--set", "invalid"])]
    [DataRow(["docs", "generate", "main.bicep", "--set", "key=one", "--set", "key=two"])]
    [DataRow(["docs", "generate", "main.bicep", "--output-file", "nested/README.md"])]
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
            root,
            "--template-root",
            Path.Combine(root, "missing"));

        result.ExitCode.Should().Be(1);
        result.Stderr.Should().Contain("does not exist");
    }

    [TestMethod]
    [DoNotParallelize]
    public async Task Generate_WithoutPath_UsesCurrentDirectory()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [new("main.bicep", "metadata name = 'Current Directory'")]);
        var previousDirectory = Directory.GetCurrentDirectory();

        try
        {
            Directory.SetCurrentDirectory(root);
            var result = await Bicep(DocsEnabledSettings(), "docs", "generate");

            result.ExitCode.Should().Be(0);
            File.Exists(Path.Combine(root, "README.md")).Should().BeTrue();
        }
        finally
        {
            Directory.SetCurrentDirectory(previousDirectory);
        }
    }

    [TestMethod]
    public void ModuleScanner_ValidatesResolutionEdgeCases()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["/module/main.bicep"] = "metadata name = 'Example'",
            ["/module/other.bicep"] = "metadata name = 'Other'",
            ["/template.scriban"] = "# Example",
        });
        var resolver = new InputOutputArgumentsResolver(fileSystem);
        var scanner = new DocsModuleScanner(fileSystem, resolver);

        scanner.ResolveModule("/module").GetFilePath().Should().Be(fileSystem.Path.GetFullPath("/module/main.bicep"));
        scanner.ResolveModule("/module/main.bicep").GetFilePath().Should().Be(fileSystem.Path.GetFullPath("/module/main.bicep"));
        scanner.ResolveOptionalFile(null).Should().BeNull();
        scanner.ResolveOptionalFile("/template.scriban").Should().NotBeNull();
        scanner.ResolveOptionalDirectory(null).Should().BeNull();
        scanner.ResolveOptionalDirectory("/module").Should().NotBeNull();
        scanner.ResolveOptionalDirectory("/module/").Should().NotBeNull();
        scanner.ValidateOutputFileName("README.md");

        var directArguments = new DocsGenerateArguments(
            "/module/main.bicep",
            null,
            null,
            null,
            [],
            "README.md",
            false,
            null);
        scanner.ResolveModules(directArguments).Should().ContainSingle();
        var mainUri = scanner.ResolveModule("/module/main.bicep");
        var otherUri = scanner.ResolveModule("/module/other.bicep");
        scanner.ResolveOutputFiles([mainUri], "MODULE.md")
            .Should().ContainSingle(pair => Path.GetFileName(pair.OutputUri.GetFilePath()) == "MODULE.md");
        FluentActions.Invoking(() => scanner.ResolveOutputFiles([mainUri], "main.bicep"))
            .Should().Throw<Exception>().WithMessage("*source file extension*");
        FluentActions.Invoking(() => scanner.ResolveOutputFiles([mainUri, otherUri], "README.md"))
            .Should().Throw<Exception>().WithMessage("*same output file*");

        var conflictingArguments = directArguments with { FilePattern = "**/main.bicep" };
        FluentActions.Invoking(() => scanner.ResolveModules(conflictingArguments))
            .Should().Throw<Exception>().WithMessage("*cannot both be specified*");

        FluentActions.Invoking(() => scanner.ResolveModule("/missing"))
            .Should().Throw<Exception>().WithMessage("*does not exist*");
        FluentActions.Invoking(() => scanner.ResolveOptionalDirectory("/missing"))
            .Should().Throw<Exception>().WithMessage("*does not exist*");
        FluentActions.Invoking(() => scanner.ValidateOutputFileName(""))
            .Should().Throw<Exception>();
        FluentActions.Invoking(() => scanner.ValidateOutputFileName("nested/README.md"))
            .Should().Throw<Exception>();
        FluentActions.Invoking(() => scanner.ValidateOutputFileName("invalid\0.md"))
            .Should().Throw<Exception>();
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow(".")]
    [DataRow("..")]
    [DataRow("nested/README.md")]
    [DataRow(@"nested\README.md")]
    [DataRow("README.md ")]
    [DataRow("README.md.")]
    [DataRow("CON")]
    [DataRow("PRN.txt")]
    [DataRow("AUX")]
    [DataRow("NUL.md")]
    [DataRow("COM1")]
    [DataRow("LPT9.txt")]
    [DataRow("CONIN$")]
    [DataRow("CONOUT$.md")]
    [DataRow("module.bicep")]
    [DataRow("module.bicepparam")]
    public void ModuleScanner_RejectsInvalidOutputFileNames(string outputFile)
    {
        var fileSystem = new MockFileSystem();
        var scanner = new DocsModuleScanner(fileSystem, new(fileSystem));

        FluentActions.Invoking(() => scanner.ValidateOutputFileName(outputFile))
            .Should().Throw<Exception>();
    }

    [TestMethod]
    public void ModuleScanner_ConvertsPathExceptionsToCommandLineErrors()
    {
        Exception[] exceptions =
        [
            new IOException("io"),
            new UnauthorizedAccessException("unauthorized"),
            new ArgumentException("argument"),
            new NotSupportedException("unsupported"),
        ];

        foreach (var exception in exceptions)
        {
            var fileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
            var path = new Mock<IPath>(MockBehavior.Strict);
            fileSystem.SetupGet(system => system.Path).Returns(path.Object);
            path.Setup(systemPath => systemPath.GetFullPath("invalid")).Throws(exception);
            var scanner = new DocsModuleScanner(fileSystem.Object, new(fileSystem.Object));

            FluentActions.Invoking(() => scanner.ResolveModule("invalid"))
                .Should().Throw<Exception>()
                .WithMessage(exception.Message);
        }
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
    public async Task OutputWriter_AtomicWrite_PreservesPrimaryFailureWhenCleanupAlsoFails()
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

        exception.Which.InnerException.Should().BeOfType<AggregateException>()
            .Which.InnerExceptions.Select(inner => inner.Message)
            .Should().Equal("write failed", "cleanup failed");
    }

    [TestMethod]
    public async Task OutputWriter_AtomicWrite_ReportsCleanupFailureAfterSuccessfulWrite()
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

        await FluentActions.Invoking(() => writer.WriteToFileAtomicallyAsync(
                IOUri.FromFilePath(Path.GetFullPath("README.md")),
                "contents"))
            .Should().ThrowAsync<BicepException>()
            .WithMessage("cleanup failed");
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

        var exception = await FluentActions.Invoking(() => writer.WriteToFileAtomicallyAsync(
                IOUri.FromFilePath(Path.GetFullPath("README.md")),
                "contents",
                cancellation.Token))
            .Should().ThrowAsync<OperationCanceledException>();

        exception.Which.Data["TemporaryFileCleanupError"].Should().Be("cleanup failed");
    }
}
