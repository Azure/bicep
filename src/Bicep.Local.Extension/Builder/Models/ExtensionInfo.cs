// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Bicep.Local.Extension.Builder.Models;

public class ExtensionInfo(string Name, string Version, bool IsSingleton)
{
    public string Name { get; } = Name;
    public string Version { get; } = Version;
    public bool IsSingleton { get; } = IsSingleton;
}
