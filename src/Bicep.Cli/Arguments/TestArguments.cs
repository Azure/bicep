// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Helpers;

namespace Bicep.Cli.Arguments
{
    public class TestArguments : ArgumentsBase, IInputArguments
    {
        public TestArguments(string[] args) : base(Constants.Command.Test)
        {
            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
                {
                    case "--no-restore":
                        NoRestore = true;
                        break;

                    case ArgumentConstants.DiagnosticsFormat:
                        ArgumentHelper.ValidateNotAlreadySet(ArgumentConstants.DiagnosticsFormat, DiagnosticsFormat);
                        DiagnosticsFormat = ArgumentHelper.ToDiagnosticsFormat(ArgumentHelper.GetValueWithValidation(ArgumentConstants.DiagnosticsFormat, args, i));
                        i++;
                        break;

                    default:
                        if (args[i].StartsWith("--"))
                        {
                            throw new CommandLineException($"Unrecognized parameter \"{args[i]}\"");
                        }
                        if (InputFile is not null)
                        {
                            throw new CommandLineException($"The input file path cannot be specified multiple times");
                        }
                        InputFile = args[i];
                        break;
                }
            }

            if (InputFile is null)
            {
                throw new CommandLineException($"The input file path was not specified");
            }

            if (DiagnosticsFormat is null)
            {
                DiagnosticsFormat = Arguments.DiagnosticsFormat.Default;
            }
        }

        public string InputFile { get; }

        public DiagnosticsFormat? DiagnosticsFormat { get; }

        public bool NoRestore { get; }
    }
}
