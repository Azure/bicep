// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Core;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepDeployCommandHandler : ExecuteTypedResponseCommandHandlerBase<string, string, string, string, string, string>
    {
        private readonly ICompilationManager compilationManager;
        private readonly EmitterSettings emitterSettings;
        private readonly IFileResolver fileResolver;
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly INamespaceProvider namespaceProvider;
        private readonly IConfigurationManager configurationManager;
        private readonly ITokenCredentialFactory credentialFactory;

        public BicepDeployCommandHandler(ICompilationManager compilationManager, ISerializer serializer, EmitterSettings emitterSettings, INamespaceProvider namespaceProvider, IFileResolver fileResolver, IModuleDispatcher moduleDispatcher, IConfigurationManager configurationManager, ITokenCredentialFactory credentialFactory)
            : base(LanguageConstants.Deploy, serializer)
        {
            this.compilationManager = compilationManager;
            this.emitterSettings = emitterSettings;
            this.namespaceProvider = namespaceProvider;
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
            this.configurationManager = configurationManager;
            this.credentialFactory = credentialFactory;
        }

        public override async Task<string> Handle(string bicepFilePath, string parameterFilePath, string id, string scope, string location, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(bicepFilePath))
            {
                throw new ArgumentException("Invalid input file");
            }
            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var configuration = configurationManager.GetConfiguration(documentUri.ToUri());
            var credential = this.credentialFactory.CreateChain(ImmutableArray.Create(CredentialType.VisualStudioCode), configuration.Cloud.ActiveDirectoryAuthorityUri);

            ArmClient armClient = new ArmClient(credential);
            DeploymentCollection? deploymentCollection = null;
            var resourceIdentifier = new ResourceIdentifier(id);

            if (scope == DeploymentScope.ResourceGroup)
            {
                var resourceGroup = armClient.GetResourceGroup(resourceIdentifier);
                deploymentCollection = resourceGroup.GetDeployments();
            }
            else if (scope == DeploymentScope.Subscription)
            {
                var subscription = armClient.GetSubscription(resourceIdentifier);
                deploymentCollection = subscription.GetDeployments();
            }
            else if (scope == DeploymentScope.ManagementGroup)
            {
                var managementGroup = armClient.GetManagementGroup(resourceIdentifier);
                deploymentCollection = managementGroup.GetDeployments();
            }

            if (deploymentCollection is not null)
            {
                string template = string.Empty;

                try
                {
                    template = GetCompiledFile(documentUri);
                }
                catch (Exception e)
                {
                    return "Deployment failed. " + e.Message;
                }

                JsonElement parameters;

                if (string.IsNullOrWhiteSpace(parameterFilePath))
                {
                    parameters = JsonDocument.Parse("{}").RootElement;
                }
                else
                {
                    string text = File.ReadAllText(parameterFilePath);
                    parameters = JsonDocument.Parse(text).RootElement;
                }

                var input = new DeploymentInput(new DeploymentProperties(DeploymentMode.Incremental)
                {
                    Template = JsonDocument.Parse(template).RootElement,
                    Parameters = parameters
                });

                if (scope == DeploymentScope.Subscription || scope == DeploymentScope.ManagementGroup)
                {
                    if (location is null)
                    {
                        return "Deployment failed. Location was not provided";
                    }
                    input.Location = location;
                }

                string deployment = "deployment_" + DateTime.UtcNow.ToString("yyyyMMddHmmffff");

                try
                {
                    var deploymentCreateOrUpdateAtScopeOperation = await deploymentCollection.CreateOrUpdateAsync(deployment, input);

                    if (deploymentCreateOrUpdateAtScopeOperation.HasValue &&
                        deploymentCreateOrUpdateAtScopeOperation.GetRawResponse().Status == 200)
                    {
                        return "Deployment successful.";
                    }
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }

            return "Deployment failed.";
        }

        private string GetCompiledFile(DocumentUri documentUri)
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

            CompilationContext? context = compilationManager.GetCompilation(fileUri);
            Compilation compilation;

            if (context is null)
            {
                SourceFileGrouping sourceFileGrouping = SourceFileGroupingBuilder.Build(this.fileResolver, this.moduleDispatcher, new Workspace(), fileUri, configuration);
                compilation = new Compilation(namespaceProvider, sourceFileGrouping, configuration, new LinterAnalyzer(configuration));
            }
            else
            {
                compilation = context.Compilation;
            }

            KeyValuePair<BicepFile, IEnumerable<IDiagnostic>> diagnosticsByFile = compilation.GetAllDiagnosticsByBicepFile()
                .FirstOrDefault(x => x.Key.FileUri == fileUri);

            if (diagnosticsByFile.Value.Any(x => x.Level == DiagnosticLevel.Error))
            {
                throw new Exception(DiagnosticsHelper.GetDiagnosticsMessage(diagnosticsByFile));
            }

            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);

            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel(), emitterSettings);
            emitter.Emit(stringWriter);

            return stringBuilder.ToString();
        }
    }

    public static class DeploymentScope
    {
        public const string ManagementGroup = nameof(ManagementGroup);
        public const string ResourceGroup = nameof(ResourceGroup);
        public const string Subscription = nameof(Subscription);
        public const string Tenant = nameof(Tenant);
    }
}
