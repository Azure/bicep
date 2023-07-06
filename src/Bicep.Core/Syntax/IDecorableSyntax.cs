// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Bicep.Core.Syntax
{
    public interface IDecorableSyntax
    {
        public IEnumerable<DecoratorSyntax> Decorators { get; }
    }
}
