// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.Core.UnitTests.Assertions;

public static class CodeLensExtensions
{
    public static string[]? CommandArguments(this CodeLens codeLens)
    {
        return codeLens.Command?.Arguments?.Children().Select(token => token.ToString()).ToArray();
    }
}
