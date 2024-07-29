// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Local.Extension.Protocol
{
    public class LocalExtensionDispatcher
    {
        private readonly ILocalExtension? genericResourceHandler;
        private readonly ImmutableDictionary<string, ILocalExtensionHandler> resourceHandlers;

        public LocalExtensionDispatcher(
            ILocalExtension? genericResourceHandler,
            ImmutableDictionary<string, ILocalExtensionHandler> resourceHandlers)
        {
            this.genericResourceHandler = genericResourceHandler;
            this.resourceHandlers = resourceHandlers;
        }

        public ILocalExtension GetHandler(string resourceType)
        {
            if (this.resourceHandlers.TryGetValue(resourceType, out var handler))
            {
                return handler;
            }

            if (this.genericResourceHandler is { })
            {
                return this.genericResourceHandler;
            }

            throw new ArgumentException($"Resource type '{resourceType}' is not supported.");
        }
    }
}
