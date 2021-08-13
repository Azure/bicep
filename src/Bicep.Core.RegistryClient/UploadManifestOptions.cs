// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.RegistryClient.Models;

namespace Bicep.Core.RegistryClient
{
    public class UploadManifestOptions
    {
        /// <summary>
        /// </summary>
        /// <param name="mediaType"></param>
        public UploadManifestOptions(ContentType mediaType = default, string tag = default)
        {
            // Set default to Docker Manifest V2 format.
            if (mediaType == default)
            {
                mediaType = ContentType.ApplicationVndDockerDistributionManifestV2Json;
            }

            MediaType = mediaType;
            Tag = tag;
        }

        /// <summary>
        /// </summary>
        //public string MediaType { get; set; }
        public ContentType MediaType { get; }

        public string Tag { get; }
    }
}
