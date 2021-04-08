// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions.Execution;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.IntegrationTests.Assertions
{
    public static class AssertionScopeExtensions
    {
        public static AssertionScope WithAnnotations<T>(this AssertionScope assertionScope, SyntaxTree syntaxTree, string contextName, IEnumerable<T>? data, Func<T, string> messageFunc, Func<T, Range> rangeFunc)
            => Core.UnitTests.Assertions.AssertionScopeExtensions.WithAnnotatedSource(
                assertionScope,
                syntaxTree,
                contextName,
                (data ?? Enumerable.Empty<T>()).Select(x => new PrintHelper.Annotation(FromRange(syntaxTree, rangeFunc(x)), messageFunc(x))));

        private static TextSpan FromRange(SyntaxTree syntaxTree, Range range)
        {
            var position = TextCoordinateConverter.GetOffset(syntaxTree.LineStarts, range.Start.Line, range.Start.Character);
            var length  = TextCoordinateConverter.GetOffset(syntaxTree.LineStarts, range.End.Line, range.End.Character) - position;

            return new TextSpan(position, length);
        }
    }
}
