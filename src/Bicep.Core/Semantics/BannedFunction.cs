// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;

namespace Bicep.Core.Semantics
{
    public class BannedFunction
    {
        private readonly DiagnosticBuilder.ErrorBuilderDelegate errorFunc;

        public BannedFunction(string name, DiagnosticBuilder.ErrorBuilderDelegate errorFunc)
        {
            this.Name = name;
            this.errorFunc = errorFunc;
        }

        public string Name { get; }

        public ErrorSymbol CreateSymbol(DiagnosticBuilder.DiagnosticBuilderInternal builder) => new ErrorSymbol(this.errorFunc(builder));

        public static BannedFunction CreateForOperator(string name, string @operator) => 
            new BannedFunction(name, builder => builder.FunctionNotSupportedOperatorAvailable(name, @operator));
    }
}
