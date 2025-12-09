// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Text;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.IO.Abstraction;
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

    public class ImportKubernetesManifestHandler(
        ILanguageServerFacade server,
        ITelemetryProvider telemetryProvider,
        IConfigurationManager configurationManager,
        IFileExplorer fileExplorer) : IJsonRpcRequestHandler<ImportKubernetesManifestRequest, ImportKubernetesManifestResponse>
    {
        private readonly TelemetryAndErrorHandlingHelper<ImportKubernetesManifestResponse> helper = new(server.Window, telemetryProvider);

        public Task<ImportKubernetesManifestResponse> Handle(ImportKubernetesManifestRequest request, CancellationToken cancellationToken)
            => helper.ExecuteWithTelemetryAndErrorHandling(async () =>
            {
                var manifestFileUri = IOUri.FromFilePath(request.ManifestFilePath);
                var manifestContents = await fileExplorer.GetFile(manifestFileUri).ReadAllTextAsync();

                var bicepFileUri = manifestFileUri.WithExtension(LanguageConstants.LanguageFileExtension);
                var bicepContents = this.Decompile(bicepFileUri, manifestContents, this.helper);

                await fileExplorer.GetFile(bicepFileUri).WriteAllTextAsync(bicepContents, cancellationToken);

                return new(new(bicepFileUri.GetFilePath()), BicepTelemetryEvent.ImportKubernetesManifestSuccess());
            });

        private string Decompile(IOUri bicepFileUri, string manifestContents, TelemetryAndErrorHandlingHelper<ImportKubernetesManifestResponse> telemetryHelper)
        {
            var declarations = new List<SyntaxBase>
            {
                new ParameterDeclarationSyntax(
                    [
                        SyntaxFactory.CreateDecorator("secure"),
                        SyntaxFactory.NewlineToken,
                    ],
                    SyntaxFactory.ParameterKeywordToken,
                    SyntaxFactory.CreateIdentifierWithTrailingSpace("kubeConfig"),
                    new VariableAccessSyntax(new(SyntaxFactory.CreateIdentifierToken("string"))),
                    null),

                new ExtensionDeclarationSyntax(
                    [],
                    SyntaxFactory.ExtensionKeywordToken,
                    SyntaxFactory.CreateIdentifierWithTrailingSpace(K8sNamespaceType.BuiltInName),
                    new ExtensionWithClauseSyntax(
                        SyntaxFactory.CreateIdentifierToken(LanguageConstants.WithKeyword),
                        SyntaxFactory.CreateObject(
                        [
                            SyntaxFactory.CreateObjectProperty("namespace", SyntaxFactory.CreateStringLiteral("default")),
                            SyntaxFactory.CreateObjectProperty("kubeConfig", SyntaxFactory.CreateIdentifier("kubeConfig"))
                        ])),
                    asClause: SyntaxFactory.EmptySkippedTrivia)
            };

            try
            {
                var reader = new StringReader(manifestContents);
                var yamlStream = new YamlStream();
                yamlStream.Load(reader);

                foreach (var yamlDocument in yamlStream.Documents)
                {
                    var syntax = ProcessResourceYaml(yamlDocument, telemetryHelper);

                    declarations.Add(syntax);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception deserializing manifest: {0}", ex);
                throw telemetryHelper.CreateException(
                    $"Failed to deserialize kubernetes manifest YAML: {ex.Message}",
                    BicepTelemetryEvent.ImportKubernetesManifestFailure("DeserializeYamlFailed"),
                    new ImportKubernetesManifestResponse(null));
            }

            var program = new ProgramSyntax(
                declarations.SelectMany(x => new SyntaxBase[] { x, SyntaxFactory.DoubleNewlineToken }),
                SyntaxFactory.CreateToken(TokenType.EndOfFile));

            var configuration = configurationManager.GetConfiguration(bicepFileUri);
            var printerOptions = configuration.Formatting.Data;
            var printerContext = PrettyPrinterV2Context.Create(printerOptions, EmptyDiagnosticLookup.Instance, EmptyDiagnosticLookup.Instance);

            return PrettyPrinterV2.Print(program, printerContext);
        }

        private static ResourceDeclarationSyntax ProcessResourceYaml(YamlDocument yamlDocument, TelemetryAndErrorHandlingHelper<ImportKubernetesManifestResponse> telemetryHelper)
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

            var (type, apiVersion) = apiVersionNode.Value.LastIndexOf('/') switch
            {
                -1 => ($"core/{kindNode.Value}", apiVersionNode.Value),
                int x => ($"{apiVersionNode.Value.Substring(0, x)}/{kindNode.Value}", apiVersionNode.Value.Substring(x + 1)),
            };

            var filteredChildren = rootNode.Children.Where(x => x.Key != kindKey && x.Key != apiVersionKey);

            var resourceBody = ConvertObjectChildren(filteredChildren);
            var symbolName = GetResourceSymbolName(type, resourceBody);

            return new ResourceDeclarationSyntax(
                [],
                SyntaxFactory.ResourceKeywordToken,
                SyntaxFactory.CreateIdentifierWithTrailingSpace(symbolName),
                SyntaxFactory.CreateStringLiteral($"{type}@{apiVersion}"),
                null,
                null,
                SyntaxFactory.AssignmentToken,
                [],
                resourceBody);
        }

        private static string GetResourceSymbolName(string type, SyntaxBase resourceBody)
        {
            var identifier = type;
            if ((resourceBody as ObjectSyntax)?.TryGetPropertyByNameRecursive("metadata", "name") is { } nameProperty &&
                (nameProperty.Value as StringSyntax)?.TryGetLiteralValue() is { } nameString)
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
