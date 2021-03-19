// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using System;
using System.Collections.Generic;

namespace Bicep.Core.Analyzers.Linter
{
    internal class LinterRule : IBicepAnalyzerRule
    {
        public LinterRule(string code, string ruleName, string description, string docUri,
                          bool enableForEdit = true,
                          bool enableForCLI = true,
                          DiagnosticLevel level = DiagnosticLevel.Warning)
        {
            this.AnalyzerName = LinterAnalyzer.AnalyzerName;
            this.Code = code;
            this.RuleName = ruleName;
            this.Description = description;
            this.DocumentationUri = docUri;
            this.DiagnosticLevel = level;
            this.EnabledForEdits = enableForEdit;
            this.EnabledForCLI = enableForCLI;
        }

        public string AnalyzerName { get; }
        public string Code { get; }
        public string RuleName { get; }

        // TODO: Decide how we want to manage configuration
        // Variants:
        //  1. enable globally or onchange or onsave (i.e. static whole file)
        //  2. set severity of diagnostic
        //  3. when fix is present what about auto application

        public bool EnabledForEdits { get; private set; }
        public bool EnabledForCLI { get; private set; }
        public Diagnostics.DiagnosticLevel DiagnosticLevel { get; private set; }

        public string Description { get; }
        public string DocumentationUri { get; }

        protected virtual string GetMessage() => this.Description;

        public virtual IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel model)
        {
            //TODO: make this truly abstract so that an exception is thrown if Analyze
            // is invoked on base class
            // throw new INvalideOperationException("must override base class Analyze")
            var span = new TextSpan(0, 1);
            yield return new AnalyzerDiagnostic(
                                this.AnalyzerName,
                                span,
                                this.DiagnosticLevel,
                                this.Code,
                                this.GetMessage());
        }

        public void ConfigureRule(bool enabledOnChange, bool enabledForDocument, DiagnosticLevel level)
        {
            this.EnabledForEdits = enabledOnChange;
            this.EnabledForCLI = enabledForDocument;
            this.DiagnosticLevel = level;
        }

        internal virtual AnalyzerDiagnostic CreateDiagnosticForSpan(TextSpan span) =>
            new(analyzerName: this.AnalyzerName, span: span, level: this.DiagnosticLevel, code: this.Code, message: this.GetMessage());

        internal virtual AnalyzerFixableDiagnostic CreateFixableDiagnosticForSpan(TextSpan span, CodeFix fix) =>
            new(analyzerName: this.AnalyzerName, span: span, level: this.DiagnosticLevel, code: this.Code, message: this.GetMessage(), codeFixes: new[] { fix }, label: null);

    }
}
