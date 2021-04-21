// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Deployments.Core.Extensions;
using Azure.Deployments.Core.Utilities;
using Azure.Deployments.Expression.Configuration;
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

        public static void RemoveNestedProperty(JObject parent, params string[] names)
        {
            if (GetNestedProperty(parent, names.SkipLast(1).ToArray()) is {} directParent)
            {
                if (directParent is JObject directParentObject &&
                    GetProperty(directParentObject, names.Last()) is {} property)
                {
                    property.Remove();
                }
            }
        }

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

                if (GetProperty(resource, "copy") is {} copyProperty)
                {
                    childResourceObject["copy"] = copyProperty.Value;
                }
                if (GetProperty(resource, "condition") is {} conditionProperty)
                {
                    childResourceObject["condition"] = conditionProperty.Value;
                }

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

        public static (JObject template, IReadOnlyDictionary<string, (FunctionExpression expression, string type)> parameters) ConvertNestedTemplateInnerToOuter(JObject template)
        {
            // this is useful to avoid naming clashes - we should prioritize names that have come from the parent template
            var paramsAccessed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            JTokenHelpers.VisitExpressions(template, expression => 
            {
                if (ExpressionHelpers.TryGetNamedFunction(expression, "parameters") is not {} function)
                {
                    return;
                }

                if (function.Parameters.Length != 1 || function.Parameters[0] is not JTokenExpression parameterName)
                {
                    throw new InvalidOperationException($"Found \"{function.Function}\" function with invalid signature");
                }

                paramsAccessed.Add(parameterName.Value.ToString());
            });

            // we don't want to populate this when we first visit parameters, because we may end up removing parameters
            // - for example if they are only in-use inside 'reference()' functions
            var parameters = new Dictionary<string, (FunctionExpression expression, string type)>(StringComparer.OrdinalIgnoreCase);

            LanguageExpression ReplaceWithParameter(FunctionExpression function, string type, LanguageExpression paramNameExpression)
            {
                var withoutProperties = new FunctionExpression(
                    function.Function,
                    function.Parameters,
                    Array.Empty<LanguageExpression>());
                
                var paramNameSerialized = ExpressionsEngine.SerializeExpression(paramNameExpression);
                var paramName = UniqueNamingResolver.EscapeIdentifier(paramNameSerialized);

                if (paramsAccessed.Contains(paramName))
                {
                    paramName = $"generated_{paramName}";
                }

                if (!parameters.ContainsKey(paramName))
                {
                    parameters[paramName] = (withoutProperties, type);
                }

                return new FunctionExpression(
                    "parameters",
                    new [] { new JTokenExpression(paramName) },
                    function.Properties);
            }

            // process references first
            template = JTokenHelpers.RewriteExpressions(template, expression =>
            {
                if (ExpressionHelpers.TryGetNamedFunction(expression, "reference") is not {} function)
                {
                    return expression;
                }

                var paramNameExpression = function;
                if (function.Parameters.Length > 0 && ExpressionHelpers.TryGetNamedFunction(function.Parameters[0], "resourceId") is {} firstParam)
                {
                    paramNameExpression = firstParam;
                }

                return ReplaceWithParameter(function, "object", paramNameExpression);
            });

            // process resourceIds
            template = JTokenHelpers.RewriteExpressions(template, expression => 
            {
                if (ExpressionHelpers.TryGetNamedFunction(expression, "resourceId") is not {} function)
                {
                    return expression;
                }

                return ReplaceWithParameter(function, "string", function);
            });

            // process variables
            template = JTokenHelpers.RewriteExpressions(template, expression => 
            {
                if (ExpressionHelpers.TryGetNamedFunction(expression, "variables") is not {} function)
                {
                    return expression;
                }

                return ReplaceWithParameter(function, "__BICEP_REPLACE", function);
            });

            // unescape escaped expressions
            template = JTokenHelpers.RewriteExpressions(template, expression => 
            {
                if (expression is not JTokenExpression jtoken)
                {
                    return expression;
                }

                var stringValue = jtoken.Value.ToString();
                if (!ExpressionsEngine.IsLanguageExpression(stringValue))
                {
                    return expression;
                }
                
                return ExpressionsEngine.ParseLanguageExpression(stringValue);
            });

            // add parameters to lookup
            JTokenHelpers.VisitExpressions(template, expression => 
            {
                if (ExpressionHelpers.TryGetNamedFunction(expression, "parameters") is not {} function)
                {
                    return;
                }

                if (function.Parameters.Length != 1 || function.Parameters[0] is not JTokenExpression parameterName)
                {
                    throw new InvalidOperationException($"Found \"{function.Function}\" function with invalid signature");
                }

                var paramNameString = parameterName.Value.ToString();
                if (!parameters.ContainsKey(paramNameString))
                {
                    var paramExpression = new FunctionExpression(
                        "parameters",
                        function.Parameters,
                        Array.Empty<LanguageExpression>());

                    parameters[parameterName.Value.ToString()] = (paramExpression, "__BICEP_REPLACE");
                }
            });

            return (template, parameters);
        }
    }
}