// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Providers
{
    public interface IMcrCompletionProvider
    {
        List<CompletionItem> GetModuleNames();
        List<CompletionItem> GetTags(string moduleName);
    }
}
