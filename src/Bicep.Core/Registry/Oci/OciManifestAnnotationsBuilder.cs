// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace Bicep.Core.Registry.Oci
{
    public class OciManifestAnnotationsBuilder
    {
        private Dictionary<string, string> annotations = new Dictionary<string, string>();

        public ImmutableDictionary<string, string> Build()
        {
            return annotations.ToImmutableDictionary();
        }

        public OciManifestAnnotationsBuilder WithDescription(string? description)
        {
            if (!string.IsNullOrWhiteSpace(description))
            {
                annotations[OciAnnotationKeys.OciOpenContainerImageDescriptionAnnotation] = description;
            }
            return this;
        }

        public OciManifestAnnotationsBuilder WithDocumentationUri(string? documentationUri)
        {
            if (!string.IsNullOrWhiteSpace(documentationUri))
            {
                annotations[OciAnnotationKeys.OciOpenContainerImageDocumentationAnnotation] = documentationUri;
            }
            return this;
        }

        public OciManifestAnnotationsBuilder WithCreatedTime(DateTime dateTime)
        {
            annotations[OciAnnotationKeys.OciOpenContainerImageCreatedAnnotation] = dateTime.ToRfc3339Format();
            return this;
        }

        public OciManifestAnnotationsBuilder WithTitle(string title)
        {
            annotations[OciAnnotationKeys.OciOpenContainerImageTitleAnnotation] = title;
            return this;
        }
    }
}
