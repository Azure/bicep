// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Globalization;
using Microsoft.Test.Apex.Services;
using Microsoft.VisualStudio;

namespace Bicep.VSLanguageServerClient.IntegrationTests.Utilities
{
    public class CommandUtility
    {
        public static void ExecuteCommand(VSConstants.VSStd2KCmdID commandId, int timeoutInSeconds = 10)
        {
            bool isEnabled = false;
            WaitFor.TryIsTrue(() =>
            {
                return isEnabled = VsHostUtility.VsHost!.ObjectModel.Commanding.QueryStatusCommand(commandId).IsEnabled == true;
            }, TimeSpan.FromSeconds(timeoutInSeconds));

            if (!isEnabled)
            {
                throw new TimeoutException(string.Format(CultureInfo.InvariantCulture, "Timed out after {0} seconds waiting for command with id - {1} to be enabled", timeoutInSeconds, commandId.ToString()));
            }

            VsHostUtility.VsHost!.ObjectModel.Commanding.ExecuteCommand(commandId, null);
        }
    }
}
