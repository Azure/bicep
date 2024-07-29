// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Local.Extension.Protocol
{
    public abstract class LocalExtension : ILocalExtension
    {
        private bool initialized;
        private ILocalExtension? InitializedExtension;

        public abstract Task<ILocalExtension> InitializeAsync();

        public async Task<ILocalExtension> EnsureInitializedAsync()
        {
            if (initialized == false || InitializedExtension is null)
            {
                InitializedExtension = await InitializeAsync();
                initialized = true;
            }
            return InitializedExtension;
        }

        public abstract Task<LocalExtensionOperationResponse> CreateOrUpdate(Azure.Deployments.Extensibility.Core.V2.Models.ResourceSpecification request, CancellationToken cancellationToken);

        public abstract Task<LocalExtensionOperationResponse> Delete(Azure.Deployments.Extensibility.Core.V2.Models.ResourceReference request, CancellationToken cancellationToken);

        public abstract Task<LocalExtensionOperationResponse> Get(Azure.Deployments.Extensibility.Core.V2.Models.ResourceReference request, CancellationToken cancellationToken);

        public abstract Task<LocalExtensionOperationResponse> Preview(Azure.Deployments.Extensibility.Core.V2.Models.ResourceSpecification request, CancellationToken cancellationToken);
    }

    public interface ILocalExtension
    {
        Task<LocalExtensionOperationResponse> CreateOrUpdate(
            Azure.Deployments.Extensibility.Core.V2.Models.ResourceSpecification request,
            CancellationToken cancellationToken);

        Task<LocalExtensionOperationResponse> Delete(
            Azure.Deployments.Extensibility.Core.V2.Models.ResourceReference request,
            CancellationToken cancellationToken);

        Task<LocalExtensionOperationResponse> Get(
            Azure.Deployments.Extensibility.Core.V2.Models.ResourceReference request,
            CancellationToken cancellationToken);

        Task<LocalExtensionOperationResponse> Preview(
            Azure.Deployments.Extensibility.Core.V2.Models.ResourceSpecification request,
            CancellationToken cancellationToken);
    }
}
