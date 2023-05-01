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
                    // Instead of using this obsolete interface, either look for semantic tokens in our output log or see if there's
                    //   another way to tell lif a language server is active.
                    // Tracked here: https://github.com/Azure/bicep/issues/8078

                    //var languageServiceBrokerExports = componentModel.DefaultExportProvider.GetExports<ILanguageServiceBroker2>();
                    //if (languageServiceBrokerExports is null || !languageServiceBrokerExports.Any())
                    //{
                    //    throw new Exception("Did not find any exports for ILanguageServiceBroker2");
                    //}
                    //var languageServiceBroker = languageServiceBrokerExports.First().Value;

                    WaitForExtensions.IsTrue(
                        () => IsBicepLanguageServerActivated(/*languageServiceBroker*/) == true,
                        TimeSpan.FromSeconds(120), //TimeSpan.FromSeconds(45),
                        conditionDescription: "Bicep language server activation failed or took too long.");
                }
            });
        }

        private bool IsBicepLanguageServerActivated(/*ILanguageServiceBroker2 languageServiceBroker*/)
        {
            //foreach (ILanguageClientInstance languageClientInstance in languageServiceBroker.ActiveLanguageClients)
            //{
            //    if (languageClientInstance.Client.Name.Equals(BicepLanguageServerClientConstants.BicepLanguageServerName))
            //    {
            //        return true;
            //    }
            //}

            return false;
        }
    }
}
