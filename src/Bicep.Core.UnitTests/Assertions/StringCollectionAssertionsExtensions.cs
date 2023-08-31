// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentAssertions;
using FluentAssertions.Collections;
using System;
using System.Linq;

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
    }
}
