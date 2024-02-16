// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core;
using Bicep.Core.Decompiler.Rewriters;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Rewriters;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Bicep.Decompiler.ArmHelpers;
using Bicep.Decompiler.Exceptions;
using Newtonsoft.Json.Linq;

namespace Bicep.Decompiler;

public class BicepDecompiler
{
    private readonly BicepCompiler bicepCompiler;

    public static string DecompilerDisclaimerMessage => DecompilerResources.DecompilerDisclaimerMessage;

    public BicepDecompiler(BicepCompiler bicepCompiler)
    {
        this.bicepCompiler = bicepCompiler;
    }

    public async Task<DecompileResult> Decompile(Uri bicepUri, string jsonContent, DecompileOptions? options = null)
    {
        var workspace = new Workspace();
        var decompileQueue = new Queue<(Uri, Uri)>();
        options ??= new DecompileOptions();

        var (program, jsonTemplateUrisByModule) = TemplateConverter.DecompileTemplate(workspace, bicepUri, jsonContent, options);
        var bicepFile = SourceFileFactory.CreateBicepFile(bicepUri, program.ToString());
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
            this.PrintFiles(workspace));
    }
    public DecompileResult DecompileParameters(string contents, Uri entryBicepparamUri, Uri? bicepFileUri)
    {
        var workspace = new Workspace();

        var program = DecompileParametersFile(contents, entryBicepparamUri, bicepFileUri);

        var bicepparamFile = SourceFileFactory.CreateBicepParamFile(entryBicepparamUri, program.ToString());

        workspace.UpsertSourceFile(bicepparamFile);

        return new DecompileResult(entryBicepparamUri, this.PrintFiles(workspace));
    }

    private ProgramSyntax DecompileParametersFile(string jsonInput, Uri entryBicepparamUri, Uri? bicepFileUri)
    {
        var statements = new List<SyntaxBase>();

        var jsonObject = JTokenHelpers.LoadJson(jsonInput, JObject.Load, ignoreTrailingContent: false);
        var bicepPath = bicepFileUri is { } ? PathHelper.GetRelativePath(entryBicepparamUri, bicepFileUri) : null;

        statements.Add(new UsingDeclarationSyntax(
            SyntaxFactory.UsingKeywordToken,
            bicepPath is { } ?
            SyntaxFactory.CreateStringLiteral(bicepPath) :
            SyntaxFactory.CreateStringLiteralWithComment("", "TODO: Provide a path to a bicep template")));

        statements.Add(SyntaxFactory.DoubleNewlineToken);

        var parameters = (TemplateHelpers.GetProperty(jsonObject, "parameters")?.Value as JObject ?? new JObject()).Properties();

        foreach (var parameter in parameters)
        {
            var metadata = parameter.Value?["metadata"];

            if (metadata is { })
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
        if (param.Value?["reference"] is not null)
        {
            return SyntaxFactory.CreateParameterAssignmentSyntax(
                param.Name,
                SyntaxFactory.CreateInvalidSyntaxWithComment("KeyVault references are not supported in Bicep Parameters files"));
        }

        var value = param.Value?["value"];

        if (value is null)
        {
            throw new Exception($"No value found parameter {param.Name}");
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

    public static string? DecompileJsonValue(string jsonInput, DecompileOptions? options = null)
    {
        var workspace = new Workspace();
        options ??= new DecompileOptions();

        var bicepUri = new Uri("file://jsonInput.json", UriKind.Absolute);
        try
        {
            var syntax = TemplateConverter.DecompileJsonValue(workspace, bicepUri, jsonInput, options);

            // TODO: Add bicepUri to BicepDecompileForPasteCommandParams to get actual formatting options.
            var context = PrettyPrinterV2Context.Create(PrettyPrinterV2Options.Default, EmptyDiagnosticLookup.Instance, EmptyDiagnosticLookup.Instance);

            return syntax is not null ? PrettyPrinterV2.Print(syntax, context) : null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private ImmutableDictionary<Uri, string> PrintFiles(Workspace workspace)
    {
        var filesToSave = new Dictionary<Uri, string>();
        foreach (var (fileUri, sourceFile) in workspace.GetActiveSourceFilesByUri())
        {
            if (sourceFile is not BicepSourceFile bicepFile)
            {
                continue;
            }

            var options = this.bicepCompiler.ConfigurationManager.GetConfiguration(fileUri).Formatting.Data;
            var context = PrettyPrinterV2Context.Create(options, bicepFile.LexingErrorLookup, bicepFile.ParsingErrorLookup);
            filesToSave[fileUri] = PrettyPrinterV2.Print(bicepFile.ProgramSyntax, context);
        }

        return filesToSave.ToImmutableDictionary();
    }

    private async Task<bool> RewriteSyntax(Workspace workspace, Uri entryUri, Func<SemanticModel, SyntaxRewriteVisitor> rewriteVisitorBuilder)
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
                var newFile = SourceFileFactory.CreateBicepFile(bicepFile.FileUri, newProgramSyntax.ToString());
                workspace.UpsertSourceFile(newFile);

                compilation = await bicepCompiler.CreateCompilation(entryUri, workspace, skipRestore: true);
            }
        }

        return hasChanges;
    }
}
