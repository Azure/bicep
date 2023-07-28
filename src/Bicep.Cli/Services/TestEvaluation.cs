// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Deployments.Core.Definitions.Schema;

namespace Bicep.Cli.Services
{
    public class TestEvaluation
    {
        public Template? Template { get; }
        public string Error { get; }
        public Dictionary<string, bool> Assertions { get; }
        public Dictionary<string, bool> FailedAssertions { get; }
        public bool Success => Error == "" && (FailedAssertions.Count == 0);
        public bool Skip => Error != "";
        public TestEvaluation(Template? template, Dictionary<string, bool>? assertions, string? error)
        {
            Template = template;
            Assertions = assertions ?? new Dictionary<string, bool>();
            Error = error ?? "";
            FailedAssertions = assertions?.Where(a => !a.Value).ToDictionary(a => a.Key, a => a.Value) ?? new Dictionary<string, bool>();
        }
    }
}