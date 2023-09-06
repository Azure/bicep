// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

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
