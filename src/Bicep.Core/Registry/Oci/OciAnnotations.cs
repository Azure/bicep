// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.Registry.Oci
{
    /// <summary>
    /// Additional information provided through arbitrary metadata.
    /// </summary>
    public class OciAnnotations
    {
        public OciAnnotations(string documentationUri)
        {
            this.DocumentationUri = documentationUri;
        }

        /// <summary>
        ///  URL to get documentation on the image.
        /// </summary>
        public string DocumentationUri { get; }
    }
}
