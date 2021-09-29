// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// TODO: remove this file after switching Bicep.Core to .NET 5 (https://github.com/Azure/bicep/issues/4651).
namespace System.Runtime.CompilerServices
{
    // This is required to use record types since the project targets 'netstandard 2.1'.
    // See https://developercommunity.visualstudio.com/t/error-cs0518-predefined-type-systemruntimecompiler/1244809.
    internal static class IsExternalInit { }
}
