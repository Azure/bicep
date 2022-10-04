// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.CodeAction;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter
{
    public abstract class LinterRuleBase : IBicepAnalyzerRule
    {
        public LinterRuleBase(
            string code,
            string description,
            Uri? docUri = null,
            DiagnosticLevel diagnosticLevel = DiagnosticLevel.Warning,
            DiagnosticStyling diagnosticStyling = DiagnosticStyling.Default)
        {
            this.AnalyzerName = LinterAnalyzer.AnalyzerName;
            this.Code = code;
            this.Description = description;
            this.Uri = docUri;
            this.DiagnosticLevel = diagnosticLevel;
            this.DiagnosticStyling = diagnosticStyling;
        }

        public string AnalyzerName { get; }

        public string Code { get; }

        public readonly string RuleConfigSection = $"{LinterAnalyzer.AnalyzerName}.rules";

        public DiagnosticLevel DiagnosticLevel { get; }

        public string Description { get; }

        public Uri? Uri { get; }

        // If specified, adds the given diagnostic label to every diagnostic created for this rule (such as for unnecessary or obsolete code).
        // Should be left as None/null for most rules.
        public DiagnosticStyling DiagnosticStyling { get; }


        /// <summary>
        /// Override to implement detailed message for rule
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual string FormatMessage(params object[] values) => this.Description;

        /// <summary>
        /// Gets a message using the supplied parameter values (if any).
        /// Otherwise returns the rule description
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public string GetMessage(params object[] values)
            => (values.Any() ? FormatMessage(values) : this.Description);

        public IEnumerable<IDiagnostic> Analyze(SemanticModel model)
        {
            if (GetDiagnosticLevel(model) == DiagnosticLevel.Off)
            {
                return Enumerable.Empty<IDiagnostic>();
            }

            return AnalyzeInternal(model);
        }

        /// <summary>
        /// Abstract method each rule must implement to provide analyzer
        /// diagnostics through the Analyze API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public abstract IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model);

        protected DiagnosticLevel GetDiagnosticLevel(SemanticModel model) => GetDiagnosticLevel(model.Configuration.Analyzers);

        protected DiagnosticLevel GetDiagnosticLevel(AnalyzersConfiguration configuration)
        {
            if (GetConfigurationValue(configuration, "level", DiagnosticLevel.ToString()) is string configuredLevel && Enum.TryParse<DiagnosticLevel>(configuredLevel, true, out var parsed))
            {
                return parsed;
            }

            return DiagnosticLevel;
        }

        /// <summary>
        /// Get a setting from defaults or local override
        /// Expectation: key names for settings are lower case
        /// </summary>
        /// <typeparam name="T">The type of the value to convert to.</typeparam>
        /// <param name="configuration">The configuration of the model being analyzed.</param>
        /// <param name="key">The linter configuration key.</param>
        /// <param name="defaultValue">The default value to use if no value is found.</param>
        /// <returns></returns>
        protected T GetConfigurationValue<T>(AnalyzersConfiguration configuration, string key, T defaultValue) =>
            configuration.GetValue($"{RuleConfigSection}.{Code}.{key}", defaultValue);

        /// <summary>
        ///  Create a simple diagnostic that displays the defined Description
        ///  of the derived rule.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        protected virtual AnalyzerDiagnostic CreateDiagnosticForSpan(DiagnosticLevel level, TextSpan span) =>
            new(analyzerName: this.AnalyzerName,
                span: span,
                level: level,
                code: this.Code,
                message: this.GetMessage(),
                documentationUri: this.Uri,
                styling: this.DiagnosticStyling);

        /// <summary>
        /// Create a diagnostic message for a span that has a customized string
        /// formatter defined in the deriving class.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="span"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected virtual AnalyzerDiagnostic CreateDiagnosticForSpan(DiagnosticLevel level, TextSpan span, params object[] values) =>
            new(analyzerName: this.AnalyzerName,
                span: span,
                level: level,
                code: this.Code,
                message: this.GetMessage(values),
                documentationUri: this.Uri,
                styling: this.DiagnosticStyling);

        protected virtual AnalyzerFixableDiagnostic CreateFixableDiagnosticForSpan(DiagnosticLevel level, TextSpan span, CodeFix fix, params object[] values) =>
            CreateFixableDiagnosticForSpan(level, span, new[] { fix }, values);

        protected virtual AnalyzerFixableDiagnostic CreateFixableDiagnosticForSpan(DiagnosticLevel level, TextSpan span, CodeFix[] fixes, params object[] values) =>
            new(analyzerName: this.AnalyzerName,
                span: span,
                level: level,
                code: this.Code,
                message: this.GetMessage(values),
                documentationUri: this.Uri,
                codeFixes: fixes,
                styling: this.DiagnosticStyling);
    }
}
