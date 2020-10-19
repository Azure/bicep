// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Completions
{
    public class BicepCompletionProvider : ICompletionProvider
    {
        public IEnumerable<CompletionItem> GetFilteredCompletions(SemanticModel model, BicepCompletionContext context)
        {
            return GetDeclarationCompletions(context)
                .Concat(GetSymbolCompletions(model, context))
                .Concat(GetDeclarationTypeCompletions(context))
                .Concat(GetObjectPropertyNameCompletions(model, context))
                .Concat(GetPropertyValueCompletions(model, context))
                .Concat(GetArrayItemCompletions(model, context));
        }

        private IEnumerable<CompletionItem> GetDeclarationCompletions(BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.DeclarationStart))
            {
                yield return CompletionItemFactory.CreateKeywordCompletion(LanguageConstants.ParameterKeyword, "Parameter keyword");
                yield return CompletionItemFactory.CreateContextualSnippetCompletion(LanguageConstants.ParameterKeyword, "Parameter declaration", "param ${1:Identifier} ${2:Type}");
                yield return CompletionItemFactory.CreateContextualSnippetCompletion(LanguageConstants.ParameterKeyword, "Parameter declaration with default value", "param ${1:Identifier} ${2:Type} = ${3:DefaultValue}");
                yield return CompletionItemFactory.CreateContextualSnippetCompletion(LanguageConstants.ParameterKeyword, "Parameter declaration with default and allowed values", @"param ${1:Identifier} ${2:Type} {
  default: $3
  allowed: [
    $4
  ]
}");
                yield return CompletionItemFactory.CreateContextualSnippetCompletion(LanguageConstants.ParameterKeyword, "Parameter declaration with options", @"param ${1:Identifier} ${2:Type} {
  $0
}");
                yield return CompletionItemFactory.CreateContextualSnippetCompletion(LanguageConstants.ParameterKeyword, "Secure string parameter", @"param ${1:Identifier} string {
  secure: true
}");

                yield return CompletionItemFactory.CreateKeywordCompletion(LanguageConstants.VariableKeyword, "Variable keyword");
                yield return CompletionItemFactory.CreateContextualSnippetCompletion(LanguageConstants.VariableKeyword, "Variable declaration", "var ${1:Identifier} = $0");

                yield return CompletionItemFactory.CreateKeywordCompletion(LanguageConstants.ResourceKeyword, "Resource keyword");
                yield return CompletionItemFactory.CreateContextualSnippetCompletion(LanguageConstants.ResourceKeyword, "Resource with defaults", @"resource ${1:Identifier} 'Microsoft.${2:Provider}/${3:Type}@${4:Version}' = {
  name: $5
  location: $6
  properties: {
    $0
  }
}");
                yield return CompletionItemFactory.CreateContextualSnippetCompletion(LanguageConstants.ResourceKeyword, "Child Resource with defaults", @"resource ${1:Identifier} 'Microsoft.${2:Provider}/${3:ParentType}/${4:ChildType}@${5:Version}' = {
  name: $6
  properties: {
    $0
  }
}");
                yield return CompletionItemFactory.CreateContextualSnippetCompletion(LanguageConstants.ResourceKeyword, "Resource without defaults", @"resource ${1:Identifier} 'Microsoft.${2:Provider}/${3:Type}@${4:Version}' = {
  name: $5
  $0
}
");
                yield return CompletionItemFactory.CreateContextualSnippetCompletion(LanguageConstants.ResourceKeyword, "Child Resource without defaults", @"resource ${1:Identifier} 'Microsoft.${2:Provider}/${3:ParentType}/${4:ChildType}@${5:Version}' = {
  name: $6
  $0
}");

                yield return CompletionItemFactory.CreateKeywordCompletion(LanguageConstants.OutputKeyword, "Output keyword");
                yield return CompletionItemFactory.CreateContextualSnippetCompletion(LanguageConstants.OutputKeyword, "Output declaration", "output ${1:Identifier} ${2:Type} = $0");
            }
        }

        private IEnumerable<CompletionItem> GetSymbolCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (context.Kind != BicepCompletionContextKind.None)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            // when we're inside an expression that is inside a property that expects a compile-time constant value,
            // we should not be emitting accessible symbol completions
            return GetAccessibleSymbols(model, context).Select(SymbolExtensions.ToCompletionItem);
        }

        private IEnumerable<CompletionItem> GetDeclarationTypeCompletions(BicepCompletionContext context)
        {
            // local function
            IEnumerable<CompletionItem> GetPrimitiveTypeCompletions() =>
                LanguageConstants.DeclarationTypes.Values.Select(CompletionItemFactory.CreateTypeCompletion);


            if (context.Kind.HasFlag(BicepCompletionContextKind.ParameterType))
            {
                return GetPrimitiveTypeCompletions().Concat(GetParameterTypeSnippets());
            }

            if (context.Kind.HasFlag(BicepCompletionContextKind.OutputType))
            {
                return GetPrimitiveTypeCompletions();
            }

            return Enumerable.Empty<CompletionItem>();
        }


        private static IEnumerable<CompletionItem> GetParameterTypeSnippets()
        {
            yield return CompletionItemFactory.CreateContextualSnippetCompletion("secureObject", "Secure object", @"object {
  secure: true
}");

            yield return CompletionItemFactory.CreateContextualSnippetCompletion("secureString", "Secure string", @"string {
  secure: true
}");
        }

        private static IEnumerable<Symbol> GetAccessibleSymbols(SemanticModel model, BicepCompletionContext context)
        {
            var accessibleSymbols = new Dictionary<string, Symbol>();

            var enclosingDeclarationSymbol = context.EnclosingDeclaration == null 
                ? null
                : model.GetSymbolInfo(context.EnclosingDeclaration);

            // local function
            void AddAccessibleSymbols(IDictionary<string, Symbol> result, IEnumerable<Symbol> symbols)
            {
                foreach (var symbol in symbols)
                {
                    if (result.ContainsKey(symbol.Name))
                    {
                        // a local declaration has the same name as a function
                        // we should leave the completions for local symbol but should not include the completions for the function
                        continue;
                    }

                    if (ReferenceEquals(symbol, enclosingDeclarationSymbol))
                    {
                        // the symbol is the same as the symbol of the enclosing declaration
                        // if we produce a completion with the matching identifier and the user selects it,
                        // this will cause a cycle. even thought the type checker will catch it, let's not suggest it to the user
                        continue;
                    }

                    result.Add(symbol.Name, symbol);
                }
            }

            AddAccessibleSymbols(accessibleSymbols, model.Root.AllDeclarations
                .Where(decl => decl.NameSyntax.IsValid && !(decl is OutputSymbol)));

            AddAccessibleSymbols(accessibleSymbols, model.Root.ImportedNamespaces
                .SelectMany(kvp => kvp.Value.Descendants.OfType<FunctionSymbol>()));
            return accessibleSymbols.Values;
        }

        private IEnumerable<CompletionItem> GetObjectPropertyNameCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.PropertyName) == false || context.Object == null)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            // in order to provide completions for property names,
            // we need to establish the type of the object first
            var declaredType = model.GetDeclaredType(context.Object);
            if (declaredType == null)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            var specifiedPropertyNames = context.Object.ToKnownPropertyNames();

            // exclude read-only properties as they can't be set
            // exclude properties whose name has been specified in the object already
            return GetProperties(declaredType)
                .Where(p => p.Flags.HasFlag(TypePropertyFlags.ReadOnly) == false && specifiedPropertyNames.Contains(p.Name) == false)
                .Select(CompletionItemFactory.CreatePropertyNameCompletion);
        }

        private static IEnumerable<TypeProperty> GetProperties(TypeSymbol type)
        {
            switch (type)
            {
                case ObjectType objectType:
                    return objectType.Properties.Values;

                case DiscriminatedObjectType discriminated:
                    return discriminated.DiscriminatorProperty.AsEnumerable();

                default:
                    return Enumerable.Empty<TypeProperty>();
            }
        }

        private static DeclaredTypeAssignment? GetDeclaredTypeAssignment(SemanticModel model, SyntaxBase? syntax) => syntax == null
            ? null
            : model.GetDeclaredTypeAssignment(syntax);

        private IEnumerable<CompletionItem> GetPropertyValueCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.PropertyValue))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            var declaredTypeAssignment = GetDeclaredTypeAssignment(model, context.Property);
            if(declaredTypeAssignment == null)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            return GetValueCompletions(model, context, declaredTypeAssignment.Reference.Type, declaredTypeAssignment.Flags);
        }

        private IEnumerable<CompletionItem> GetArrayItemCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.ArrayItem))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            var declaredTypeAssignment = GetDeclaredTypeAssignment(model, context.Array);
            if (declaredTypeAssignment == null || !(declaredTypeAssignment.Reference.Type is ArrayType arrayType))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            return GetValueCompletions(model, context, arrayType.Item.Type, declaredTypeAssignment.Flags);
        }

        private static IEnumerable<CompletionItem> GetValueCompletions(SemanticModel model, BicepCompletionContext context, TypeSymbol type, DeclaredTypeFlags flags)
        {
            var completions = GetValueCompletionsForType(type);

            if (flags != DeclaredTypeFlags.Constant)
            {
                completions = completions.Concat(GetAccessibleSymbols(model, context).Select(SymbolExtensions.ToCompletionItem));
            }

            return completions;
        }

        private static IEnumerable<CompletionItem> GetValueCompletionsForType(TypeSymbol? propertyType)
        {
            switch (propertyType)
            {
                case PrimitiveType _ when ReferenceEquals(propertyType, LanguageConstants.Bool):
                    yield return CompletionItemFactory.CreateKeywordCompletion(LanguageConstants.TrueKeyword, LanguageConstants.TrueKeyword);
                    yield return CompletionItemFactory.CreateKeywordCompletion(LanguageConstants.FalseKeyword, LanguageConstants.FalseKeyword);
                    
                    break;

                case StringLiteralType stringLiteral:
                    yield return CompletionItemFactory.CreatePlaintextCompletion(CompletionItemKind.EnumMember, stringLiteral.Name, stringLiteral.Name);
                    
                    break;

                case ArrayType _:
                    yield return CompletionItemFactory.CreateSnippetCompletion(CompletionItemKind.Value, "[]", "[$0]", "[]");
                    
                    break;

                case ObjectType _:
                    yield return CompletionItemFactory.CreateSnippetCompletion(CompletionItemKind.Value, "{}", "{$0}", "{}");
                    
                    break;

                case UnionType union:
                    var aggregatedCompletions = union.Members.SelectMany(typeRef => GetValueCompletionsForType(typeRef.Type));
                    foreach (var completion in aggregatedCompletions)
                    {
                        yield return completion;
                    }

                    break;
            }
        }
    }
}