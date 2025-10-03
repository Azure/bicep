// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem
{
    public class NestedRuntimeMemberAccessValidator : AstVisitor
    {
        private readonly SemanticModel semanticModel;

        private readonly ResourceTypeResolver resourceTypeResolver;

        private readonly IDiagnosticWriter diagnosticWriter;

        public NestedRuntimeMemberAccessValidator(SemanticModel semanticModel, ResourceTypeResolver resourceTypeResolver, IDiagnosticWriter diagnosticWriter)
        {
            this.semanticModel = semanticModel;
            this.resourceTypeResolver = resourceTypeResolver;
            this.diagnosticWriter = diagnosticWriter;
        }

        public static void Validate(SemanticModel semanticModel, ResourceTypeResolver resourceTypeResolver, IDiagnosticWriter diagnosticWriter)
        {
            var visitor = new NestedRuntimeMemberAccessValidator(semanticModel, resourceTypeResolver, diagnosticWriter);

            visitor.Visit(semanticModel.Root.Syntax);
        }

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            if (syntax.IndexExpression is StringSyntax stringSyntax &&
                this.resourceTypeResolver.TryResolveRuntimeExistingResourceSymbolAndBodyType(syntax.BaseExpression) is ({ } resourceSymbol, { } resourceBodyType))
            {
                if (stringSyntax.TryGetLiteralValue() is { } propertyName)
                {
                    // Validate property access via string literal index (myResource['sku']).
                    this.FlagIfNotReadableAtDeployTime(syntax, propertyName, resourceSymbol, resourceBodyType);
                }
                else
                {
                    // Block property access via interpolated string index (myResource['${myParam}']),
                    // since we we cannot tell whether the property has NestedRuntimeProperty flag or not.
                    this.FlagNestedRuntimeMemberAccess(syntax, resourceSymbol, resourceBodyType);
                }
            }

            base.VisitArrayAccessSyntax(syntax);
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            if (this.resourceTypeResolver.TryResolveRuntimeExistingResourceSymbolAndBodyType(syntax.BaseExpression) is (ResourceSymbol resourceSymbol, { } resourceBodyType) &&
                resourceSymbol.DeclaringResource.IsExistingResource())
            {
                this.FlagIfNotReadableAtDeployTime(syntax, syntax.PropertyName.IdentifierName, resourceSymbol, resourceBodyType);
            }

            base.VisitPropertyAccessSyntax(syntax);
        }

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            // This one checks for runtime functions like storageAccount.listKeys().
            if (this.resourceTypeResolver.TryResolveRuntimeExistingResourceSymbolAndBodyType(syntax.BaseExpression) is (ResourceSymbol resourceSymbol, { } resourceBodyType) &&
                resourceSymbol.DeclaringResource.IsExistingResource())
            {
                this.FlagIfFunctionRequiresInlining(syntax, resourceSymbol, resourceBodyType);
            }

            base.VisitInstanceFunctionCallSyntax(syntax);
        }

        private void FlagIfNotReadableAtDeployTime(SyntaxBase syntax, string propertyName, ResourceSymbol resourceSymbol, ObjectType resourceBodyType)
        {
            if (!AzResourceTypeProvider.ReadWriteDeployTimeConstantPropertyNames.Contains(propertyName) &&
                resourceBodyType.Properties.TryGetValue(propertyName, out var propertyType) &&
                !propertyType.Flags.HasFlag(TypePropertyFlags.ReadableAtDeployTime))
            {
                this.FlagNestedRuntimeMemberAccess(syntax, resourceSymbol, resourceBodyType);
            }
        }

        protected void FlagIfFunctionRequiresInlining(FunctionCallSyntaxBase syntax, ResourceSymbol resourceSymbol, ObjectType resourceBodyType)
        {
            if (this.semanticModel.GetSymbolInfo(syntax) is FunctionSymbol functionSymbol &&
                functionSymbol.FunctionFlags.HasFlag(FunctionFlags.RequiresInlining))
            {
                FlagNestedRuntimeMemberAccess(syntax, resourceSymbol, resourceBodyType);
            }
        }

        private void FlagNestedRuntimeMemberAccess(SyntaxBase errorSyntax, ResourceSymbol resourceSymbol, ObjectType resourceBodyType)
        {
            var resourceSymbolName = resourceSymbol.Name;
            var runtimeIdentifierPropertyNames = GetRuntimeIdentifierPropertyNames(resourceBodyType);
            var accessiblePropertyNames = GetAccessiblePropertyNames(resourceBodyType);
            var accessibleFunctionNames = GetAccessibleFunctionNames(resourceBodyType);

            var diagnostic = DiagnosticBuilder.ForPosition(errorSyntax)
                .NestedRuntimePropertyAccessNotSupported(resourceSymbolName, runtimeIdentifierPropertyNames, accessiblePropertyNames, accessibleFunctionNames);

            this.diagnosticWriter.Write(diagnostic);
        }

        private static IEnumerable<string> GetRuntimeIdentifierPropertyNames(ObjectType resourceBodyType) => resourceBodyType.Properties
            .Where(kv => !kv.Value.Flags.HasFlag(TypePropertyFlags.ReadableAtDeployTime))
            .Select(kv => kv.Key)
            .Intersect(AzResourceTypeProvider.UniqueIdentifierProperties);

        private static IEnumerable<string> GetAccessiblePropertyNames(ObjectType resourceBodyType) => resourceBodyType.Properties
            .Where(kv => kv.Value.Flags.HasFlag(TypePropertyFlags.ReadableAtDeployTime) && !kv.Value.Flags.HasFlag(TypePropertyFlags.WriteOnly))
            .Select(kv => kv.Key)
            .Append(AzResourceTypeProvider.ResourceNamePropertyName) // Adding "name" and "id" because they will be inlined.
            .Append(AzResourceTypeProvider.ResourceIdPropertyName)
            .Distinct();

        private static IEnumerable<string> GetAccessibleFunctionNames(ObjectType resourceBodyType) => resourceBodyType.MethodResolver.functionOverloads
            .Where(x => !x.Flags.HasFlag(FunctionFlags.RequiresInlining))
            .Select(x => x.Name);
    }
}
