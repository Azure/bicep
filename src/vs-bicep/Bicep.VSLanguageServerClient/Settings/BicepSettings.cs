// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;

namespace Bicep.VSLanguageServerClient.Settings
{
    public class BicepSettings
    {
        public event EventHandler<EventArgs>? Changed;

        private IWritableBicepSettingsStorage? WritableStorage => Storage as IWritableBicepSettingsStorage;

        private IBicepSettingsStorage? Storage
        {
            get
            {
                var serviceForComponentModel = ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel).GUID);
                if (serviceForComponentModel is IComponentModel componentModel)
                {
                    var writableBicepSettingsStorages = componentModel.DefaultExportProvider.GetExports<IWritableBicepSettingsStorage>();

                    if (writableBicepSettingsStorages is not null && writableBicepSettingsStorages.Any())
                    {
                        var storage = writableBicepSettingsStorages.First().Value;
                        storage.SettingsChanged += OnStorageSettingsChanged;
                    }
                }

                return null;
            }
        }

        private void OnStorageSettingsChanged(object sender, EventArgs e)
        {
            Changed?.Invoke(null, EventArgs.Empty);
        }

        public virtual void ResetSettings() => WritableStorage?.ResetSettings();

        public int? IndentSize
        {
            get => Storage?.Get(SettingsConstants.FormatterIndentSizeKey, 2);
            set => WritableStorage?.Set(SettingsConstants.FormatterIndentSizeKey, value);
        }

        public IndentType? IndentType
        {
            get => Storage?.Get(SettingsConstants.FormatterIndentTypeKey, Settings.IndentType.Spaces);
            set => WritableStorage?.Set(SettingsConstants.FormatterIndentTypeKey, value);
        }

        public int? TabSize
        {
            get => Storage?.Get(SettingsConstants.FormatterTabSizeKey, 2);
            set => WritableStorage?.Set(SettingsConstants.FormatterTabSizeKey, value);
        }
    }
}
