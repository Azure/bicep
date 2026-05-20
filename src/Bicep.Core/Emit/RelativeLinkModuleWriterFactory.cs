// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Semantics;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Emit;

internal class RelativeLinkModuleWriterFactory : IModuleWriterFactory
{
    private readonly IOUri parentModelUri;
    private readonly IReadOnlyDictionary<ISemanticModel, IOUri> urisByModel;

    public RelativeLinkModuleWriterFactory(
        IOUri parentModelUri,
        IReadOnlyDictionary<ISemanticModel, IOUri> urisByModel)
    {
        this.parentModelUri = parentModelUri;
        this.urisByModel = urisByModel;
    }

    public (WrittenTemplateKind, ITemplateWriter) CreateTemplateWriter(ISemanticModel model) => model switch
    {
        TemplateSpecSemanticModel templateSpecModel
            => (WrittenTemplateKind.TemplateLink, new TemplateSpecWriter(templateSpecModel)),
        _ => (WrittenTemplateKind.TemplateLink, new RelativelyLinkedTemplateWriter(urisByModel[model], parentModelUri)),
    };
}
