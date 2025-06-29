// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.SourceGraph;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Helpers;
using FluentAssertions.Execution;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.IntegrationTests.Assertions
{
    public static class AssertionScopeExtensions
    {
        public static AssertionScope WithAnnotations<T>(this AssertionScope assertionScope, LanguageClientFile bicepFile, string contextName, IEnumerable<T>? data, Func<T, string> messageFunc, Func<T, Range> rangeFunc)
            => Core.UnitTests.Assertions.AssertionScopeExtensions.WithAnnotatedSource(
                assertionScope,
                bicepFile.Text,
                bicepFile.LineStarts,
                contextName,
                (data ?? []).Select(x => new PrintHelper.Annotation(bicepFile.GetSpan(rangeFunc(x)), messageFunc(x))));
    }
}
