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
            FunctionFlags.ResoureDecorator |
            FunctionFlags.ModuleDecorator |
            FunctionFlags.OutputDecorator;

        public static bool HasDecoratorFlag(this FunctionFlags functionFlags) => (functionFlags & DecoratorFlags) != 0;
    }
}
