// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;
using Bicep.VSLanguageServerClient.Settings;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IVsServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Bicep.VSLanguageServerClient.Vsix
{
    [Guid(BicepGuidList.EditorFactoryGuidString)]
    public class BicepEditorFactory : IVsEditorFactory
    {
        private readonly IVsEditorFactory defaultEditorFactory;

        public BicepEditorFactory(IVsEditorFactory defaultEditorFactory)
        {
            this.defaultEditorFactory = defaultEditorFactory;
        }

        #region IVsEditorFactory Members

        /// This method is called by shell each time a file is opened. It attempts to create the editor
        /// instance for the particular file. Is only invoked for file extensions that are explicitly
        /// associated with the editor in the associated pkgdef and those that are specified in the
        /// user defined file extension mappings associated with the editor in:
        /// Tools | Options | Text Editor | File Extensions
        /// <summary>
        public virtual int CreateEditorInstance(
            uint vsCreateEditorFlags,
            string fileName,
            string physicalView,
            IVsHierarchy hierarchy,
            uint itemId,
            IntPtr existingDocDataPtr,
            out IntPtr docViewPtr,
            out IntPtr docDataPtr,
            out string caption,
            out Guid cmdUIGuid,
            out int result)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return this.defaultEditorFactory.CreateEditorInstance(vsCreateEditorFlags, fileName, physicalView, hierarchy, itemId, existingDocDataPtr,
                out docViewPtr, out docDataPtr, out caption, out cmdUIGuid, out result);
        }


        public virtual int SetSite(IVsServiceProvider psp)
        {
            return VSConstants.S_OK;
        }

        public int MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
        {
            pbstrPhysicalView = string.Empty;    // initialize out parameter

            // we are both a primary and a text view.
            if ((VSConstants.LOGVIEWID_Primary == rguidLogicalView) ||
                (VSConstants.LOGVIEWID_TextView == rguidLogicalView))
            {
                return VSConstants.S_OK;        // primary view uses NULL as pbstrPhysicalView
            }
            else
            {
                return VSConstants.E_NOTIMPL;   // you must return E_NOTIMPL for any unrecognized rguidLogicalView values
            }
        }

        public int Close()
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}
