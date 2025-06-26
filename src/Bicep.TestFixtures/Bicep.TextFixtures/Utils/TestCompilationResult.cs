// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Newtonsoft.Json.Linq;

namespace Bicep.TextFixtures.Utils
{
    public record TestCompilationResult(JToken? Template, ImmutableArray<IDiagnostic> Diagnostics, Compilation Compilation)
    {
        public BicepFile EntryPointFile => (BicepFile)Compilation.SourceFileGrouping.EntryPoint;

        public static TestCompilationResult FromCompilation(Compilation compilation)
        {
            var templateResult = compilation.Emitter.Template();
            var semanticModel = compilation.GetEntrypointSemanticModel();
            var emitter = new TemplateEmitter(semanticModel);
            var diagnostics = semanticModel.GetAllDiagnostics();

            JToken? template = null;
            if (!semanticModel.HasErrors())
            {
                var stringWriter = new StringWriter();
                var result = emitter.Emit(stringWriter);

                if (result.Status != EmitStatus.Failed)
                {
                    template = JToken.Parse(stringWriter.ToString());
                }
            }

            return new(template, diagnostics, compilation);
        }
    }
}
