// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Win32;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.UnitTests.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Looks for the first match of the regex, and returns the value of each group in an array
        /// NOTE: you'll need "using Bicep.Core.Extensions;" to use deconstruction syntax with the return value
        /// </summary>
        /// <example><![CDATA[
        ///   using Bicep.Core.Extensions;
        ///   var (registry, repo) = module.target.ExtractRegexGroups(
        ///       "^br:(.+?)/(.+?)[:@](.+?)$");
        ///   => ["mockregistry.io", "test/module1", "v1"]
        /// ]]>// </example>
        public static string[] ExtractRegexGroups(this string s, Regex regex)
        {
            var match = regex.Match(s);

            if (match.Captures.Count == 0)
            {
                throw new Exception($"{nameof(ExtractRegexGroups)}: No matches were found for regex {regex} in string \"{s}\"");
            }

            if (match.Groups.Count == 1)
            {
                throw new Exception($"{nameof(ExtractRegexGroups)}: No groups were found in regex {regex}");
            }

            return match.Groups.SelectArray(group => group.Value)
                .Skip(1) // Ignore the first group, which is the entire match
                .ToArray();
        }

        // NOTE: you'll need "using Bicep.Core.Extensions;" to use deconstruction syntax with the return value
        public static string[] ExtractRegexGroups(this string s, string regex)
        {
            return ExtractRegexGroups(s, new Regex(regex));
        }

        /// <summary>
        /// Looks for the first match of the regex, and returns the value of each group requested in groupNamesToExtract
        /// NOTE: you'll need "using Bicep.Core.Extensions;" to use deconstruction syntax with the return value
        /// </summary>
        /// <example><![CDATA[
        ///   using Bicep.Core.Extensions;
        ///   var (registry, repo) = module.target.ExtractRegexGroups(
        ///       "^br:(?<registry>.+?)/(?<repo>.+?)[:@](?<tag>.+?)$",
        ///       ["registry", "repo", "tag"]);
        ///   => ["mockregistry.io", "test/module1", "v1"]
        /// ]]>// </example>
        public static string[] ExtractRegexGroups(this string s, Regex regex, string[] groupNamesToExtract)
        {
            var match = regex.Match(s);
            if (match.Captures.Count == 0)
            {

                throw new Exception($"{nameof(ExtractRegexGroups)}: No matches were found for regex {regex} in string \"{s}\"");
            }

            return groupNamesToExtract.SelectArray(group => match.Groups[group].Value);
        }

        // NOTE: you'll need "using Bicep.Core.Extensions;" to use deconstruction syntax with the return value
        public static string[] ExtractRegexGroups(this string s, string regexExpression, string[] groupNamesToExtract)
        {
            return ExtractRegexGroups(s, new Regex(regexExpression), groupNamesToExtract);
        }
    }
}
