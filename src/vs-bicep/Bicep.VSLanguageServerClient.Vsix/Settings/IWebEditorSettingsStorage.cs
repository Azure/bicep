// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.VSLanguageServerClient.Vsix.Settings
{
    /// <summary>
    /// Settings storage. Exported via MEF for a particular content type.
    /// Editor uses exported object to retrieve its settings such as indentation
    /// style, tab size, formatting options and so on.
    /// </summary>
    public interface IWebEditorSettingsStorage
    {
        event EventHandler<EventArgs> SettingsChanged;

        void LoadFromStorage();

        string GetString(string name, string defaultValue);
        int GetInteger(string name, int defaultValue);
        bool GetBoolean(string name, bool defaultValue);
    }
}
