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
    public const string HttpOption = "http";
    public const string PipeOption = "pipe";
    public const string SocketOption = "socket";
}

public class CommandLineOptions
{
    [Option(BicepExtensionCommandLineArgument.SocketOption, Required = false, HelpText = "Start the gRPC server listening on a Unix domain socket at the specified file path. Use this for inter-process communication on Unix-like systems. Example: --socket /tmp/bicep-extension.sock")]
    public string? Socket { get; set; }

    [Option(BicepExtensionCommandLineArgument.PipeOption, Required = false, HelpText = "Start the gRPC server listening on a named pipe with the specified name. Use this for inter-process communication on Windows or when working with tools that prefer named pipes. Example: --pipe bicep-extension-pipe")]
    public string? Pipe { get; set; }

    [Option(BicepExtensionCommandLineArgument.DescribeOption, Required = false, Default = false, HelpText = "Output the Bicep extension's type definitions and resource schema as JSON to stdout, then exit. Use this to discover available resource types and their properties without starting the server. Cannot be used with connection options.")]
    public bool Describe { get; set; }

    [Option(BicepExtensionCommandLineArgument.HttpOption, Required = false, HelpText = "Start the gRPC server listening on localhost at the specified TCP port (1-65535). Use this for network-based communication or when debugging with gRPC tools. Example: --http 5000")]
    public int? Http { get; set; }
}

