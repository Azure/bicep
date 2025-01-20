// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.Registry.Indexing; //asdfg split into PublicRegistry/PrivateRegistry

public class DefaultRegistryModuleMetadata : IRegistryModuleMetadata
{
    public string Registry { get; init; }
    public string ModuleName { get; init; }

    private readonly MightBeLazyAsync<RegistryMetadataDetails> details;
    private readonly MightBeLazyAsync<ImmutableArray<RegistryModuleVersionMetadata>> versions;

    //asdfg CachableModuleMetadata metadata,
    //private AsyncLazy<RegistryMetadataDetails>? lazyDetails;
    // asdfg private Func<Task<RegistryMetadataDetails>> getDetailsFunc;
    //private Func<Task<ImmutableArray<RegistryModuleVersionMetadata>>>? getVersionsFunc;
    //private AsyncLazy<ImmutableArray<RegistryModuleVersionMetadata>>? lazyVersions;

    public DefaultRegistryModuleMetadata(
        string registry,
        string moduleName,
        RegistryMetadataDetails details,
        ImmutableArray<RegistryModuleVersionMetadata> versions)
    {
        //asdfg this.Metadata = metadata;
        this.Registry = registry;
        this.ModuleName = moduleName;

        this.details = new MightBeLazyAsync<RegistryMetadataDetails>(details);
        this.versions = new(versions);
    }

    public DefaultRegistryModuleMetadata(
            string registry,
            string moduleName,
            Func<Task<RegistryMetadataDetails>> getDetailsAsyncFunc,
            Func<Task<ImmutableArray<RegistryModuleVersionMetadata>>> getVersionsAsyncFunc)
    {
        //asdfg this.Metadata = metadata;
        this.Registry = registry;
        this.ModuleName = moduleName;

        this.details = new(getDetailsAsyncFunc);
        //asdfg this.getDetailsFunc = getDetailsFunc;
        //this.getVersionsFunc = getVersionsFunc;
        this.versions = new(getVersionsAsyncFunc);
    }

    ////asdfg this.Metadata = metadata;
    //this.Registry = registry;
    //this.ModuleName = moduleName;
    //this.GetModuleMetadataFunc = getDetailsFunc;
    //this.GetVersionsTask = versionsMetadata is null ? null :
    //    Task.FromResult(versionsMetadata.Value.Select(v =>
    //        new CachableVersionMetadata(v.Version, v)).ToImmutableArray());

    public async Task<RegistryMetadataDetails> TryGetDetailsAsync()
    {
        try
        {
            return await details.GetValueAsync();
        }
        catch
        {
            return new RegistryMetadataDetails(null, null);
        }
    }

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

    public async Task<ImmutableArray<RegistryModuleVersionMetadata>> TryGetVersionsAsync()
    {
        try //asdfg
        {
            return await versions.GetValueAsync();
        }
        catch
        {
            return [];
        }
    }

    public ImmutableArray<RegistryModuleVersionMetadata> GetCachedVersions()
    {
        return versions.TryGetValue(); //asdfg try/catch??
    }
}
