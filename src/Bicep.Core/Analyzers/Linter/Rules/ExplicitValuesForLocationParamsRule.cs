// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

// asdfg Also worth bearing in mind that LookUpModuleSourceFile can return you an ArmTemplateFile or TemplateSpecFile (with ArmTemplateSemanticModel & TemplateSpecSemanticModel) in cases where the module is referencing a template spec or JSON template
namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class ExplicitValuesForLocationParamsRule : LinterRuleBase
    {
        public new const string Code = NoHardcodedLocationRule.Code; //asdfg "explicit-values-for-location-params";

        public ExplicitValuesForLocationParamsRule() : base(
            code: Code,
            description: "asdfg CoreResources.ExplicitValuesForLocationParamsRuleDescription",
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"))
        {
        }

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            return Enumerable.Empty<IDiagnostic>();
        }
    }
}