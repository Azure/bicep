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

    private readonly RegistryMetadataDetails? details; //asdfg extract into a class
    private readonly ImmutableArray<RegistryModuleVersionMetadata>? versions;

    //asdfg CachableModuleMetadata metadata,
    private AsyncLazy<RegistryMetadataDetails>? lazyDetails;
    // asdfg private Func<Task<RegistryMetadataDetails>> getDetailsFunc;
    //private Func<Task<ImmutableArray<RegistryModuleVersionMetadata>>>? getVersionsFunc;
    private AsyncLazy<ImmutableArray<RegistryModuleVersionMetadata>>? lazyVersions;

    public DefaultRegistryModuleMetadata(
        string registry,
        string moduleName,
        RegistryMetadataDetails details,
        ImmutableArray<RegistryModuleVersionMetadata> versions)
    {
        //asdfg this.Metadata = metadata;
        this.Registry = registry;
        this.ModuleName = moduleName;

        this.details = details;
        this.versions = versions;
        this.lazyDetails = null;
        this.lazyVersions = null;
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

        this.lazyDetails = new AsyncLazy<RegistryMetadataDetails>(getDetailsAsyncFunc, new(new JoinableTaskContext()));
        //asdfg this.getDetailsFunc = getDetailsFunc;
        //this.getVersionsFunc = getVersionsFunc;
        this.lazyVersions = new AsyncLazy<ImmutableArray<RegistryModuleVersionMetadata>>(getVersionsAsyncFunc, new(new JoinableTaskContext()));

        this.details = null;
        this.versions = null;
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
        return details.HasValue ? details.Value : await lazyDetails!/*asdfg*/.GetValueAsync();//asdfg try/catch?
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
        return versions.HasValue ? versions.Value : await lazyVersions!/*asdfg*/.GetValueAsync();//asdfg try/catch?
    }

    public ImmutableArray<RegistryModuleVersionMetadata> GetCachedVersions()
    {
        if (versions is not null)
        {
            return versions.Value;
        }
        else if (lazyVersions!/*asdfg*/.IsValueFactoryCompleted)
        {
            return lazyVersions.GetValue(); //asdfg can this throw?
        }
        else
        {
            return [];
        }
    }
}
