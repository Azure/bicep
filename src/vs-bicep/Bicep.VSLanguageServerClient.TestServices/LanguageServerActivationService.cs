// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.ComponentModel.Composition;
using System.Linq;
using Bicep.VSLanguageServerClient.TestServices.Utilitites;
using Microsoft.Test.Apex.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Shell;

namespace Bicep.VSLanguageServerClient.TestServices
{
    // ILanguageServiceBroker2.ActiveLanguageClients is marked obsolete. We will replace it when an alternate option is available.
#pragma warning disable CS0618 // Type or member is obsolete
    [Export(typeof(LanguageServerActivationService))]
    public class LanguageServerActivationService : VisualStudioTestService
    {
        public void WaitForLanguageServerActivation()
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
                        throw new Exception("Did not find any exports for ILanguageServiceBroker2");
                    }

                    var languageServiceBroker = languageServiceBrokerExports.First().Value;

                    WaitForExtensions.IsTrue(
                        () => IsBicepLanguageServerActivated(languageServiceBroker) == true,
                        TimeSpan.FromSeconds(45),
                        conditionDescription: "Bicep language server activation failed.");
                }
            });
        }

        private bool IsBicepLanguageServerActivated(ILanguageServiceBroker2 languageServiceBroker)
        {
            foreach (ILanguageClientInstance languageClientInstance in languageServiceBroker.ActiveLanguageClients)
            {
                if (languageClientInstance.Client.Name.Equals(BicepLanguageServerClientConstants.BicepLanguageServerName))
                {
                    return true;
                }
            }

            return false;
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
