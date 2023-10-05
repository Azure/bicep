// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parsing;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
