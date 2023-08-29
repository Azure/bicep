// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    // This handler is used to build compiled parameters.json file for given a bicep file path.
    // It returns build-params succeeded/failed message, which can be displayed approriately in IDE output window
    public class BicepBuildParamsCommandHandler : ExecuteTypedResponseCommandHandlerBase<string, string>
    {
        private readonly ICompilationManager compilationManager;
        private readonly BicepCompiler bicepCompiler;

        public BicepBuildParamsCommandHandler(ICompilationManager compilationManager, BicepCompiler bicepCompiler, ISerializer serializer)
            : base(LangServerConstants.BuildParamsCommand, serializer)
        {
            this.compilationManager = compilationManager;
            this.bicepCompiler = bicepCompiler;
        }

        public override async Task<string> Handle(string bicepParamsFilePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(bicepParamsFilePath))
            {
                throw new ArgumentException("Invalid input file path");
            }

            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepParamsFilePath);
            string output = await GenerateCompiledParametersFileAndReturnOutputMessage(bicepParamsFilePath, documentUri);

            return output;
        }

        private async Task<string> GenerateCompiledParametersFileAndReturnOutputMessage(string bicepParamsFilePath, DocumentUri documentUri)
        {
            string compiledFilePath = PathHelper.ResolveParametersFileOutputPath(bicepParamsFilePath, OutputFormatOption.Json);
            string compiledFile = Path.GetFileName(compiledFilePath);

            // If the template exists and contains bicep generator metadata, we can go ahead and replace the file.
            // If not, we'll fail the generate params.
            if (File.Exists(compiledFilePath) && !TemplateIsParametersFile(File.ReadAllText(compiledFilePath)))
            {
                return "Building parameters file failed. The file \"" + compiledFile + "\" already exists but does not contain the schema for a parameters file. If overwriting the file is intended, delete it manually and retry the Generate Parameters command.";
            }

            var compilation = await new CompilationHelper(bicepCompiler, compilationManager).GetRefreshedCompilation(documentUri);
            var fileUri = documentUri.ToUriEncoded();

            var diagnosticsByFile = compilation.GetAllDiagnosticsByBicepFile().FirstOrDefault(x => x.Key.FileUri == fileUri);

            if (diagnosticsByFile.Value.Any(x => x.Level == DiagnosticLevel.Error))
            {
                return "Building parameters file failed. Please fix below errors:\n" + DiagnosticsHelper.GetDiagnosticsMessage(diagnosticsByFile);
            }

            var paramsSemanticModel = compilation.GetEntrypointSemanticModel();

            if (paramsSemanticModel.Root.TryGetBicepFileSemanticModelViaUsing(out var bicepSemanticModel, out _))
            {
                var bicepFileUsingPathUri = bicepSemanticModel.Root.FileUri;

                static string DefaultOutputPath(string path) => PathHelper.GetDefaultBuildOutputPath(path);
                var paramsOutputPath = PathHelper.ResolveDefaultOutputPath(bicepParamsFilePath, null, compiledFilePath, DefaultOutputPath);

                var model = compilation.GetEntrypointSemanticModel();
                var emitter = new TemplateEmitter(model);

                using var fileStream = new FileStream(compiledFilePath, FileMode.Create, FileAccess.Write);

                var result = new ParametersEmitter(model).Emit(fileStream);
            }

            return "Building parameters file succeeded. Processed file " + compiledFile;
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
