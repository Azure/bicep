// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.VSLanguageServerClient.Settings
{
    public class BicepSettings
    {
        private readonly BicepSettingsStorage bicepSettingsStorage;
        public event EventHandler<EventArgs>? Changed;

        public BicepSettings()
        {
            bicepSettingsStorage = new BicepSettingsStorage();
            bicepSettingsStorage.LoadFromStorage();

            bicepSettingsStorage.SettingsChanged += OnStorageSettingsChanged;
        }

        private void OnStorageSettingsChanged(object sender, EventArgs e)
        {
            Changed?.Invoke(null, EventArgs.Empty);
        }

        internal void ResetSettings()
        {
            bicepSettingsStorage.ResetSettings();
        }

        public int TabSize
        {
            get => bicepSettingsStorage.GetInteger("FormatterTabSize", 2);

            internal set
            {
                bicepSettingsStorage.SetInteger("FormatterTabSize", value);
            }
        }

        public int IndentSize
        {
            get => bicepSettingsStorage.GetInteger("FormatterIndentSize", 2);

            set
            {
                bicepSettingsStorage.SetInteger("FormatterIndentSize", value);
            }
        }
    }
}
