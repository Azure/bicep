// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Bicep.VSLanguageServerClient.Settings;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace Bicep.VSLanguageServerClient.Vsix.Settings
{
    [Export(typeof(IWritableBicepSettingsStorage))]
    [ContentType(BicepLanguageServerClientConstants.BicepContentType)]
    [Order(Before = "Default")]
    public class BicepSettingsStorage : IVsTextManagerEvents4, IWritableBicepSettingsStorage
    {
        private readonly Dictionary<string, int> _integerSettings = new Dictionary<string, int>();
        private readonly Guid _languageServiceId;
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        private IVsTextManager4? _textManager;
        public event EventHandler<EventArgs>? SettingsChanged;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        public BicepSettingsStorage(Guid languageServiceId)
        {
            _languageServiceId = languageServiceId;
        }

        public void ResetSettings()
        {
            _integerSettings.Clear();

            FireSettingsChanged();
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

        private void FireSettingsChanged()
        {
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        private LANGPREFERENCES3? _langPrefs;
        /// <summary>
        /// VS language preferences: tab size, indent size, spaces or tabs.
        /// </summary>
        private LANGPREFERENCES3? LangPrefs => _langPrefs;

        private void SetLangPrefs(LANGPREFERENCES3 newPreferences) => _textManager?.SetUserPreferences4(null, new[] { newPreferences }, null);

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
                _textManager = textManager;
                var hr = textManager.GetUserPreferences4(null, langPrefs, null);
                if (hr == VSConstants.S_OK)
                {
                    _langPrefs = langPrefs[0];
                }
            }
        }

        public T Get<T>(string name, T defaultValue)
        {
            if (defaultValue is int)
            {
                return (T)(object)GetInteger(name, Convert.ToInt32(defaultValue));
            }

            return defaultValue;
        }


        public int GetInteger(string name, int defaultValue)
        {
            if (LangPrefs is null)
            {
                return -1;
            }

            var langPrefsValue = LangPrefs.Value;

            switch (name)
            {
                case SettingsConstants.FormatterIndentSizeKey:
                    return (int)langPrefsValue.uIndentSize;

                case SettingsConstants.FormatterIndentTypeKey:
                    if (langPrefsValue.fInsertTabs != 0 && langPrefsValue.uTabSize != 0 && langPrefsValue.uIndentSize % langPrefsValue.uTabSize == 0)
                    {
                        return (int)IndentType.Tabs;
                    }
                    return (int)IndentType.Spaces;

                case SettingsConstants.FormatterTabSizeKey:
                    return (int)langPrefsValue.uTabSize;
            }

            int value;
            return _integerSettings.TryGetValue(name, out value) ? value : defaultValue;
        }

        public void Set<T>(string name, T value)
        {
            if (value is int)
            {
                SetInteger(name, Convert.ToInt32(value));
            }
            else
            {
                Debug.Fail("Unknown editor setting type");
            }
        }

        public void SetInteger(string name, int value)
        {
            if (LangPrefs is null)
            {
                return;
            }

            LANGPREFERENCES3 langPrefs = LangPrefs.Value;

            switch (name)
            {
                case SettingsConstants.FormatterIndentSizeKey:
                    if (langPrefs.uIndentSize != (uint)value)
                    {
                        langPrefs.uIndentSize = (uint)value;
                        SetLangPrefs(langPrefs);
                    }
                    break;

                case SettingsConstants.FormatterIndentTypeKey:
                    if (langPrefs.fInsertTabs != (uint)value)
                    {
                        langPrefs.fInsertTabs = (uint)value;
                        SetLangPrefs(langPrefs);
                    }
                    break;

                case SettingsConstants.FormatterTabSizeKey:
                    if (langPrefs.uTabSize != (uint)value)
                    {
                        langPrefs.uTabSize = (uint)value;
                        SetLangPrefs(langPrefs);
                    }
                    break;

                default:
                    if (!_integerSettings.ContainsKey(name) || value != _integerSettings[name])
                    {
                        _integerSettings[name] = value;
                        SettingsChanged?.Invoke(this, EventArgs.Empty);
                    }
                    break;
            }
        }
    }

}
