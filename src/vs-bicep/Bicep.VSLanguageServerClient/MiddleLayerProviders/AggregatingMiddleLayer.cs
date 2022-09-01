// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Client;
using Newtonsoft.Json.Linq;

namespace Bicep.VSLanguageServerClient.MiddleLayerProviders
{
    /// <summary>
    /// Composite middle layer that aggregates all instances of <see cref="ILanguageClientMiddleLayer"/>
    /// and delegates the interception to appropriate middle layer.
    /// </summary>
    public class AggregatingMiddleLayer : ILanguageClientMiddleLayer
    {
        private readonly ILanguageClientMiddleLayer[] languageClientMiddleLayers;

        public AggregatingMiddleLayer(params ILanguageClientMiddleLayer[] languageClientMiddleLayers)
        {
            this.languageClientMiddleLayers = languageClientMiddleLayers;
        }

        public bool CanHandle(string methodName)
        {
            if (languageClientMiddleLayers.Any(ml => ml.CanHandle(methodName)))
            {
                return true;
            }

            return false;
        }

        public async Task HandleNotificationAsync(string methodName, JToken methodParam, Func<JToken, Task> sendNotification)
        {
            bool handled = false;

            foreach (ILanguageClientMiddleLayer languageClientMiddleLayer in languageClientMiddleLayers)
            {
                if (languageClientMiddleLayer.CanHandle(methodName))
                {
                    await languageClientMiddleLayer.HandleNotificationAsync(methodName, methodParam, sendNotification);

                    handled = true;
                    break;
                }
            }

            if (!handled)
            {
                await sendNotification(methodParam);
            }
        }

        public async Task<JToken?> HandleRequestAsync(string methodName, JToken methodParam, Func<JToken, Task<JToken?>> sendRequest)
        {
            JToken? result = null;
            bool handled = false;

            foreach (ILanguageClientMiddleLayer languageClientMiddleLayer in languageClientMiddleLayers)
            {
                if (languageClientMiddleLayer.CanHandle(methodName))
                {
                    result = await languageClientMiddleLayer.HandleRequestAsync(methodName, methodParam, sendRequest);

                    handled = true;
                    break;
                }
            }

            if (!handled)
            {
                result = await sendRequest(methodParam);
            }

            return result;
        }
    }
}
