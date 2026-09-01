// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Extensions;

namespace Bicep.Core.Registry.Oci;

public class OciManifestAnnotationsBuilder
{
    private Dictionary<string, string> annotations = new();

    public ImmutableDictionary<string, string> Build()
    {
        return annotations.ToImmutableDictionary();
    }

    public OciManifestAnnotationsBuilder WithDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return this;
        }

        return AddAnnotation(OciAnnotationKeys.OciOpenContainerImageDescriptionAnnotation, description);
    }

    public OciManifestAnnotationsBuilder WithDocumentationUri(string? documentationUri)
    {
        if (string.IsNullOrWhiteSpace(documentationUri))
        {
            return this;
        }

        return AddAnnotation(OciAnnotationKeys.OciOpenContainerImageDocumentationAnnotation, documentationUri);
    }

    public OciManifestAnnotationsBuilder WithCreatedTime(DateTime dateTime)
        => AddAnnotation(OciAnnotationKeys.OciOpenContainerImageCreatedAnnotation, dateTime.ToRfc3339Format());

    public OciManifestAnnotationsBuilder WithTitle(string title)
        => AddAnnotation(OciAnnotationKeys.OciOpenContainerImageTitleAnnotation, title);

    public OciManifestAnnotationsBuilder WithBicepSerializationFormatV1()
        => AddAnnotation(OciAnnotationKeys.BicepSerializationFormatAnnotation, "v1");

    public OciManifestAnnotationsBuilder Add(string key, string value) => AddAnnotation(key, value);

    private OciManifestAnnotationsBuilder AddAnnotation(string key, string value)
    {
        annotations[key] = value;
        return this;
    }
}
