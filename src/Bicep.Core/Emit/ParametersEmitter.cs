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

 // Emits bicepparams files
public class ParametersEmitter
{
    private readonly SemanticModel model;

    public ParametersEmitter(SemanticModel model)
    {
        this.model = model;
    }

    /// <summary>
    /// The JSON spec requires UTF8 without a BOM, so we use this encoding to write JSON files.
    /// </summary>
    public static Encoding UTF8EncodingWithoutBom { get; } = new UTF8Encoding(false);

    public EmitResult Emit(Stream stream)
    {
        using var sw = new StreamWriter(stream, UTF8EncodingWithoutBom, 4096, leaveOpen: true);

        return Emit(sw);
    }

    public EmitResult Emit(TextWriter textWriter) => this.EmitOrFail(() =>
    {
        using var writer = new JsonTextWriter(textWriter)
        {
            // don't close the textWriter when writer is disposed
            CloseOutput = false,
            Formatting = Formatting.Indented
        };

        var emitter = new ParametersJsonWriter(model);
        emitter.Write(writer);
        writer.Flush();
    });

    private EmitResult EmitOrFail(Action write)
    {
        // collect all the diagnostics
        var diagnostics = model.GetAllDiagnostics();

        if (diagnostics.Any(d => d.Level == DiagnosticLevel.Error))
        {
            return new EmitResult(EmitStatus.Failed, diagnostics);
        }

        write();

        return new EmitResult(EmitStatus.Succeeded, diagnostics);
    }
}
