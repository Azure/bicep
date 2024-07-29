// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.TypeSystem.Types;
using Bicep.Local.Extension.Protocol;
using Google.Protobuf;

namespace Bicep.Local.Deploy.Extensibility
{
    public record LocalExtensionKey(string ExtensionName, string ExtensionVersion);

    public class LocalExtensionFactoryManager(ILocalExtensionFactory extensionFactory) : ILocalExtensionFactoryManager
    {
        private readonly Dictionary<LocalExtensionKey, Func<Task<ILocalExtension>>> RegisteredLocalExtensions = [];
        private readonly Dictionary<LocalExtensionKey, ILocalExtension> InitializedLocalExtensions = [];

        public void InitializeLocalExtensions(IReadOnlyList<BinaryExtensionReference> binaryExtensions)
        {
            foreach (var (namespaceType, binaryUri) in binaryExtensions)
            {
                LocalExtensionKey providerKey = new(namespaceType.Settings.ArmTemplateProviderName, namespaceType.Settings.ArmTemplateProviderVersion);
                RegisteredLocalExtensions[providerKey] = () => extensionFactory.CreateLocalExtensionAsync(providerKey, binaryUri);
            }

            RegisterBuiltInExtensions();
        }

        private void RegisterBuiltInExtensions() { }

        public async Task<ILocalExtension> GetLocalExtensionAsync(string providerName, string providerVersion)
        {
            LocalExtensionKey providerKey = new(providerName, providerVersion);
            if (!RegisteredLocalExtensions.TryGetValue(providerKey, out var localExtension))
            {
                throw new ArgumentException($"Provider name: '{providerName}' and version: '{providerVersion}' was not found.");
            }

            // Ensure loading the extension once and on-demand.
            if (!InitializedLocalExtensions.TryGetValue(providerKey, out var initializedLocalExtension))
            {
                initializedLocalExtension = await localExtension();
                InitializedLocalExtensions[providerKey] = initializedLocalExtension;
            }

            return initializedLocalExtension;
        }
    }
}
