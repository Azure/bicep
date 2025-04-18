// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Entities;
using Azure.Deployments.Core.Helpers;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
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
    // This handler is used to generate compiled .json file for given a bicep file path.
    // It returns build succeeded/failed message, which can be displayed appropriately in IDE output window
    public class BicepBuildCommandHandler : ExecuteTypedResponseCommandHandlerBase<DocumentUri, string>
    {
        private readonly BicepCompiler bicepCompiler;
        private readonly ICompilationManager compilationManager;

        public BicepBuildCommandHandler(BicepCompiler bicepCompiler, ICompilationManager compilationManager, ISerializer serializer)
            : base(LangServerConstants.BuildCommand, serializer)
        {
            this.bicepCompiler = bicepCompiler;
            this.compilationManager = compilationManager;
        }

        public override async Task<string> Handle(DocumentUri documentUri, CancellationToken cancellationToken)
        {
            var filePath = HandlerHelper.ValidateLocalFilePath(documentUri);
            return await GenerateCompiledFileAndReturnBuildOutputMessageAsync(filePath, documentUri);
        }

        private async Task<string> GenerateCompiledFileAndReturnBuildOutputMessageAsync(string bicepFilePath, DocumentUri documentUri)
        {
            string compiledFilePath = PathHelper.GetJsonOutputPath(bicepFilePath);

            // If the template exists and contains bicep generator metadata, we can go ahead and replace the file.
            // If not, we'll fail the build.
            if (File.Exists(compiledFilePath) && !TemplateContainsBicepGeneratorMetadata(File.ReadAllText(compiledFilePath)))
            {
                return "Bicep build failed. The output file \"" + compiledFilePath + "\" already exists and was not generated by Bicep. If overwriting the file is intended, delete it manually and retry the build command.";
            }

            var fileUri = documentUri.ToIOUri();

            var compilation = await new CompilationHelper(bicepCompiler, compilationManager).GetRefreshedCompilation(documentUri);

            var diagnosticsByFile = compilation.GetAllDiagnosticsByBicepFile()
                .FirstOrDefault(x => x.Key.FileHandle.Uri == fileUri);

            if (diagnosticsByFile.Value.Any(x => x.IsError()))
            {
                return "Bicep build failed. Please fix below errors:\n" + DiagnosticsHelper.GetDiagnosticsMessage(diagnosticsByFile);
            }

            using var fileStream = new FileStream(compiledFilePath, FileMode.Create, FileAccess.ReadWrite);
            var model = compilation.GetEntrypointSemanticModel();
            var emitter = new TemplateEmitter(model);
            EmitResult result = emitter.Emit(fileStream);

            return "Bicep build succeeded. Created ARM template file: \"" + compiledFilePath + "\"";
        }

        // Returns true if the template contains bicep _generator metadata, false otherwise
        public bool TemplateContainsBicepGeneratorMetadata(string template)
        {
            try
            {
                if (!string.IsNullOrEmpty(template))
                {
                    JToken jtoken = template.FromJson<JToken>();
                    if (TemplateHelpers.TryGetTemplateGeneratorObject(jtoken, out DeploymentTemplateGeneratorMetadata? generator))
                    {
                        if (generator?.Name == "bicep")
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }
    }
}
