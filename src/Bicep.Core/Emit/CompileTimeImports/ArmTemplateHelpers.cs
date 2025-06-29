// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Templates.Engines;

namespace Bicep.Core.Emit.CompileTimeImports;

internal static class ArmTemplateHelpers
{
    internal static ITemplateSchemaNode DereferenceArmType(SchemaValidationContext context, string typePointer)
    {
        if (LocalSchemaRefResolver.ResolveLocalReference(context, typePointer, out var node, out var failureMessage))
        {
            return node;
        }

        throw new InvalidOperationException($"Invalid ARM template type reference ({typePointer}) encountered: {failureMessage}");
    }
}
