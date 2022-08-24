// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Test.Apex.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Shell;

namespace Bicep.VSLanguageServerClient.TestServices
{
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

                    if (languageServiceBrokerExports is not null && languageServiceBrokerExports.Any())
                    {
                        var languageServiceBroker = languageServiceBrokerExports.First().Value;

                        if (languageServiceBroker is not null)
                        {
                            List<Lazy<ILanguageClient>> relevantLanguageClients = new();

#pragma warning disable CS0618 // Type or member is obsolete
                            foreach (Lazy<ILanguageClient, IContentTypeMetadata> languageClientAndMetadata in languageServiceBroker.LanguageClients)
                            {
                                if (languageClientAndMetadata.Metadata.ContentTypes.Contains(BicepLanguageServerClientConstants.BicepContentType))
                                {
                                    relevantLanguageClients.Add(languageClientAndMetadata);
                                }
                            }
#pragma warning restore CS0618 // Type or member is obsolete

                            TaskCompletionSource<bool> tcs = new();

                            // Wait for all language clients of interest to have been created.
                            if (relevantLanguageClients.Any(lc => !lc.IsValueCreated))
                            {
#pragma warning disable CS0618 // Type or member is obsolete
                                languageServiceBroker.LanguageClientLoaded += OnLanguageClientLoaded;
#pragma warning restore CS0618 // Type or member is obsolete

                                await tcs.Task;
                            }

                            var languageClientBrokerExports = componentModel.DefaultExportProvider.GetExports<ILanguageClientBroker>();
                            var languageClientBroker = languageClientBrokerExports.First().Value;

                            // They are all now created, but may not have completed initialization. I can't find a good way to detect this
                            //   other than using LoadAsync completion.
                            foreach (Lazy<ILanguageClient, IContentTypeMetadata> languageClientAndMetadata in relevantLanguageClients)
                            {
                                await languageClientBroker.LoadAsync((ILanguageClientMetadata)languageClientAndMetadata.Metadata, languageClientAndMetadata.Value);
                            }

                            void OnLanguageClientLoaded(object sender, LanguageClientLoadedEventArgs e)
                            {
                                if (relevantLanguageClients.All(lc => lc.IsValueCreated))
                                {
#pragma warning disable CS0618 // Type or member is obsolete
                                    languageServiceBroker.LanguageClientLoaded -= OnLanguageClientLoaded;
#pragma warning restore CS0618 // Type or member is obsolete
                                    tcs.SetResult(true);
                                }
                            }
                        }
                    }
                }
            });
        }
    }
}
