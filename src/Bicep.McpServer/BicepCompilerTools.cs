// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.ComponentModel;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.PrettyPrintV2;
using Bicep.IO.Abstraction;
using ModelContextProtocol.Server;

namespace Bicep.McpServer;

[McpServerToolType]
public sealed class BicepCompilerTools(
    BicepCompiler compiler)
{
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

    [McpServerTool(Title = "Get Bicep File Diagnostics", Destructive = false, Idempotent = true, OpenWorld = true, ReadOnly = true, UseStructuredContent = true)]
    [Description("""
    Analyzes a Bicep file (.bicep) or Bicep parameters file (.bicepparam) and returns all compilation diagnostics including errors, warnings, and informational messages.
    
    Use this tool to:
    - Validate Bicep syntax and identify compilation errors before deployment
    - Check for warnings about deprecated features, security issues, or best practice violations
    - Troubleshoot why a Bicep file isn't compiling
    - Understand the severity and location of issues in the code
    
    Each diagnostic includes the error code (e.g., BCP033), severity level (Error/Warning/Info), descriptive message, exact position in the file, and a link to documentation for more details.
    The file path must be absolute. Diagnostics are returned for the specified file and any modules it references.
    """)]
    public async Task<ImmutableArray<DiagnosticDefinition>> GetBicepFileDiagnostics(
        [Description("The path to the .bicep or .bicepparam file")] string filePath)
    {
        var fileUri = IOUri.FromFilePath(filePath);
        if (!fileUri.HasBicepExtension() && !fileUri.HasBicepParamExtension())
        {
            throw new ArgumentException("The specified file must have a .bicep or .bicepparam extension.", nameof(filePath));
        }

        var compilation = await compiler.CreateCompilation(fileUri);

        return [.. compilation
            .GetAllDiagnosticsByBicepFile()
            .SelectMany(kvp => kvp.Value.Select(x => new DiagnosticDefinition(
                kvp.Key.FileHandle.Uri.ToUri(),
                x.Code,
                x.Level.ToString(),
                x.Message,
                x.Uri,
                x.Span.Position,
                x.Span.Length)))];
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
    
    The file path must be absolute. Formatting preserves semantic meaning and only changes whitespace and layout. Files with syntax errors will still be formatted to the extent possible.
    """)]
    public async Task<FormatResult> FormatBicepFile(
        [Description("The path to the .bicep or .bicepparam file")] string filePath)
    {
        var fileUri = IOUri.FromFilePath(filePath);
        if (!fileUri.HasBicepExtension() && !fileUri.HasBicepParamExtension())
        {
            throw new ArgumentException("The specified file must have a .bicep or .bicepparam extension.", nameof(filePath));
        }

        var compilation = await compiler.CreateCompilation(fileUri);
        var sourceFile = compilation.GetEntrypointSemanticModel().SourceFile;

        var options = sourceFile.Configuration.Formatting.Data;
        var context = PrettyPrinterV2Context.Create(options, sourceFile.LexingErrorLookup, sourceFile.ParsingErrorLookup);

        return new FormatResult(PrettyPrinterV2.Print(sourceFile.ProgramSyntax, context));
    }
}
