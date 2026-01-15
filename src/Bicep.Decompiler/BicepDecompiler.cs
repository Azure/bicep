// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Azure.Core;
using Azure.Deployments.Core.Definitions.Identifiers;
using Bicep.Core;
using Bicep.Core.Decompiler.Rewriters;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Rewriters;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Decompiler.ArmHelpers;
using Bicep.Decompiler.Exceptions;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Bicep.Decompiler;

public class BicepDecompiler
{
    public static BicepDecompiler Create(Action<IServiceCollection>? configureServices = null)
    {
        var services = new ServiceCollection();
        configureServices?.Invoke(services);

        services.AddBicepDecompiler();

        return services
            .BuildServiceProvider()
            .GetRequiredService<BicepDecompiler>();
    }

    private readonly BicepCompiler bicepCompiler;

    public static string DecompilerDisclaimerMessage => DecompilerResources.DecompilerDisclaimerMessage;

    public BicepDecompiler(BicepCompiler bicepCompiler)
    {
        this.bicepCompiler = bicepCompiler;
    }

    public async Task<DecompileResult> Decompile(IOUri bicepUri, string jsonContent, DecompileOptions? options = null)
    {
        var workspace = new ActiveSourceFileSet();
        var decompileQueue = new Queue<(IOUri, IOUri)>();
        options ??= new DecompileOptions();

        var (program, jsonTemplateUrisByModule) = TemplateConverter.DecompileTemplate(bicepCompiler.SourceFileFactory, workspace, bicepUri, jsonContent, options);
        var bicepFile = this.bicepCompiler.SourceFileFactory.CreateBicepFile(bicepUri, program.ToString());
        workspace.UpsertSourceFile(bicepFile);

        await RewriteSyntax(workspace, bicepUri, semanticModel => new ParentChildResourceNameRewriter(semanticModel));
        await RewriteSyntax(workspace, bicepUri, semanticModel => new DependsOnRemovalRewriter(semanticModel));
        await RewriteSyntax(workspace, bicepUri, semanticModel => new ForExpressionSimplifierRewriter(semanticModel));
        for (var i = 0; i < 5; i++)
        {
            // This is a little weird. If there are casing issues nested inside casing issues (e.g. in an object), then the inner casing issue will have no type information
            // available, as the compilation will not have associated a type with it (since there was no match on the outer object). So we need to correct the outer issue first,
            // and then move to the inner one. We need to recompute the entire compilation to do this. It feels simpler to just do this in passes over the file, rather than on demand.
            if (!await RewriteSyntax(workspace, bicepUri, semanticModel => new TypeCasingFixerRewriter(semanticModel)))
            {
                break;
            }
        }

        return new DecompileResult(
            bicepUri,
            PrintFiles(workspace));
    }

    public DecompileResult DecompileParameters(string contents, IOUri entryBicepparamUri, IOUri? bicepFileUri, DecompileParamOptions? options = null)
    {
        options ??= new();

        var workspace = new ActiveSourceFileSet();

        var program = DecompileParametersFile(contents, entryBicepparamUri, bicepFileUri, options);

        var bicepparamFile = this.bicepCompiler.SourceFileFactory.CreateBicepParamFile(entryBicepparamUri, program.ToString());

        workspace.UpsertSourceFile(bicepparamFile);

        return new(entryBicepparamUri, PrintFiles(workspace));
    }

