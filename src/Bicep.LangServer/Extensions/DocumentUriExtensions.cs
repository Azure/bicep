// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static class DocumentUriExtensions
    {
        public static IOUri ToIOUri(this DocumentUri documentUri)
        {
            var scheme = new IOUriScheme(documentUri.Scheme ?? "");

            if (!scheme.IsFile && documentUri.Authority.Length == 0)
            {
                // For untitled documents with no authority (e.g., DocumentUri = untitled:Untitled-<number>),
                // Omnisharp sets the authority to an empty string, which may be incorrect.
                // IOUri distinguishes between null and empty authority, and requires an absolute path when the authority is empty.
                // To avoid validation issues, we set the authority to null for untitled documents.
                return new(scheme, null, documentUri.Path, documentUri.Query, documentUri.Fragment);
            }

            return new(scheme, documentUri.Authority, documentUri.Path, documentUri.Query, documentUri.Fragment);
        }

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
