// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Newtonsoft.Json.Linq;

namespace Bicep.Testing.Utils
{
    public class TestCompilationResult
    {
        private readonly Lazy<ImmutableArray<IDiagnostic>> diagnostics;
        private readonly Lazy<JToken?> parameters;
        private readonly Lazy<JToken?> template;

        private TestCompilationResult(Compilation compilation)
        {
            this.Compilation = compilation;
            this.diagnostics = new(this.GetDiagnostics);
            this.parameters = new(this.GetParameters);
            this.template = new(this.GetTemplate);
        }

        public JToken? Parameters => this.parameters.Value;

        public JToken? Template => this.template.Value;

        public ImmutableArray<IDiagnostic> Diagnostics => this.diagnostics.Value;

        public Compilation Compilation { get; }

        public BicepSourceFile EntryPointFile => this.Compilation.SourceFileGrouping.EntryPoint;

        public static TestCompilationResult FromCompilation(Compilation compilation)
            => new(compilation);

        private ImmutableArray<IDiagnostic> GetDiagnostics()
            => this.Compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

        private JToken? GetParameters()
        {
            var semanticModel = this.Compilation.GetEntrypointSemanticModel();
            if (semanticModel.SourceFileKind is not BicepSourceFileKind.ParamsFile)
            {
                return null;
            }

            var emitter = new ParametersEmitter(semanticModel);
            if (semanticModel.HasErrors())
            {
                return null;
            }

            var stringWriter = new StringWriter();
            var result = emitter.Emit(stringWriter);

            return result.Status != EmitStatus.Failed
                ? JToken.Parse(stringWriter.ToString())
                : null;
        }

        private JToken? GetTemplate()
        {
            var semanticModel = this.Compilation.GetEntrypointSemanticModel();
            if (semanticModel.SourceFileKind is not BicepSourceFileKind.BicepFile)
            {
                return null;
            }

            var emitter = new TemplateEmitter(semanticModel);
            if (semanticModel.HasErrors())
            {
                return null;
            }

            var stringWriter = new StringWriter();
            var result = emitter.Emit(stringWriter);

            return result.Status != EmitStatus.Failed
                ? JToken.Parse(stringWriter.ToString())
                : null;
        }
    }
}