    private ProgramSyntax DecompileParametersFile(string jsonInput, IOUri entryBicepparamUri, IOUri? bicepFileUri, DecompileParamOptions options)
    {
        var statements = new List<SyntaxBase>();

        var jsonObject = JTokenHelpers.LoadJson(jsonInput, JObject.Load, ignoreTrailingContent: options.IgnoreTrailingInput);

        if (options.IncludeUsingDeclaration)
        {
            var bicepPath = bicepFileUri?.GetPathRelativeTo(entryBicepparamUri);
            statements.Add(new UsingDeclarationSyntax(
                [],
                SyntaxFactory.UsingKeywordToken,
                bicepPath is not null
                    ? SyntaxFactory.CreateStringLiteral(bicepPath)
                    : SyntaxFactory.CreateStringLiteralWithComment("", "TODO: Provide a path to a bicep template"),
                SyntaxFactory.EmptySkippedTrivia));

            statements.Add(SyntaxFactory.DoubleNewlineToken);
        }

        var parameters = (TemplateHelpers.GetProperty(jsonObject, "parameters")?.Value as JObject ?? new JObject()).Properties();

        foreach (var parameter in parameters)
        {
            var metadata = parameter.Value["metadata"];

            if (metadata is not null)
            {
                statements.Add(ParseParameterWithComment(metadata));
                statements.Add(SyntaxFactory.NewlineToken);
            }

            statements.Add(ParseParam(parameter));
            statements.Add(SyntaxFactory.DoubleNewlineToken);
        }

        var program = new ProgramSyntax(statements, SyntaxFactory.CreateToken(Core.Parsing.TokenType.EndOfFile));

        return program;
    }

    private static SyntaxBase ParseParam(JProperty param)
    {
        if (param.Value?["reference"] is JObject reference)
        {
            return SyntaxFactory.CreateParameterAssignmentSyntax(
                param.Name,
                ParseKeyVaultReference(param.Name, reference));
        }

        var value = param.Value?["value"];

        if (value is null)
        {
            throw new ConversionFailedException($"No value found parameter {param.Name}", param);
        }

        return SyntaxFactory.CreateParameterAssignmentSyntax(
            param.Name,
            ParseJToken(value));
    }

    private static SyntaxBase ParseJToken(JToken value)
        => value switch
        {
            JObject jObject => ParseJObject(jObject),
            JArray jArray => ParseJArray(jArray),
            JValue jValue => ParseJValue(jValue),
            _ => throw new ConversionFailedException($"Unrecognized token type {value.Type}", value),
        };

    private static SyntaxBase ParseJValue(JValue value)
    => value.Type switch
    {
        JTokenType.Integer => SyntaxFactory.CreatePositiveOrNegativeInteger(value.Value<long>()),
        JTokenType.String => SyntaxFactory.CreateStringLiteral(value.ToString()),
        JTokenType.Boolean => SyntaxFactory.CreateBooleanLiteral(value.Value<bool>()),
        JTokenType.Null => SyntaxFactory.CreateNullLiteral(),
        _ => throw new NotImplementedException($"Unrecognized token type {value.Type}")
    };

    private static SyntaxBase ParseJArray(JArray jArray)
    {
        var itemSyntaxes = new List<SyntaxBase>();

        foreach (var item in jArray)
        {
            itemSyntaxes.Add(ParseJToken(item));
        }

        return SyntaxFactory.CreateArray(itemSyntaxes);
    }

    private static SyntaxBase ParseJObject(JObject jObject)
    {
        var propertySyntaxes = new List<ObjectPropertySyntax>();

        foreach (var property in jObject.Properties())
        {
            propertySyntaxes.Add(SyntaxFactory.CreateObjectProperty(property.Name, ParseJToken(property.Value)));
        }

        return SyntaxFactory.CreateObject(propertySyntaxes);
    }

