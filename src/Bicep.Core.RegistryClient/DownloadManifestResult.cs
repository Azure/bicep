// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;

namespace Bicep.Core.RegistryClient
{
    public class DownloadManifestResult
    {
        public DownloadManifestResult(string digest, Stream content)
        {
            Digest = digest;
            Content = content;
        }

        public string Digest { get; }

        /// <summary>
        /// </summary>
        public Stream Content { get; }
    }
}
