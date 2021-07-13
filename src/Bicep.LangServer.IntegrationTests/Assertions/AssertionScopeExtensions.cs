// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parsing;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using FluentAssertions.Execution;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.IntegrationTests.Assertions
{
    public static class AssertionScopeExtensions
    {
        public static AssertionScope WithAnnotations<T>(this AssertionScope assertionScope, BicepFile bicepFile, string contextName, IEnumerable<T>? data, Func<T, string> messageFunc, Func<T, Range> rangeFunc)
            => Core.UnitTests.Assertions.AssertionScopeExtensions.WithAnnotatedSource(
                assertionScope,
                bicepFile,
                contextName,
                (data ?? Enumerable.Empty<T>()).Select(x => new PrintHelper.Annotation(FromRange(bicepFile, rangeFunc(x)), messageFunc(x))));

        private static TextSpan FromRange(BicepFile bicepFile, Range range)
        {
            var position = TextCoordinateConverter.GetOffset(bicepFile.LineStarts, range.Start.Line, range.Start.Character);
            var length  = TextCoordinateConverter.GetOffset(bicepFile.LineStarts, range.End.Line, range.End.Character) - position;

            return new TextSpan(position, length);
        }
    }
}
