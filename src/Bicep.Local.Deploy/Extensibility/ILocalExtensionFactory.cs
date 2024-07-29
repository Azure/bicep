// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Local.Extension.Protocol;

namespace Bicep.Local.Deploy.Extensibility
{
    public interface ILocalExtensionFactory
    {
        Task<ILocalExtension> CreateLocalExtensionAsync(LocalExtensionKey extensionKey, Uri extensionBinaryUri);
    }
}
