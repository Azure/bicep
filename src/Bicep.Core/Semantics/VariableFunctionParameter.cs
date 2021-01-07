// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class VariableFunctionParameter
    {
        public VariableFunctionParameter(string namePrefix, string description, TypeSymbol type, int minimumCount)
        {
            this.NamePrefix = namePrefix;
            this.Type = type;
            this.MinimumCount = minimumCount;
            this.Description = description;
        }

        public string NamePrefix { get; }
        
        public TypeSymbol Type { get; }

        public int MinimumCount { get; }
        
        public string Description { get; }

        public string GetNamedSignature(int index) => $"{this.NamePrefix}{index} : {this.Type}";

        public string GenericSignature => $"... : {this.Type}";
    }
}