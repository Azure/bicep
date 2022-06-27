// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests
{
    public record SemanticTokenInfo(TextSpan Span, SemanticTokenType Type, SemanticTokenModifier Modifier);
}
