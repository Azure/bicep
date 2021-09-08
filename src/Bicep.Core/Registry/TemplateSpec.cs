// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Azure.ResourceManager.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Registry
{
    // TODO: remove this once Template Spec track 2 SDK is available.
    public class TemplateSpec
    {
        private TemplateSpec(string mainTemplateContents)
        {
            this.MainTemplateContents = mainTemplateContents;
        }

        public string MainTemplateContents { get; }

        public static TemplateSpec FromGenericResourceData(GenericResourceData data)
        {
            try
            {
                var templateSpecProperties = JObject.FromObject(data.Properties);

                if (!templateSpecProperties.TryGetValue("mainTemplate", out var mainTemplate))
                {
                    throw CreateMalformedTemplateSpecException();
                }

                return new(mainTemplate.ToString());
            }
            catch (Exception exception) when (exception is ArgumentException or JsonException)
            {
                throw CreateMalformedTemplateSpecException();
            }
        }

        private static TemplateSpecException CreateMalformedTemplateSpecException() => new("The referenced Template Spec is malformed.");
    }
}
