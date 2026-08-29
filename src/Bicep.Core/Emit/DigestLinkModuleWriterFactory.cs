// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;

namespace Bicep.Core.Emit;

internal class DigestLinkModuleWriterFactory : IModuleWriterFactory
{
    private readonly IReadOnlyDictionary<ISemanticModel, string> digestsByModel;

    public DigestLinkModuleWriterFactory(IReadOnlyDictionary<ISemanticModel, string> digestsByModel)
    {
        this.digestsByModel = digestsByModel;
    }

    public (WrittenTemplateKind, ITemplateWriter) CreateTemplateWriter(ISemanticModel model) => model switch
    {
        TemplateSpecSemanticModel templateSpecModel
            => (WrittenTemplateKind.TemplateLink, new TemplateSpecWriter(templateSpecModel)),
        _ => (WrittenTemplateKind.TemplateLink, new DigestLinkedTemplateWriter(digestsByModel[model])),
    };
}
