// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Decompiler;
using Bicep.LanguageServer.Telemetry;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public record BicepDecompileForPasteCommandParams(
        string bicepContent,
        int rangeOffset,
        int rangeLength,
        string jsonContent,
        bool queryCanPaste,  // True if client is testing clipboard text for menu enabling only, false if the user actually requested a paste,
        string languageId
    );

    public record BicepDecompileForPasteCommandResult
    (
        string DecompileId, // Used to synchronize `ry events
        string Output,
        string? PasteContext,
        string? PasteType,
        string? ErrorMessage, // This is null if pasteType == null, otherwise indicates an error trying to decompile to the given paste type
        string? Bicep, // Null if pasteType == null or errorMessage != null
        string? Disclaimer
    );

    /// <summary>
    /// Handles a request from the client to analyze/decompile a JSON fragment for possible conversion into Bicep (for pasting into a Bicep file)
    /// </summary>
    public class BicepDecompileForPasteCommandHandler(
        ISerializer serializer,
        ILanguageServerFacade server,
        ITelemetryProvider telemetryProvider,
        ISourceFileFactory sourceFileFactory,
        BicepCompiler bicepCompiler)
        : ExecuteTypedResponseCommandHandlerBase<BicepDecompileForPasteCommandParams, BicepDecompileForPasteCommandResult>(LangServerConstants.DecompileForPasteCommand, serializer)
    {
        private readonly TelemetryAndErrorHandlingHelper<BicepDecompileForPasteCommandResult> telemetryHelper = new(server.Window, telemetryProvider);

        private static readonly Uri JsonDummyUri = new("file:///from-clipboard.json", UriKind.Absolute);
        private static readonly Uri BicepDummyUri = PathHelper.ChangeToBicepExtension(JsonDummyUri);
        private static readonly Uri BicepParamsDummyUri = PathHelper.ChangeToBicepparamExtension(JsonDummyUri);

        public enum PasteType
        {
            None = 0,
            FullTemplate = 1,  // Full template
            SingleResource = 2, // Single resource
            ResourceList = 3,// List of multiple resources
            JsonValue = 4, // Single JSON value (number, object, array etc)
            BicepValue = 5, // JSON value that is also valid Bicep (e.g. "[1, {}]")
            FullParams = 6 // Full parameters file
        }
        public enum PasteContext
        {
            None,
            String, // Pasting inside a string
            ParamsWithUsingDeclaration, // Pasting inside a parameters file with an existing using declaration
        }


        private enum LanguageId
        {
            Bicep,
            BicepParams,
        }

        private static LanguageId GetLanguageId(string languageId)
        {
            return languageId switch
            {
                "bicep" => LanguageId.Bicep,
                "bicep-params" => LanguageId.BicepParams,
                _ => throw new ArgumentException($"Unexpected languageId value {languageId}"),
            };
        }
        private static string LanguageIdAsString(LanguageId languageId)
        {
            return languageId switch
            {
                LanguageId.Bicep => "bicep",
                LanguageId.BicepParams => "bicep-params",
                _ => throw new ArgumentException($"Unexpected languageId value {languageId}"),
            };
        }


        private record ResultAndTelemetry(BicepDecompileForPasteCommandResult Result, BicepTelemetryEvent? SuccessTelemetry);

        public override Task<BicepDecompileForPasteCommandResult> Handle(BicepDecompileForPasteCommandParams parameters, CancellationToken cancellationToken)
        {
            return telemetryHelper.ExecuteWithTelemetryAndErrorHandling((Func<Task<(BicepDecompileForPasteCommandResult result, BicepTelemetryEvent? successTelemetry)>>)(async () =>
            {
                var (result, successTelemetry) = await TryDecompileForPaste(
                    parameters.bicepContent,
                    parameters.rangeOffset,
                    parameters.rangeLength,
                    parameters.jsonContent,
                    parameters.queryCanPaste,
                    GetLanguageId(parameters.languageId));
                return (result, successTelemetry);
            }));
        }

        private static PasteContext GetPasteContext(string bicepContents, int offset, int length, LanguageId languageId)
        {
            var newContents = string.Concat(bicepContents.AsSpan()[..offset], bicepContents.AsSpan(offset + length));
            BaseParser parser = languageId switch
            {
                LanguageId.Bicep => new Parser(newContents),
                LanguageId.BicepParams => new ParamsParser(newContents),
                _ => throw new ArgumentException($"Unexpected languageId value {languageId}"),
            };
            var program = parser.Program();

            // Find the innermost string that contains the given offset, and which isn't inside an interpolation hole.
            // Note that a hole can contain nested strings which may contain holes...
            var stringSyntax = (StringSyntax?)program.TryFindMostSpecificNodeInclusive(offset, syntax =>
            {
                if (syntax is not StringSyntax stringSyntax)
                {
                    return false;
                }

                // The inclusive version of this function does not quite match what we want (and exclusive misses some valid offsets)...
                //
                // Example: 'str' (the syntax span includes the quotes)
                //   Span start is on the first "'", span end (exclusive) is after the last "'"
                //   An insertion with the cursor on the beginning "'" will end up before the string, not inside it.
                //   An insertion with the cursor on the ending "'" will end up in the string
                if (offset <= syntax.Span.Position || offset >= syntax.GetEndPosition())
                {
                    // Ignore this node
                    return false;
                }

                foreach (var interpolation in stringSyntax.Expressions)
                {
                    // Remove expression holes from consideration (if they contain strings that will be caught in the next iteration)
                    //
                    // Example: 'str${v1}', the expression node 'v1' does *not* include the ${ and } delimiters
                    //   Span start is on the 'v', span end (exclusive) is on the '}'
                    //   An insertion with the cursor on the v, 1 or '{' will end up inside the expression hole
                    if (offset >= interpolation.Span.Position && offset <= interpolation.GetEndPosition())
                    {
                        // Ignore this node
                        return false;
                    }
                }

                return true;

            });

            if (stringSyntax is not null)
            {
                return PasteContext.String;
            }

            if (languageId == LanguageId.BicepParams && program.TryFindMostSpecificNodeInclusive(0, syntax => syntax is UsingDeclarationSyntax) is not null)
            {
                return PasteContext.ParamsWithUsingDeclaration;
            }

            return PasteContext.None;
        }
        private static string DisclaimerMessage => BicepDecompiler.DecompilerDisclaimerMessage;

        private static void Log(StringBuilder output, string message)
        {
            output.AppendLine(message);
            Trace.TraceInformation(message);
        }

        private async Task<ResultAndTelemetry> TryDecompileForPaste(string bicepContents, int rangeOffset, int rangeLength, string json, bool queryCanPaste, LanguageId languageId)
        {
            StringBuilder output = new();
            var decompileId = Guid.NewGuid().ToString();
            var pasteContext = GetPasteContext(bicepContents, rangeOffset, rangeLength, languageId);

            if (pasteContext == PasteContext.String)
            {
                // Don't convert to Bicep if inside a string
                return new(new(decompileId, output.ToString(), PasteContextAsString(pasteContext), PasteType: null, ErrorMessage: null,
                        Bicep: null, Disclaimer: null),
                    GetSuccessTelemetry(queryCanPaste, decompileId, json, pasteContext, pasteType: null, bicep: null, languageId: languageId));
            }

            if (string.IsNullOrWhiteSpace(json))
            {
                return new(new(decompileId, output.ToString(), PasteContextAsString(pasteContext), PasteType: null, ErrorMessage: null,
                        Bicep: null, Disclaimer: null),
                    GetSuccessTelemetry(queryCanPaste, decompileId, json, pasteContext, pasteType: null, bicep: null, languageId: languageId));
            }

            var (pasteType, constructedJsonTemplate) = languageId switch
            {
                LanguageId.Bicep => TryConstructFullJsonTemplate(json),
                LanguageId.BicepParams => TryConstructFullJsonParams(json),
                _ => (PasteType.None, null),
            };
            switch (pasteType)
            {
                case PasteType.None:
                    {
                        // It's not a template or resource.  Try treating it as a JSON value.
                        var resultAndTelemetry = TryConvertFromJsonValue(output, json, decompileId, pasteContext, queryCanPaste, languageId);
                        if (resultAndTelemetry is not null)
                        {
                            return resultAndTelemetry;
                        }

                        break;
                    }
                case PasteType.FullParams:
                    {
                        // It's a full parameters file
                        var result = TryConvertFromConstructedParameters(output, json, decompileId, pasteContext, pasteType, queryCanPaste, constructedJsonTemplate, languageId);
                        if (result is not null)
                        {
                            return result;
                        }

                        break;
                    }
                default:
                    {
                        // It's a full or partial template and we have converted it into a full template to parse
                        var result = await TryConvertFromConstructedTemplate(output, json, decompileId, pasteContext, pasteType, queryCanPaste, constructedJsonTemplate, languageId);
                        if (result is not null)
                        {
                            return result;
                        }

                        break;
                    }
            }

            // It's not anything we know how to convert to Bicep
            return new(
                new(
                    decompileId, output.ToString(), PasteContextAsString(pasteContext), PasteType: null, ErrorMessage: null,
                    Bicep: null, Disclaimer: null),
                GetSuccessTelemetry(queryCanPaste, decompileId, json, pasteContext, pasteType: null, bicep: null, languageId: languageId));
        }

        private async Task<ResultAndTelemetry?> TryConvertFromConstructedTemplate(StringBuilder output, string json, string decompileId, PasteContext pasteContext, PasteType pasteType, bool queryCanPaste, string? constructedJsonTemplate, LanguageId languageId)
        {
            ImmutableDictionary<Uri, string> filesToSave;
            try
            {
                // Decompile the full template
                Debug.Assert(constructedJsonTemplate is not null);
                Log(output, string.Format(LangServerResources.Decompile_DecompilationStartMsg, "clipboard text"));

                var decompiler = new BicepDecompiler(bicepCompiler);
                var options = GetDecompileOptions(pasteType);
                (_, filesToSave) = await decompiler.Decompile(BicepDummyUri, constructedJsonTemplate, options: options);
            }
            catch (Exception ex)
            {
                // We don't ever throw. If we reached here, the pasted text was in a format we think we can handle but there was some
                //   sort of error.  Tell the client it can be pasted and let the client show the end user the error if they do.
                //   deal with any bicep errors found.
                var message = ex.Message;
                Log(output, $"Decompilation failed: {message}");

                return new(new(decompileId, output.ToString(), PasteContextAsString(pasteContext), PasteTypeAsString(pasteType), message, Bicep: null, Disclaimer: null),
                    GetSuccessTelemetry(queryCanPaste, decompileId, json, pasteContext, PasteTypeAsString(pasteType), bicep: null, languageId: languageId));
            }

            // Get Bicep output from the main file (all others are currently ignored)
            var bicepOutput = filesToSave.Single(kvp => BicepDummyUri.Equals(kvp.Key)).Value;

            if (string.IsNullOrWhiteSpace(bicepOutput))
            {
                return null;
            }

            // Ensure ends with newline
            bicepOutput = bicepOutput.TrimEnd() + "\n";

            // Show disclaimer and return result
            Log(output, DisclaimerMessage);
            return new(new(decompileId, output.ToString(), PasteContextAsString(pasteContext), PasteTypeAsString(pasteType), null, bicepOutput, DisclaimerMessage),
                GetSuccessTelemetry(queryCanPaste, decompileId, json, pasteContext, PasteTypeAsString(pasteType), bicepOutput, languageId: languageId));
        }

        private ResultAndTelemetry? TryConvertFromConstructedParameters(StringBuilder output, string json, string decompileId, PasteContext pasteContext, PasteType pasteType, bool queryCanPaste, string? constructedJsonTemplate, LanguageId languageId)
        {
            ImmutableDictionary<Uri, string> filesToSave;
            try
            {
                // Decompile the full template
                Debug.Assert(constructedJsonTemplate is not null);
                Log(output, string.Format(LangServerResources.Decompile_DecompilationStartMsg, "clipboard text"));

                var decompiler = new BicepDecompiler(bicepCompiler);
                (_, filesToSave) = decompiler.DecompileParameters(constructedJsonTemplate, BicepParamsDummyUri, null, new()
                {
                    IncludeUsingDeclaration = pasteContext != PasteContext.ParamsWithUsingDeclaration
                });
            }
            catch (Exception ex)
            {
                // We don't ever throw. If we reached here, the pasted text was in a format we think we can handle but there was some
                //   sort of error.  Tell the client it can be pasted and let the client show the end user the error if they do.
                //   deal with any bicep errors found.
                var message = ex.Message;
                Log(output, $"Decompilation failed: {message}");

                return new(new(decompileId, output.ToString(), PasteContextAsString(pasteContext), PasteTypeAsString(pasteType), message, Bicep: null, Disclaimer: null),
                    GetSuccessTelemetry(queryCanPaste, decompileId, json, pasteContext, PasteTypeAsString(pasteType), bicep: null, languageId: languageId));
            }

            // Get Bicep output from the main file (all others are currently ignored)
            var bicepOutput = filesToSave.Single(kvp => BicepParamsDummyUri.Equals(kvp.Key)).Value;

            if (string.IsNullOrWhiteSpace(bicepOutput))
            {
                return null;
            }

            // Ensure ends with newline
            bicepOutput = bicepOutput.TrimEnd() + "\n";

            // Show disclaimer and return result
            Log(output, DisclaimerMessage);
            return new(new(decompileId, output.ToString(), PasteContextAsString(pasteContext), PasteTypeAsString(pasteType), null, bicepOutput, DisclaimerMessage),
                GetSuccessTelemetry(queryCanPaste, decompileId, json, pasteContext, PasteTypeAsString(pasteType), bicepOutput, languageId: languageId));
        }

        private static string PasteContextAsString(PasteContext pasteContext)
        {
            return pasteContext switch
            {
                PasteContext.None => "none",
                PasteContext.String => "string",
                PasteContext.ParamsWithUsingDeclaration => "none",
                _ => throw new($"Unexpected pasteContext value {pasteContext}"),
            };
        }

        private BicepTelemetryEvent? GetSuccessTelemetry(bool queryCanPaste, string decompileId, string json, PasteContext pasteContext, string? pasteType, string? bicep, LanguageId languageId)
        {

            // Don't log telemetry if we're just determining if we can paste, because this will happen a lot
            //   (on changing between editors for instance)
            // TODO: but we don't call back for telemetry if we use the result
            return queryCanPaste ?
                null :
                BicepTelemetryEvent.DecompileForPaste(decompileId, PasteContextAsString(pasteContext), pasteType, json.Length, bicep?.Length, LanguageIdAsString(languageId));
        }

        private static DecompileOptions GetDecompileOptions(PasteType pasteType)
        {
            return new()
            {
                // For partial template pastes, we don't error out on missing parameters and variables because they won't
                //   ever have definitions in the pasted portion
                AllowMissingParamsAndVars = pasteType != PasteType.FullTemplate,
                // ... but don't allow them in nested templates, which should be fully complete and valid
                AllowMissingParamsAndVarsInNestedTemplates = false,
                IgnoreTrailingInput = pasteType != PasteType.JsonValue,
            };
        }

        private ResultAndTelemetry? TryConvertFromJsonValue(StringBuilder output, string json, string decompileId, PasteContext pasteContext, bool queryCanPaste, LanguageId languageId)
        {
            // Is it valid JSON that we can convert into Bicep?
            var pasteType = PasteType.JsonValue;
            var options = GetDecompileOptions(pasteType);
            var bicep = BicepDecompiler.DecompileJsonValue(sourceFileFactory, json, options);
            if (bicep is null)
            {
                return null;
            }

            // Technically we've already converted, but we only want to show this message if we think the pasted text is convertible
            Log(output, string.Format(LangServerResources.Decompile_DecompilationStartMsg, "clipboard text"));

            // Even though it's valid JSON, it might also be valid Bicep, in which case we want to leave it alone if we're
            //   doing an automatic copy/paste conversion.

            // Is the input already a valid Bicep expression with comments removed?
            var parser = new Parser("var v = " + json);
            _ = parser.Program();
            if (!parser.LexingErrorLookup.Any() && !parser.ParsingErrorLookup.Any())
            {
                // We still want to have the converted bicep available (via the "bicep" output) in the case
                //   that the user is explicitly doing a Paste as Bicep command, so allow "bicep" to keep its value.
                pasteType = PasteType.BicepValue;
            }
            else
            {
                // An edge case - it could be a valid Bicep expression with comments and newlines.  This would be
                //   valid if pasting inside a multi-line array.  We don't want to convert it in this case because the
                //   comments would be removed by the decompiler.
                parser = new("var v = [\n" + json + "\n]");
                _ = parser.Program();

                if (!parser.LexingErrorLookup.Any() && !parser.ParsingErrorLookup.Any())
                {
                    // We still want to have the converted bicep available (via the "bicep" output) in the case
                    //   that the user is explicitly doing a Paste as Bicep command, so allow "bicep" to keep its value.
                    pasteType = PasteType.BicepValue;
                }
            }

            return new(new(decompileId, output.ToString(), PasteContextAsString(pasteContext), PasteTypeAsString(pasteType),
                    ErrorMessage: null, bicep, Disclaimer: null),
                GetSuccessTelemetry(queryCanPaste, decompileId, json, pasteContext, PasteTypeAsString(pasteType), bicep, languageId: languageId));

        }

        /// <summary>
        /// If the given JSON matches a pattern that we know how to paste as Bicep, convert it into a full template to be decompiled
        /// </summary>
        private (PasteType pasteType, string? fullJsonTemplate) TryConstructFullJsonTemplate(string json)
        {
            using var streamReader = new StringReader(json);
            using var reader = new JsonTextReader(streamReader);
            reader.SupportMultipleContent = true; // Allows for handling of lists of resources separated by commas

            if (LoadValue(reader, readFirst: true) is not { } value)
            {
                return (PasteType.None, null);
            }

            if (value.Type != JTokenType.Object)
            {
                return (PasteType.None, null);
            }

            var obj = (JObject)value;
            if (TryGetStringProperty(obj, "$schema") is { } schema)
            {
                // Template converter will do a more thorough check, we just want to know if it *looks* like a template
                var looksLikeArmSchema = LanguageConstants.ArmTemplateSchemaRegex.IsMatch(schema);
                if (looksLikeArmSchema)
                {
                    // Json is already a full template
                    return (PasteType.FullTemplate, json);
                }
                else
                {
                    return (PasteType.None, null);
                }
            }

            // If it's a resource object or a list of resource objects, accept it
            if (IsResourceObject(obj))
            {
                return ConstructFullTemplateFromSequenceOfResources(obj, reader);
            }

            return (PasteType.None, null);
        }

        /// <summary>
        /// Handles an optionally comma-separated sequence of JSON resource objects:
        ///
        ///   {
        ///     apiVersion: "..."
        ///     ...
        ///   },
        ///   {
        ///     apiVersion: "..."
        ///     ...
        ///   }
        ///
        /// Note that this is not a valid JSON construct by itself, unless it's just a single resource
        /// </summary>
        private static (PasteType pasteType, string constructedJsonTemplate) ConstructFullTemplateFromSequenceOfResources(JObject firstResourceObject, JsonTextReader reader)
        {
            Debug.Assert(IsResourceObject(firstResourceObject));
            Debug.Assert(reader.TokenType == JsonToken.EndObject, "Reader should be on end squiggly of first resource object");

            var resourceObjects = new List<JObject>();

            var obj = firstResourceObject;
            while (obj is not null)
            {
                if (IsResourceObject(obj))
                {
                    resourceObjects.Add(obj);
                }

                try
                {
                    if (!reader.Read())
                    {
                        break;
                    }

                    SkipComments(reader);

                    if (reader.TokenType != JsonToken.StartObject)
                    {
                        break;
                    }

                    obj = LoadValue(reader, readFirst: false) as JObject;
                }
                catch (Exception)
                {
                    // Ignore any additional JSON
                    break;
                }
            }

            var resourcesAsJson = string.Join(",\n", resourceObjects.Select(ro => ro.ToString()));
            var templateJson = """{ "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#", "contentVersion": "1.0.0.0", "resources": [""" +
                    resourcesAsJson
                    + "]}";

            return (
                resourceObjects.Count == 1 ? PasteType.SingleResource : PasteType.ResourceList,
                templateJson
            );
        }

        private static void SkipComments(JsonTextReader reader)
        {
            while (reader.TokenType == JsonToken.Comment)
            {
                reader.Read();
            }
        }

        private static bool IsResourceObject(JObject? obj)
        {
            return obj is not null
              && !string.IsNullOrEmpty(TryGetStringProperty(obj, "type"))
              && !string.IsNullOrEmpty(TryGetStringProperty(obj, "name"))
              && !string.IsNullOrEmpty(TryGetStringProperty(obj, "apiVersion"));
        }

        private static JToken? LoadValue(JsonTextReader reader, bool readFirst)
        {
            try
            {
                if (readFirst && !reader.Read())
                {
                    return null;
                }
                if (reader.TokenType == JsonToken.None)
                {
                    return null;
                }

                return JToken.Load(reader, new()
                {
                    CommentHandling = CommentHandling.Ignore,
                });
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private static JProperty? TryGetProperty(JObject obj, string name)
            => obj.Property(name, StringComparison.OrdinalIgnoreCase);

        private static string? TryGetStringProperty(JObject obj, string name)
            => (TryGetProperty(obj, name)?.Value as JValue)?.Value as string;

        /// <summary>
        /// If the given JSON matches a pattern that we know how to paste as Bicep, convert it into a full json params to be decompiled
        /// </summary>
        private (PasteType pasteType, string? fullJsonTemplate) TryConstructFullJsonParams(string json)
        {
            using var streamReader = new StringReader(json);
            using var reader = new JsonTextReader(streamReader);
            reader.SupportMultipleContent = true; // Allows for handling of lists of resources separated by commas

            if (LoadValue(reader, readFirst: true) is not { } value)
            {
                return (PasteType.None, null);
            }

            if (value.Type != JTokenType.Object)
            {
                return (PasteType.None, null);
            }

            var obj = (JObject)value;
            if (TryGetStringProperty(obj, "$schema") is not { } schema)
            {
                return (PasteType.None, null);
            }

            // Template converter will do a more thorough check, we just want to know if it *looks* like a template
            var looksLikeArmSchema = LanguageConstants.ArmParametersSchemaRegex.IsMatch(schema);
            if (looksLikeArmSchema)
            {
                // Json is already a full json params
                return (PasteType.FullParams, json);
            }

            return (PasteType.None, null);

        }

        private static string? PasteTypeAsString(PasteType pasteType) => pasteType switch
        {
            PasteType.None => null,
            PasteType.FullTemplate => "fullTemplate",
            PasteType.SingleResource => "resource",
            PasteType.ResourceList => "resourceList",
            PasteType.JsonValue => "jsonValue",
            PasteType.BicepValue => "bicepValue",
            PasteType.FullParams => "fullParams",
            _ => throw new($"Unexpected pasteType value {pasteType}"),
        };
    }
}
