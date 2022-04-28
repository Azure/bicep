// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.CompilationManager;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepDeploymentMissingParametersHandler : ExecuteTypedResponseCommandHandlerBase<string, string, IEnumerable<string>>
    {
        private readonly ICompilationManager compilationManager;

        public BicepDeploymentMissingParametersHandler(ICompilationManager compilationManager, ISerializer serializer)
            : base(LangServerConstants.GetDeployMissingParameters, serializer)
        {
            this.compilationManager = compilationManager;
        }

        public override Task<IEnumerable<string>> Handle(string documentPath, string parametersFilePath, CancellationToken cancellationToken)
        {
            var documentUri = DocumentUri.FromFileSystemPath(documentPath);
            var compilationContext = compilationManager.GetCompilation(documentUri);

            if (compilationContext is null)
            {
                return Task.FromResult(Enumerable.Empty<string>());
            }

            var semanticModel = compilationContext.Compilation.GetEntrypointSemanticModel();
            var parameterDeclarations = semanticModel.Root.ParameterDeclarations;
            var paramsWithoutModifiers = new List<ParameterDeclarationSyntax>();

            foreach (var parameterDeclaration in semanticModel.Root.ParameterDeclarations)
            {
                if (parameterDeclaration.DeclaringParameter is ParameterDeclarationSyntax parameterDeclarationSyntax &&
                    parameterDeclarationSyntax.Modifier is null)
                {
                    paramsWithoutModifiers.Add(parameterDeclarationSyntax);
                }
            }

            if (string.IsNullOrEmpty(parametersFilePath))
            {
                return Task.FromResult(paramsWithoutModifiers.Select(x => x.Name.IdentifierName));
            }

            var parametersFileContents = File.ReadAllText(parametersFilePath);
            Dictionary<string, dynamic>? dict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(parametersFileContents);

            if (dict is null)
            {
                return Task.FromResult(paramsWithoutModifiers.Select(x => x.Name.IdentifierName));
            }

            var missingParameters = paramsWithoutModifiers.Where(x => !dict.ContainsKey(x.Name.IdentifierName));
            return Task.FromResult(missingParameters.Select(x => x.Name.IdentifierName));
        }
    }
}
