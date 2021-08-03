// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem.Az
{
    public static class AzFunctionProvider
    {
        public static FunctionOverload GetResourceOverload(IResourceTypeProvider typeProvider)
        {
            return new FunctionOverloadBuilder("resource")
                .WithDynamicReturnType((binder, fileResolver, diagnostics, arguments, argumentTypes) => {
                    var argsArray = arguments.ToArray();

                    if (argsArray.Length < 1)
                    {
                        // should have already been validated
                        return ErrorType.Empty();
                    }

                    if (argsArray[0].Expression is not StringSyntax typeSyntax ||
                        typeSyntax.TryGetLiteralValue() is not {} typeString ||
                        ResourceTypeReference.TryParse(typeString) is not {} typeReference)
                    {
                        return ErrorType.Create(DiagnosticBuilder.ForPosition(argsArray[0]).InvalidResourceType());
                    }

                    if (typeReference.Types.Length != argsArray.Length - 1)
                    {
                        // TODO add a dedicated diagnostic for this (mismatch in types & names)
                        return ErrorType.Create(DiagnosticBuilder.ForPosition(argsArray[0]).TopLevelChildResourceNameIncorrectQualifierCount(typeReference.Types.Length - 1));
                    }

                    // TODO type checking for constant name

                    return typeProvider.GetType(typeReference, ResourceTypeGenerationFlags.PermitLiteralNameProperty | ResourceTypeGenerationFlags.ExistingResource);
                }, LanguageConstants.CreateResourceScopeReference(ResourceScope.Resource))
                // TODO fix up the descriptions and param names
                .WithDescription("Obtains a reference to a resource.")
                .WithRequiredParameter("type", LanguageConstants.String, "The type of the resource")
                .WithVariableParameter("resourceName", LanguageConstants.String, minimumCount: 1, "The resource name segment")
                .WithFlags(FunctionFlags.RequiresInlining)
                .Build();
        }
    }
}