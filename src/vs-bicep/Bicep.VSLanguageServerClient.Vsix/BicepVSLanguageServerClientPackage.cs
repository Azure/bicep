// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using Bicep.VSLanguageServerClient.Settings;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Bicep.VSLanguageServerClient.Vsix
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// To get loaded into VS, the package must be referenced by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(BicepGuids.PackageGuidString)]
    [ProvideLanguageExtension(typeof(BicepLanguageService), BicepLanguageServerClientConstants.BicepFileExtension)]
    [ProvideLanguageService(
        typeof(BicepLanguageService),
        BicepLanguageServerClientConstants.LanguageName,
        100,
        ShowCompletion = false,
        ShowDropDownOptions = false,
        EnableAdvancedMembersOption = false,
        DefaultToInsertSpaces = true,
        EnableLineNumbers = true,
        RequestStockColors = false)]
    [ProvideBindingPath()]
    public sealed class BicepVSLanguageServerClientPackage : AsyncPackage
    {
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that relies on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // Register bicep language service
            var bicepLanguageService = new BicepLanguageService();
            ((IServiceContainer)this).AddService(typeof(BicepLanguageService), bicepLanguageService, true);
        }

        #endregion
    }
}
