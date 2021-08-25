// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.Configuration
{
    public class BicepConfig
    {
        public BicepConfig(Uri uri, string contents)
        {
            Uri = uri;
            Contents = contents;
        }

        public Uri Uri { get; }

        public string Contents { get; }
    }
}
