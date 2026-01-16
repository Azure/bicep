// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Google.Protobuf.Compiler;

namespace Bicep.Local.Extension.Builder.Models;

public class ExtensionInfo(string name, string version, bool isSingleton)
{
    public string Name { get; } = ValidateValue(name, nameof(name));
    public string Version { get; } = ValidateValue(version, nameof(version));
    public bool IsSingleton { get; } = isSingleton;

    private static string ValidateValue(string value, string parameterName)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value;
    } 
}
