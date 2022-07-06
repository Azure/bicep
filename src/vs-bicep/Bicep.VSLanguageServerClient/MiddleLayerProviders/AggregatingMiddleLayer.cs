// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Client;
using Newtonsoft.Json.Linq;

namespace Bicep.VSLanguageServerClient.MiddleLayerProviders
{
    public class AggregatingMiddleLayer : ILanguageClientMiddleLayer
    {
        private readonly ILanguageClientMiddleLayer[] _languageClientMiddleLayers;

        public event EventHandler<string>? NotificationHandled;
        public event EventHandler<string>? RequestHandled;

        public AggregatingMiddleLayer(params ILanguageClientMiddleLayer[] languageClientMiddleLayers)
        {
            _languageClientMiddleLayers = languageClientMiddleLayers;
        }

        public bool CanHandle(string methodName)
        {
            if (_languageClientMiddleLayers.Any(ml => ml.CanHandle(methodName)))
            {
                return true;
            }

            // If anyone is listening to our events (currently only test code), we need to indicate we handle
            //   the event as otherwise our Handle(Notification/Request)Async calls won't be invoked.
            //   This does have the unfortunate side effect that we don't work well with other middle layers.
            if (NotificationHandled is not null
                || RequestHandled is not null)
            {
                return true;
            }

            return false;
        }

        public async Task HandleNotificationAsync(string methodName, JToken methodParam, Func<JToken, Task> sendNotification)
        {
            bool handled = false;

            foreach (ILanguageClientMiddleLayer languageClientMiddleLayer in _languageClientMiddleLayers)
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

            NotificationHandled?.Invoke(this, methodName);
        }

        public async Task<JToken?> HandleRequestAsync(string methodName, JToken methodParam, Func<JToken, Task<JToken?>> sendRequest)
        {
            JToken? result = null;
            bool handled = false;

            foreach (ILanguageClientMiddleLayer languageClientMiddleLayer in _languageClientMiddleLayers)
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

            RequestHandled?.Invoke(this, methodName);
            return result;
        }
    }
}
