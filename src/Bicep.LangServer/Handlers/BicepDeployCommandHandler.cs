// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.Deploy;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
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
        private readonly IDeploymentManager deploymentManager;
        private readonly EmitterSettings emitterSettings;
        private readonly IFileResolver fileResolver;
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly INamespaceProvider namespaceProvider;
        private readonly IConfigurationManager configurationManager;

        public BicepDeployCommandHandler(ICompilationManager compilationManager, IDeploymentManager deploymentManager, ISerializer serializer, EmitterSettings emitterSettings, INamespaceProvider namespaceProvider, IFileResolver fileResolver, IModuleDispatcher moduleDispatcher, IConfigurationManager configurationManager)
            : base(LanguageConstants.Deploy, serializer)
        {
            this.compilationManager = compilationManager;
            this.deploymentManager = deploymentManager;
            this.emitterSettings = emitterSettings;
            this.namespaceProvider = namespaceProvider;
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
            this.configurationManager = configurationManager;
        }

        public override async Task<string> Handle(string bicepFilePath, string parameterFilePath, string id, string scope, string location, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(bicepFilePath))
            {
                throw new ArgumentException("Invalid input file");
            }

            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            string template = string.Empty;

            try
            {
                template = GetCompiledFile(documentUri);
            }
            catch (Exception e)
            {
                return "Deployment failed. " + e.Message;
            }

            string deploymentOutput = await deploymentManager.CreateDeployment(documentUri.ToUri(), template, parameterFilePath, id, scope, location);

            return deploymentOutput;
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
}