    private static SyntaxBase ParseParameterWithComment(JToken jToken)
    {
        var metadata = jToken.ToString();
        var commentSyntax = SyntaxFactory.CreateEmptySyntaxWithComment(
$@"
Parameter metadata is not supported in Bicep Parameters files

Following metadata was not decompiled:
{metadata}
");

        return commentSyntax;
    }

    private static SyntaxBase ParseKeyVaultReference(string paramName, JObject kvReference)
    {
        var keyVault = kvReference["keyVault"]?["id"]?.ToString();
        var secretName = kvReference["secretName"]?.ToString();
        var secretVersion = kvReference["secretVersion"]?.ToString();

        if (string.IsNullOrWhiteSpace(keyVault) || string.IsNullOrWhiteSpace(secretName))
        {
            throw new ConversionFailedException($"Invalid Key Vault reference for parameter {paramName}. Key vault Id and secret name are required.", kvReference);
        }


        if (!ResourceIdentifier.TryParse(keyVault, out var resourceId) || resourceId is null)
        {
            throw new ConversionFailedException($"Invalid Key Vault reference for parameter {paramName}. Key vault Id is not a valid resource Id.", kvReference);
        }

        var subscriptionId = resourceId.SubscriptionId;
        var resourceGroup = resourceId.ResourceGroupName;
        var vaultName = resourceId.Name;

        if (string.IsNullOrWhiteSpace(subscriptionId) ||
            string.IsNullOrWhiteSpace(resourceGroup) ||
            string.IsNullOrWhiteSpace(vaultName))
        {
            throw new ConversionFailedException($"Invalid Key Vault resource Id for parameter {paramName}. Subscription Id, resource group name and vault name are required.", kvReference);
        }

        var args = new List<SyntaxBase>
        {
            SyntaxFactory.CreateStringLiteral(subscriptionId),
            SyntaxFactory.CreateStringLiteral(resourceGroup),
            SyntaxFactory.CreateStringLiteral(vaultName),
            SyntaxFactory.CreateStringLiteral(secretName),
        };

        if (!string.IsNullOrEmpty(secretVersion))
        {
            args.Add(SyntaxFactory.CreateStringLiteral(secretVersion));
        }

        return SyntaxFactory.CreateInstanceFunctionCall(
            SyntaxFactory.CreateIdentifier(AzNamespaceType.BuiltInName),
            AzNamespaceType.GetSecretFunctionName,
            [.. args]);
    }

    public static string? DecompileJsonValue(ISourceFileFactory sourceFileFactory, string jsonInput, DecompileOptions? options = null)
    {
        var workspace = new ActiveSourceFileSet();
        options ??= new DecompileOptions();

        var bicepUri = new IOUri("file", "", "/jsonInput.json");
        try
        {
            var syntax = TemplateConverter.DecompileJsonValue(sourceFileFactory, workspace, bicepUri, jsonInput, options);

            // TODO: Add bicepUri to BicepDecompileForPasteCommandParams to get actual formatting options.
            var context = PrettyPrinterV2Context.Create(PrettyPrinterV2Options.Default, EmptyDiagnosticLookup.Instance, EmptyDiagnosticLookup.Instance);

            return syntax is not null ? PrettyPrinterV2.Print(syntax, context) : null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static ImmutableDictionary<IOUri, string> PrintFiles(ActiveSourceFileSet activeSourceFiles)
    {
        var filesToSave = new Dictionary<IOUri, string>();
        foreach (var sourceFile in activeSourceFiles)
        {
            if (sourceFile is not BicepSourceFile bicepFile)
            {
                continue;
            }

            var options = bicepFile.Configuration.Formatting.Data;
            var context = PrettyPrinterV2Context.Create(options, bicepFile.LexingErrorLookup, bicepFile.ParsingErrorLookup);
            filesToSave[sourceFile.FileHandle.Uri] = PrettyPrinterV2.Print(bicepFile.ProgramSyntax, context);
        }

        return filesToSave.ToImmutableDictionary();
    }

    private async Task<bool> RewriteSyntax(ActiveSourceFileSet workspace, IOUri entryUri, Func<SemanticModel, SyntaxRewriteVisitor> rewriteVisitorBuilder)
    {
        var hasChanges = false;
        var compilation = await bicepCompiler.CreateCompilation(entryUri, workspace, skipRestore: true, forceRestore: false);

        // force enumeration here with .ToImmutableArray() as we're going to be modifying the sourceFileGrouping collection as we iterate
        var sourceFiles = compilation.SourceFileGrouping.SourceFiles.ToImmutableArray();
        foreach (var bicepFile in sourceFiles.OfType<BicepFile>())
        {
            var newProgramSyntax = rewriteVisitorBuilder(compilation.GetSemanticModel(bicepFile)).Rewrite(bicepFile.ProgramSyntax);

            if (!object.ReferenceEquals(bicepFile.ProgramSyntax, newProgramSyntax))
            {
                hasChanges = true;
                var newFile = this.bicepCompiler.SourceFileFactory.CreateBicepFile(bicepFile.FileHandle.Uri, newProgramSyntax.ToString());
                workspace.UpsertSourceFile(newFile);

                compilation = await bicepCompiler.CreateCompilation(entryUri, workspace, skipRestore: true);
            }
        }

        return hasChanges;
    }
}
