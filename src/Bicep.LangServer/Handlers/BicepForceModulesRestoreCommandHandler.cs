// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Deployments.Core.Entities;
using Azure.Deployments.Core.Helpers;
using Azure.Deployments.Core.Json;
using Bicep.Core.Analyzers.Linter;
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
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using System.Collections.Immutable;

namespace Bicep.LanguageServer.Handlers
{
    // This handler is used to generate compiled .json file for given a bicep file path.
    // It returns build succeeded/failed message, which can be displayed approriately in IDE output window
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
                throw new ArgumentException("Invalid input file");
            }

            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            Task<string> buildOutput = GenerateForceModulesRestoreOutputMessage(bicepFilePath, documentUri);

            return buildOutput;
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
                // Fail the build if there's configuration errors.
                return exception.Message;
            }
            Workspace workspace = new Workspace();
            SourceFileGrouping sourceFileGrouping = SourceFileGroupingBuilder.Build(this.fileResolver, this.moduleDispatcher, workspace, fileUri, configuration);
            var originalModulesToRestore = sourceFileGrouping.ModulesToRestore;

            // Ignore modules to restore logic, include all modules to be restored, A distinct() is done later in the processing
            var modulesToRestore = sourceFileGrouping.SourceFilesByModuleDeclaration.Select(kvp => kvp.Key).Union(sourceFileGrouping.ModulesToRestore).ToImmutableHashSet();

            // restore is supposed to only restore the module references that are syntactically valid
            bool restoreStatus = await moduleDispatcher.RestoreModules(configuration, moduleDispatcher.GetValidModuleReferences(modulesToRestore, configuration), true);
            
            return $"Force modules restore succeeded. Restored x modules ${restoreStatus}";
        }

        // Returns true if the template contains bicep _generator metadata, false otherwise
        public bool TemplateContainsBicepGeneratorMetadata(string template)
        {
            try
            {
                if (!string.IsNullOrEmpty(template))
                {
                    JToken jtoken = template.FromJson<JToken>();
                    if (TemplateHelpers.TryGetTemplateGeneratorObject(jtoken, out DeploymentTemplateGeneratorMetadata generator))
                    {
                        if (generator.Name == "bicep")
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
