// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Test.Apex.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Shell;

namespace Bicep.VSLanguageServerClient.TestServices
{
#pragma warning disable CS0618 // Type or member is obsolete
    [Export(typeof(LanguageServerActivationService))]
    public class LanguageServerActivationService : VisualStudioTestService
    {
        public void WaitForContentTypeReady()
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var serviceForComponentModel = ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel).GUID);
                if (serviceForComponentModel is IComponentModel componentModel)
                {
                    var languageServiceBrokerExports = componentModel.DefaultExportProvider.GetExports<ILanguageServiceBroker2>();

                    if (languageServiceBrokerExports is null || !languageServiceBrokerExports.Any())
                    {
                        return;
                    }

                    var languageServiceBroker = languageServiceBrokerExports.First().Value;

                    if (languageServiceBroker is null)
                    {
                        return;
                    }

                    foreach (Lazy<ILanguageClient, IContentTypeMetadata> languageClientAndMetadata in languageServiceBroker.LanguageClients)
                    {
                        if (languageClientAndMetadata.Metadata.ContentTypes.Contains(BicepLanguageServerClientConstants.BicepContentType))
                        {
                            var languageClientBrokerExports = componentModel.DefaultExportProvider.GetExports<ILanguageClientBroker>();

                            if (!languageClientBrokerExports.Any())
                            {
                                return;
                            }

                            var languageClientBroker = languageClientBrokerExports.First().Value;

                            await languageClientBroker.LoadAsync((ILanguageClientMetadata)languageClientAndMetadata.Metadata, languageClientAndMetadata.Value);
                            return;
                        }
                    }
                }
            });
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
