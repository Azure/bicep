// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Syntax
{
    public class JsonSyntaxTree : SyntaxTree
    {
        public JsonSyntaxTree(Uri fileUri, ImmutableArray<int> lineStarts, JObject? templateObject, ProgramSyntax programSyntax)
            : base(fileUri, lineStarts, programSyntax)
        {
            this.TemplateObject = templateObject;
        }

        public JObject? TemplateObject { get; }
    }
}
