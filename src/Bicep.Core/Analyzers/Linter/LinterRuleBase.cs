// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.CodeAction;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter
{
    public abstract class LinterRuleBase : IBicepAnalyzerRule
    {
        private AnalyzersConfiguration configuration = new(rawConfiguration: null);

        public LinterRuleBase(
            string code,
            string description,
            Uri? docUri = null,
            DiagnosticLevel diagnosticLevel = DiagnosticLevel.Warning,
            DiagnosticLabel? diagnosticLabel = null)
        {
            this.AnalyzerName = LinterAnalyzer.AnalyzerName;
            this.Code = code;
            this.Description = description;
            this.Uri = docUri;
            this.DiagnosticLevel = diagnosticLevel;
            this.DiagnosticLabel = diagnosticLabel;
        }

        public string AnalyzerName { get; }

        public string Code { get; }

        public readonly string RuleConfigSection = $"{LinterAnalyzer.AnalyzerName}:rules";

        public DiagnosticLevel DiagnosticLevel { get; private set; }

        public string Description { get; }

        public Uri? Uri { get; }

        public DiagnosticLabel? DiagnosticLabel { get; }


        /// <summary>
        /// Override to implement detailed message for rule
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual string FormatMessage(params object[] values) => this.Description;

        public virtual void Configure(AnalyzersConfiguration configuration)
        {
            this.configuration = configuration;

            var levelValue = this.GetConfigurationValue("level", this.DiagnosticLevel.ToString());

            if (Enum.TryParse<DiagnosticLevel>(levelValue, true, out var level))
            {
                this.DiagnosticLevel = level;
            }
        }

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
            try
            {
                // Expand the iteration immediately or the try/catch won't catch exceptions occuring during the rule analysis
                // TODO: We need exception handling further up the tree in order to handle external linters.
                return AnalyzeInternal(model).ToArray();
            }
            catch (Exception ex)
            {
                return new AnalyzerDiagnostic(
                    this.AnalyzerName,
                    new TextSpan(0, 0),
                    DiagnosticLevel.Warning,
                    LinterAnalyzer.FailedRuleCode,
                    string.Format(CoreResources.LinterRuleExceptionMessageFormat,this.AnalyzerName, ex.Message)).AsEnumerable();
            }
        }

        /// <summary>
        /// Abstract method each rule must implement to provide analyzer
        /// diagnostics through the Analyze API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public abstract IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model);

        /// <summary>
        /// Get a setting from defaults or local override
        /// Expectation: key names for settings are lower case
        /// </summary>
        /// <typeparam name="T">The type of the value to convert to.</typeparam>
        /// <param name="key">The linter configuration key.</param>
        /// <param name="defaultValue">The default value to use if no value is found.</param>
        /// <returns></returns>
        protected T GetConfigurationValue<T>(string key, T defaultValue) =>
            this.configuration.GetValue($"{RuleConfigSection}:{Code}:{key}", defaultValue);

        /// <summary>
        ///  Create a simple diagnostic that displays the defined Description
        ///  of the derived rule.
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        protected virtual AnalyzerDiagnostic CreateDiagnosticForSpan(TextSpan span) =>
            new(analyzerName: this.AnalyzerName,
                span: span,
                level: this.DiagnosticLevel,
                code: this.Code,
                message: this.GetMessage(),
                documentationUri: this.Uri,
                label: this.DiagnosticLabel);

        /// <summary>
        /// Create a diagnostic message for a span that has a customized string
        /// formatter defined in the deriving class.
        /// </summary>
        /// <param name="span"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected virtual AnalyzerDiagnostic CreateDiagnosticForSpan(TextSpan span, params object[] values) =>
            new(analyzerName: this.AnalyzerName,
                span: span,
                level: this.DiagnosticLevel,
                code: this.Code,
                message: this.GetMessage(values),
                documentationUri: this.Uri,
                label: this.DiagnosticLabel);

        protected virtual AnalyzerFixableDiagnostic CreateFixableDiagnosticForSpan(TextSpan span, CodeFix fix) =>
            new(analyzerName: this.AnalyzerName,
                span: span,
                level: this.DiagnosticLevel,
                code: this.Code,
                message: this.GetMessage(),
                documentationUri: this.Uri,
                codeFixes: new[] { fix },
                label: this.DiagnosticLabel);
    }
}
