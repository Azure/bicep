// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Azure.Deployments.Core.Definitions.Schema;

namespace Bicep.Core.Emit.CompileTimeImports;

internal static class ArmTemplateHelpers
{
    private const string ArmTypeRefPrefix = "#/definitions/";

    internal static ITemplateSchemaNode DereferenceArmType(SchemaValidationContext context, string typePointer)
    {
        // TODO make LocalSchemaRefResolver in Azure.Deployments.Templates public
        if (!typePointer.StartsWith(ArmTypeRefPrefix) ||
            typePointer[ArmTypeRefPrefix.Length..].Contains('/') ||
            !context.Definitions.TryGetValue(typePointer[ArmTypeRefPrefix.Length..], out var typeDefinition))
        {
            throw new InvalidOperationException($"Invalid ARM template type reference ({typePointer}) encountered");
        }

        return typeDefinition;
    }
}
