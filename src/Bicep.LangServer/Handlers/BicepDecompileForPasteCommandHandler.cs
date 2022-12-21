// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Decompiler;
using Bicep.LanguageServer.Telemetry;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Handlers
{
    public record BicepDecompileForPasteCommandParams(
        string jsonContent,
        bool queryCanPaste  // True if client is testing clipboard text for menu enabling only, false if the user actually requested a paste
    );

    public record BicepDecompileForPasteCommandResult
    (
        string DecompileId, // Used to synchronize telemetry events
        string Output,
        string? PasteType,
        string? ErrorMessage, // This is null if pasteType == null, otherwise indicates an error trying to decompile to the given paste type
        string? Bicep, // Null if pasteType == null or errorMessage != null
        string? Disclaimer // Non-null if the decompilation is not foolproof (for templates, resources etc)
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
            return telemetryHelper.ExecuteWithTelemetryAndErrorHandling(async () =>
            {
                var (result, successTelemetry) = await TryDecompileForPaste(parameters.jsonContent, parameters.queryCanPaste);
                return (result, successTelemetry);
            });
        }

        private async Task<ResultAndTelemetry> TryDecompileForPaste(string json, bool queryCanPaste)
        {
            StringBuilder output = new StringBuilder();
            string decompileId = Guid.NewGuid().ToString();

            var (pasteType, constructedJsonTemplate, needsDisclaimer) = ConstructFullJsonTemplate(output, json);
            if (pasteType is null)
            {
                // It's not anything we know how to convert to Bicep
                return new ResultAndTelemetry(
                    new BicepDecompileForPasteCommandResult(
                        decompileId, output.ToString(), PasteType: null, ErrorMessage: null,
                        Bicep: null, Disclaimer: null),
                    GetSuccessTelemetry(queryCanPaste, decompileId, json, pasteType: null, bicep: null));
            }

            ImmutableDictionary<Uri, string> filesToSave;
            try
            {
                // Decompile the full template
                Debug.Assert(constructedJsonTemplate is not null);
                Log(output, String.Format(LangServerResources.Decompile_DecompilationStartMsg, "clipboard text"));
                var singleFileResolver = new SingleFileResolver(JsonDummyUri, constructedJsonTemplate);

                var decompiler = new BicepDecompiler(this.bicepCompiler, singleFileResolver);
                var options = new DecompileOptions()
                {
                    // For partial template pastes, we don't error out on missing parameters and variables because they won't
                    //   ever have definitions in the pasted portion
                    AllowMissingParamsAndVars = pasteType != PasteType_FullTemplate,
                    // ... but don't allow them in nested templates, which should be fully complete and valid
                    AllowMissingParamsAndVarsInNestedTemplates = false
                };
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
                    new BicepDecompileForPasteCommandResult(decompileId, output.ToString(), pasteType, message, Bicep: null, Disclaimer: null),
                    GetSuccessTelemetry(queryCanPaste, decompileId, json, pasteType, bicep: null));
            }

            // Show disclaimer and completion
            string disclaimerMessage = "";
            if (needsDisclaimer)
            {
                disclaimerMessage = $"{LangServerResources.DecompileAsPaste_AutoConvertWarning}\n{BicepDecompiler.DecompilerDisclaimerMessage}";
                Log(output, disclaimerMessage);
            }

            // Get Bicep output from the main file (all others are currently ignored)
            string bicepOutput = filesToSave.Single(kvp => BicepDummyUri.Equals(kvp.Key)).Value;

            // Ensure ends with newline
            bicepOutput = bicepOutput.TrimEnd() + "\n";

            // Return result
            return new ResultAndTelemetry(
                new BicepDecompileForPasteCommandResult(decompileId, output.ToString(), pasteType, null, bicepOutput, disclaimerMessage),
                // Don't log telemetry if we're just determining if we can paste, because this will happen a lot
                //   (on changing between editors for instance)
                GetSuccessTelemetry(queryCanPaste, decompileId, json, pasteType, bicepOutput));
        }

        private static void Log(StringBuilder output, string message)
        {
            output.AppendLine(message);
            Trace.TraceInformation(message);
        }

        private BicepTelemetryEvent? GetSuccessTelemetry(bool queryCanPaste, string decompileId, string json, string? pasteType, string? bicep)
        {
            // Don't log telemetry if we're just determining if we can paste, because this will happen a lot
            //   (on changing between editors for instance)
            // TODO: but we don't call back for telemetry if we use the result
            return queryCanPaste ?
                null :
                BicepTelemetryEvent.DecompileForPaste(decompileId, pasteType, json.Length, bicep?.Length);
        }

        /// <summary>
        /// If the given JSON matches a pattern that we know how to paste as Bicep, convert it into a full template to be decompiled
        /// </summary>
        private (string? pasteType, string? fullJsonTemplate, bool needsDisclaimer) ConstructFullJsonTemplate(StringBuilder output, string json)
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
                        var looksLikeArmSchema = schema.Contains("deploymentTemplate.json", StringComparison.InvariantCultureIgnoreCase) == true;
                        if (looksLikeArmSchema)
                        {
                            return this.ConstructFullTemplateFromTemplateObject(json);
                        }
                        else
                        {
                            return (PasteType_None, null, needsDisclaimer: false);
                        }
                    }

                    // If it's a resource object or a list of resource objects, accept it
                    if (IsResourceObject(obj))
                    {
                        return ConstructFullTemplateFromSequenceOfResources(json, obj, reader);
                    }
                }
            }

            return (PasteType_None, null, needsDisclaimer: false);
        }

        private (string pasteType, string constructedJsonTemplate, bool needsDisclaimer) ConstructFullTemplateFromTemplateObject(string json)
        {
            // Json is already a full template
            return (PasteType_FullTemplate, json, needsDisclaimer: true);
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
        private (string pasteType, string constructedJsonTemplate, bool needsDisclaimer) ConstructFullTemplateFromSequenceOfResources(string json, JObject firstResourceObject, JsonTextReader reader)
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
                templateJson,
                true
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

                return JContainer.Load(reader, new JsonLoadSettings
                {
                    CommentHandling = CommentHandling.Ignore,
                });
            }
            catch (JsonException)
            {
                return null;
            }
        }

        public static JProperty? TryGetProperty(JObject obj, string name)
            => obj.Property(name, StringComparison.OrdinalIgnoreCase);

        public static string? TryGetStringProperty(JObject obj, string name)
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
    }
}
