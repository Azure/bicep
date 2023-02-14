// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using System.Linq;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
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
    public EmitResult EmitEmptyParametersFile(Stream stream, string existingContent)
    {
        using var sw = new StreamWriter(stream, UTF8EncodingWithoutBom, 4096, leaveOpen: true);

        return EmitEmptyParametersFile(sw, existingContent);
    }

    /// <summary>
    /// Emits a template to the specified json writer if there are no errors. No writes are made to the writer if there are compilation errors.
    /// </summary>
    /// <param name="textWriter">The text writer to write the template</param>
    /// <param name="existingContent">Existing content of the parameters file</param>
    public EmitResult EmitEmptyParametersFile(TextWriter textWriter, string existingContent) => this.EmitOrFail(() =>
    {
        using var writer = new JsonTextWriter(textWriter)
        {
            // don't close the textWriter when writer is disposed
            CloseOutput = false,
            Formatting = Formatting.Indented
        };

        var emitter = new PlaceholderParametersJsonWriter(this.model);
        emitter.Write(writer, existingContent);
        writer.Flush();

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