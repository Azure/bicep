// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.ComponentModel;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.IO.Abstraction;
using ModelContextProtocol.Server;

namespace Bicep.McpServer.Core;

[McpServerToolType]
public sealed class BicepCompilerTools(
    BicepCompiler compiler)
{
    private static readonly IOUri InMemoryBicepFileUri = new(IOUriScheme.Untitled, null, "/main.bicep");
    private static readonly IOUri InMemoryBicepParamFileUri = new(IOUriScheme.Untitled, null, "/main.bicepparam");

    public record DiagnosticDefinition(
        [Description("The .bicep or .bicepparam file URI")]
        Uri FileUri,
        [Description("The diagnostic code")]
        string Code,
        [Description("The diagnostic level")]
        string Level,
        [Description("The diagnostic message")]
        string Message,
        [Description("The documentation URI, if present")]
        Uri? DocumentationUri,
        [Description("The character position of the diagnostic in the file, as a zero-based index")]
        int Position,
        [Description("The character length of the diagnostic")]
        int Length);

    public record FormatResult(
        [Description("The formatted Bicep or Bicep parameters file content")]
        string Content);

    public record GetFileReferencesResult(
        [Description("The list of files being referenced by the specified Bicep or Bicep parameters file")]
        ImmutableArray<Uri> FileUris);

    public record BuildBicepResult(
        [Description("Whether the compilation succeeded without errors")]
        bool Success,
        [Description("The compiled ARM template JSON, or null if compilation failed")]
        string? Template,
        [Description("The list of diagnostics from compilation")]
        ImmutableArray<DiagnosticDefinition> Diagnostics);

    public record BuildBicepparamResult(
        [Description("Whether the compilation succeeded without errors")]
        bool Success,
        [Description("The compiled parameters JSON, or null if compilation failed")]
        string? Parameters,
        [Description("The compiled ARM template JSON, or null if not applicable or compilation failed")]
        string? Template,
        [Description("The list of diagnostics from compilation")]
        ImmutableArray<DiagnosticDefinition> Diagnostics);

    [McpServerTool(Title = "Build Bicep", Destructive = false, Idempotent = true, OpenWorld = true, ReadOnly = true, UseStructuredContent = true)]
    [Description("""
    Compiles a Bicep file (.bicep) to an ARM template JSON string and returns the result along with any diagnostics.

    Use this tool to:
    - Compile Bicep source code to ARM template JSON
    - Check for compilation errors before deployment
    - Obtain the ARM template output for inspection or deployment

    The compiled ARM template JSON is returned along with any compilation diagnostics (errors, warnings, and informational messages).
    Provide either an absolute file path or in-memory content. If compilation fails due to errors, the Template field will be null.
    """)]
    public async Task<BuildBicepResult> BuildBicep(
        [Description("The path to the .bicep file. Required if content is not provided.")] string? filePath = null,
        [Description("The in-memory .bicep file content. Required if filePath is not provided.")] string? content = null)
    {
        var compilation = await CreateCompilation(
            filePath,
            content,
            InMemoryBicepFileUri,
            fileUri => fileUri.HasBicepExtension(),
            "The specified file must have a .bicep extension.",
            (fileUri, fileContent) => compiler.SourceFileFactory.CreateBicepFile(fileUri, fileContent));
        var result = compilation.Emitter.Template();

        return new BuildBicepResult(result.Success, result.Template, GetDiagnostics(result.Diagnostics));
    }

    [McpServerTool(Title = "Build Bicep Parameters", Destructive = false, Idempotent = true, OpenWorld = true, ReadOnly = true, UseStructuredContent = true)]
    [Description("""
    Compiles a Bicep parameters file (.bicepparam) to a parameters JSON string and returns the result along with any diagnostics.

    Use this tool to:
    - Compile Bicep parameters source code to ARM parameters JSON
    - Check for compilation errors before deployment
    - Obtain the parameters JSON output for inspection or deployment

    The compiled parameters JSON and the associated ARM template JSON are returned along with any compilation diagnostics (errors, warnings, and informational messages).
    Provide either an absolute file path or in-memory content. If compilation fails due to errors, the Parameters field will be null.
    """)]
    public async Task<BuildBicepparamResult> BuildBicepparam(
        [Description("The path to the .bicepparam file. Required if content is not provided.")] string? filePath = null,
        [Description("The in-memory .bicepparam file content. Required if filePath is not provided.")] string? content = null)
    {
        var compilation = await CreateCompilation(
            filePath,
            content,
            InMemoryBicepParamFileUri,
            fileUri => fileUri.HasBicepParamExtension(),
            "The specified file must have a .bicepparam extension.",
            (fileUri, fileContent) => compiler.SourceFileFactory.CreateBicepParamFile(fileUri, fileContent));
        var result = compilation.Emitter.Parameters();

        return new BuildBicepparamResult(result.Success, result.Parameters, result.Template?.Template, GetDiagnostics(result.Diagnostics));
    }

    [McpServerTool(Title = "Format Bicep File", Destructive = false, Idempotent = true, OpenWorld = true, ReadOnly = true, UseStructuredContent = true)]
    [Description("""
    Formats a Bicep file (.bicep) or Bicep parameters file (.bicepparam) according to official Bicep formatting standards.
    
    Use this tool to:
    - Apply consistent code formatting (indentation, spacing, line breaks) to Bicep files
    - Clean up manually edited or generated Bicep code before saving
    - Ensure code follows team formatting conventions
    
    The formatter respects configuration settings from bicepconfig.json if present in the file's directory hierarchy, including:
    - Indentation style (spaces vs tabs) and size
    - Newline character (LF, CRLF, CR)
    - Whether to insert a final newline
    
    Provide either an absolute file path or in-memory content. Formatting preserves semantic meaning and only changes whitespace and layout. Files with syntax errors will still be formatted to the extent possible.
    """)]
    public async Task<FormatResult> FormatBicepFile(
        [Description("The path to the .bicep or .bicepparam file. Required if content is not provided.")] string? filePath = null,
        [Description("The in-memory .bicep or .bicepparam file content. Required if filePath is not provided.")] string? content = null)
    {
        var compilation = await CreateCompilation(
            filePath,
            content,
            InMemoryBicepFileUri,
            fileUri => fileUri.HasBicepExtension() || fileUri.HasBicepParamExtension(),
            "The specified file must have a .bicep or .bicepparam extension.",
            (fileUri, fileContent) => compiler.SourceFileFactory.CreateSourceFile(fileUri, fileContent));
        var sourceFile = compilation.GetEntrypointSemanticModel().SourceFile;

        var options = sourceFile.Configuration.Formatting.Data;
        var context = PrettyPrinterV2Context.Create(options, sourceFile.LexingErrorLookup, sourceFile.ParsingErrorLookup);

        return new FormatResult(PrettyPrinterV2.Print(sourceFile.ProgramSyntax, context));
    }

    [McpServerTool(Title = "Get File References", Destructive = false, Idempotent = true, OpenWorld = true, ReadOnly = true, UseStructuredContent = true)]
    [Description("""
    Analyzes a Bicep file (.bicep) or Bicep parameters file (.bicepparam) and returns a list of all files referenced by the entry file, including modules, parameter files, and any other dependencies.

    Use this tool to:
    - Identify all files that a Bicep or Bicep parameters file depends on
    - Perform dependency or impact analysis before making changes
    - Understand the structure and external links of a Bicep deployment

    The result is a list of absolute URIs for each referenced file. The file path must be absolute. Only direct references are included; transitive references require additional calls.
    """)]
    public async Task<GetFileReferencesResult> GetFileReferences(
        [Description("The path to the .bicep or .bicepparam file")] string filePath)
    {
        var fileUri = IOUri.FromFilePath(filePath);
        if (!fileUri.HasBicepExtension() && !fileUri.HasBicepParamExtension())
        {
            throw new ArgumentException("The specified file must have a .bicep or .bicepparam extension.", nameof(filePath));
        }

        var compilation = await compiler.CreateCompilation(fileUri);

        // fetch all diagnostics to ensure that all files are loaded
        var diagnostics = compilation.GetAllDiagnosticsByBicepFile();

        var fileUris = new HashSet<IOUri>();
        foreach (var otherModel in compilation.GetAllBicepModels())
        {
            fileUris.Add(otherModel.SourceFile.FileHandle.Uri);
            fileUris.UnionWith(otherModel.SourceFile.GetReferencedAuxiliaryFileUris());
            if (otherModel.Configuration.ConfigFileUri is { } configFileUri)
            {
                fileUris.Add(configFileUri);
            }
        }

        return new(
            [.. fileUris.Select(x => x.ToUri()).OrderBy(x => x.ToString())]);
    }

    private static ImmutableArray<DiagnosticDefinition> GetDiagnostics(ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>> diagnosticsByFile)
    {
        return [.. diagnosticsByFile.SelectMany(kvp => kvp.Value.Select(x => new DiagnosticDefinition(
            kvp.Key.FileHandle.Uri.ToUri(),
            x.Code,
            x.Level.ToString(),
            x.Message,
            x.Uri,
            x.Span.Position,
            x.Span.Length)))];
    }

    private async Task<Compilation> CreateCompilation(
        string? filePath,
        string? content,
        IOUri inMemoryFileUri,
        Func<IOUri, bool> hasExpectedExtension,
        string invalidExtensionError,
        Func<IOUri, string, ISourceFile> sourceFileFactory)
    {
        if (filePath is null && content is null)
        {
            throw new ArgumentException("Either filePath or content must be provided.");
        }

        var fileUri = filePath is null ? inMemoryFileUri : IOUri.FromFilePath(filePath);
        if (!hasExpectedExtension(fileUri))
        {
            throw new ArgumentException(invalidExtensionError, nameof(filePath));
        }

        if (content is null)
        {
            return await compiler.CreateCompilation(fileUri);
        }

        var workspace = new ActiveSourceFileSet();
        workspace.UpsertSourceFile(sourceFileFactory(fileUri, content));

        return await compiler.CreateCompilation(fileUri, workspace);
    }
}
