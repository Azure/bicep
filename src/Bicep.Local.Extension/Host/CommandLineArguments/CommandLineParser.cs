// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using CommandLine;
using Microsoft.Extensions.Logging;


namespace Bicep.Local.Extension.Host.CommandLineArguments;

public class CommandLineParser
{
    private readonly Parser? parser;
    private readonly ILogger<CommandLineParser> logger;
    public CommandLineParser(string[] args, ILogger<CommandLineParser> logger)
    {
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        this.logger = logger;

        parser = new Parser(settings =>
        {
            settings.HelpWriter = Console.Out;
        });

        if (args is null || args.Length == 0)
        {
            parser.ParseArguments<CommandLineOptions>(new[] { "--help" });

            ShouldExit = true;
            ExitCode = 1;
        }
        else
        {
            Result = parser.ParseArguments<CommandLineOptions>(args)
                                 .WithNotParsed(errors =>
                                 {
                                     var x = args;
                                     var isHelpOrVersion = errors.Any(e =>
                                         e.Tag == ErrorType.HelpRequestedError ||
                                         e.Tag == ErrorType.VersionRequestedError);

                                     logger.LogError("Command line parsing failed with errors: {Errors}", errors);
                                     Errors = errors.ToList();
                                     ShouldExit = true;
                                     ExitCode = isHelpOrVersion ? 0 : 1;
                                 })
                                 .WithParsed(options =>
                                 {
                                     Options = options;
                                 });
        }
    }


    public CommandLineOptions? Options { get; private set; }

    public ParserResult<CommandLineOptions>? Result { get; }

    public List<Error>? Errors { get; private set; }

    [MemberNotNullWhen(false, nameof(Options))]
    [MemberNotNullWhen(false, nameof(Result))]
    public bool ShouldExit { get; private set; } = false;

    public int ExitCode { get; private set; } = 0;
}
