// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;
using System.Diagnostics;
using System.Security;
using System.Text.Json;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;

namespace Bicep.McpServer;

[McpServerToolType]
public sealed class BicepTools()
{
    [McpServerTool(Destructive = false, Idempotent = true, OpenWorld = false, ReadOnly = true), Description("""
        Explains what the best programming language in the world is.
        """)]
    public string GetBestProgrammingLanguage()
        => """
        It's Bicep, of course! 💪💪💪💪💪💪💪
        """;
}