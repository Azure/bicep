// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Emit.Options;
using Bicep.Core.FileSystem;
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
    public record BicepGenerateParamsCommandParams(
        string BicepFilePath,
        OutputFormatOption OutputFormat,
        IncludeParamsOption IncludeParams
    );

    // This handler is used to generate compiled parameters.json file for given a bicep file path.
    // It returns generate-params succeeded/failed message, which can be displayed appropriately in IDE output window
    public class BicepGenerateParamsCommandHandler : ExecuteTypedResponseCommandHandlerBase<BicepGenerateParamsCommandParams, string>
    {
        private readonly ICompilationManager compilationManager;
        private readonly BicepCompiler bicepCompiler;

        public BicepGenerateParamsCommandHandler(ICompilationManager compilationManager, BicepCompiler bicepCompiler, ISerializer serializer)
            : base(LangServerConstants.GenerateParamsCommand, serializer)
        {
            this.compilationManager = compilationManager;
            this.bicepCompiler = bicepCompiler;
        }

        public override async Task<string> Handle(BicepGenerateParamsCommandParams parameters, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(parameters.BicepFilePath))
            {
                throw new ArgumentException("Invalid input file path");
            }

            DocumentUri documentUri = DocumentUri.FromFileSystemPath(parameters.BicepFilePath);
            string output = await GenerateCompiledParametersFileAndReturnOutputMessage(parameters.BicepFilePath, parameters.OutputFormat, parameters.IncludeParams, documentUri);

            return output;
        }

        private async Task<string> GenerateCompiledParametersFileAndReturnOutputMessage(string bicepFilePath, OutputFormatOption outputFormat, IncludeParamsOption includeParams, DocumentUri documentUri)
        {
            var compiledFilePath = PathHelper.ResolveParametersFileOutputPath(bicepFilePath, outputFormat);
            var compiledFile = Path.GetFileName(compiledFilePath);
            var extension = Path.GetExtension(compiledFilePath).ToLowerInvariant();

            // If the template exists and has a .json extension and contains the Bicep metadata, fail the generate params.
            // If not, continue to update the file.
            if (extension == LanguageConstants.JsonFileExtension && File.Exists(compiledFilePath) && !TemplateIsParametersFile(File.ReadAllText(compiledFilePath)))
            {
                return "Generating parameters file failed. The file \"" + compiledFile + "\" already exists. If overwriting the file is intended, delete it manually and retry the Generate Parameters command.";
            }

            var compilation = await new CompilationHelper(bicepCompiler, compilationManager).GetRefreshedCompilation(documentUri);
            var fileUri = documentUri.ToIOUri();

            var diagnosticsByFile = compilation.GetAllDiagnosticsByBicepFile()
                .FirstOrDefault(x => x.Key.FileHandle.Uri == fileUri);

            if (diagnosticsByFile.Value.Any(x => x.IsError()))
            {
                return "Generating parameters file failed. Please fix below errors:\n" + DiagnosticsHelper.GetDiagnosticsMessage(diagnosticsByFile);
            }

            var existingContent = File.Exists(compiledFilePath) ? File.ReadAllText(compiledFilePath) : string.Empty;

            var model = compilation.GetEntrypointSemanticModel();
            var emitter = new TemplateEmitter(model);
            using var fileStream = new FileStream(compiledFilePath, FileMode.Create, FileAccess.Write);
            var result = emitter.EmitTemplateGeneratedParameterFile(fileStream, existingContent, outputFormat, includeParams);

            return "Generating parameters file succeeded. Processed file " + compiledFile;
        }

        // Returns true if the template contains the parameters file schema, false otherwise
        public bool TemplateIsParametersFile(string template)
        {
            try
            {
                if (!string.IsNullOrEmpty(template))
                {
                    JToken jtoken = template.FromJson<JToken>();
                    var schema = jtoken.SelectToken("$schema")?.ToString();

                    if (schema != null && schema.ContainsInsensitively("deploymentParameters.json"))
                    {
                        return true;
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
