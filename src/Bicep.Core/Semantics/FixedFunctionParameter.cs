// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class FixedFunctionParameter
    {
        public FixedFunctionParameter(string name, string description, TypeSymbol type, bool required)
        {
            this.Name = name;
            this.Type = type;
            this.Required = required;
            this.Description = description;
        }

        public string Name { get; }

        public string Description { get; }

        public TypeSymbol Type { get; }

        public bool Required { get; }

        public string Signature => this.Required
            ? $"{this.Name}: {this.Type}"
            : $"[{this.Name}: {this.Type}]";
    }
}
