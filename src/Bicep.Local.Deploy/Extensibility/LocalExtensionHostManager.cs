// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.TypeSystem.Types;
using Google.Protobuf;

namespace Bicep.Local.Deploy.Extensibility
{
    public record BinaryExtensionReference(NamespaceType NamespaceType, Uri BinaryExtensionUri);

    public class LocalExtensionHostManager : IAsyncDisposable, ILocalExtensionHost
    {
        private readonly ILocalExtensionFactoryManager localExtensionFactoryManager;
        private readonly ILocalExtensionHost localExtensionHost;
        private bool LocalExtensionsInitialized;

        public LocalExtensionHostManager(ILocalExtensionFactoryManager localExtensionFactoryManager, ILocalExtensionHost localExtensionHost)
        {
            this.localExtensionFactoryManager = localExtensionFactoryManager;
            this.localExtensionHost = localExtensionHost;
        }

        public void InitializeLocalExtensions(IReadOnlyList<BinaryExtensionReference> binaryExtensions)
        {
            if (LocalExtensionsInitialized == false)
            {
                localExtensionFactoryManager.InitializeLocalExtensions(binaryExtensions);
                LocalExtensionsInitialized = true;
            }
        }

        public void InitializeLocalExtensionsLifecycleManagement() { }

        public Task<HttpResponseMessage> CallExtensibilityHost(LocalDeploymentEngineHost.ExtensionInfo extensionInfo, HttpContent content, CancellationToken cancellationToken)
        {
            EnsureLocalExtensionsInitialized();
            return localExtensionHost.CallExtensibilityHost(extensionInfo, content, cancellationToken);
        }

        private void EnsureLocalExtensionsInitialized()
        {
            if (LocalExtensionsInitialized == false)
            {
                throw new ArgumentNullException($"Local extensions not initialized. Make sure to invoke: '{nameof(InitializeLocalExtensions)}' first.");
            }
        }

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
