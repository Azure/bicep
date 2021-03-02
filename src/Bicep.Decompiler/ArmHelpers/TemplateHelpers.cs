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

        public static JToken? GetNestedProperty(JObject parent, params string[] names)
        {
            JToken? current = parent;
            foreach (var name in names)
            {
                if (current is not JObject currentObject)
                {
                    return null;
                }

                current = GetProperty(currentObject, name)?.Value;
            }

            return current;
        }

        public static bool HasProperty(JObject parent, string name)
            => GetProperty(parent, name) != null;

        public static void AssertUnsupportedProperty(JObject parent, string propertyName, string message)
        {
            if (HasProperty(parent, propertyName))
            {
                throw new ConversionFailedException(message, parent);
            }
        }

        public static JToken AssertRequiredProperty(JObject parent, string propertyName, string message)
        {
            if (GetProperty(parent, propertyName) is not {} value)
            {
                throw new ConversionFailedException(message, parent);
            }

            return value.Value;            
        }

        public static (string type, string name, string apiVersion) ParseResource(JObject resource)
        {
            var type = AssertRequiredProperty(resource, "type", $"Unable to parse \"type\" for resource").ToString();
            var name = AssertRequiredProperty(resource, "name", $"Unable to parse \"name\" for resource").ToString();
            var apiVersion = AssertRequiredProperty(resource, "apiVersion", $"Unable to parse \"apiVersion\" for resource").ToString();
            
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