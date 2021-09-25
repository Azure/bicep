// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;

namespace Bicep.Core.RegistryClient
{
    public class DownloadBlobResult
    {
        /// <summary>
        /// </summary>
        /// <param name="digest"></param>
        /// <param name="content"></param>
        public DownloadBlobResult(string digest, Stream content)
        {
            Digest = digest;
            Content = content;
        }

        /// <summary>
        /// </summary>
        public string Digest { get; }

        /// <summary>
        /// </summary>
        public Stream Content { get; }
    }
}
