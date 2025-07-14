// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.CodeAction;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Text;

namespace Bicep.Core.Analyzers.Linter
{
    public abstract class LinterRuleBase : IBicepAnalyzerRule
    {
        public LinterRuleBase(
            string code,
            string description,
            LinterRuleCategory category,
            DiagnosticStyling diagnosticStyling = DiagnosticStyling.Default,
            // This should normally be left unspecified so that the default diagnostic level is set based on the category.  Only specify
            //   if it needs to default to something other than the category's default diagnostic level.
            DiagnosticLevel? overrideCategoryDefaultDiagnosticLevel = default)
        {
            this.Code = code;
            this.Description = description;
            this.Uri = new Uri($"https://aka.ms/bicep/linter-diagnostics#{Code}");
            this.Category = category;
            this.DiagnosticStyling = diagnosticStyling;
            this.OverrideCategoryDefaultDiagnosticLevel = overrideCategoryDefaultDiagnosticLevel;
        }

        public string Code { get; }

        public LinterRuleCategory Category { get; }

        public readonly string RuleConfigSection = $"{LinterAnalyzer.AnalyzerName}.rules";

        public DiagnosticLevel DefaultDiagnosticLevel =>
            OverrideCategoryDefaultDiagnosticLevel.HasValue ? OverrideCategoryDefaultDiagnosticLevel.Value : GetDefaultDiagosticLevelForCategory(this.Category);

        public string Description { get; }

        public Uri Uri { get; }

        public DiagnosticLevel? OverrideCategoryDefaultDiagnosticLevel { get; }

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
        public string GetMessage(params object[] values) => FormatMessage(values);

        public IEnumerable<IDiagnostic> Analyze(SemanticModel model, IServiceProvider serviceProvider)
        {
            if (GetDiagnosticLevel(model) == DiagnosticLevel.Off)
            {
                return [];
            }

            return AnalyzeInternal(model, serviceProvider, GetDiagnosticLevel(model));
        }

        /// <summary>
        /// Abstract method each rule must implement to provide analyzer
        /// diagnostics through the Analyze API
        /// </summary>
        public virtual IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, IServiceProvider serviceProvider, DiagnosticLevel diagnosticLevel)
        {
            return AnalyzeInternal(model, diagnosticLevel);
        }

        /// <summary>
        /// Abstract method each rule must implement to provide analyzer
        /// diagnostics through the Analyze API
        /// </summary>
        public virtual IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            throw new NotImplementedException($"{this.GetType().Name} must implement one of the overloads of {nameof(AnalyzeInternal)}");
        }

        protected DiagnosticLevel GetDiagnosticLevel(SemanticModel model) => GetDiagnosticLevel(model.Configuration.Analyzers);

        protected DiagnosticLevel GetDiagnosticLevel(AnalyzersConfiguration configuration)
        {
            if (GetConfigurationValue(configuration, "level", DefaultDiagnosticLevel.ToString()) is string configuredLevel && Enum.TryParse<DiagnosticLevel>(configuredLevel, true, out var parsed))
            {
                return parsed;
            }

            return DefaultDiagnosticLevel;
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
        /// Create a diagnostic message for a span that has a customized string
        /// formatter defined in the deriving class.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="span"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected virtual Diagnostic CreateDiagnosticForSpan(DiagnosticLevel level, TextSpan span, params object[] values) => new(
            span,
            level,
            DiagnosticSource.CoreLinter,
            Code,
            GetMessage(values))
        {
            Uri = Uri,
            Styling = DiagnosticStyling
        };

        /// <summary>
        /// Create a diagnostic message for a span that has a customized string.
        /// </summary>
        protected Diagnostic CreateDiagnostic(TextSpan span, string message) => new(
            span,
            DefaultDiagnosticLevel,
            DiagnosticSource.CoreLinter,
            Code,
            message)
        {
            Uri = Uri,
            Styling = DiagnosticStyling
        };

        protected virtual Diagnostic CreateFixableDiagnosticForSpan(DiagnosticLevel level, TextSpan span, CodeFix fix, params object[] values) =>
            CreateFixableDiagnosticForSpan(level, span, [fix], values);

        protected virtual Diagnostic CreateFixableDiagnosticForSpan(DiagnosticLevel level, TextSpan span, CodeFix[] fixes, params object[] values) =>
            CreateDiagnosticForSpan(level, span, values) with { Fixes = [.. fixes] };

        public static DiagnosticLevel GetDefaultDiagosticLevelForCategory(LinterRuleCategory category) =>
            category switch
            {
                // Note: In general the default diagnostic level for a category should be either Warning or Off
                LinterRuleCategory.BestPractice => DiagnosticLevel.Warning,
                LinterRuleCategory.Portability => DiagnosticLevel.Off,
                LinterRuleCategory.PotentialCodeIssues => DiagnosticLevel.Warning,
                LinterRuleCategory.ResourceLocationRules => DiagnosticLevel.Off,
                LinterRuleCategory.Security => DiagnosticLevel.Warning,
                LinterRuleCategory.Style => DiagnosticLevel.Warning,

                // This is an exception to the "Warning" or "Off" only rule - these will cause actual deployment errors, so default level is Error
                LinterRuleCategory.DeploymentError => DiagnosticLevel.Error,
                // For stacks incompatibilities, the default level is Info so we can inform but not disrupt users who are not exclusively Deployment stack users.
                LinterRuleCategory.DeploymentStackIncompatibility => DiagnosticLevel.Info,

                // Unexpected values
                _ => throw new ArgumentOutOfRangeException($"LinterRuleCategory (unexpected value \"{category}\")")
            };
    }
}
