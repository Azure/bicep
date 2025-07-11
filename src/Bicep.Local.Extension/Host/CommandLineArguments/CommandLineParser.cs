// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using CommandLine;

namespace Bicep.Local.Extension.Host.CommandLineArguments;

public class CommandLineParser
{
    private readonly Parser? parser;

    public CommandLineParser(string[] args)
    {
        if (args is null || args.Length == 0)
        {
            ShowHelpAndExit();
        }
        else
        {
            parser = new Parser(settings =>
            {
                settings.IgnoreUnknownArguments = true;
                settings.HelpWriter = Console.Error;
            });

            ParserResult = parser.ParseArguments<CommandLineOptions>(args)
                                 .WithNotParsed(errors =>
                                 {
                                     var x = args;
                                     var isHelpOrVersion = errors.Any(e =>
                                         e.Tag == ErrorType.HelpRequestedError ||
                                         e.Tag == ErrorType.VersionRequestedError);

                                     ShouldExit = true;
                                     ExitCode = isHelpOrVersion ? 0 : 1;
                                 })
                                 .WithParsed(options =>
                                 {
                                     // add code to validate the options, or add validate method to options that is called here to
                                     // ensure behavior is correct
                                 });

            Options = ParserResult.Value;
        }
    }

    
    public CommandLineOptions? Options { get; private set; }

    public ParserResult<CommandLineOptions>? ParserResult { get; }

    [MemberNotNullWhen(false, nameof(Options))]
    [MemberNotNullWhen(false, nameof(ParserResult))]
    public bool ShouldExit { get; private set; } = false;

    public int ExitCode { get; private set; } = 0;

    private void ShowHelpAndExit()
    {
        // Let the library generate help by parsing --help
        var helpParser = new Parser(settings => { settings.HelpWriter = Console.Error; });
        helpParser.ParseArguments<CommandLineOptions>(new[] { "--help" });

        ShouldExit = true;
        ExitCode = 1;
    }
}
