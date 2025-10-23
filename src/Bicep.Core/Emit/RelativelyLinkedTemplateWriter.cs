// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Core.Emit;

internal class RelativelyLinkedTemplateWriter : ITemplateWriter
{
    private readonly IOUri templateUri;
    private readonly IOUri parentTemplateUri;

    public RelativelyLinkedTemplateWriter(IOUri templateUri, IOUri parentTemplateUri)
    {
        this.templateUri = templateUri;
        this.parentTemplateUri = parentTemplateUri;
    }

    public void Write(SourceAwareJsonTextWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("relativePath");
        writer.WriteValue(templateUri.GetPathRelativeTo(parentTemplateUri));
        writer.WriteEndObject();
    }
}
