// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Configuration;
using System.Runtime.InteropServices;
using System.Text;
using Bicep.Core;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.IO.Abstraction;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Utils;
using MediatR;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public record BicepEditConfigFileParams(
        string BicepFilePath
    );

    public record BicepEditConfigFileResult(
        bool Found,
        string? Error = null
    );

    public class BicepEditConfigFileCommandHandler : ExecuteTypedResponseCommandHandlerBase<BicepEditConfigFileParams, BicepEditConfigFileResult>
    {
        private readonly IConfigurationManager configurationManager;
        private readonly ILanguageServerFacade server;

        public BicepEditConfigFileCommandHandler(
            ISerializer serializer,
            IConfigurationManager configurationManager,
            ILanguageServerFacade server)
        : base(LangServerConstants.EditConfigFileCommand, serializer)
        {
            this.configurationManager = configurationManager;
            this.server = server;
        }

        public override async Task<BicepEditConfigFileResult> Handle(BicepEditConfigFileParams request, CancellationToken cancellationToken)
        {
            var documentUri = new Uri(request.BicepFilePath);
            this.configurationManager.PurgeCache();
            var configuration = configurationManager.GetConfiguration(documentUri);
            IOUri? configFileUri = configuration.ConfigFileUri;
            if (configFileUri is null)
            {
                return new BicepEditConfigFileResult(Found: false);
            }
            else
            {
                var configFilePath = configFileUri.Value.GetLocalFilePath();
                var result = await server.Window.ShowDocument(new()
                {
                    Uri = DocumentUri.File(configFilePath),
                });

                return result.Success ? new BicepEditConfigFileResult(true) : new BicepEditConfigFileResult(true, $"Unable to open configuration file \"{configFilePath}\"");
            }
        }
    }
}
