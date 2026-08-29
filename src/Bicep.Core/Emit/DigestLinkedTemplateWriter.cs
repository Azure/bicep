// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Emit;

/// <summary>
/// Writes a nested deployment link that points at another layer of the same OCI artifact.
/// </summary>
internal class DigestLinkedTemplateWriter : ITemplateWriter
{
    private readonly string digest;

    public DigestLinkedTemplateWriter(string digest)
    {
        this.digest = digest;
    }

    public void Write(SourceAwareJsonTextWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("digest");
        writer.WriteValue(digest);
        writer.WriteEndObject();
    }
}
