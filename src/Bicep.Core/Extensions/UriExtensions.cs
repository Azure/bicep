// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;

namespace Bicep.Core.Extensions
{
    // TODO: remove after file abstraction migration is done.
    public static class UriExtensions
    {
        public static IOUri ToIOUri(this Uri uri)
        {
            var path = Uri.UnescapeDataString(uri.AbsolutePath);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !path.StartsWith('/'))
            {
                path = '/' + path;
            }

            return new IOUri(uri.Scheme, uri.Authority, path);
        }
    }
}
