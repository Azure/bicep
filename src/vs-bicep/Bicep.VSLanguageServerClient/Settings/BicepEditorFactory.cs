// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Bicep.VSLanguageServerClient.Settings
{
    [Guid(BicepGuids.EditorFactoryGuidString)]
    public class BicepEditorFactory : IVsEditorFactory
    {
        #region IVsEditorFactory Members

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public int CreateEditorInstance(uint grfCreateDoc,
                                        string pszMkDocument,
                                        string pszPhysicalView,
                                        IVsHierarchy pvHier,
                                        uint itemid,
                                        IntPtr punkDocDataExisting,
                                        out IntPtr ppunkDocView,
                                        out IntPtr ppunkDocData,
                                        out string pbstrEditorCaption,
                                        out Guid pguidCmdUI,
                                        out int pgrfCDW)
        {
            // Initialize to null
            ppunkDocView = IntPtr.Zero;
            ppunkDocData = IntPtr.Zero;
            pguidCmdUI = Guid.NewGuid();// Editor.DefGuidList.CLSID_TextEditorFactory;
            pgrfCDW = 0;
            pbstrEditorCaption = string.Empty;

            return VSConstants.S_OK;
        }

        public int MapLogicalView(ref Guid rguidLogicalView, out string? pbstrPhysicalView)
        {
            pbstrPhysicalView = null;    // initialize out parameter

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

        public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}
