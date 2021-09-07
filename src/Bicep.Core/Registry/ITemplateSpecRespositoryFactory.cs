// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Azure.Core;

namespace Bicep.Core.Registry
{
    public interface ITemplateSpecRepositoryFactory
    {
        ITemplateSpecRepository CreateRepository(Uri? endpoint, string subscriptionId, TokenCredential? tokenCredential = null);
    }
}
