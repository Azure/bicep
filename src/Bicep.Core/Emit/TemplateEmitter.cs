// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using System.Linq;
using System.Text;
using Azure.Deployments.Core.Extensions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Newtonsoft.Json;

namespace Bicep.Core.Emit
{
    public class TemplateEmitter
    {
        private readonly SemanticModel model;

        private string? templateHash;

        /// <summary>
        /// The JSON spec requires UTF8 without a BOM, so we use this encoding to write JSON files.
        /// </summary>
        private Encoding UTF8EncodingWithoutBom => new UTF8Encoding(false);

        public TemplateEmitter(SemanticModel model)
        {
            this.model = model;
        }

        private void CalculateTemplateHash()
        {
            using MemoryStream memoryStream = new();
            using (var streamWriter = new StreamWriter(memoryStream, UTF8EncodingWithoutBom))
            using (var writer = new JsonTextWriter(streamWriter))
            {
                new TemplateWriter(writer, this.model).Write(true, null);
            }
            this.templateHash = TemplateHashExtensions.ComputeTemplateHash(UTF8EncodingWithoutBom.GetString(memoryStream.ToArray()));
        }

        /// <summary>
        /// Emits a template to the specified stream if there are no errors. No writes are made to the stream if there are compilation errors.
        /// </summary>
        /// <param name="stream">The stream to write the template</param>
        public EmitResult Emit(Stream stream) => EmitOrFail(() =>
        {
            using var writer = new JsonTextWriter(new StreamWriter(stream, UTF8EncodingWithoutBom, 4096, true))
            {
                Formatting = Formatting.Indented
            };

            new TemplateWriter(writer, this.model).Write(true, this.templateHash);
        });

        /// <summary>
        /// Emits a template to the specified text writer if there are no errors. No writes are made to the writer if there are compilation errors.
        /// </summary>
        /// <param name="textWriter">The text writer to write the template</param>
        public EmitResult Emit(TextWriter textWriter) => EmitOrFail(() =>
        {
            using var writer = new JsonTextWriter(textWriter)
            {
                Formatting = Formatting.Indented
            };

            new TemplateWriter(writer, this.model).Write(true, this.templateHash);
        });

        /// <summary>
        /// Emits a template to the specified json writer if there are no errors. No writes are made to the writer if there are compilation errors.
        /// </summary>
        /// <param name="writer">The json writer to write the template</param>
        public EmitResult Emit(JsonTextWriter writer) => this.EmitOrFail(() =>
        {
            new TemplateWriter(writer, this.model).Write(true, this.templateHash);
        });

        private EmitResult EmitOrFail(Action write)
        {
            // collect all the diagnostics
            var diagnostics = this.model.GetAllDiagnostics();

            if (diagnostics.Any(d => d.Level == DiagnosticLevel.Error))
            {
                return new EmitResult(EmitStatus.Failed, diagnostics);
            }
            
            this.CalculateTemplateHash();
            write();

            return new EmitResult(EmitStatus.Succeeded, diagnostics);
        }
    }
}

