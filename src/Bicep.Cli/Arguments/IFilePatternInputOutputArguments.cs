// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments
{
    public interface IFilePatternInputOutputArguments<T> : IFilePatternArguments, IInputOutputArguments<T>
        where T : IFilePatternInputOutputArguments<T>
    {
    }
}
