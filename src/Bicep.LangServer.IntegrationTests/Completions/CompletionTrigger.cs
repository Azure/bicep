// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests.Completions
{
    [DebuggerDisplay("Position = {Position}, SetName = {SetName}")]
    public sealed class CompletionTrigger(Position position, string setName)
    {
        public Position Position { get; } = position;

        public string SetName { get; } = setName;
    }
}
