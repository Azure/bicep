// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;

namespace Bicep.Core.RegistryClient
{
    public class DownloadManifestResult
    {
        internal DownloadManifestResult(Stream content)
        {
            Content = content;
        }

        /// <summary>
        /// </summary>
        public Stream Content { get; }
    }
}
