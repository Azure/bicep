// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Providers
{
    public interface IMcrCompletionProvider
    {
        Task Initialize();

        List<CompletionItem> GetModuleNames();

        List<CompletionItem> GetVersions(string moduleName);

        string? GetReadmeLink(string modulePath);
    }
}
