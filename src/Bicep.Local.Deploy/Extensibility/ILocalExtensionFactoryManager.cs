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
    public interface ILocalExtensionFactoryManager
    {
        void InitializeLocalExtensions(IReadOnlyList<BinaryExtensionReference> binaryExtensions);
        Task<ILocalExtension> GetLocalExtensionAsync(string providerName, string providerVersion);
    }
}
