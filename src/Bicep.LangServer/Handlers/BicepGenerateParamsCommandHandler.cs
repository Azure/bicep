// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Emit.Options;
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
        private readonly IFileExplorer fileExplorer;
        private readonly BicepCompiler bicepCompiler;

        public BicepGenerateParamsCommandHandler(ICompilationManager compilationManager, IFileExplorer fileExplorer, BicepCompiler bicepCompiler, ISerializer serializer)
            : base(LangServerConstants.GenerateParamsCommand, serializer)
        {
            this.compilationManager = compilationManager;
            this.fileExplorer = fileExplorer;
            this.bicepCompiler = bicepCompiler;
        }

        public override async Task<string> Handle(BicepGenerateParamsCommandParams parameters, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(parameters.BicepFilePath))
            {
                throw new ArgumentException("Invalid input file path");
            }

            DocumentUri documentUri = DocumentUri.FromFileSystemPath(parameters.BicepFilePath);
            string output = await GenerateCompiledParametersFileAndReturnOutputMessage(parameters.OutputFormat, parameters.IncludeParams, documentUri);

            return output;
        }

        private async Task<string> GenerateCompiledParametersFileAndReturnOutputMessage(OutputFormatOption outputFormat, IncludeParamsOption includeParams, DocumentUri documentUri)
        {
            var bicepFileUri = documentUri.ToIOUri();
            var extension = outputFormat == OutputFormatOption.BicepParam ? LanguageConstants.ParamsFileExtension : LanguageConstants.JsonFileExtension;
            var compiledFileUri = bicepFileUri.WithExtension(extension);
            var compiledFile = this.fileExplorer.GetFile(compiledFileUri);

            // If the template exists and has a .json extension and contains the Bicep metadata, fail the generate params.
            // If not, continue to update the file.
            if (extension == LanguageConstants.JsonFileExtension && compiledFile.Exists())
            {
                var template = await compiledFile.ReadAllTextAsync();

                if (!TemplateIsParametersFile(template))
                {
                    return "Generating parameters file failed. The file \"" + compiledFile + "\" already exists. If overwriting the file is intended, delete it manually and retry the Generate Parameters command.";
                }
            }

            var compilation = await new CompilationHelper(bicepCompiler, compilationManager).GetRefreshedCompilation(documentUri);

            if (compilation.HasErrors())
            {
                var diagnosticsByFile = compilation.GetAllDiagnosticsByBicepFile();

                return "Generating parameters file failed. Please fix below errors:\n" + DiagnosticsHelper.GetDiagnosticsMessage(diagnosticsByFile);
            }

            var existingContent = compiledFile.Exists() ? await compiledFile.ReadAllTextAsync() : "";

            var model = compilation.GetEntrypointSemanticModel();
            var emitter = new TemplateEmitter(model);
            using var fileStream = new FileStream(compiledFileUri, FileMode.Create, FileAccess.Write);
            var result = emitter.EmitTemplateGeneratedParameterFile(fileStream, existingContent, outputFormat, includeParams);

            return $"Generating parameters file succeeded. Processed file '{compiledFile.Uri}'";
        }

        // Returns true if the template contains the parameters file schema, false otherwise
        public static bool TemplateIsParametersFile(string template)
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
