// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Google.Protobuf.Compiler;

namespace Bicep.Local.Extension.Builder.Models;

public class ExtensionInfo
{
    public string Name { get; }
    public string Version { get; }
    public bool IsSingleton { get; }
    
    public ExtensionInfo(string name, string version, bool isSingleton)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(version);
        Name = name;
        Version = version;
        IsSingleton = isSingleton;    
    }
}
