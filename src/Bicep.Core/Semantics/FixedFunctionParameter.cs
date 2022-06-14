// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class FixedFunctionParameter
    {
        public FixedFunctionParameter(string name, string description, TypeSymbol type, FunctionParameterFlags flags)
        {
            this.Name = name;
            this.Type = type;
            this.Flags = flags;
            this.Description = description;
        }

        public string Name { get; }

        public string Description { get; }

        public TypeSymbol Type { get; }

        public FunctionParameterFlags Flags { get; }

        public bool Required => Flags.HasFlag(FunctionParameterFlags.Required);

        public string Signature => this.Required
            ? $"{this.Name}: {this.Type}"
            : $"[{this.Name}: {this.Type}]";
    }

    public enum FunctionParameterFlags
    {
        Default = 0,
        Required = 1 << 0,
        FilePath = 1 << 1,
        ExpectedJsonFile = 1 << 2,
    }
}
