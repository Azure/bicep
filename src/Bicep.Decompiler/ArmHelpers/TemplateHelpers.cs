// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Azure.Deployments.Expression.Engines;
using Azure.Deployments.Expression.Expressions;
using Bicep.Decompiler.Exceptions;
using Newtonsoft.Json.Linq;

namespace Bicep.Decompiler.ArmHelpers
{
    public static class TemplateHelpers
    {
        public static JProperty? GetProperty(JObject parent, string name)
            => parent.Property(name, StringComparison.OrdinalIgnoreCase);

        public static bool HasProperty(JObject parent, string name)
            => GetProperty(parent, name) != null;

        public static void AssertUnsupportedProperty(JObject resource, string propertyName, string message)
        {
            if (HasProperty(resource, propertyName))
            {
                throw new ConversionFailedException(message, resource);
            }
        }

        public static (string type, string name, string apiVersion) ParseResource(JObject resource)
        {
            var type = GetProperty(resource, "type")?.Value.Value<string>() ?? throw new ConversionFailedException($"Unable to parse 'type' for resource", resource);
            var name = GetProperty(resource, "name")?.Value.Value<string>() ?? throw new ConversionFailedException($"Unable to parse 'name' for resource", resource);
            var apiVersion = GetProperty(resource, "apiVersion")?.Value.Value<string>() ?? throw new ConversionFailedException($"Unable to parse 'apiVersion' for resource", resource);
            
            return (type, name, apiVersion);
        }

        public static IEnumerable<JObject> FlattenAndNormalizeResource(JToken resourceJtoken)
        {
            var resource = resourceJtoken as JObject ?? throw new ConversionFailedException($"Unable to read resource", resourceJtoken);

            var (parentType, parentName, _) = ParseResource(resource);

            var childResources = GetProperty(resource, "resources");
            if (childResources != null)
            {
                childResources.Remove();
            }

            yield return resource;

            var childResourcesArray = childResources?.Value as JArray;
            if (childResourcesArray is null)
            {
                yield break;
            }

            foreach (var childResource in childResourcesArray)
            {
                var childResourceObject = childResource as JObject ?? throw new ConversionFailedException($"Unable to read child resource", childResource);
                
                var (childType, childName, _) = ParseResource(childResourceObject);

                // child may sometimes be specified using the fully-qualified type and name
                if (!StringComparer.OrdinalIgnoreCase.Equals(parentType.Split("/")[0], childType.Split("/")[0]))
                {
                    childResourceObject["type"] = $"{parentType}/{childType}";
                    childResourceObject["name"] = ExpressionsEngine.SerializeExpression(ExpressionHelpers.Concat(ExpressionHelpers.ParseExpression(parentName), new JTokenExpression("/"), ExpressionHelpers.ParseExpression(childName)));
                }

                foreach (var result in FlattenAndNormalizeResource(childResourceObject))
                {
                    // recurse
                    yield return result;
                }
            }
        }
    }
}