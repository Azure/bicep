// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Configuration;
using Bicep.Core.Emit;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using System.Collections.Immutable;
using System.Text;
using Bicep.Core.Syntax;

namespace Bicep.LanguageServer.Handlers
{
    // This handler is used to force the modules restore for given a bicep file.
    // It returns Restore (force) succeeded/failed message, which can be displayed approriately in IDE output window
    public class BicepForceModulesRestoreCommandHandler : ExecuteTypedResponseCommandHandlerBase<string, string>
    {
        private readonly ICompilationManager compilationManager;
        private readonly EmitterSettings emitterSettings;
        private readonly IFeatureProvider features;
        private readonly IFileResolver fileResolver;
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly INamespaceProvider namespaceProvider;
        private readonly IConfigurationManager configurationManager;

        public BicepForceModulesRestoreCommandHandler(ICompilationManager compilationManager, ISerializer serializer, IFeatureProvider features, EmitterSettings emitterSettings, INamespaceProvider namespaceProvider, IFileResolver fileResolver, IModuleDispatcher moduleDispatcher, IConfigurationManager configurationManager)
            : base(LangServerConstants.ForceModulesRestoreCommand, serializer)
        {
            this.compilationManager = compilationManager;
            this.emitterSettings = emitterSettings;
            this.features = features;
            this.namespaceProvider = namespaceProvider;
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
            this.configurationManager = configurationManager;
        }

        public override Task<string> Handle(string bicepFilePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(bicepFilePath))
            {
                throw new ArgumentException("Invalid input file path");
            }

            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            Task<string> restoreOutput = GenerateForceModulesRestoreOutputMessage(bicepFilePath, documentUri);

            return restoreOutput;
        }

        private async Task<string> GenerateForceModulesRestoreOutputMessage(string bicepFilePath, DocumentUri documentUri)
        {
            var fileUri = documentUri.ToUri();
            RootConfiguration? configuration = null;

            try
            {
                configuration = this.configurationManager.GetConfiguration(fileUri);
            }
            catch (ConfigurationException exception)
            {
                // Fail the restore if there's configuration errors.
                return exception.Message;
            }
            Workspace workspace = new Workspace();
            SourceFileGrouping sourceFileGrouping = SourceFileGroupingBuilder.Build(this.fileResolver, this.moduleDispatcher, workspace, fileUri, configuration);

            // Ignore modules to restore logic, include all modules to be restored
            var modulesToRestore = sourceFileGrouping.UriResultByModule.Keys
                .OfType<ModuleDeclarationSyntax>()
                .ToImmutableHashSet();

            // RestoreModules() does a distinct but we'll do it also to prevent deuplicates in outputs and logging
            var modulesToRestoreReferences = this.moduleDispatcher.GetValidModuleReferences(modulesToRestore, configuration)
                .Distinct()
                .OrderBy(key => key.FullyQualifiedReference);

            if (!modulesToRestoreReferences.Any()) {
                return $"Restore (force) skipped. No modules references in input file.";
            }

            // restore is supposed to only restore the module references that are syntactically valid
            await this.moduleDispatcher.RestoreModules(configuration, modulesToRestoreReferences, forceModulesRestore: true);

            // if all are marked as success
            var sbRestoreSummary = new StringBuilder();
            foreach(var module in modulesToRestoreReferences) {
                var restoreStatus = this.moduleDispatcher.GetModuleRestoreStatus(module, configuration, out _);
                sbRestoreSummary.Append($"{Environment.NewLine}  * {module.FullyQualifiedReference}: {restoreStatus}");
            }

            return $"Restore (force) summary: {sbRestoreSummary}";
        }
    }
}
