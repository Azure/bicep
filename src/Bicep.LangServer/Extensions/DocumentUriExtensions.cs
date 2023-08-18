// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using OmniSharp.Extensions.LanguageServer.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static class DocumentUriExtensions
    {
        public static Uri ToUriEscaped(this DocumentUri documentUri)
        {
            return documentUri
                .With(new() { Path = documentUri.Path.Replace("%", "%25") })
                .ToUri();
        }
    }
}
