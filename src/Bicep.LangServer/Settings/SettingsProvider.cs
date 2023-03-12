// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Settings
{
    /// <summary>
    /// Provider used to cache settings information received from <see cref="ConfigurationSettingsHandler"/>
    /// </summary>
    public class SettingsProvider : ISettingsProvider
    {
        private readonly ConcurrentDictionary<string, bool> SettingsCache = new();

        public void AddOrUpdateSetting(string name, bool value)
        {
            SettingsCache.AddOrUpdate(name, value, (name, prev) => value);
        }

        public bool GetSetting(string name)
        {
            if (SettingsCache.TryGetValue(name, out var value))
            {
                return value;
            }

            return false;
        }
    }
}
