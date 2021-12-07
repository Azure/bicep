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
    public sealed class NoLocationExprOutsideParamsRule : LocationRuleBase
    {
        //asdfg var->param rule should show on the variable, not the usage

        // Sub-rules: asdfg
        //
        // 1) If a location parameter exists, it must be of type string
        // 2) Parameter location may optionally default to resourceGroup().location or deployment().location or the string 'global'.
        // 3) Outside of the default value for parameter location, the expressions resourceGroup().location and deployment().location may not be used in a template.
        // 4) Each resource's location property and each module's location parameter must be either an expression or the string 'global' (usually this expression will be the value of the location parameter)
        // 5) When consuming a module with a location parameter, the parameter must be given an explicit value (it may not be left to its default value)

        public new const string Code = "no-hardcoded-location";

        public NoLocationExprOutsideParamsRule() : base(
            code: Code,
            description: "asdfg CoreResources.NoLocationExprOutsideParamsRule",
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"))
        {
        }

        public override string FormatMessage(params object[] values)
            => string.Format((string)values[0]);

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            RuleVisitor visitor = new RuleVisitor(this, model);
            visitor.Visit(model.SourceFile.ProgramSyntax);
            return visitor.diagnostics;
        }

        private sealed class RuleVisitor : SyntaxVisitor
        {
            public List<IDiagnostic> diagnostics = new List<IDiagnostic>();

            private NoLocationExprOutsideParamsRule parent;
            private SemanticModel model;

            public RuleVisitor(NoLocationExprOutsideParamsRule parent, SemanticModel model)
            {
                this.parent = parent;
                this.model = model;
            }

            public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
            {
                // {deployment,resourceroup}().location are acceptable inside of the "location" parameter's
                //   default value, so don't traverse into the parameter's default value any further because that would
                //   flag those expressions.
                return;
            }

            public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
            {
                // Sub-rule 3: Outside of the default value for parameter location, the expressions resourceGroup().location and deployment().location may not be used in a template.
                // Looking for expressions of the form:
                //
                //   [az.]deployment().location
                //   [az.]resourceGroup().location
                //
                if (LanguageConstants.IdentifierComparer.Equals(syntax.PropertyName.IdentifierName, RGOrDeploymentLocationPropertyName))
                {
                    // We're accessing property 'location' on something.
                    // Is that something a call to {az.,}deployment() or {az.,}resourceGroup()?
                    string? resourceGroupOrDeploymentFunction = syntax.BaseExpression switch
                    {
                        FunctionCallSyntax functionCall =>
                            IsBuiltinFunctionAzCall(
                                functionCall,
                                DeploymentFunctionName,
                                ResourceGroupFunctionName)
                            ? functionCall.Name.IdentifierName
                            : null,

                        InstanceFunctionCallSyntax instanceFunctionCall =>
                            IsBuiltinAzFunctionCall(
                                instanceFunctionCall,
                                DeploymentFunctionName,
                                ResourceGroupFunctionName)
                            ? instanceFunctionCall.Name.IdentifierName
                            : null,

                        _ => null
                    };

                    if (resourceGroupOrDeploymentFunction != null)
                    {
                        string functionCall = $"{resourceGroupOrDeploymentFunction}().{RGOrDeploymentLocationPropertyName}";
                        string msg = String.Format( //asdfg fixes
                                                    //asdfg CoreResources.NoHardcodedLocation_DoNotUseDeploymentOrResourceGroupLocation,
                                "Use a parameter here instead of '{0}'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters.",
                                functionCall
                                );
                        this.diagnostics.Add(parent.CreateDiagnosticForSpan(syntax.Span, msg));
                        ;
                    }
                }

                base.VisitPropertyAccessSyntax(syntax);
            }
        }
    }
}