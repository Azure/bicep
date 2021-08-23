// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;

namespace Bicep.Core.CodeAction
{
    public interface IDisableLinterRule
    {
        public string AnalyzerCode { get; }

        public TextSpan LinterRuleSpan { get; }
    }
}
