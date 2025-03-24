// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static class DocumentUriExtensions
    {
        public static IOUri ToIOUri(this DocumentUri documentUri) => new(documentUri.Scheme ?? "", documentUri.Authority, documentUri.Path, documentUri.Query, documentUri.Fragment);

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
