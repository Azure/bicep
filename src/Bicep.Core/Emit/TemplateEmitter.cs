// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using System.Linq;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Newtonsoft.Json;

namespace Bicep.Core.Emit
{
    public class TemplateEmitter
    {
        private readonly SemanticModel model;

        private readonly EmitterSettings settings;

        private (string,int)[]? sourceMap;

        public TemplateEmitter(SemanticModel model, EmitterSettings settings)
        {
            this.model = model;
            this.settings = settings;
            this.sourceMap = null;
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
        public EmitResult EmitParametersFile(Stream stream, string existingContent) => EmitOrFail(() =>
        {
            using var writer = new JsonTextWriter(new StreamWriter(stream, UTF8EncodingWithoutBom, 4096, leaveOpen: true))
            {
                Formatting = Formatting.Indented
            };

            this.EmitParametersFile(writer, existingContent);
        });

        /// <summary>
        /// Emits a template to the specified json writer if there are no errors. No writes are made to the writer if there are compilation errors.
        /// </summary>
        /// <param name="writer">The json writer to write the template</param>
        /// <param name="existingContent">Existing content of the parameters file</param>
        public EmitResult EmitParametersFile(JsonTextWriter writer, string existingContent) => this.EmitOrFail(() =>
        {
            new ParametersFileTemplateWriter(this.model, this.settings).Write(writer, existingContent);
        });

        /// <summary>
        /// Emits a template to the specified stream if there are no errors. No writes are made to the stream if there are compilation errors.
        /// </summary>
        /// <param name="stream">The stream to write the template</param>
        public EmitResult Emit(Stream stream) => EmitOrFail(() =>
        {
            using var writer = new JsonTextWriter(new StreamWriter(stream, UTF8EncodingWithoutBom, 4096, leaveOpen: true))
            {
                Formatting = Formatting.Indented
            };

            var emitter = new TemplateWriter(this.model, this.settings);
            emitter.Write(writer);
            this.sourceMap = emitter.sourceMap;
        });

        /// <summary>
        /// Emits a template to the specified text writer if there are no errors. No writes are made to the writer if there are compilation errors.
        /// </summary>
        /// <param name="textWriter">The text writer to write the template</param>
        public EmitResult Emit(TextWriter textWriter) => EmitOrFail(() =>
        {
            using var writer = new JsonTextWriter(textWriter)
            {
                // don't close the textWriter when writer is disposed
                CloseOutput = false,
                Formatting = Formatting.Indented
            };

            var emitter = new TemplateWriter(this.model, this.settings);
            emitter.Write(writer);
            this.sourceMap = emitter.sourceMap;
        });

        /// <summary>
        /// Emits a template to the specified json writer if there are no errors. No writes are made to the writer if there are compilation errors.
        /// </summary>
        /// <param name="writer">The json writer to write the template</param>
        public EmitResult Emit(JsonTextWriter writer) => this.EmitOrFail(() =>
        {
            var emitter = new TemplateWriter(this.model, this.settings);
            emitter.Write(writer);
            this.sourceMap = emitter.sourceMap;
        });

        private EmitResult EmitOrFail(Action write)
        {
            // collect all the diagnostics
            var diagnostics = this.model.GetAllDiagnostics();

            if (diagnostics.Any(d => d.Level == DiagnosticLevel.Error))
            {
                return new EmitResult(EmitStatus.Failed, diagnostics);
            }

            write();

            return new EmitResult(EmitStatus.Succeeded, diagnostics, sourceMap);
        }
    }
}
