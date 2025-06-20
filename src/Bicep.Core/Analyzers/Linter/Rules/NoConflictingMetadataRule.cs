// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class NoConflictingMetadataRule : LinterRuleBase
{
    public new const string Code = "no-conflicting-metadata";

    public NoConflictingMetadataRule() : base(
        code: Code,
        description: CoreResources.NoConflictingMetadataRuleDescription,
        LinterRuleCategory.PotentialCodeIssues)
    { }

    public override string FormatMessage(params object[] values)
        => string.Format(CoreResources.NoConflictingMetadataRuleMessageFormat, values);

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        => Visitor.FindMetadataPropertiesInConflictWithDecorators(model)
            .Select(conflict => CreateDiagnosticForSpan(diagnosticLevel,
                conflict.OffendingMetadataProperty.Key.Span,
                conflict.ConflictInfo.MetadataPropertyName,
                conflict.ConflictInfo.DecoratorName));

    private record MetadataPropertySetByDecorator(string MetadataPropertyName, string DecoratorNamespace, string DecoratorName) { }

    private class Visitor : AstVisitor
    {
        private static readonly ImmutableArray<MetadataPropertySetByDecorator> metadataPropertySetByDecorators =
        [
            new(LanguageConstants.MetadataDescriptionPropertyName, SystemNamespaceType.BuiltInName, LanguageConstants.MetadataDescriptionPropertyName),
        ];

        private readonly List<(ObjectPropertySyntax, MetadataPropertySetByDecorator)> metadataPropertiesInConflictWithDecorators = new();
        private readonly SemanticModel model;

        private Visitor(SemanticModel model)
        {
            this.model = model;
        }

        public static IEnumerable<(ObjectPropertySyntax OffendingMetadataProperty, MetadataPropertySetByDecorator ConflictInfo)>
        FindMetadataPropertiesInConflictWithDecorators(SemanticModel model)
        {
            Visitor visitor = new(model);
            model.SourceFile.ProgramSyntax.Accept(visitor);

            return visitor.metadataPropertiesInConflictWithDecorators;
        }

        protected override void VisitInternal(SyntaxBase node)
        {
            if (node is DecorableSyntax decorable && GetMetadataObject(decorable) is { } metadataObject)
            {
                foreach (var potentialConflict in metadataPropertySetByDecorators)
                {
                    metadataPropertiesInConflictWithDecorators.AddRange(metadataObject.Properties
                        .Where(property => property.TryGetKeyText() is string keyName &&
                            LanguageConstants.IdentifierComparer.Equals(keyName, potentialConflict.MetadataPropertyName) &&
                            SemanticModelHelper.TryGetDecoratorInNamespace(model, decorable, potentialConflict.DecoratorNamespace, potentialConflict.DecoratorName) is not null)
                        .Select(property => (property, potentialConflict)));
                }
            }
        }

        private ObjectSyntax? GetMetadataObject(DecorableSyntax syntax)
            => SemanticModelHelper.TryGetDecoratorInNamespace(model, syntax, SystemNamespaceType.BuiltInName, LanguageConstants.ParameterMetadataPropertyName) is { } mdDecorator
                ? mdDecorator.Arguments.FirstOrDefault()?.Expression as ObjectSyntax
                : null;
    }
}
