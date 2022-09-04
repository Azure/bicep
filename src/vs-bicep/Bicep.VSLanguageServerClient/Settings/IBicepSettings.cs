// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.VSLanguageServerClient.Settings
{
    public interface IBicepSettings
    {
        Task LoadTextManagerAsync();

        Task<int> GetIntegerAsync(string name, int defaultValue);
    }
}
