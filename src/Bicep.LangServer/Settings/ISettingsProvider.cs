// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.LanguageServer.Settings
{
    public interface ISettingsProvider
    {
        void AddOrUpdateSetting(string name, bool value);

        bool GetSetting(string name);
    }
}
