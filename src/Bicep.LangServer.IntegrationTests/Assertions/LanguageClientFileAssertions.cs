// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Parsing;
using Bicep.Core.UnitTests.Assertions;
using Bicep.LangServer.IntegrationTests.Helpers;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace Bicep.LangServer.IntegrationTests.Assertions
{
    public static class LanguageClientFileExtensions
    {
        public static LanguageClientFileAssertions Should(this LanguageClientFile instance)
        {
            return new LanguageClientFileAssertions(instance);
        }
    }

    public class LanguageClientFileAssertions : ReferenceTypeAssertions<LanguageClientFile, LanguageClientFileAssertions>
    {
        public LanguageClientFileAssertions(LanguageClientFile file)
            : base(file)
        {

        }

        protected override string Identifier => nameof(LanguageClientFile);

        public AndConstraint<LanguageClientFileAssertions> HaveSourceText(string expected, string because = "", params object[] becauseArgs)
        {
            var actual = this.Subject.Text.NormalizeNewlines();
            expected = expected.NormalizeNewlines();

            actual.Should().EqualWithLineByLineDiff(expected, because, becauseArgs);

            return new AndConstraint<LanguageClientFileAssertions>(this);
        }
    }
}
