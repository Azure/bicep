// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio;

namespace Bicep.VSLanguageServerClient.Settings
{
    public class BicepSettings : IBicepSettings
    {
        /// <summary>
        /// VS language preferences for: tab size, indent size, spaces or tabs.
        /// </summary>
        private LANGPREFERENCES3? LangPrefs;
        private IVsTextManager4? TextManager;

        public async Task LoadTextManagerAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var sVstextManager = ServiceProvider.GlobalProvider.GetService(typeof(SVsTextManager).GUID);

            if (sVstextManager is IVsTextManager4 textManager)
            {
                TextManager = textManager;
            }
        }

        private async Task LoadLanguagePreferencesAsync()
        {
            var langPrefs = new LANGPREFERENCES3[1];
            langPrefs[0].guidLang = new Guid(BicepGuids.LanguageServiceGuidString);

            if (TextManager is not null)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var hr = TextManager.GetUserPreferences4(null, langPrefs, null);
                if (hr == VSConstants.S_OK)
                {
                    LangPrefs = langPrefs[0];
                }
            }
        }

        public async Task<int> GetIntegerAsync(string name, int defaultValue)
        {
            await LoadLanguagePreferencesAsync();

            if (LangPrefs is null)
            {
                return -1;
            }

            int result = defaultValue;

            var langPrefsValue = LangPrefs.Value;

            switch (name)
            {
                case BicepLanguageServerClientConstants.FormatterIndentTypeKey:
                    result = langPrefsValue.fInsertTabs == 1 ? (int)IndentType.Tabs : (int)IndentType.Spaces;
                    break;

                case BicepLanguageServerClientConstants.FormatterTabSizeKey:
                    result = (int)langPrefsValue.uTabSize;
                    break;

                case BicepLanguageServerClientConstants.FormatterIndentSizeKey:
                    result = (int)langPrefsValue.uIndentSize;
                    break;
            }

            return result;
        }
    }
}
