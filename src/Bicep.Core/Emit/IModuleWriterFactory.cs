// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;

namespace Bicep.Core.Emit;

public interface IModuleWriterFactory
{
    (WrittenTemplateKind, ITemplateWriter) CreateTemplateWriter(ISemanticModel semanticModel);
}
