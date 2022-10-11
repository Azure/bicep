// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using OmniSharp.Extensions.LanguageServer.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Providers
{
    public interface IFileLanguageTracker
    {
        void NotifyFileClose(DocumentUri documentUri);

        void NotifyFileOpen(DocumentUri documentUri, string languageId);

        string? TryGetLanguageId(DocumentUri documentUri);
    }
}
