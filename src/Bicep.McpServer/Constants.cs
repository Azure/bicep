// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.McpServer;

public static class Constants
{
    public const string ServerInstructions = """
        This MCP server exposes a number of tools to improve accuracy and quality when authoring Bicep files.
        Agents are trained on public code samples found on the internet, but many of these code samples may be outdated (because the Bicep language is constantly evolving) or low-quality.
        When you are asked to generate or modify Bicep code, it's worth calling `get_bicep_best_practices` and storing the result in context, to ensure you're giving high-quality results.
        """;

}