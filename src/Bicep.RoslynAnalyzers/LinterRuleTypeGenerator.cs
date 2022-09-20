// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace Bicep.RoslynAnalyzers
{
    [Generator(LanguageNames.CSharp)]
    public class LinterRuleTypeGenerator : IIncrementalGenerator
    {
        private const string AttributeNamespace = "Bicep.RoslynAnalyzers";
        private const string AttributeName = "LinterRuleTypesGeneratorAttribute";
        private const string FullyQualifiedAttributeName = AttributeNamespace + "." + AttributeName;
        private const string GeneratorAttributeText = @$"// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace {AttributeNamespace}
{{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class {AttributeName} : Attribute
    {{
        public {AttributeName}()
        {{
        }}
    }}
}}
";

        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            initContext.RegisterPostInitializationOutput(context => context.AddSource($"{nameof(LinterRuleTypeGenerator)}.Attribute.cs", GeneratorAttributeText));

            // TODO: Finding the attribute should use the new ForAttributeWithMetadataName() method, but
            // it requires VS build and dotnet to actually work correctly which may happen in 17.4
            // https://github.com/dotnet/roslyn/pull/63347
            var generatorMethods = initContext.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: (syntax, cancellationToken) => syntax is MethodDeclarationSyntax method && method.AttributeLists.Count > 0,
                    transform: (syntaxContext, cancellationToken) => TryGetGeneratorMethodContext(syntaxContext, cancellationToken))
                .Where(method => method is not null);

            var bicepAnalyzerRuleClasses = initContext.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: (syntax, cancellationToken) => syntax is ClassDeclarationSyntax,
                    transform: (syntaxContext, cancellationToken) => TryGetLinterRuleContext(syntaxContext, cancellationToken))
                .Where(ruleClass => ruleClass is not null);

            var combined = generatorMethods.Combine(bicepAnalyzerRuleClasses.Collect());

            initContext.RegisterSourceOutput(combined, GenerateSource);
        }

        
        private static GeneratorMethodContext? TryGetGeneratorMethodContext(GeneratorSyntaxContext syntaxContext, CancellationToken cancellationToken)
        {
            if (syntaxContext.Node is not MethodDeclarationSyntax method)
            {
                // shouldn't really happen
                return null;
            }

            var attributeSyntax = method.AttributeLists
                .SelectMany(attributeListSyntax => attributeListSyntax.Attributes)
                .FirstOrDefault(attributeSyntax => IsGeneratorAttribute(syntaxContext, attributeSyntax, cancellationToken));

            if(attributeSyntax is null)
            {
                // did not find our generator attribute
                return null;
            }

            var classSyntax = method.FirstAncestorOrSelf<ClassDeclarationSyntax>();
            if (classSyntax is null)
            {
                return null;
            }

            if (syntaxContext.SemanticModel.GetDeclaredSymbol(classSyntax, cancellationToken: cancellationToken) is not INamedTypeSymbol classSymbol)
            {
                return null;
            }

            return new(method, classSymbol);
        }

        private static LinterRuleClassContext? TryGetLinterRuleContext(GeneratorSyntaxContext syntaxContext, CancellationToken cancellationToken)
        {
            if (syntaxContext.Node is not ClassDeclarationSyntax @class ||
                syntaxContext.SemanticModel.GetDeclaredSymbol(@class) is not INamedTypeSymbol classSymbol)
            {
                // shouldn't happen
                return null;
            }

            if(classSymbol.IsAbstract)
            {
                // reject abstract classes
                return null;
            }

            var ruleTypeSymbol = syntaxContext.SemanticModel.Compilation.GetTypeByMetadataName("Bicep.Core.Analyzers.Interfaces.IBicepAnalyzerRule");
            if(ruleTypeSymbol is null)
            {
                // bicep rule interface doesn't exist
                return null;
            }

            if(!syntaxContext.SemanticModel.Compilation.ClassifyConversion(classSymbol, ruleTypeSymbol).IsImplicit)
            {
                // class does not implement the rule interface
                return null;
            }

            return new(classSymbol);
        }

        private static bool IsGeneratorAttribute(GeneratorSyntaxContext syntaxContext, AttributeSyntax attributeSyntax, CancellationToken cancellationToken)
        {
            // get attribute constructor symbol
            if (syntaxContext.SemanticModel.GetSymbolInfo(attributeSyntax, cancellationToken).Symbol is not IMethodSymbol attributeConstructorSymbol)
            {
                return false;
            }

            // attribute constructor symbol should be contained in an attribute type symbol
            var attributeTypeName = attributeConstructorSymbol.ContainingType?.ToDisplayString();

            // c# type names are case-sensitive
            return string.Equals(attributeTypeName, FullyQualifiedAttributeName, StringComparison.Ordinal);
        }

        private void GenerateSource(SourceProductionContext context, (GeneratorMethodContext? generatorContext, ImmutableArray<LinterRuleClassContext?> linterRules) info)
        {
            var (generatorContext, linterRules) = info;

            if (generatorContext?.ClassSymbol.ContainingNamespace is null)
            {
                return;
            }

            var builder = new StringBuilder();
            builder.AppendLine(@"// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
");
                        

            builder.AppendLine(@$"namespace {generatorContext.ClassSymbol.ContainingNamespace.ToDisplayString()}");
            builder.AppendLine("{");

            builder.AppendLine(@$"    public partial class {generatorContext.ClassSymbol.Name}");
            builder.AppendLine("    {");

            builder.AppendLine($@"        public partial IEnumerable<Type> {generatorContext.MethodSyntax.Identifier}()");
            builder.AppendLine("        {");

            builder.AppendLine("            return new[]");
            builder.AppendLine("            {");

            var ruleClassNames = linterRules
                .Select(rule => rule?.RuleClassSymbol.ToDisplayString())
                .OrderBy(className => className);

            foreach(var linterRuleClassName in ruleClassNames)
            {
                builder.AppendLine($"                typeof({linterRuleClassName}),");
            }

            builder.AppendLine("            };");

            builder.AppendLine("        }");
            builder.AppendLine("    }");
            builder.AppendLine("}");

            context.AddSource($"{nameof(LinterRuleTypeGenerator)}.{generatorContext.ClassSymbol.Name}.{generatorContext.MethodSyntax.Identifier}.g.cs", builder.ToString());
        }
    }
}
