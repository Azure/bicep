// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;

namespace Bicep.LanguageServer.Providers
{
    public interface IAccessTokenProvider
    {
        Task CacheAccessTokenAsync(Uri uri);

        Task<AccessToken> GetAccessTokenAsync(Uri uri);
    }
}
