// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit.Options;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Newtonsoft.Json;

namespace Bicep.Core.Emit;

public class TemplateEmitter
{
    private readonly SemanticModel model;

    public TemplateEmitter(SemanticModel model)
    {
        this.model = model;
    }

    /// <summary>
    /// The JSON spec requires UTF8 without a BOM, so we use this encoding to write JSON files.
    /// </summary>
    public static Encoding UTF8EncodingWithoutBom { get; } = new UTF8Encoding(false);

    /// <summary>
    /// Emits a template to the specified stream if there are no errors. No writes are made to the stream if there are compilation errors.
    /// </summary>
    /// <param name="stream">The stream to write the template</param>
    /// <param name="existingContent">Existing content of the parameters file</param>
    /// <param name="outputFormat">Output file format (json or bicepparam)</param>
    /// <param name="includeParams">Include parameters (requiredonly or all)</param>
    public EmitResult EmitTemplateGeneratedParameterFile(Stream stream, string existingContent, OutputFormatOption outputFormat, IncludeParamsOption includeParams)
    {
        using var sw = new StreamWriter(stream, UTF8EncodingWithoutBom, 4096, leaveOpen: true);

        return EmitTemplateGeneratedParameterFile(sw, existingContent, outputFormat, includeParams);
    }

    /// <summary>
    /// Emits a template to the specified json writer if there are no errors. No writes are made to the writer if there are compilation errors.
    /// </summary>
    /// <param name="textWriter">The text writer to write the template</param>
    /// <param name="existingContent">Existing content of the parameters file</param>
    /// <param name="outputFormat">Output file format (json or bicepparam)</param>
    /// <param name="includeParams">Include parameters (requiredonly or all)</param>
    public EmitResult EmitTemplateGeneratedParameterFile(TextWriter textWriter, string existingContent, OutputFormatOption outputFormat, IncludeParamsOption includeParams) => this.EmitOrFail(() =>
    {
        if (model is not SemanticModel bicepSemanticModel)
        {
            throw new InvalidOperationException($"This action is only supported for Bicep files");
        }

        switch (outputFormat)
        {
            case OutputFormatOption.BicepParam:
                {
                    var bicepParamEmitter = new PlaceholderParametersBicepParamWriter(bicepSemanticModel, includeParams);
                    bicepParamEmitter.Write(textWriter, existingContent);

                    break;
                }
            case OutputFormatOption.Json:
            default:
                {
                    using var writer = new JsonTextWriter(textWriter)
                    {
                        // don't close the textWriter when writer is disposed
                        CloseOutput = false,
                        Formatting = Formatting.Indented
                    };

                    var jsonEmitter = new PlaceholderParametersJsonWriter(bicepSemanticModel, includeParams);
                    jsonEmitter.Write(writer, existingContent);
                    writer.Flush();

                    break;
                }
        }

        return null;
    });

    /// <summary>
    /// Emits a template to the specified stream if there are no errors. No writes are made to the stream if there are compilation errors.
    /// </summary>
    /// <param name="stream">The stream to write the template</param>
    public EmitResult Emit(Stream stream)
    {
        using var sw = new StreamWriter(stream, UTF8EncodingWithoutBom, 4096, leaveOpen: true)
        {
            NewLine = "\n",
        };

        return Emit(sw);
    }

    /// <summary>
    /// Emits a template to the specified text writer if there are no errors. No writes are made to the writer if there are compilation errors.
    /// </summary>
    /// <param name="textWriter">The text writer to write the template</param>
    public EmitResult Emit(TextWriter textWriter) => EmitOrFail(() =>
    {
        var sourceFileToTrack = model switch
        {
            SemanticModel bicepModel when bicepModel.Features.SourceMappingEnabled => bicepModel.SourceFile,
            _ => null,
        };
        using var writer = new SourceAwareJsonTextWriter(textWriter, sourceFileToTrack)
        {
            // don't close the textWriter when writer is disposed
            CloseOutput = false,
            Formatting = Formatting.Indented
        };

        var emitter = TemplateWriterFactory.CreateTemplateWriter(this.model);
        emitter.Write(writer);
        writer.Flush();

        return writer.SourceMap;
    });

    private EmitResult EmitOrFail(Func<SourceMap?> write)
    {
        // collect all the diagnostics
        var diagnostics = model switch
        {
            SemanticModel x => x.GetAllDiagnostics(),
            _ => [],
        };

        if (diagnostics.Any(d => d.IsError()))
        {
            return new EmitResult(EmitStatus.Failed, diagnostics, model.Features);
        }

        var sourceMap = write();

        return new EmitResult(EmitStatus.Succeeded, diagnostics, model.Features, sourceMap);
    }

    /// <summary>
    /// Checks emit-blocking diagnostics without invoking aggregate diagnostics or linters.
    /// </summary>
    private bool HasEmitBlockingErrors()
    {
        // Analyzers call this while aggregate diagnostics are being assembled, so avoid GetAllDiagnostics() re-entry.
        var sourceFile = model.SourceFile;
        var diagnosticWriter = ToListDiagnosticWriter.Create();
        var visitor = new SemanticDiagnosticVisitor(diagnosticWriter);
        visitor.Visit(model.Root);

        foreach (var missingDeclarationSyntax in sourceFile.ProgramSyntax.Children.OfType<MissingDeclarationSyntax>())
        {
            model.TypeManager.GetTypeInfo(missingDeclarationSyntax);
        }

        diagnosticWriter.WriteMultiple(model.TypeManager.GetAllDiagnostics());
        diagnosticWriter.WriteMultiple(model.EmitLimitationInfo.Diagnostics);
        ScopeHelper.GetResourceScopeInfo(model, diagnosticWriter);
        ScopeHelper.GetModuleScopeInfo(model, diagnosticWriter);

        return sourceFile.LexingErrorLookup.Any(diagnostic => diagnostic.IsError())
            || sourceFile.ParsingErrorLookup.Any(diagnostic => diagnostic.IsError())
            || diagnosticWriter.GetDiagnostics().Any(diagnostic => diagnostic.IsError());
    }

    /// <summary>
    /// Emits a symbolic-name-keyed ARM template without mutating the semantic model's normal emitter settings.
    /// </summary>
    internal string? EmitWithForcedSymbolicNames()
    {
        if (HasEmitBlockingErrors())
        {
            return null;
        }

        try
        {
            var settings = new EmitterSettings(model, forceEnableSymbolicNames: true);
            using var stringWriter = new StringWriter { NewLine = "\n" };
            using (var writer = new SourceAwareJsonTextWriter(stringWriter, sourceFileToTrack: null)
            {
                CloseOutput = false,
                Formatting = Formatting.Indented,
            })
            {
                var templateWriter = TemplateWriterFactory.CreateTemplateWriter(model, settings);
                templateWriter.Write(writer);
                writer.Flush();
            }

            return stringWriter.ToString();
        }
        catch
        {
            return null;
        }
    }
}
