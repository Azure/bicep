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
    public class ParametersEmitter
    {
        private readonly ParamsSemanticModel paramSemanticModel;

        private readonly EmitterSettings settings;

        public ParametersEmitter(ParamsSemanticModel paramSemanticModel)
        {
            this.paramSemanticModel = paramSemanticModel;
            this.settings = new(paramSemanticModel.Features);
        }

        /// <summary>
        /// The JSON spec requires UTF8 without a BOM, so we use this encoding to write JSON files.
        /// </summary>
        public static Encoding UTF8EncodingWithoutBom { get; } = new UTF8Encoding(false);

         public EmitResult EmitParamsFile(Stream stream) => EmitOrFail(() =>
        {
            using var writer = new JsonTextWriter(new StreamWriter(stream, UTF8EncodingWithoutBom, 4096, leaveOpen: true))
            {
                Formatting = Formatting.Indented
            };

            new ParametersJsonWriter(paramSemanticModel).Write(writer);
        });

        public EmitResult EmitParamsFile(JsonTextWriter writer) => this.EmitOrFail(() =>
        {
            new ParametersJsonWriter(paramSemanticModel).Write(writer);
        });


        private EmitResult EmitOrFail(Action write)
        {
            // collect all the diagnostics
            var diagnostics = paramSemanticModel.GetAllDiagnostics();

            if (diagnostics.Any(d => d.Level == DiagnosticLevel.Error))
            {
                return new EmitResult(EmitStatus.Failed, diagnostics);
            }

            write();

            return new EmitResult(EmitStatus.Succeeded, diagnostics);
        }
    }
}
