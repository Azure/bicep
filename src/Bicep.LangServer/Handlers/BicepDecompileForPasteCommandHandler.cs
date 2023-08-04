// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Extensions;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrint;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Bicep.Decompiler;
using Bicep.LanguageServer.Telemetry;
using Bicep.LanguageServer.Utils;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Bicep.LanguageServer.Handlers
{
    public record BicepDecompileForPasteCommandParams(
        string bicepContent,
        int rangeOffset,
        int rangeLength,
        string jsonContent,
        bool queryCanPaste  // True if client is testing clipboard text for menu enabling only, false if the user actually requested a paste
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
    public class BicepDecompileForPasteCommandHandler : ExecuteTypedResponseCommandHandlerBase<BicepDecompileForPasteCommandParams, BicepDecompileForPasteCommandResult>
    {
        private readonly TelemetryAndErrorHandlingHelper<BicepDecompileForPasteCommandResult> telemetryHelper;
        private readonly BicepCompiler bicepCompiler;

        private static readonly Uri JsonDummyUri = new Uri("file://from-clipboard.json", UriKind.Absolute);
        private static readonly Uri BicepDummyUri = PathHelper.ChangeToBicepExtension(JsonDummyUri);

        public const string? PasteType_None = null;
        public const string PasteType_FullTemplate = "fullTemplate"; // Full template
        public const string PasteType_SingleResource = "resource"; // Single resource
        public const string PasteType_ResourceList = "resourceList"; // List of multiple resources
        public const string PasteType_JsonValue = "jsonValue"; // Single JSON value (number, object, array etc)
        public const string PasteType_BicepValue = "bicepValue"; // JSON value that is also valid Bicep (e.g. "[1, {}]")

        public enum PasteContext
        {
            None,
            String, // Pasting inside of a string
        }

        private record ResultAndTelemetry(BicepDecompileForPasteCommandResult Result, BicepTelemetryEvent? SuccessTelemetry);

        public BicepDecompileForPasteCommandHandler(
            ISerializer serializer,
            ILanguageServerFacade server,
            ITelemetryProvider telemetryProvider,
            BicepCompiler bicepCompiler
        )
            : base(LangServerConstants.DecompileForPasteCommand, serializer)
        {
            this.telemetryHelper = new TelemetryAndErrorHandlingHelper<BicepDecompileForPasteCommandResult>(server.Window, telemetryProvider);
            this.bicepCompiler = bicepCompiler;
        }

        public override Task<BicepDecompileForPasteCommandResult> Handle(BicepDecompileForPasteCommandParams parameters, CancellationToken cancellationToken)
        {
            return telemetryHelper.ExecuteWithTelemetryAndErrorHandling((Func<Task<(BicepDecompileForPasteCommandResult result, BicepTelemetryEvent? successTelemetry)>>)(async () =>
            {
                var (result, successTelemetry) = await TryDecompileForPaste(
                    (string)parameters.bicepContent,
                    parameters.rangeOffset,
                    parameters.rangeLength,
                    parameters.jsonContent,
                    parameters.queryCanPaste);
                return (result: (BicepDecompileForPasteCommandResult)result, successTelemetry: (BicepTelemetryEvent?)successTelemetry);
            }));
        }

        private PasteContext GetPasteContext(string bicepContents, int offset, int length)
        {
            var contents = bicepContents;
            var newContents = contents.Substring(0, offset) + contents.Substring(offset + length);
            var parser = new Parser(newContents);
            var program = parser.Program();

            // Find the innermost string that contains the given offset, and which isn't inside an interpolation hole.
            // Note that a hole can contain nested strings which may contain holes...
            var stringSyntax = (StringSyntax?)program.TryFindMostSpecificNodeInclusive(offset, syntax =>
            {
                if (syntax is StringSyntax stringSyntax)
                {
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
                }
                else
                {
                    return false;
                }
            });

            return stringSyntax is null ? PasteContext.None : PasteContext.String;
        }
        private string DisclaimerMessage => $"{BicepDecompiler.DecompilerDisclaimerMessage}";

        private static void Log(StringBuilder output, string message)
        {
            output.AppendLine(message);
            Trace.TraceInformation(message);
        }

        private async Task<ResultAndTelemetry> TryDecompileForPaste(string bicepContents, int rangeOffset, int rangeLength, string json, bool queryCanPaste)
        {
            StringBuilder output = new StringBuilder();
            string decompileId = Guid.NewGuid().ToString();
            var pasteContext = GetPasteContext(bicepContents, rangeOffset, rangeLength);

            if (pasteContext == PasteContext.String)
            {
                // Don't convert to Bicep if inside a string
                return new ResultAndTelemetry(
                    new BicepDecompileForPasteCommandResult(
                        decompileId, output.ToString(), PasteContextAsString(pasteContext), PasteType: null, ErrorMessage: null,
                        Bicep: null, Disclaimer: null),
                    GetSuccessTelemetry(queryCanPaste, decompileId, json, pasteContext, pasteType: null, bicep: null));
            }

            if (!string.IsNullOrWhiteSpace(json))
            {
                var (pasteType, constructedJsonTemplate) = TryConstructFullJsonTemplate(json);
                if (pasteType is null)
                {
                    // It's not a template or resource.  Try treating it as a JSON value.
                    var resultAndTelemetry = TryConvertFromJsonValue(output, json, decompileId, pasteContext, queryCanPaste);
                    if (resultAndTelemetry is not null)
                    {
                        return resultAndTelemetry;
                    }
                }
                else
                {
                    // It's a full or partial template and we have converted it into a full template to parse
                    var result = await TryConvertFromConstructedTemplate(output, json, decompileId, pasteContext, pasteType, queryCanPaste, constructedJsonTemplate);
                    if (result is not null)
                    {
                        return result;
                    }
                }
            }

            // It's not anything we know how to convert to Bicep
            return new ResultAndTelemetry(
                new BicepDecompileForPasteCommandResult(
                    decompileId, output.ToString(), PasteContextAsString(pasteContext), PasteType: null, ErrorMessage: null,
                    Bicep: null, Disclaimer: null),
                GetSuccessTelemetry(queryCanPaste, decompileId, json, pasteContext, pasteType: null, bicep: null));
        }

        private async Task<ResultAndTelemetry?> TryConvertFromConstructedTemplate(StringBuilder output, string json, string decompileId, PasteContext pasteContext, string pasteType, bool queryCanPaste, string? constructedJsonTemplate)
        {
            ImmutableDictionary<Uri, string> filesToSave;
            try
            {
                // Decompile the full template
                Debug.Assert(constructedJsonTemplate is not null);
                Log(output, String.Format(LangServerResources.Decompile_DecompilationStartMsg, "clipboard text"));
                var singleFileResolver = new SingleFileResolver(JsonDummyUri, constructedJsonTemplate);

                var decompiler = new BicepDecompiler(this.bicepCompiler, singleFileResolver);
                var options = GetDecompileOptions(pasteType);
                (_, filesToSave) = await decompiler.Decompile(JsonDummyUri, BicepDummyUri, options);
            }
            catch (Exception ex)
            {
                // We don't ever throw. If we reached here, the pasted text was in a format we think we can handle but there was some
                //   sort of error.  Tell the client it can be pasted and let the client show the end user the error if they do.
                //   deal with any bicep errors found.
                var message = ex.Message;
                Log(output, $"Decompilation failed: {message}");

                return new ResultAndTelemetry(
                    new BicepDecompileForPasteCommandResult(decompileId, output.ToString(), PasteContextAsString(pasteContext), pasteType, message, Bicep: null, Disclaimer: null),
                    GetSuccessTelemetry(queryCanPaste, decompileId, json, pasteContext, pasteType, bicep: null));
            }

            // Get Bicep output from the main file (all others are currently ignored)
            string bicepOutput = filesToSave.Single(kvp => BicepDummyUri.Equals(kvp.Key)).Value;

            if (string.IsNullOrWhiteSpace(bicepOutput))
            {
                return null;
            }

            // Ensure ends with newline
            bicepOutput = bicepOutput.TrimEnd() + "\n";

            // Show disclaimer and return result
            Log(output, DisclaimerMessage);
            return new ResultAndTelemetry(
                new BicepDecompileForPasteCommandResult(decompileId, output.ToString(), PasteContextAsString(pasteContext), pasteType, null, bicepOutput, DisclaimerMessage),
                GetSuccessTelemetry(queryCanPaste, decompileId, json, pasteContext, pasteType, bicepOutput));
        }

        private string? PasteContextAsString(PasteContext pasteContext)
        {
            return pasteContext switch
            {
                PasteContext.None => "none",
                PasteContext.String => "string",
                _ => throw new Exception($"Unexpected pasteContext value {pasteContext}"),
            };
        }

        private BicepTelemetryEvent? GetSuccessTelemetry(bool queryCanPaste, string decompileId, string json, PasteContext pasteContext, string? pasteType, string? bicep)
        {

            // Don't log telemetry if we're just determining if we can paste, because this will happen a lot
            //   (on changing between editors for instance)
            // TODO: but we don't call back for telemetry if we use the result
            return queryCanPaste ?
                null :
                BicepTelemetryEvent.DecompileForPaste(decompileId, PasteContextAsString(pasteContext), pasteType, json.Length, bicep?.Length);
        }

        private DecompileOptions GetDecompileOptions(string pasteType)
        {
            return new DecompileOptions()
            {
                // For partial template pastes, we don't error out on missing parameters and variables because they won't
                //   ever have definitions in the pasted portion
                AllowMissingParamsAndVars = pasteType != PasteType_FullTemplate,
                // ... but don't allow them in nested templates, which should be fully complete and valid
                AllowMissingParamsAndVarsInNestedTemplates = false,
                IgnoreTrailingInput = pasteType == PasteType_JsonValue ? false : true,
            };
        }

        private ResultAndTelemetry? TryConvertFromJsonValue(StringBuilder output, string json, string decompileId, PasteContext pasteContext, bool queryCanPaste)
        {
            var singleFileResolver = new SingleFileResolver(JsonDummyUri, json);

            // Is it valid JSON that we can convert into Bicep?
            var decompiler = new BicepDecompiler(this.bicepCompiler, singleFileResolver);
            var pasteType = PasteType_JsonValue;
            var options = GetDecompileOptions(pasteType);
            var bicep = decompiler.DecompileJsonValue(json, options);
            if (bicep is not null)
            {
                // Technically we've already converted, but we only want to show this message if we think the pasted text is convertible
                Log(output, String.Format(LangServerResources.Decompile_DecompilationStartMsg, "clipboard text"));

                // Even though it's valid JSON, it might also be valid Bicep, in which case we want to leave it alone if we're
                //   doing an automatic copy/paste conversion.

                // Is the input already a valid Bicep expression with comments removed?
                var parser = new Parser("var v = " + json);
                _ = parser.Program();
                if (!parser.LexingErrorLookup.Any() && !parser.ParsingErrorLookup.Any())
                {
                    // We still want to have the converted bicep available (via the "bicep" output) in the case
                    //   that the user is explicitly doing a Paste as Bicep command, so allow "bicep" to keep its value.
                    pasteType = PasteType_BicepValue;
                }
                else
                {
                    // An edge case - it could be a valid Bicep expression with comments and newlines.  This would be
                    //   valid if pasting inside a multi-line array.  We don't want to convert it in this case because the
                    //   comments would be removed by the decompiler.
                    parser = new Parser("var v = [\n" + json + "\n]");
                    _ = parser.Program();

                    if (!parser.LexingErrorLookup.Any() && !parser.ParsingErrorLookup.Any())
                    {
                        // We still want to have the converted bicep available (via the "bicep" output) in the case
                        //   that the user is explicitly doing a Paste as Bicep command, so allow "bicep" to keep its value.
                        pasteType = PasteType_BicepValue;
                    }
                }

                return new ResultAndTelemetry(
                    new BicepDecompileForPasteCommandResult(
                        decompileId, output.ToString(), PasteContextAsString(pasteContext), pasteType,
                        ErrorMessage: null, bicep, Disclaimer: null),
                    GetSuccessTelemetry(queryCanPaste, decompileId, json, pasteContext, pasteType, bicep));
            }

            return null;
        }

        /// <summary>
        /// If the given JSON matches a pattern that we know how to paste as Bicep, convert it into a full template to be decompiled
        /// </summary>
        private (string? pasteType, string? fullJsonTemplate) TryConstructFullJsonTemplate(string json)
        {
            using var streamReader = new StringReader(json);
            using var reader = new JsonTextReader(streamReader);
            reader.SupportMultipleContent = true; // Allows for handling of lists of resources separated by commas

            if (LoadValue(reader, readFirst: true) is JToken value)
            {
                if (value.Type == JTokenType.Object)
                {
                    var obj = (JObject)value;
                    if (TryGetStringProperty(obj, "$schema") is string schema)
                    {
                        // Template converter will do a more thorough check, we just want to know if it *looks* like a template
                        var looksLikeArmSchema = LanguageConstants.ArmTemplateSchemaRegex.IsMatch(schema);
                        if (looksLikeArmSchema)
                        {
                            return this.ConstructFullTemplateFromTemplateObject(json);
                        }
                        else
                        {
                            return (PasteType_None, null);
                        }
                    }

                    // If it's a resource object or a list of resource objects, accept it
                    if (IsResourceObject(obj))
                    {
                        return ConstructFullTemplateFromSequenceOfResources(obj, reader);
                    }
                }
            }

            return (PasteType_None, null);
        }

        private (string pasteType, string constructedJsonTemplate) ConstructFullTemplateFromTemplateObject(string json)
        {
            // Json is already a full template
            return (PasteType_FullTemplate, json);
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
        private (string pasteType, string constructedJsonTemplate) ConstructFullTemplateFromSequenceOfResources(JObject firstResourceObject, JsonTextReader reader)
        {
            Debug.Assert(IsResourceObject(firstResourceObject));
            Debug.Assert(reader.TokenType == JsonToken.EndObject, "Reader should be on end squiggly of first resource object");

            var resourceObjects = new List<JObject>();

            JObject? obj = firstResourceObject;
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
            var templateJson = @"{ ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"", ""contentVersion"": ""1.0.0.0"", ""resources"": [" +
                    resourcesAsJson
                    + "]}";

            return (
                resourceObjects.Count == 1 ? PasteType_SingleResource : PasteType_ResourceList,
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
                else if (reader.TokenType == JsonToken.None)
                {
                    return null;
                }

                return JToken.Load(reader, new JsonLoadSettings
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
    }

    /// <summary>
    /// A simple IFileResolver implementation that provides just enough capability to provide content for a single file
    /// </summary>
    class SingleFileResolver : IFileResolver
    {
        public Uri Uri { get; }
        public string contents { get; }

        public SingleFileResolver(Uri uri, string contents)
        {
            this.Uri = uri;
            this.contents = contents;
        }

        public bool DirExists(Uri fileUri)
        {
            throw new NotImplementedException();
        }

        public bool FileExists(Uri uri)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Uri> GetDirectories(Uri fileUri, string pattern = "")
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Uri> GetFiles(Uri fileUri, string pattern = "")
        {
            throw new NotImplementedException();
        }

        public string GetRelativePath(string relativeTo, string path)
        {
            throw new NotImplementedException();
        }

        public IDisposable? TryAcquireFileLock(Uri fileUri)
        {
            throw new NotImplementedException();
        }

        public bool TryRead(Uri fileUri, [NotNullWhen(true)] out string? fileContents, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            if (fileUri.Equals(this.Uri))
            {
                failureBuilder = null;
                fileContents = this.contents;
                return true;
            }

            failureBuilder = x => x.UnableToLoadNonFileUri(fileUri);
            fileContents = null;
            return false;
        }

        public bool TryRead(Uri fileUri, [NotNullWhen(true)] out string? fileContents, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder, Encoding fileEncoding, int maxCharacters, [NotNullWhen(true)] out Encoding? detectedEncoding)
        {
            throw new NotImplementedException();
        }

        public bool TryReadAsBase64(Uri fileUri, [NotNullWhen(true)] out string? fileBase64, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder, int maxCharacters = -1)
        {
            throw new NotImplementedException();
        }

        public bool TryReadAtMostNCharacters(Uri fileUri, Encoding fileEncoding, int n, [NotNullWhen(true)] out string? fileContents)
        {
            throw new NotImplementedException();
        }

        public Uri? TryResolveFilePath(Uri parentFileUri, string childFilePath)
        {
            return new Uri(Path.Combine(parentFileUri.LocalPath, childFilePath), UriKind.Absolute);
        }

        public void Write(Uri fileUri, Stream contents)
        {
            throw new NotImplementedException();
        }

        public void DeleteFileIfExists(Uri uri)
        {
            throw new NotImplementedException();
        }
    }
}
