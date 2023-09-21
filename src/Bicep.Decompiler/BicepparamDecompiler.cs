// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Decompiler.Rewriters;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Navigation;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.Rewriters;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Bicep.Decompiler.ArmHelpers;
using Bicep.Decompiler.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Decompiler;

public class BicepparamDecompiler
{
    private readonly BicepCompiler bicepCompiler;
    private readonly IFileResolver fileResolver;

    public BicepparamDecompiler(BicepCompiler bicepCompiler, IFileResolver fileResolver)
    {
        this.bicepCompiler = bicepCompiler;
        this.fileResolver = fileResolver;
    }

    public DecompileResult Decompile(Uri entryJsonUri, Uri entryBicepparamUri, Uri? bicepFileUri)
    {
        var workspace = new Workspace();

        if (!fileResolver.TryRead(entryJsonUri).IsSuccess(out var jsonInput))
        {
            throw new InvalidOperationException($"Failed to read {entryJsonUri}");
        }

        var program = DecompileParamFile(jsonInput, entryBicepparamUri, bicepFileUri);

        var bicepparamFile = SourceFileFactory.CreateBicepParamFile(entryBicepparamUri, program.ToText());

        workspace.UpsertSourceFile(bicepparamFile);

        return new DecompileResult(entryBicepparamUri, PrintFiles(workspace));
    }

    private ProgramSyntax DecompileParamFile(string jsonInput, Uri entryBicepparamUri, Uri? bicepFileUri)
    {
        var statements = new List<SyntaxBase>();

        var jsonObject = JTokenHelpers.LoadJson(jsonInput, JObject.Load, ignoreTrailingContent: false);
        var bicepPath = bicepFileUri is { } ? PathHelper.GetRelativePath(entryBicepparamUri, bicepFileUri) : null;

        statements.Add(new UsingDeclarationSyntax(
            SyntaxFactory.CreateIdentifierToken("using"),
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

    private SyntaxBase ParseParam(JProperty param)
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

    private SyntaxBase ParseJToken(JToken value)
        => value switch
        {
            JObject jObject => ParseJObject(jObject),
            JArray jArray => ParseJArray(jArray),
            JValue jValue => ParseJValue(jValue),
            _ => throw new ConversionFailedException($"Unrecognized token type {value.Type}", value),
        };

    private SyntaxBase ParseJValue(JValue value)
    => value.Type switch
    {
        JTokenType.Integer => SyntaxFactory.CreatePositiveOrNegativeInteger(value.Value<long>()),
        JTokenType.String => SyntaxFactory.CreateStringLiteral(value.ToString()),
        JTokenType.Boolean => SyntaxFactory.CreateBooleanLiteral(value.Value<bool>()),
        JTokenType.Null => SyntaxFactory.CreateNullLiteral(),
        _ => throw new NotImplementedException($"Unrecognized token type {value.Type}")
    };

    private SyntaxBase ParseJArray(JArray jArray)
    {
        var itemSyntaxes = new List<SyntaxBase>();

        foreach (var item in jArray)
        {
            itemSyntaxes.Add(ParseJToken(item));
        }

        return SyntaxFactory.CreateArray(itemSyntaxes);
    }

    private SyntaxBase ParseJObject(JObject jObject)
    {
        var propertySyntaxes = new List<ObjectPropertySyntax>();

        foreach (var property in jObject.Properties())
        {
            propertySyntaxes.Add(SyntaxFactory.CreateObjectProperty(property.Name, ParseJToken(property.Value)));
        }

        return SyntaxFactory.CreateObject(propertySyntaxes);
    }

    private SyntaxBase ParseParameterWithComment(JToken jToken)
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

    private static ImmutableDictionary<Uri, string> PrintFiles(Workspace workspace)
    {
        var filesToSave = new Dictionary<Uri, string>();
        foreach (var (fileUri, sourceFile) in workspace.GetActiveSourceFilesByUri())
        {
            if (sourceFile is not BicepParamFile bicepFile)
            {
                continue;
            }

            filesToSave[fileUri] = PrettyPrinter.PrintProgram(bicepFile.ProgramSyntax, GetPrettyPrintOptions(), bicepFile.LexingErrorLookup, bicepFile.ParsingErrorLookup);
        }

        return filesToSave.ToImmutableDictionary();
    }

    private static PrettyPrintOptions GetPrettyPrintOptions() => new PrettyPrintOptions(NewlineOption.LF, IndentKindOption.Space, 2, false);
}
