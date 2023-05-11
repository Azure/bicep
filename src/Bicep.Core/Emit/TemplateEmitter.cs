// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using System.Linq;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit.Options;
using Bicep.Core.PrettyPrint;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
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
        switch (outputFormat)
        {
            case OutputFormatOption.BicepParam:
            {
                var bicepParamEmitter = new PlaceholderParametersBicepParamWriter(this.model, includeParams);
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

                var jsonEmitter = new PlaceholderParametersJsonWriter(this.model, includeParams);
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
        using var sw = new StreamWriter(stream, UTF8EncodingWithoutBom, 4096, leaveOpen: true);

        return Emit(sw);
    }

    /// <summary>
    /// Emits a template to the specified text writer if there are no errors. No writes are made to the writer if there are compilation errors.
    /// </summary>
    /// <param name="textWriter">The text writer to write the template</param>
    public EmitResult Emit(TextWriter textWriter) => EmitOrFail(() =>
    {
        var sourceFileToTrack = this.model.Features.SourceMappingEnabled ? this.model.SourceFile : default;
        using var writer = new SourceAwareJsonTextWriter(this.model.FileResolver, textWriter, sourceFileToTrack)
        {
            // don't close the textWriter when writer is disposed
            CloseOutput = false,
            Formatting = Formatting.Indented
        };

        var emitter = new TemplateWriter(this.model);
        emitter.Write(writer);
        writer.Flush();

        return writer.SourceMap;
    });

    private EmitResult EmitOrFail(Func<SourceMap?> write)
    {
        // collect all the diagnostics
        var diagnostics = this.model.GetAllDiagnostics();

        if (diagnostics.Any(d => d.Level == DiagnosticLevel.Error))
        {
            return new EmitResult(EmitStatus.Failed, diagnostics);
        }

        var sourceMap = write();

        return new EmitResult(EmitStatus.Succeeded, diagnostics, sourceMap);
    }
}
