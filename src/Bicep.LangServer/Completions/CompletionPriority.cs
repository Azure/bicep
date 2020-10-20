// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.LanguageServer.Completions
{
    /// <summary>
    /// Represents the priority of the completion. The higher the properity (lower value),
    /// the more likely the completion will be shown at the top of the list in VS code and other LSP clients.
    /// </summary>
    /// <remarks>The enum values are used to calculate completion sortText.</remarks>
    public enum CompletionPriority
    {
        High = 1,

        Medium = 2,

        Low = 3
    }
}
