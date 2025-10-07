// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Emit.Options;
using Bicep.Core.Extensions;
using Bicep.Core.Json;
using Bicep.IO.Abstraction;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Utils;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    // This handler is used to build compiled parameters.json file for given a bicep file path.
    // It returns build-params succeeded/failed message, which can be displayed appropriately in IDE output window
    public class BicepBuildParamsCommandHandler : ExecuteTypedResponseCommandHandlerBase<DocumentUri, string>
    {
        private readonly ICompilationManager compilationManager;
        private readonly IFileExplorer fileExplorer;
        private readonly BicepCompiler bicepCompiler;

        public BicepBuildParamsCommandHandler(ICompilationManager compilationManager, IFileExplorer fileExplorer, BicepCompiler bicepCompiler, ISerializer serializer)
            : base(LangServerConstants.BuildParamsCommand, serializer)
        {
            this.compilationManager = compilationManager;
            this.fileExplorer = fileExplorer;
            this.bicepCompiler = bicepCompiler;
        }

        public override async Task<string> Handle(DocumentUri documentUri, CancellationToken cancellationToken)
        {
            string output = await GenerateCompiledParametersFileAndReturnOutputMessage(documentUri, cancellationToken);

            return output;
        }

        private async Task<string> GenerateCompiledParametersFileAndReturnOutputMessage(DocumentUri documentUri, CancellationToken cancellationToken)
        {
            var bicepParamFileUri = documentUri.ToIOUri();
            var jsonParamFileUri = bicepParamFileUri.WithExtension(".parameters.json");
            var jsonParamFile = this.fileExplorer.GetFile(jsonParamFileUri);

            // If the template exists and has a .json extension and contains the Bicep metadata, fail the build params.
            // If not, continue to update the file.
            if (jsonParamFile.Exists() && !ContainsDeploymentParametersSchema(jsonParamFile))
            {
                return $@"Building parameters file failed. The file ""{jsonParamFileUri}"" already exists. If overwriting the file is intended, delete it manually and retry the Build Parameters command.";
            }

            var compilation = await new CompilationHelper(bicepCompiler, compilationManager).GetRefreshedCompilation(documentUri);
            var paramsResult = compilation.Emitter.Parameters();

            if (paramsResult.Success != true || paramsResult.Parameters is not { } parameters)
            {
                var diagnosticsByFile = compilation.GetAllDiagnosticsByBicepFile();

                return "Building parameters file failed. Please fix below errors:\n" + DiagnosticsHelper.GetDiagnosticsMessage(diagnosticsByFile);
            }

            await jsonParamFile.WriteAllTextAsync(parameters, cancellationToken);

            return $"Building parameters file succeeded. Processed file {jsonParamFileUri}";
        }

        // Returns true if the template contains the parameters file schema, false otherwise
        private static bool ContainsDeploymentParametersSchema(IFileHandle parametersFile)
        {
            try
            {

                var parametersText = parametersFile.ReadAllText();
                var parametersElement = JsonElementFactory.CreateElement(parametersText);

                return
                    parametersElement.TryGetProperty("$schema", out var schemaElement) &&
                    schemaElement.GetString() is { } schema &&
                    schema.Contains("deploymentParameters.json", StringComparison.OrdinalIgnoreCase);

            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
