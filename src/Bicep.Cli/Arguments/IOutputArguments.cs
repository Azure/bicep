// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Cli.Arguments
{
    public interface IOutputArguments<T> where T : IOutputArguments<T>
    {
        abstract static Func<T, IOUri, string> OutputFileExtensionResolver { get; }

        string? OutputDir { get; }

        string? OutputFile { get; }
    }
}
