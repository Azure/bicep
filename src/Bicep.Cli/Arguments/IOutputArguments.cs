// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments
{
    public interface IOutputArguments<T> where T : IOutputArguments<T>
    {
        abstract static string OutputFileExtension { get; }

        string? OutputDir { get; }

        string? OutputFile { get; }
    }
}
