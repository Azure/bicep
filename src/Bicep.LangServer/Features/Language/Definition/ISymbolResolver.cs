// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Features.Language.Definition
{
    public interface ISymbolResolver
    {
        SymbolResolutionResult? ResolveSymbol(DocumentUri uri, Position position);
    }
}

