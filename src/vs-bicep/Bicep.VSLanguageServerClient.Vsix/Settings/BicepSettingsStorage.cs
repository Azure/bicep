// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace Bicep.VSLanguageServerClient.Vsix.Settings
{
    [Export(typeof(IWritableWebEditorSettingsStorage))]
    [ContentType(BicepLanguageServerClientConstants.BicepContentType)]
    [Name("Bicep Editor Settings")]
    [Order(Before = "Default")]
    public class BicepSettingsStorage : IVsTextManagerEvents4, IWritableWebEditorSettingsStorage
    {
        public event EventHandler<EventArgs>? SettingsChanged;

        private readonly Dictionary<string, bool> _booleanSettings = new Dictionary<string, bool>();
        private readonly Dictionary<string, string> _stringSettings = new Dictionary<string, string>();
        private readonly Guid _languageServiceId = new Guid(BicepGuidList.LanguageServiceGuidString);

        public void ResetSettings()
        {
            _booleanSettings.Clear();
            _stringSettings.Clear();

            FireSettingsChanged();
        }

        public string GetString(string name, string defaultValue)
        {
            if (_stringSettings.TryGetValue(name, out string value))
            {
                return value;
            }

            return defaultValue;
        }

        public void SetString(string name, string value)
        {
            // Not allowed to save null strings
            value ??= string.Empty;

            if (!_stringSettings.ContainsKey(name) || !value.Equals(_stringSettings[name], StringComparison.Ordinal))
            {
                _stringSettings[name] = value;

                FireSettingsChanged();
            }
        }

        /// <summary>
        /// Loads the yaml settings from storage when VS is opened.
        /// We need this so that validation settings can be applied to any open .yaml files first
        /// time VS is opened and user hasn't accessed the dialog under Tools -> Options ->Text Editor -> Yaml
        /// </summary>
        public void LoadFromStorage()
        {
            _ = LoadLanguagePreferences();
        }

        public bool GetBoolean(string name, bool defaultValue)
        {
            if (_booleanSettings.TryGetValue(name, out bool value))
            {
                return value;
            }

            return defaultValue;
        }

        public void SetBoolean(string name, bool value)
        {
            if (!_booleanSettings.ContainsKey(name) || value != _booleanSettings[name])
            {
                _booleanSettings[name] = value;

                FireSettingsChanged();
            }
        }

        private void FireSettingsChanged()
        {
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        public int GetInteger(string name, int defaultValue)
        {
            throw new NotImplementedException();
        }

        public void SetInteger(string name, int value)
        {
        }

        public void BeginBatchChange()
        {
        }

        public void EndBatchChange()
        {
        }

        private LANGPREFERENCES3? _langPrefs;
        /// <summary>
        /// VS language preferences: tab size, indent size, spaces or tabs.
        /// </summary>
        //private LANGPREFERENCES3 LangPrefs => _langPrefs.Value;

        #region IVsTextManagerEvents4
        public int OnUserPreferencesChanged4(VIEWPREFERENCES3[] viewPrefs, LANGPREFERENCES3[] langPrefs, FONTCOLORPREFERENCES2[] colorPrefs)
        {
            if (langPrefs != null && langPrefs[0].guidLang == _languageServiceId)
            {
                _langPrefs = langPrefs[0];
                SettingsChanged?.Invoke(this, EventArgs.Empty);
            }

            return VSConstants.S_OK;
        }
        #endregion

        private async Task LoadLanguagePreferences()
        {
            var langPrefs = new LANGPREFERENCES3[1];
            langPrefs[0].guidLang = _languageServiceId;

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var sVstextManager = ServiceProvider.GlobalProvider.GetService(typeof(SVsTextManager).GUID);

            if (sVstextManager is IVsTextManager4 textManager)
            {
                var hr = textManager.GetUserPreferences4(null, langPrefs, null);
                if (hr == VSConstants.S_OK)
                {
                    _langPrefs = langPrefs[0];
                }
            }
        }
    }
}
