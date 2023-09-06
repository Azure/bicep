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
        public static Uri ToUriEncoded(this DocumentUri documentUri)
        {
#pragma warning disable RS0030 // Do not use banned APIs
            return documentUri
                .With(new() { Path = documentUri.Path.Replace("%", "%25") })
                .ToUri();
#pragma warning restore RS0030 // Do not use banned APIs
        }
    }
}
