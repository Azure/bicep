// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.Syntax;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using SharpYaml.Serialization;

namespace Bicep.LanguageServer.Handlers
{
    [Method("bicep/importKubernetesManifest", Direction.ClientToServer)]
    public record ImportKubernetesManifestRequest(string ManifestFilePath)
        : IRequest<ImportKubernetesManifestResponse>;

    public record ImportKubernetesManifestResponse(string BicepFilePath);

    public class ImportKubernetesManifestHandler : IJsonRpcRequestHandler<ImportKubernetesManifestRequest, ImportKubernetesManifestResponse>
    {
        public async Task<ImportKubernetesManifestResponse> Handle(ImportKubernetesManifestRequest request, CancellationToken cancellationToken)
        {
            var bicepFilePath = Path.ChangeExtension(request.ManifestFilePath, ".bicep");
            var manifestContents = await File.ReadAllTextAsync(request.ManifestFilePath);

            var bicepContents = Decompile(manifestContents);

            await File.WriteAllTextAsync(bicepFilePath, bicepContents, cancellationToken);

            return new(bicepFilePath);
        }

        public static string Decompile(string manifestContents)
        {
            manifestContents = StringUtils.ReplaceNewlines(manifestContents, "\n");

            var resources = manifestContents.Split("\n---\n");

            var declarations = new List<SyntaxBase>();

            declarations.Add(new ParameterDeclarationSyntax(
                new SyntaxBase[] {
                    SyntaxFactory.CreateDecorator("secure"),
                    SyntaxFactory.NewlineToken,
                },
                SyntaxFactory.CreateToken(Core.Parsing.TokenType.Identifier, "param"),
                SyntaxFactory.CreateIdentifier("kubeConfig"),
                new SimpleTypeSyntax(SyntaxFactory.CreateToken(TokenType.Identifier, "string")),
                null));

            declarations.Add(new ImportDeclarationSyntax(
                Enumerable.Empty<SyntaxBase>(),
                SyntaxFactory.CreateToken(Core.Parsing.TokenType.Identifier, "import"),
                SyntaxFactory.CreateIdentifier("kubernetes"),
                SyntaxFactory.CreateToken(Core.Parsing.TokenType.Identifier, "as"),
                SyntaxFactory.CreateIdentifier("k8s"),
                SyntaxFactory.CreateObject(new [] {
                    SyntaxFactory.CreateObjectProperty("namespace", SyntaxFactory.CreateStringLiteral("default")),
                    SyntaxFactory.CreateObjectProperty("kubeConfig", SyntaxFactory.CreateIdentifier("kubeConfig")),
                })
            ));

            foreach (var resource in resources)
            {
                var yamlValue = new Serializer().Deserialize(resource);

                var syntax = ProcessResource(yamlValue);

                declarations.Add(syntax);
            }

            var program = new ProgramSyntax(
                declarations.SelectMany(x => new SyntaxBase[] { x, SyntaxFactory.DoubleNewlineToken }),
                SyntaxFactory.CreateToken(TokenType.EndOfFile, ""),
                Enumerable.Empty<IDiagnostic>());

            return PrettyPrinter.PrintProgram(program, new PrettyPrintOptions(NewlineOption.LF, IndentKindOption.Space, 2, false));
        }

        private static ResourceDeclarationSyntax ProcessResource(object? resource)
        {
            if (resource is not Dictionary<object, object> dictValue)
            {
                throw new NotImplementedException($"Unsupported type {resource?.GetType()}");
            }

            if (!(dictValue.TryGetValue("kind", out var kindObj) && kindObj is string type &&
                dictValue.TryGetValue("apiVersion", out var apiVersionObj) && apiVersionObj is string apiVersion))
            {
                throw new NotImplementedException($"Expected properties 'type' & 'kind'");
            }

            (type, apiVersion) = apiVersion.LastIndexOf('/') switch {
                -1 => ($"core/{type}", apiVersion),
                int x => ($"{apiVersion.Substring(0, x)}/{type}", apiVersion.Substring(x + 1)),
            };

            var filteredResource = dictValue
                .Where(x => x.Key as string != "kind" && x.Key as string != "apiVersion")
                .ToDictionary(x => x.Key, x => x.Value);

            var resourceBody = ProcessValue(filteredResource);

            var identifier = type;
            if ((resourceBody as ObjectSyntax)?.TryGetPropertyByNameRecursive("metadata", "name") is {} nameProperty &&
                (nameProperty.Value as StringSyntax)?.TryGetLiteralValue() is {} nameString)
            {
                identifier = $"{type}_{nameString}";
            }

            var identifierBuilder = new StringBuilder();
            var capitalizeNext = false;
            for (var i = 0; i < identifier.Length; i++)
            {
                var c = identifier[i];
                var isValidChar =
                    (c >= 'a' && c <= 'z') ||
                    (c >= 'A' && c <= 'Z') ||
                    (i > 0 && c >= '0' && c <= '9') ||
                    (i > 0 && c == '_');

                if (capitalizeNext && (c >= 'a' && c <= 'z'))
                {
                    // 32 equals 'a' - 'A'
                    c -= (char)32;
                }

                if (isValidChar)
                {
                    identifierBuilder.Append(c);
                }
                capitalizeNext = !isValidChar;
            }

            var sanitizedIdentifier = identifierBuilder.ToString();

            return new ResourceDeclarationSyntax(
                Enumerable.Empty<SyntaxBase>(),
                SyntaxFactory.CreateToken(Core.Parsing.TokenType.Identifier, "resource"),
                SyntaxFactory.CreateIdentifier(sanitizedIdentifier),
                SyntaxFactory.CreateStringLiteral($"{type}@{apiVersion}"),
                null,
                SyntaxFactory.AssignmentToken,
                resourceBody);
        }

        private static SyntaxBase ProcessValue(object value)
        {
            switch (value)
            {
                case Dictionary<object, object> dictValue:
                    var objectProperties = new List<ObjectPropertySyntax>();
                    foreach (var kvp in dictValue)
                    {
                        if (kvp.Key is not string keyName)
                        {
                            throw new NotImplementedException($"Unsupported object key {kvp.Key.GetType()}");
                        }

                        var objectProperty = SyntaxFactory.CreateObjectProperty(keyName, ProcessValue(kvp.Value));
                        objectProperties.Add(objectProperty);
                    }
                    return SyntaxFactory.CreateObject(objectProperties);
                case List<object> listValue:
                    var arrayProperties = new List<SyntaxBase>();
                    foreach (var prop in listValue)
                    {
                        arrayProperties.Add(ProcessValue(prop));
                    }
                    return SyntaxFactory.CreateArray(arrayProperties);
                case string stringValue:
                    return SyntaxFactory.CreateStringLiteral(stringValue);
                case bool boolValue:
                    return SyntaxFactory.CreateBooleanLiteral(boolValue);
                case int intValue:
                    return SyntaxFactory.CreatePositiveOrNegativeInteger(intValue);
                default:
                    throw new NotImplementedException($"Unsupported type {value.GetType()}");
            }
        }
    }
}
