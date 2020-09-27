// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core;
using Bicep.Core.SemanticModel;
using Bicep.Core.TypeSystem;
using Bicep.LanguageServer.Snippets;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Completions
{
    public class BicepCompletionProvider : ICompletionProvider
    {
        public IEnumerable<CompletionItem> GetFilteredCompletions(SemanticModel model, BicepCompletionContext context)
        {
            return GetDeclarationCompletions(context)
                .Concat(GetSymbolCompletions(model, context))
                .Concat(GetPrimitiveTypeCompletions(context));
        }

        private IEnumerable<CompletionItem> GetDeclarationCompletions(BicepCompletionContext completionContext)
        {
            if (completionContext.Kind.HasFlag(BicepCompletionContextKind.Declaration))
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
            completionContext.Kind.HasFlag(BicepCompletionContextKind.Declaration) == false
                ? GetAccessibleSymbols(model).Select(sym => sym.ToCompletionItem())
                : Enumerable.Empty<CompletionItem>();

        private IEnumerable<CompletionItem> GetPrimitiveTypeCompletions(BicepCompletionContext completionContext) =>
            completionContext.Kind.HasFlag(BicepCompletionContextKind.Declaration) == false
                ? LanguageConstants.DeclarationTypes.Values.Select(CreateTypeCompletion)
                : Enumerable.Empty<CompletionItem>();

        private IEnumerable<Symbol> GetAccessibleSymbols(SemanticModel model)
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
                .Where(decl => !(decl is OutputSymbol)));

            AddAccessibleSymbols(accessibleSymbols, model.Root.ImportedNamespaces
                .SelectMany(ns => ns.Descendants.OfType<FunctionSymbol>()));

            return accessibleSymbols.Values;
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