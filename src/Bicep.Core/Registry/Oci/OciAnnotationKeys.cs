// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry.Oci
{
    public static class OciAnnotationKeys
    {
        // From https://github.com/opencontainers/image-spec/blob/main/annotations.md

        // URL to get documentation on the image (string)
        public const string OciOpenContainerImageDocumentationAnnotation = "org.opencontainers.image.documentation";
        // Human-readable description of the software packaged in the image (string)
        public const string OciOpenContainerImageDescriptionAnnotation = "org.opencontainers.image.description";
        // date and time on which the image was built, conforming to RFC 3339.
        public const string OciOpenContainerImageCreatedAnnotation = "org.opencontainers.image.created";
        // Human-readable title of the image (string)
        public const string OciOpenContainerImageTitleAnnotation = "org.opencontainers.image.title";

        public const string BicepSerializationFormatAnnotation = "bicep.serialization.format";

        public const string DeploymentsEntryPointAnnotation = "ms.azure.deployments.entryPoint";
    }
}

