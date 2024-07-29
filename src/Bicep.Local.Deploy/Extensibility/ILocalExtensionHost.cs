// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Local.Deploy.Extensibility
{
    public interface ILocalExtensionHost
    {
        Task<HttpResponseMessage> CallExtensibilityHost(LocalDeploymentEngineHost.ExtensionInfo extensionInfo, HttpContent content, CancellationToken cancellationToken);
    }
}
