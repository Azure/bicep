// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using ModelContextProtocol.Protocol;

namespace Bicep.McpServer.Helpers;

public static class McpHelper
{
    public static async ValueTask<CallToolResult> ExecuteWithFullErrorLogging(Func<ValueTask<string>> func)
    {
        try
        {
            var output = await func();
            return new CallToolResult
            {
                Content = [
                    new TextContentBlock
                    {
                        Text = output,
                    }
                ],
            };
        }
        catch (Exception ex)
        {
            return new CallToolResult
            {
                Content = [
                    new TextContentBlock
                    {
                        Text = $"An error occurred while executing the tool: {ex}",
                    }
                ],
                IsError = true,
            };
        }
    }

    public static CallToolResult ExecuteWithFullErrorLogging(Func<string> func)
    {
        try
        {
            var output = func();
            return new CallToolResult
            {
                Content = [
                    new TextContentBlock
                    {
                        Text = output,
                    }
                ],
            };
        }
        catch (Exception ex)
        {
            return new CallToolResult
            {
                Content = [
                    new TextContentBlock
                    {
                        Text = $"An error occurred while executing the tool: {ex}",
                    }
                ],
                IsError = true,
            };
        }
    }
}