// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Diagnostics;

namespace Bicep.LangServer.IntegrationTests.Completions
{
    [DebuggerDisplay("Position = {Position}, SetName = {SetName}")]
    public sealed class CompletionTrigger
    {
        public CompletionTrigger(Position position, string setName)
        {
            this.Position = position;
            this.SetName = setName;
        }

        public Position Position { get; }

        public string SetName { get; }
    }
}
