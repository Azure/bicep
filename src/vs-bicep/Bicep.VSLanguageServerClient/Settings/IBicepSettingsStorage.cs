// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.VSLanguageServerClient.Settings
{
    /// <summary>
    /// Bicep settings storage.
    /// </summary>
    public interface IBicepSettingsStorage : IDisposable
    {
        event EventHandler<EventArgs> SettingsChanged;
        T Get<T>(string name, T defaultValue);
    }
}
