// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.IO.Abstraction;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Extensions
{
    public static class IOUriExtensions
    {
        public static DocumentUri ToDocumentUri(this IOUri uri) => new(uri.Scheme, uri.Authority ?? "", uri.Path, uri.Query, uri.Fragment);
    }
}
