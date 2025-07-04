// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments
{
    public interface IInputOutputArguments<T> : IInputArguments, IOutputArguments<T>
        where T : IInputOutputArguments<T>
    {
    }
}
