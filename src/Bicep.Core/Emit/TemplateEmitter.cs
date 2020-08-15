using System;
using System.IO;
using System.Linq;
using System.Text;
using Bicep.Core.Diagnostics;
using Newtonsoft.Json;

namespace Bicep.Core.Emit
{
    public class TemplateEmitter
    {
        private readonly SemanticModel.SemanticModel model;

        public TemplateEmitter(SemanticModel.SemanticModel model)
        {
            this.model = model;
        }

        /// <summary>
        /// Emits a template to the specified file if there are no errors. The specified file is not touched if there are compilation errors.
        /// </summary>
        /// <param name="fileName">The path to the file.</param>
        public EmitResult Emit(string fileName)
        {
            return EmitOrFail(() => CreateWriter(CreateFileStream(fileName)));
        }

        /// <summary>
        /// Emits a template to the specified stream if there are no errors. No writes are made to the stream if there are compilation errors.
        /// </summary>
        /// <param name="stream">The stream to write the template</param>
        public EmitResult Emit(Stream stream) => EmitOrFail(() => CreateWriter(stream));

        /// <summary>
        /// Emits a template to the specified text writer if there are no errors. No writes are made to the writer if there are compilation errors.
        /// </summary>
        /// <param name="writer">The text writer to write the template</param>
        public EmitResult Emit(TextWriter writer) => EmitOrFail(() => CreateWriter(writer));

        /// <summary>
        /// Emits a template to the specified json writer if there are no errors. No writes are made to the writer if there are compilation errors.
        /// </summary>
        /// <param name="writer">The json writer to write the template</param>
        public EmitResult Emit(JsonTextWriter writer) => this.EmitOrFail(() => writer);

        private EmitResult EmitOrFail(Func<JsonTextWriter> writerFactory)
        {
            // collect all the diagnostics
            var diagnostics = this.model.GetAllDiagnostics();

            if (diagnostics.Any(d => d.Level == DiagnosticLevel.Error))
            {
                return new EmitResult(EmitStatus.Failed, diagnostics);
            }

            using var writer = writerFactory();
            new TemplateWriter(writer, this.model).Write();

            return new EmitResult(EmitStatus.Succeeded, diagnostics);
        }

        private Stream CreateFileStream(string fileName) => new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);

        private JsonTextWriter CreateWriter(Stream stream) => CreateWriter(new StreamWriter(stream, Encoding.UTF8, 4096, true));

        private JsonTextWriter CreateWriter(TextWriter textWriter) =>
            new JsonTextWriter(textWriter)
            {
                Formatting = Formatting.Indented
            };
    }
}
