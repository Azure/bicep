// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class StringCollectionAssertionsExtensions
    {
        public static AndConstraint<StringCollectionAssertions> BeEquivalentToPaths(this StringCollectionAssertions instance, string[] expected, string because = "", params object[] becauseArgs)
        {
            instance.Subject.Select(p => NormalizePath(p)).Should().BeEquivalentTo(expected.Select(p => NormalizePath(p)), because, becauseArgs);

            return new AndConstraint<StringCollectionAssertions>(instance);
        }

        private static string NormalizePath(string path)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                return path.ToLowerInvariant();
            }
            else
            {
                return path;
            }
        }

        [CustomAssertion]
        public static AndConstraint<StringCollectionAssertions> NotContainAny(this StringCollectionAssertions instance, string[] collection, string because = "", params object[] becauseArgs)
        {
            foreach (var item in collection)
            {
                int index = Array.IndexOf(instance.Subject.ToArray(), item);
                if (index >= 0)
                {
                    Execute.Assertion
                        .BecauseOf(because, becauseArgs)
                        .FailWith("Expected collection {context:collection} to not contain {0}{reason}, but found it at index {1}", item, index);
                }
            }

            return new AndConstraint<StringCollectionAssertions>(instance);
        }
    }
}
