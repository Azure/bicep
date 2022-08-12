// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharpLocation = OmniSharp.Extensions.LanguageServer.Protocol.Models.Location;
using OmniSharpRange = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;
using VSLocation = Microsoft.VisualStudio.LanguageServer.Protocol.Location;
using VSPosition = Microsoft.VisualStudio.LanguageServer.Protocol.Position;
using VSRange = Microsoft.VisualStudio.LanguageServer.Protocol.Range;

namespace Bicep.VSLanguageServerClient.MiddleLayerProviders
{
    /// <summary>
    /// This middle layer supports goto defintion in Visual Studio
    /// VS lsp model is different from O#. We need to convert the output obtained from bicep language server to a format that is
    /// serializable by VS language server client.
    /// </summary>
    public class HandleGotoDefintionMiddleLayer : ILanguageClientMiddleLayer
    {
        public bool CanHandle(string methodName)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                return false;
            }

            return methodName.Equals(Methods.TextDocumentDefinitionName, StringComparison.Ordinal);
        }

        public Task HandleNotificationAsync(string methodName, JToken methodParam, Func<JToken, Task> sendNotification) => sendNotification(methodParam);

        public async Task<JToken?> HandleRequestAsync(string methodName, JToken methodParam, Func<JToken, Task<JToken?>> sendRequest)
        {
            if (CanHandle(Methods.TextDocumentDefinitionName))
            {
                JToken? jToken = await sendRequest(methodParam);
                var locations = new List<VSLocation>();

                if (jToken is not null)
                {
                    foreach (var child in jToken.Children())
                    {
                        var locationOrLocationLink = child.ToObject<LocationOrLocationLink>();

                        if (locationOrLocationLink is not null)
                        {
                            var vsLocation = GetVSLocation(locationOrLocationLink);

                            if (vsLocation is not null)
                            {
                                locations.Add(vsLocation);
                            }
                        }
                    }

                    return JToken.FromObject(locations);
                }
            }

            return await sendRequest(methodParam);
        }

        public VSLocation? GetVSLocation(LocationOrLocationLink locationOrLocationLink)
        {
            if (locationOrLocationLink is not null)
            {
                if (locationOrLocationLink.IsLocationLink && locationOrLocationLink.LocationLink is LocationLink locationLink)
                {
                    return GetVSLocation(locationLink.TargetUri, locationLink.TargetSelectionRange);
                }
                else if (locationOrLocationLink.IsLocation && locationOrLocationLink.Location is OmniSharpLocation omnisharpLocation)
                {
                    return GetVSLocation(omnisharpLocation.Uri, omnisharpLocation.Range);
                }
            }

            return null;
        }

        private VSLocation GetVSLocation(DocumentUri documentUri, OmniSharpRange omniSharpRange)
        {
            var start = omniSharpRange.Start;
            var end = omniSharpRange.End;

            var vsRange = new VSRange()
            {
                Start = new VSPosition(start.Line, start.Character),
                End = new VSPosition(end.Line, end.Character)
            };

            return new VSLocation() { Uri = documentUri.ToUri(), Range = vsRange };
        }
    }
}
