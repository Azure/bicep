// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Semantics;

namespace Bicep.Core.Emit;

public class InlineModuleWriterFactory : IModuleWriterFactory
{
    public (WrittenTemplateKind, ITemplateWriter) CreateTemplateWriter(ISemanticModel semanticModel)
         => semanticModel switch
         {
             SemanticModel bicepModel => (WrittenTemplateKind.Template, new TemplateWriter(bicepModel)),
             ArmTemplateSemanticModel armTemplateModel
                => (WrittenTemplateKind.Template, new ArmTemplateWriter(armTemplateModel)),
             TemplateSpecSemanticModel templateSpecModel
                => (WrittenTemplateKind.TemplateLink, new TemplateSpecWriter(templateSpecModel)),
             _ => throw new ArgumentException($"Unknown semantic model type: \"{semanticModel.GetType()}\"."),
         };
}
