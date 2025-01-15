// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Microsoft.VisualStudio.Threading;

namespace Bicep.Core.Registry.Indexing; //asdfg split into PublicRegistry/PrivateRegistry
//asdfg

public class DefaultRegistryModuleMetadata : IRegistryModuleMetadata
{
    public string Registry { get; init; }
    public string ModuleName { get; init; }

    //asdfg CachableModuleMetadata metadata,
    private AsyncLazy<RegistryMetadataDetails> lazyDetails;
    // asdfg private Func<Task<RegistryMetadataDetails>> getDetailsFunc;
    //private Func<Task<ImmutableArray<RegistryModuleVersionMetadata>>> getVersionsFunc;
    private AsyncLazy<ImmutableArray<RegistryModuleVersionMetadata>> lazyVersions;

    public DefaultRegistryModuleMetadata(
        string registry,
        string moduleName,
        Func<Task<RegistryMetadataDetails>> getDetailsFunc,
        Func<Task<ImmutableArray<RegistryModuleVersionMetadata>>> getVersionsFunc)
    {
        //asdfg this.Metadata = metadata;
        this.Registry = registry;
        this.ModuleName = moduleName;
        this.lazyDetails = new AsyncLazy<RegistryMetadataDetails>(getDetailsFunc, new(new JoinableTaskContext()));
        //asdfg this.getDetailsFunc = getDetailsFunc;
        //this.getVersionsFunc = getVersionsFunc;
        this.lazyVersions = new AsyncLazy<ImmutableArray<RegistryModuleVersionMetadata>>(getVersionsFunc, new(new JoinableTaskContext()));
    }

    ////asdfg this.Metadata = metadata;
    //this.Registry = registry;
    //this.ModuleName = moduleName;
    //this.GetModuleMetadataFunc = getDetailsFunc;
    //this.GetVersionsTask = versionsMetadata is null ? null :
    //    Task.FromResult(versionsMetadata.Value.Select(v =>
    //        new CachableVersionMetadata(v.Version, v)).ToImmutableArray());

    public async Task<RegistryMetadataDetails> TryGetDetailsAsync() => await lazyDetails.GetValueAsync();// getDetailsFunc(); //asdfg try/catch?

    //    public Task<RegistryMetadataDetails> TryGetDetails() //asdfg GetDetailsOrEmpty?
    //    {
    //        var detailsTask = getDetailsFunc();
    //        if (detailsTask.IsCompletedSuccessfully)
    //        {
    //#pragma warning disable VSTHRD103 // Call async methods when in an async method
    //            return Task.FromResult(detailsTask.Result);
    //#pragma warning restore VSTHRD103 // Call async methods when in an async method
    //        }
    //        else
    //        {
    //            return Task.FromResult(new RegistryMetadataDetails(null, null));
    //        }
    //    }

    public async Task<ImmutableArray<RegistryModuleVersionMetadata>> TryGetVersionsAsync() => await lazyVersions.GetValueAsync();// getVersionsFunc();//asdfg try/catch?

    public ImmutableArray<RegistryModuleVersionMetadata> GetCachedVersions()
    {
        if (lazyVersions.IsValueFactoryCompleted)
        {
            return lazyVersions.GetValue(); //asdfg can this throw?
        }
        else
        {
            return [];
        }
    }
}
