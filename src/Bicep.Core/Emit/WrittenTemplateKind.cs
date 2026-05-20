// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;

namespace Bicep.Core.Emit;

public enum WrittenTemplateKind
{
    Template = 0,
    TemplateLink,
}

public static class WrittenTemplateKindExtensions
{
    public static string ToPropertyName(this WrittenTemplateKind kind) => kind switch
    {
        WrittenTemplateKind.Template => "template",
        WrittenTemplateKind.TemplateLink => "templateLink",
        _ => throw new UnreachableException(),
    };
}
