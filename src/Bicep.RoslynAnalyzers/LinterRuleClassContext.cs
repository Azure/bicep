// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bicep.RoslynAnalyzers
{
    public class LinterRuleClassContext
    {
        public INamedTypeSymbol RuleClassSymbol { get; private set; }

        public LinterRuleClassContext(INamedTypeSymbol namedTypeSymbol)
        {
            this.RuleClassSymbol = namedTypeSymbol;
        }
    }
}
