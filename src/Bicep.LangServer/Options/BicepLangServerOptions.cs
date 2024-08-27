// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Options;

public record BicepLangServerOptions(bool VsCompatibilityMode = false)
{
    public static BicepLangServerOptions Default = new();
}
