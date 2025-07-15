// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Bicep.Local.Extension.Host.CommandLineArguments;

public class BicepExtensionCommandLineArgument
{
    public const string DescribeOption = "describe";
    public const string WaitForDebuggerOption = "wait-for-debugger";
    public const string HttpOption = "http";
    public const string PipeOption = "pipe";
    public const string SocketOption = "socket";
}

public class CommandLineOptions
{
    [Option(BicepExtensionCommandLineArgument.SocketOption, Required = false, HelpText = "The path to the domain socket to connect on")]
    public string? Socket { get; set; }

    [Option(BicepExtensionCommandLineArgument.PipeOption, Required = false, HelpText = "The named pipe to connect on")]
    public string? Pipe { get; set; }

    [Option(BicepExtensionCommandLineArgument.DescribeOption, Required = false, Default = false, HelpText = "Emits index and type definitions to std out")]
    public bool Describe { get; set; }

    [Option(BicepExtensionCommandLineArgument.HttpOption, Required = false, HelpText = "The port to expose extension on in HTTP mode")]
    public int? Http { get; set; }

}

