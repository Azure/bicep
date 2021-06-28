// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Cli.Arguments
{
    public interface ICoreArguments
    {
        public bool OutputToStdOut { get; }
        
        public string InputFile { get; }

        public string? OutputDir { get; }

        public string? OutputFile { get; }

        public string CommandName { get; }
    }
}
