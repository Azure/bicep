// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Deployments.Core.Definitions.Schema;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Emit.CompileTimeImports;

internal static class ArmTemplateHelpers
{
    private const string ArmTypeRefPrefix = "#/definitions/";
    internal static SchemaValidationContext ContextFor(ArmTemplateFile templateFile)
    {
        if (templateFile.Template is not {} template)
        {
            throw new InvalidOperationException($"Source template of {templateFile.FileUri} is not valid");
        }

        return SchemaValidationContext.ForTemplate(template);
    }

    internal static ITemplateSchemaNode DerefArmType(SchemaValidationContext context, string typePointer)
    {
        // TODO make LocalSchemaRefResolver in Azure.Deployments.Templates public
        if (!typePointer.StartsWith(ArmTypeRefPrefix) ||
            typePointer.Substring(ArmTypeRefPrefix.Length).Contains('/') ||
            !context.Definitions.TryGetValue(typePointer.Substring(ArmTypeRefPrefix.Length), out var typeDefinition))
        {
            throw new InvalidOperationException($"Invalid ARM template type reference ({typePointer}) encountered");
        }

        return typeDefinition;
    }

    internal static IEnumerable<string> EnumerateTypeReferencesUsedIn(SchemaValidationContext context, string typePointer)
        => EnumerateTypeReferencesUsedIn(DerefArmType(context, typePointer));

    internal static IEnumerable<string> EnumerateTypeReferencesUsedIn(ITemplateSchemaNode schemaNode)
    {
        if (schemaNode.Ref?.Value is string @ref)
        {
            yield return @ref;
        }

        if (schemaNode.AdditionalProperties?.SchemaNode is {} addlPropertiesType)
        {
            foreach (var nested in EnumerateTypeReferencesUsedIn(addlPropertiesType))
            {
                yield return nested;
            }
        }

        if (schemaNode.Properties is {} properties)
        {
            foreach (var nested in properties.Values.SelectMany(EnumerateTypeReferencesUsedIn))
            {
                yield return nested;
            }
        }

        if (schemaNode.Items?.SchemaNode is {} itemsType)
        {
            foreach (var nested in EnumerateTypeReferencesUsedIn(itemsType))
            {
                yield return nested;
            }
        }

        if (schemaNode.PrefixItems is {} prefixItemTypes)
        {
            foreach (var nested in prefixItemTypes.SelectMany(EnumerateTypeReferencesUsedIn))
            {
                yield return nested;
            }
        }
    }
}
