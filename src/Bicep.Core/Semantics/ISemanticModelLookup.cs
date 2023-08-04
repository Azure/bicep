// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics;

public interface ISemanticModelLookup
{
    public ISemanticModel GetSemanticModel(ISourceFile sourceFile);
}
