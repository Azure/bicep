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
    public class LocalExtensionDispatcherBuilder
    {
        private ILocalExtension? genericResourceHandler;
        private readonly Dictionary<string, ILocalExtensionHandler> resourceHandlers = new(StringComparer.OrdinalIgnoreCase);

        public LocalExtensionDispatcherBuilder AddHandler(ILocalExtensionHandler handler)
        {
            if (!this.resourceHandlers.TryAdd(handler.ResourceType, handler))
            {
                throw new ArgumentException($"Resource type '{handler.ResourceType}' has already been registered.");
            }

            this.resourceHandlers[handler.ResourceType] = handler;
            return this;
        }

        public LocalExtensionDispatcherBuilder AddGenericHandler(ILocalExtension handler)
        {
            if (this.genericResourceHandler is not null)
            {
                throw new ArgumentException($"Generic resource handler has already been registered.");
            }

            this.genericResourceHandler = handler;
            return this;
        }

        public LocalExtensionDispatcher Build()
        {
            return new(
                this.genericResourceHandler,
                this.resourceHandlers.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase));
        }
    }
}
