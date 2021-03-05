// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Extensions
{
    public static class FunctionFlagsExtensions
    {
        private const FunctionFlags DecoratorFlags =
            FunctionFlags.ParameterDecorator |
            FunctionFlags.VariableDecorator |
            FunctionFlags.ResourceDecorator |
            FunctionFlags.ModuleDecorator |
            FunctionFlags.OutputDecorator;

        public static bool HasAnyDecoratorFlag(this FunctionFlags functionFlags) => (functionFlags & DecoratorFlags) != 0;

        public static bool HasAllDecoratorFlags(this FunctionFlags functionFlags) => (functionFlags & DecoratorFlags) == DecoratorFlags;
    }
}
