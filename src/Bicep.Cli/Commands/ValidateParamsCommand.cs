// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.Commands
{
    public class ValidateParamsCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly CompilationService compilationService;

        public ValidateParamsCommand(
            ILogger logger,
            IDiagnosticLogger diagnosticLogger,
            CompilationService compilationService)
        {
            this.logger = logger;
            this.diagnosticLogger = diagnosticLogger;
            this.compilationService = compilationService;
        }

        public async Task<int> RunAsync(ValidateParamsArguments args)
        {
            var bicepFilePath = PathHelper.ResolvePath(args.BicepFile);

            if (!IsBicepFile(bicepFilePath))
            {
                logger.LogError(CliResources.UnrecognizedBicepFileExtensionMessage, bicepFilePath);
                return 1;
            }
            var paramsInput = Environment.GetEnvironmentVariable("BICEP_PARAMETER_INPUT");

            if(paramsInput is null)
            {
                throw new CommandLineException("No value is set for BICEP_PARAMETER_INPUT environment variable");
            }

            var bicepCompilation = await compilationService.CompileAsync(bicepFilePath, false);

            var parameterDeclarations = bicepCompilation.GetEntrypointSemanticModel().Parameters;

            var parametersJson = JObject.Parse(paramsInput);

            var validationDiagnostics = ToListDiagnosticWriter.Create();

            foreach(var parameter in parametersJson.Properties())
            {   
                //Skip type check if parameter value is null (as those treated as being omitted by ARM Engine)
                if(parameterDeclarations.TryGetValue(parameter.Name, out var parameterMetadata) 
                    && parameter.Value.Type != JTokenType.Null)
                {
                    var declaredType = parameterMetadata.TypeReference.Type;
                    var assignedType = SystemNamespaceType.ConvertJsonToBicepType(parameter.Value);

                    if(!TypeValidator.AreTypesAssignable(assignedType, declaredType))
                    {
                        var diagnostic = DiagnosticBuilder
                                        .ForDocumentStart()
                                        .InvalidParameterValueAssignmentType(parameter.Name, declaredType);
                        validationDiagnostics.Write(diagnostic);
                    }
                }  
                else
                {
                    var diagnostic = DiagnosticBuilder
                                    .ForDocumentStart()
                                    .ParameterNotPresentInTemplate(parameter.Name, bicepFilePath);
                        validationDiagnostics.Write(diagnostic);
                } 
            }

            LogDiagnostics(bicepCompilation.SourceFileGrouping.EntryPoint, validationDiagnostics.GetDiagnostics());

            return diagnosticLogger.ErrorCount > 0 ? 1 : 0;
        }

        private void LogDiagnostics(BicepSourceFile bicepFile, IReadOnlyList<IDiagnostic> diagnostics)
        {
            foreach(var diagnostic in diagnostics)
            {
                diagnosticLogger.LogDiagnostic(bicepFile.FileUri, diagnostic, bicepFile.LineStarts);
            }
        }

        private bool IsBicepFile(string inputPath) => PathHelper.HasBicepExtension(PathHelper.FilePathToFileUrl(inputPath));
    }
}
