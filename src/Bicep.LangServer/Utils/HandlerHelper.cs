// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Semantics;
using Bicep.IO.Abstraction;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Utils;

public static class HandlerHelper
{
    public static string ValidateLocalFilePath(DocumentUri documentUri)
    {
        if (documentUri.ToIOUri().TryGetLocalFilePath() is not { } localPath)
        {
            throw new ArgumentException($"Invalid input URI: {documentUri}. Expected a file URI.");
        }

        return localPath;
    }
}
