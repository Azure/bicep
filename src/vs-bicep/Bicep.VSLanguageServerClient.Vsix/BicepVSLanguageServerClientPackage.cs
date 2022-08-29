// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Bicep.VSLanguageServerClient.Settings;
using Bicep.VSLanguageServerClient.Vsix.Settings;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

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
    [Guid(BicepGuidList.PackageGuidString)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideLanguageExtension(typeof(BicepLanguageService), BicepLanguageServerClientConstants.BicepFileExtension)]
    [ProvideLanguageService(typeof(BicepLanguageService), BicepLanguageServerClientConstants.LanguageName, 100,
        ShowCompletion = false,
        ShowDropDownOptions = false,
        EnableAdvancedMembersOption = false,
        DefaultToInsertSpaces = true,
        EnableLineNumbers = true,
        RequestStockColors = false)]
    [ProvideEditorExtension(typeof(BicepEditorFactory), BicepLanguageServerClientConstants.BicepFileExtension, 50)]
    [ProvideEditorLogicalView(typeof(BicepEditorFactory), VSConstants.LOGVIEWID.TextView_string)]
    [ProvideLanguageExtension(typeof(BicepLanguageService), BicepLanguageServerClientConstants.BicepFileExtension)]
    [ProvideEditorExtension(typeof(BicepEditorFactory), ".bicep", 50)]
    [ProvideBindingPath()]
    public class BicepVSLanguageServerClientPackage : Package
    {
        #region Package Members
        protected override void Initialize()
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            base.Initialize();

            // register bicep language service
            var bicepLanguageService = new BicepLanguageService();
            ((IServiceContainer)this).AddService(typeof(BicepLanguageService), bicepLanguageService, true);

            // register bicep editor factory
            IServiceProvider serviceProvider = this;

            ThreadHelper.ThrowIfNotOnUIThread();
            var sVsUIShellOpenDocument = serviceProvider.GetService(typeof(SVsUIShellOpenDocument));

            if (sVsUIShellOpenDocument is not null){
                var shellOpenDocument = sVsUIShellOpenDocument as IVsUIShellOpenDocument;

                if (shellOpenDocument is not null)
                {
                    var editorGuid = new Guid(BicepGuidList.EditorFactoryGuidString);
                    var guidNull = Guid.Empty;
                    shellOpenDocument.GetStandardEditorFactory(0, ref editorGuid, null, ref guidNull, out var physView, out IVsEditorFactory defaultEditorFactory);

                    RegisterEditorFactory(new BicepEditorFactory(defaultEditorFactory));

                    LoadBicepSettingsStorage();
                }
            }
        }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that relies on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        //protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        //{
        //    // When initialized asynchronously, the current thread may be a background thread at this point.
        //    // Do any initialization that requires the UI thread after switching to the UI thread.
        //    await base.InitializeAsync(cancellationToken, progress).ConfigureAwait(false);

        //    await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        //    IServiceContainer container = this;
        //    container.AddService(typeof(BicepLanguageService), new BicepLanguageService(), true);

        //    var bicepEditorFactory = new BicepEditorFactory();
        //    RegisterEditorFactory(bicepEditorFactory);

        //    LoadBicepSettingsStorage();
        //}

        private void LoadBicepSettingsStorage()
        {
            var storage = new BicepSettingsStorage(new Guid(BicepGuidList.LanguageServiceGuidString));
            storage.LoadFromStorage();
        }

        #endregion
    }
}
