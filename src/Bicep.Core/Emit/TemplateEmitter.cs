using System.Linq;
using Bicep.Core.Parser;

namespace Bicep.Core.Emit
{
    public class TemplateEmitter
    {
        private readonly SemanticModel.SemanticModel semanticModel;

        public TemplateEmitter(SemanticModel.SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
        }

        public EmitResult Emit(string fileName)
        {
            // collect all the errors
            var diagnostics = this.semanticModel.GetAllDiagnostics();

            if (diagnostics.Any())
            {
                // TODO: This needs to account for warnings when we add severity.
                return new EmitResult(EmitStatus.Failed, diagnostics);
            }

            return new EmitResult(EmitStatus.Succeeded, new Error[0]);
        }
    }
}
