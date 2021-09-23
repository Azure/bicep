// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.RegistryClient.Models;

namespace Bicep.Core.RegistryClient
{
    public class DownloadManifestOptions
    {
        public DownloadManifestOptions(ContentType mediaType = default)
        {
            // Set default to Docker Manifest V2 format.
            if (mediaType == default)
            {
                mediaType = ContentType.ApplicationVndDockerDistributionManifestV2Json;
            }

            MediaType = mediaType;
        }

        public ContentType MediaType { get; }
    }
}
