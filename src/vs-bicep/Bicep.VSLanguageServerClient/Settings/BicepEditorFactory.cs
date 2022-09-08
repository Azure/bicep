// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Bicep.VSLanguageServerClient.Settings
{
    [Guid(BicepGuids.EditorFactoryGuidString)]
    public class BicepEditorFactory : IVsEditorFactory
    {
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
            ppunkDocView = IntPtr.Zero;
            ppunkDocData = IntPtr.Zero;
            pguidCmdUI = Guid.NewGuid();
            pgrfCDW = 0;
            pbstrEditorCaption = string.Empty;

            return VSConstants.S_OK;
        }

        public int MapLogicalView(ref Guid rguidLogicalView, out string? pbstrPhysicalView)
        {
            pbstrPhysicalView = null;

            // If rguidLogicalView is primary and a text view, return success
            if ((VSConstants.LOGVIEWID_Primary == rguidLogicalView) ||
                (VSConstants.LOGVIEWID_TextView == rguidLogicalView))
            {
                return VSConstants.S_OK;
            }
            // Return E_NOTIMPL for any unrecognized rguidLogicalView values
            else
            {
                return VSConstants.E_NOTIMPL;
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
    }
}
