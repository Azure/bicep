// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Workspaces;
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
    // This handler is used to generate compiled parameters.json file for given a bicep file path.
    // It returns generate-params succeeded/failed message, which can be displayed approriately in IDE output window
    public class BicepGenerateParamsCommandHandler : ExecuteTypedResponseCommandHandlerBase<string, string>
    {
        private readonly ICompilationManager compilationManager;
        private readonly BicepCompiler bicepCompiler;

        public BicepGenerateParamsCommandHandler(ICompilationManager compilationManager, BicepCompiler bicepCompiler, ISerializer serializer)
            : base(LangServerConstants.GenerateParamsCommand, serializer)
        {
            this.compilationManager = compilationManager;
            this.bicepCompiler = bicepCompiler;
        }

        public override async Task<string> Handle(string bicepFilePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(bicepFilePath))
            {
                throw new ArgumentException("Invalid input file path");
            }

            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            string output = await GenerateCompiledParametersFileAndReturnOutputMessage(bicepFilePath, documentUri);

            return output;
        }

        private async Task<string> GenerateCompiledParametersFileAndReturnOutputMessage(string bicepFilePath, DocumentUri documentUri)
        {
            string compiledFilePath = PathHelper.ResolveParametersFileOutputPath(bicepFilePath);
            string compiledFile = Path.GetFileName(compiledFilePath);

            // If the template exists and contains bicep generator metadata, we can go ahead and replace the file.
            // If not, we'll fail the generate params.
            if (File.Exists(compiledFilePath) && !TemplateIsParametersFile(File.ReadAllText(compiledFilePath)))
            {
                return "Generating parameters file failed. The file \"" + compiledFile + "\" already exists but does not contain the schema for a parameters file. If overwriting the file is intended, delete it manually and retry the Generate Parameters command.";
            }

            var compilation = await new CompilationHelper(bicepCompiler, compilationManager).GetCompilation(documentUri);
            var fileUri = documentUri.ToUri();

            var diagnosticsByFile = compilation.GetAllDiagnosticsByBicepFile()
                .FirstOrDefault(x => x.Key.FileUri == fileUri);

            if (diagnosticsByFile.Value.Any(x => x.Level == DiagnosticLevel.Error))
            {
                return "Generating parameters file failed. Please fix below errors:\n" + DiagnosticsHelper.GetDiagnosticsMessage(diagnosticsByFile);
            }

            var existingContent = File.Exists(compiledFilePath) ? File.ReadAllText(compiledFilePath) : string.Empty;

            var model = compilation.GetEntrypointSemanticModel();
            var emitter = new TemplateEmitter(model);
            using var fileStream = new FileStream(compiledFilePath, FileMode.Create, FileAccess.Write);
            var result = emitter.EmitEmptyParametersFile(fileStream, existingContent);

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
