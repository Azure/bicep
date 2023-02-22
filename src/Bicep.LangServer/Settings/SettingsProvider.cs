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
    public class SettingsProvider : ISettingsProvider
    {
        private readonly ConcurrentDictionary<string, bool> SettingsCache = new();

        public void AddOrUpdateSetting(string name, bool value)
        {
            SettingsCache.AddOrUpdate(name, name => value, (name, value) => value);
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
