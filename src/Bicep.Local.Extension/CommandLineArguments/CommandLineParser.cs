// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Bicep.Local.Extension.CommandLineArguments;


internal class CommandLineParser
{
    private readonly Parser parser;

    public CommandLineParser(string[] args)
    {
        this.parser = new Parser(settings =>
        {
            settings.IgnoreUnknownArguments = true;
        });

        CommandLine = parser.ParseArguments<CommandLineOptions>(args)
                            .WithNotParsed(errors => throw new ArgumentException("Invalid command line arguments", nameof(args)))

        Options = CommandLine.Value;
    }

    public CommandLineOptions Options { get; private set; }

    public ParserResult<CommandLineOptions> CommandLine { get; }
}
