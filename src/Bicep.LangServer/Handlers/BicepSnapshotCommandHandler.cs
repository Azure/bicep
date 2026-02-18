// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Utils.Snapshots;
using Bicep.IO.Abstraction;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    // This handler is used to generate a snapshot (.snapshot.json) file for a given bicep parameters file.
    // It returns snapshot generation succeeded/failed message, which can be displayed appropriately in IDE output window
    public class BicepSnapshotCommandHandler : ExecuteTypedResponseCommandHandlerBase<DocumentUri, string>
    {
        private readonly ICompilationManager compilationManager;
        private readonly IFileExplorer fileExplorer;
        private readonly BicepCompiler bicepCompiler;

        public BicepSnapshotCommandHandler(ICompilationManager compilationManager, IFileExplorer fileExplorer, BicepCompiler bicepCompiler, ISerializer serializer)
            : base(LangServerConstants.SnapshotCommand, serializer)
        {
            this.compilationManager = compilationManager;
            this.fileExplorer = fileExplorer;
            this.bicepCompiler = bicepCompiler;
        }

        public override async Task<string> Handle(DocumentUri documentUri, CancellationToken cancellationToken)
        {
            string output = await GenerateSnapshotFileAndReturnOutputMessage(documentUri, cancellationToken);

            return output;
        }

        private async Task<string> GenerateSnapshotFileAndReturnOutputMessage(DocumentUri documentUri, CancellationToken cancellationToken)
        {
            var bicepParamFileUri = documentUri.ToIOUri();
            var snapshotFileUri = bicepParamFileUri.WithExtension(".snapshot.json");
            var snapshotFile = this.fileExplorer.GetFile(snapshotFileUri);

            var compilation = await new CompilationHelper(bicepCompiler, compilationManager).GetRefreshedCompilation(documentUri);
            var paramsResult = compilation.Emitter.Parameters();

            if (paramsResult.Success != true || paramsResult.Template?.Template is not { } templateContent || paramsResult.Parameters is not { } parametersContent)
            {
                var diagnosticsByFile = compilation.GetAllDiagnosticsByBicepFile();

                return "Generating snapshot file failed. Please fix below errors:\n" + DiagnosticsHelper.GetDiagnosticsMessage(diagnosticsByFile);
            }

            try
            {
                var snapshot = await SnapshotHelper.GetSnapshot(
                    targetScope: compilation.GetEntrypointSemanticModel().TargetScope,
                    templateContent: templateContent,
                    parametersContent: parametersContent,
                    tenantId: null,
                    subscriptionId: null,
                    resourceGroup: null,
                    location: null,
                    deploymentName: null,
                    cancellationToken: cancellationToken,
                    externalInputs: []);

                if (snapshot.Diagnostics.Length > 0)
{
                    var diagnosticsMessage = string.Join("\n", snapshot.Diagnostics.Select(d => $"  {d}"));
                    return $"Snapshot generation completed with warnings:\n{diagnosticsMessage}\n\nSnapshot file created at {snapshotFileUri}";
                }

                var contents = SnapshotHelper.Serialize(snapshot);
                await snapshotFile.WriteAllTextAsync(contents, cancellationToken);

                return $"Snapshot generation succeeded. Created file {snapshotFileUri}";
            }
            catch (Exception ex)
            {
                return $"Snapshot generation failed: {ex.Message}";
            }
        }
    }
}
