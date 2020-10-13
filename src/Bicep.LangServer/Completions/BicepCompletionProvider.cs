// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.LanguageServer.Snippets;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Completions
{
    public class BicepCompletionProvider : ICompletionProvider
    {
        private const string MarkdownNewLine = "  \n";

        public IEnumerable<CompletionItem> GetFilteredCompletions(SemanticModel model, BicepCompletionContext context)
        {
            return GetDeclarationCompletions(context)
                .Concat(GetSymbolCompletions(model, context))
                .Concat(GetDeclarationTypeCompletions(context))
                .Concat(GetObjectPropertyCompletions(model, context));
        }

        private IEnumerable<CompletionItem> GetDeclarationCompletions(BicepCompletionContext completionContext)
        {
            if (completionContext.Kind.HasFlag(BicepCompletionContextKind.DeclarationStart))
            {
                yield return CreateKeywordCompletion(LanguageConstants.ParameterKeyword, "Parameter keyword");
                yield return CreateSnippetCompletion(LanguageConstants.ParameterKeyword, "Parameter declaration", "param ${1:Identifier} ${2:Type}");
                yield return CreateSnippetCompletion(LanguageConstants.ParameterKeyword, "Parameter declaration with default value", "param ${1:Identifier} ${2:Type} = ${3:DefaultValue}");
                yield return CreateSnippetCompletion(LanguageConstants.ParameterKeyword, "Parameter declaration with default and allowed values", @"param ${1:Identifier} ${2:Type} {
  default: $3
  allowed: [
    $4
  ]
}");
                yield return CreateSnippetCompletion(LanguageConstants.ParameterKeyword, "Parameter declaration with options", @"param ${1:Identifier} ${2:Type} {
  $0
}");
                yield return CreateSnippetCompletion(LanguageConstants.ParameterKeyword, "Secure string parameter", @"param ${1:Identifier} string {
  secure: true
}");

                yield return CreateKeywordCompletion(LanguageConstants.VariableKeyword, "Variable keyword");
                yield return CreateSnippetCompletion(LanguageConstants.VariableKeyword, "Variable declaration", "var ${1:Identifier} = $0");

                yield return CreateKeywordCompletion(LanguageConstants.ResourceKeyword, "Resource keyword");
                yield return CreateSnippetCompletion(LanguageConstants.ResourceKeyword, "Resource with defaults", @"resource ${1:Identifier} 'Microsoft.${2:Provider}/${3:Type}@${4:Version}' = {
  name: $5
  location: $6
  properties: {
    $0
  }
}");
                yield return CreateSnippetCompletion(LanguageConstants.ResourceKeyword, "Child Resource with defaults", @"resource ${1:Identifier} 'Microsoft.${2:Provider}/${3:ParentType}/${4:ChildType}@${5:Version}' = {
  name: $6
  properties: {
    $0
  }
}");
                yield return CreateSnippetCompletion(LanguageConstants.ResourceKeyword, "Resource without defaults", @"resource ${1:Identifier} 'Microsoft.${2:Provider}/${3:Type}@${4:Version}' = {
  name: $5
  $0
}
");
                yield return CreateSnippetCompletion(LanguageConstants.ResourceKeyword, "Child Resource without defaults", @"resource ${1:Identifier} 'Microsoft.${2:Provider}/${3:ParentType}/${4:ChildType}@${5:Version}' = {
  name: $6
  $0
}");

                yield return CreateKeywordCompletion(LanguageConstants.OutputKeyword, "Output keyword");
                yield return CreateSnippetCompletion(LanguageConstants.OutputKeyword, "Output declaration", "output ${1:Identifier} ${2:Type} = $0");
            }
        }

        private IEnumerable<CompletionItem> GetSymbolCompletions(SemanticModel model, BicepCompletionContext completionContext) =>
            completionContext.Kind == BicepCompletionContextKind.None
                ? GetAccessibleSymbols(model).Select(sym => sym.ToCompletionItem())
                : Enumerable.Empty<CompletionItem>();

        private IEnumerable<CompletionItem> GetDeclarationTypeCompletions(BicepCompletionContext completionContext)
        {
            // local function
            IEnumerable<CompletionItem> GetPrimitiveTypeCompletions() =>
                LanguageConstants.DeclarationTypes.Values.Select(CreateTypeCompletion);


            if (completionContext.Kind.HasFlag(BicepCompletionContextKind.ParameterType))
            {
                return GetPrimitiveTypeCompletions().Concat(GetParameterTypeSnippets());
            }

            if (completionContext.Kind.HasFlag(BicepCompletionContextKind.OutputType))
            {
                return GetPrimitiveTypeCompletions();
            }

            return Enumerable.Empty<CompletionItem>();
        }


        private static IEnumerable<CompletionItem> GetParameterTypeSnippets()
        {
            yield return CreateSnippetCompletion("secureObject", "Secure object", @"object {
  secure: true
}");

            yield return CreateSnippetCompletion("secureString", "Secure string", @"string {
  secure: true
}");
        }

        private static IEnumerable<Symbol> GetAccessibleSymbols(SemanticModel model)
        {
            var accessibleSymbols = new Dictionary<string, Symbol>();

            // local function
            void AddAccessibleSymbols(IDictionary<string, Symbol> result, IEnumerable<Symbol> symbols)
            {
                foreach (var declaration in symbols)
                {
                    if (result.ContainsKey(declaration.Name) == false)
                    {
                        result.Add(declaration.Name, declaration);
                    }
                }
            }

            AddAccessibleSymbols(accessibleSymbols, model.Root.AllDeclarations
                .Where(decl => decl.NameSyntax.IsValid && !(decl is OutputSymbol)));

            AddAccessibleSymbols(accessibleSymbols, model.Root.ImportedNamespaces
                .SelectMany(kvp => kvp.Value.Descendants.OfType<FunctionSymbol>()));
            return accessibleSymbols.Values;
        }

        private IEnumerable<CompletionItem> GetObjectPropertyCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.PropertyName) == false || context.Object == null)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            // in order to provide completions for property names,
            // we need to establish the type of the object first
            var type = model.GetTypeInfo(context.Object);

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
                .Select(CreatePropertyCompletion);
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

        private static CompletionItem CreatePropertyCompletion(TypeProperty property)
        {
            return new CompletionItem
            {
                Kind = CompletionItemKind.Property,
                Label = property.Name,
                InsertTextFormat = InsertTextFormat.PlainText,
                // property names containg spaces need to be escaped
                InsertText = Lexer.IsValidIdentifier(property.Name) ? property.Name : StringUtils.EscapeBicepString(property.Name),
                CommitCharacters = new Container<string>(" ", ":"),
                Detail = FormatPropertyDetail(property),
                Documentation = new StringOrMarkupContent(new MarkupContent
                {
                    Kind = MarkupKind.Markdown,
                    Value = FormatPropertyDocumentation(property)
                })
            };
        }

        private static string FormatPropertyDetail(TypeProperty property) =>
            property.Flags.HasFlag(TypePropertyFlags.Required)
                ? $"{property.Name} (Required)"
                : property.Name;

        private static string FormatPropertyDocumentation(TypeProperty property)
        {
            var buffer = new StringBuilder();

            buffer.Append($"Type: `{property.TypeReference.Type}`{MarkdownNewLine}");

            if (property.Flags.HasFlag(TypePropertyFlags.ReadOnly))
            {
                // this case will be used for dot property access completions
                // this flag is not possible in property name completions
                buffer.Append($"Read-only property{MarkdownNewLine}");
            }

            if (property.Flags.HasFlag(TypePropertyFlags.WriteOnly))
            {
                buffer.Append($"Write-only property{MarkdownNewLine}");
            }

            if (property.Flags.HasFlag(TypePropertyFlags.Constant))
            {
                buffer.Append($"Requires a compile-time constant value.{MarkdownNewLine}");
            }

            return buffer.ToString();
        }

        private static CompletionItem CreateKeywordCompletion(string keyword, string detail) =>
            new CompletionItem
            {
                Kind = CompletionItemKind.Keyword,
                Label = keyword,
                InsertTextFormat = InsertTextFormat.PlainText,
                InsertText = keyword,
                CommitCharacters = new Container<string>(" "),
                Detail = detail
            };

        private static CompletionItem CreateSnippetCompletion(string label, string detail, string snippet)
        {
            return new CompletionItem
            {
                Kind = CompletionItemKind.Snippet,
                Label = label,
                InsertTextFormat = InsertTextFormat.Snippet,
                InsertText = snippet,
                Detail = detail,
                Documentation = new StringOrMarkupContent(new MarkupContent
                {
                    Kind = MarkupKind.Markdown,
                    Value = $"```bicep\n{new Snippet(snippet).FormatDocumentation()}\n```"
                })
            };
        }

        private static CompletionItem CreateTypeCompletion(TypeSymbol type) =>
            new CompletionItem
            {
                Kind = CompletionItemKind.Class,
                Label = type.Name,
                InsertTextFormat = InsertTextFormat.PlainText,
                InsertText = type.Name,
                CommitCharacters = new Container<string>(" "),
                Detail = type.Name
            };
    }
}