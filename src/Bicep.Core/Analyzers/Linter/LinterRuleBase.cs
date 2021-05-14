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
using System.Diagnostics;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter
{
    public abstract class LinterRuleBase : IBicepAnalyzerRule
    {
        public LinterRuleBase(string code, string ruleName, string description, string docUri,
                          DiagnosticLevel diagnosticLevel = DiagnosticLevel.Warning,
                          DiagnosticLabel? diagnosticLabel = null)
        {
            this.ConfigHelper = new ConfigHelper();
            this.AnalyzerName = LinterAnalyzer.AnalyzerName;
            this.Code = code;
            this.RuleName = ruleName;
            this.Description = description;
            this.DocumentationUri = docUri;
            this.DiagnosticLevel = diagnosticLevel;
            this.DiagnosticLabel = diagnosticLabel;

            LoadConfiguration();
        }

        internal const string FailedRuleCode = "Linter Rule Error";

        private readonly ConfigHelper ConfigHelper;
        public string AnalyzerName { get; }
        public string Code { get; }
        public string RuleName { get; }

        public const string RuleConfigSection = "Linter:Core:Rules";

        // TODO: Decide how we want to manage configuration
        // Variants:
        //  1. enable globally or onchange or onsave (i.e. static whole file)
        //  2. set severity of diagnostic
        //  3. when fix is present what about auto application

        public bool Enabled => this.DiagnosticLevel != DiagnosticLevel.Off;
        public Diagnostics.DiagnosticLevel DiagnosticLevel { get; private set; }

        public string Description { get; }
        public string DocumentationUri { get; }
        public Diagnostics.DiagnosticLabel? DiagnosticLabel { get; }

        private void LoadConfiguration()
        {
            var configDiagLevel = GetConfiguration(nameof(this.DiagnosticLevel), this.DiagnosticLevel.ToString());
            if (DiagnosticLevel.TryParse<DiagnosticLevel>(configDiagLevel, true, out var lvl))
            {
                this.DiagnosticLevel = lvl;
            }
        }

        /// <summary>
        /// GetMessage allows a linter rule display message to be dynamic without
        /// resorting to side-effect inducing work in the Description property.
        /// Should be overridden in any rule with a complex message requirement.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetMessage() => this.Description;

        /// <summary>
        /// Gets a formatted message using the supplied parameter values.
        /// In the base class this ignores the parameters and will throw
        /// an exception if called in Debug build.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected virtual string GetFormattedMessage(params object[] values)
        {
            Debug.Assert(values == null || values.Length == 0, "LinterRule GetFormattedMessage when needed should always be overridden. Values are ignored in base class.");
            return this.Description;
        }

        public IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel model)
        {
            try
            {
                // Expand the iteration immediately or the try/catch won't catch exceptions occuring during the rule analysis
                // TODO: We need exception handling further up the tree in order to handle external linters.
                return AnalyzeInternal(model).ToArray();
            }
            catch (Exception ex)
            {
                return new[]{ new AnalyzerDiagnostic(this.AnalyzerName,
                                                    new TextSpan(0, 0),
                                                    DiagnosticLevel.Warning,
                                                    FailedRuleCode,
                                                    string.Format(CoreResources.LinterRuleExceptionMessageFormat, ex.Message),
                                                    null)
                };
            }
        }

        internal abstract IEnumerable<IBicepAnalyzerDiagnostic> AnalyzeInternal(SemanticModel model);

        public void ConfigureRule(DiagnosticLevel level)
        {
            this.DiagnosticLevel = level;
        }

        protected bool GetConfiguration(string name, bool defaultValue)
            => ConfigHelper.GetValue($"{RuleConfigSection}:{Code}:{name}", defaultValue);

        protected string GetConfiguration(string name, string defaultValue)
            => ConfigHelper.GetValue($"{RuleConfigSection}:{Code}:{name}", defaultValue);

        protected IEnumerable<string> GetConfiguration(string name, string[] defaultValue)
            => ConfigHelper.GetValue($"{RuleConfigSection}:{Code}:{name}", defaultValue);

        /// <summary>
        ///  Create a simple diagnostic that displays the defined Description
        ///  of the derived rule.
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        internal virtual AnalyzerDiagnostic CreateDiagnosticForSpan(TextSpan span) =>
            new(analyzerName: this.AnalyzerName,
                span: span,
                level: this.DiagnosticLevel,
                code: this.Code,
                message: this.GetMessage(),
                label: this.DiagnosticLabel);

        /// <summary>
        /// Create a diagnostic message for a span that has a customized string
        /// formatter defined in the deriving class.
        /// </summary>
        /// <param name="span"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        internal virtual AnalyzerDiagnostic CreateDiagnosticForSpan(TextSpan span, params object[] values) =>
            new(analyzerName: this.AnalyzerName,
                span: span,
                level: this.DiagnosticLevel,
                code: this.Code,
                message: this.GetFormattedMessage(values),
                label: this.DiagnosticLabel);

        internal virtual AnalyzerFixableDiagnostic CreateFixableDiagnosticForSpan(TextSpan span, CodeFix fix) =>
            new(analyzerName: this.AnalyzerName,
                span: span,
                level: this.DiagnosticLevel,
                code: this.Code,
                message: this.GetMessage(),
                codeFixes: new[] { fix },
                label: this.DiagnosticLabel);

    }
}
