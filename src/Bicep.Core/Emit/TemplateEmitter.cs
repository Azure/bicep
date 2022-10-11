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

        public TemplateEmitter(SemanticModel model)
        {
            this.model = model;
            this.settings = new(model.Features);
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
            new PlaceholderParametersJsonWriter(this.model, this.settings).Write(writer, existingContent);
        });

        /// <summary>
        /// Emits a template to the specified stream if there are no errors. No writes are made to the stream if there are compilation errors.
        /// </summary>
        /// <param name="stream">The stream to write the template</param>
        public EmitResult Emit(Stream stream) => EmitOrFail(() =>
        {
            var sourceFileToTrack = this.settings.EnableSourceMapping ? this.model.SourceFile : default;
            using var writer = new SourceAwareJsonTextWriter(this.model.FileResolver, new StreamWriter(stream, UTF8EncodingWithoutBom, 4096, leaveOpen: true), sourceFileToTrack)
            {
                Formatting = Formatting.Indented
            };

            var emitter = new TemplateWriter(this.model, this.settings);
            emitter.Write(writer);

            return writer.SourceMap;
        });
        /// <summary>
        /// Emits a template to the specified text writer if there are no errors. No writes are made to the writer if there are compilation errors.
        /// </summary>
        /// <param name="textWriter">The text writer to write the template</param>
        public EmitResult Emit(TextWriter textWriter) => EmitOrFail(() =>
        {
            var sourceFileToTrack = this.settings.EnableSourceMapping ? this.model.SourceFile : default;
            using var writer = new SourceAwareJsonTextWriter(this.model.FileResolver, textWriter, sourceFileToTrack)
            {
                // don't close the textWriter when writer is disposed
                CloseOutput = false,
                Formatting = Formatting.Indented
            };


            var emitter = new TemplateWriter(this.model, this.settings);
            emitter.Write(writer);
            writer.Flush();

            return writer.SourceMap;
        });

        /// <summary>
        /// Emits a template to the specified json writer if there are no errors. No writes are made to the writer if there are compilation errors.
        /// </summary>
        /// <param name="writer">The json writer to write the template</param>
        public EmitResult Emit(SourceAwareJsonTextWriter writer) => this.EmitOrFail(() =>
        {
            var emitter = new TemplateWriter(this.model, this.settings);
            emitter.Write(writer);

            return writer.SourceMap;
        });

        private EmitResult EmitOrFail(Action write) => this.EmitOrFail(() =>
        {
            write();
            return default;
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
}
