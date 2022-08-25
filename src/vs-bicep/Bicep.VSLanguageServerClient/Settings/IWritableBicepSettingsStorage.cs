// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.VSLanguageServerClient.Settings
{
    /// <summary>
    /// Writable settings storage. Exported via MEF for a particular content type.
    /// Editor uses exported object to store settings such as indentation style, 
    /// tab size, formatting options and so on.
    /// </summary>
    public interface IWritableBicepSettingsStorage : IBicepSettingsStorage
    {
        void ResetSettings();
        void Set<T>(string name, T value);
    }
}
