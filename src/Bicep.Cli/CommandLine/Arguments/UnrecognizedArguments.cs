// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Cli.CommandLine.Arguments
{
    public class UnrecognizedArguments : ArgumentsBase
    {
        public UnrecognizedArguments(string suppliedArguments)
        {
            SuppliedArguments = suppliedArguments;
        }

        public string SuppliedArguments { get; }
    }
}
