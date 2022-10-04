// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Bicep.LanguageServer.Telemetry;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using SharpYaml;
using SharpYaml.Serialization;

namespace Bicep.LanguageServer.Handlers
{
    [Method("bicep/importKubernetesManifest", Direction.ClientToServer)]
    public record ImportKubernetesManifestRequest(string ManifestFilePath)
        : IRequest<ImportKubernetesManifestResponse>;

    public record ImportKubernetesManifestResponse(string? BicepFilePath);

    public class ImportKubernetesManifestHandler : IJsonRpcRequestHandler<ImportKubernetesManifestRequest, ImportKubernetesManifestResponse>
    {
        private readonly TelemetryAndErrorHandlingHelper<ImportKubernetesManifestResponse> helper;

        public ImportKubernetesManifestHandler(ILanguageServerFacade server, ITelemetryProvider telemetryProvider)
        {
            this.helper = new TelemetryAndErrorHandlingHelper<ImportKubernetesManifestResponse>(server.Window, telemetryProvider, new(null));
        }

        public Task<ImportKubernetesManifestResponse> Handle(ImportKubernetesManifestRequest request, CancellationToken cancellationToken)
            => helper.ExecuteWithTelemetryAndErrorHandling(async () => {
                var bicepFilePath = Path.ChangeExtension(request.ManifestFilePath, ".bicep");
                var manifestContents = await File.ReadAllTextAsync(request.ManifestFilePath);

                var bicepContents = Decompile(manifestContents);

                await File.WriteAllTextAsync(bicepFilePath, bicepContents, cancellationToken);

                return new(new(bicepFilePath), BicepTelemetryEvent.ImportKubernetesManifestSuccess());
            });

        public static string Decompile(string manifestContents)
        {
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


            try
            {
                var reader = new StringReader(manifestContents);
                var yamlStream = new YamlStream();
                yamlStream.Load(reader);

                foreach (var yamlDocument in yamlStream.Documents)
                {
                    var syntax = ProcessResourceYaml(yamlDocument);

                    declarations.Add(syntax);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception deserializing manifest: {0}", ex);

                throw new TelemetryAndErrorHandlingException(
                    $"Failed to deserialize kubernetes manifest YAML: {ex.Message}",
                    BicepTelemetryEvent.ImportKubernetesManifestFailure("DeserializeYamlFailed"));
            }

            var program = new ProgramSyntax(
                declarations.SelectMany(x => new SyntaxBase[] { x, SyntaxFactory.DoubleNewlineToken }),
                SyntaxFactory.CreateToken(TokenType.EndOfFile, ""),
                Enumerable.Empty<IDiagnostic>());

            return PrettyPrinter.PrintProgram(program, new PrettyPrintOptions(NewlineOption.LF, IndentKindOption.Space, 2, false));
        }

        private static ResourceDeclarationSyntax ProcessResourceYaml(YamlDocument yamlDocument)
        {
            if (yamlDocument.RootNode is not YamlMappingNode rootNode)
            {
                throw new YamlException(yamlDocument.RootNode.Start, yamlDocument.RootNode.End, $"Expected dictionary node.");
            }

            var kindKey = rootNode.Children.Keys.FirstOrDefault(x => x is YamlScalarNode scalar && scalar.Value == "kind");
            var apiVersionKey = rootNode.Children.Keys.FirstOrDefault(x => x is YamlScalarNode scalar && scalar.Value == "apiVersion");

            if (kindKey is null || apiVersionKey is null)
            {
                throw new YamlException(rootNode.Start, rootNode.End, $"Failed to find 'kind' and 'apiVersion' keys for resource declaration.");
            }

            if (rootNode.Children[kindKey] is not YamlScalarNode kindNode)
            {
                throw new YamlException(kindKey.Start, kindKey.End, "Unable to process 'kind' for resource declaration.");
            }

            if (rootNode.Children[apiVersionKey] is not YamlScalarNode apiVersionNode)
            {
                throw new YamlException(apiVersionKey.Start, apiVersionKey.End, "Unable to process 'apiVersion' for resource declaration.");
            }

            var (type, apiVersion) = apiVersionNode.Value.LastIndexOf('/') switch {
                -1 => ($"core/{kindNode.Value}", apiVersionNode.Value),
                int x => ($"{apiVersionNode.Value.Substring(0, x)}/{kindNode.Value}", apiVersionNode.Value.Substring(x + 1)),
            };

            var filteredChildren = rootNode.Children.Where(x => x.Key != kindKey && x.Key != apiVersionKey);

            var resourceBody = ConvertObjectChildren(filteredChildren);
            var symbolName = GetResourceSymbolName(type, resourceBody);

            return new ResourceDeclarationSyntax(
                Enumerable.Empty<SyntaxBase>(),
                SyntaxFactory.CreateToken(Core.Parsing.TokenType.Identifier, "resource"),
                SyntaxFactory.CreateIdentifier(symbolName),
                SyntaxFactory.CreateStringLiteral($"{type}@{apiVersion}"),
                null,
                SyntaxFactory.AssignmentToken,
                resourceBody);
        }

        private static string GetResourceSymbolName(string type, SyntaxBase resourceBody)
        {
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
                    // ASCII codes for lc and uppercase chars are 32 apart.
                    // Subtract 32 from the ASCII code to convert to uppercase.
                    c -= (char)32;
                }

                if (isValidChar)
                {
                    identifierBuilder.Append(c);
                }
                capitalizeNext = !isValidChar;
            }

            return identifierBuilder.ToString();
        }

        private static SyntaxBase ConvertValue(YamlNode value)
        {
            switch (value)
            {
                case YamlMappingNode dictValue:
                    return ConvertObjectChildren(dictValue.Children);
                case YamlSequenceNode listValue:
                    var items = listValue.Children.Select(ConvertValue);
                    return SyntaxFactory.CreateArray(items);
                case YamlScalarNode scalarNode:
                    if (scalarNode.Style == SharpYaml.ScalarStyle.Plain)
                    {
                        // If the user hasn't provided quotes, there's no way to differentiate between strings, ints & bools. We have to guess...
                        if (bool.TryParse(scalarNode.Value, out var boolVal))
                        {
                            return SyntaxFactory.CreateBooleanLiteral(boolVal);
                        }

                        if (long.TryParse(scalarNode.Value, out var longVal))
                        {
                            return SyntaxFactory.CreatePositiveOrNegativeInteger(longVal);
                        }
                    }

                    return SyntaxFactory.CreateStringLiteral(scalarNode.Value);
                default:
                    throw new InvalidOperationException($"Unsupported type {value.GetType()}");
            }
        }

        private static ObjectSyntax ConvertObjectChildren(IEnumerable<KeyValuePair<YamlNode, YamlNode>> children)
        {
            var objectProperties = new List<ObjectPropertySyntax>();
            foreach (var kvp in children)
            {
                if (kvp.Key is not YamlScalarNode keyNode)
                {
                    throw new InvalidOperationException($"Unsupported object key {kvp.Key.GetType()}");
                }

                var objectProperty = SyntaxFactory.CreateObjectProperty(keyNode.Value, ConvertValue(kvp.Value));
                objectProperties.Add(objectProperty);
            }
            return SyntaxFactory.CreateObject(objectProperties);
        }
    }
}
