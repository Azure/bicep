// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Extensions
{
    public static class FunctionFlagsExtensions
    {
        public static bool HasAnyDecoratorFlag(this FunctionFlags functionFlags) => (functionFlags & FunctionFlags.AnyDecorator) != 0;

        public static bool HasAllDecoratorFlags(this FunctionFlags functionFlags) => (functionFlags & FunctionFlags.AnyDecorator) == FunctionFlags.AnyDecorator;
    }
}
