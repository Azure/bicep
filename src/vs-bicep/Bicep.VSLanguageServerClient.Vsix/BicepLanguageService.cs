// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Bicep.VSLanguageServerClient.Vsix
{
    [Guid("77291DD5-511B-45C8-BD20-62618C39D692")]
    public class BicepLanguageService : IVsLanguageInfo
    {
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public int GetCodeWindowManager(IVsCodeWindow pCodeWin, out IVsCodeWindowManager? ppCodeWinMgr)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        {
            ppCodeWinMgr = null;
            return VSConstants.E_NOTIMPL;
        }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public int GetColorizer(IVsTextLines pBuffer, out IVsColorizer? ppColorizer)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        {
            ppColorizer = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetFileExtensions(out string pbstrExtensions)
        {
            pbstrExtensions = ".bicep";
            return VSConstants.S_OK;
        }

        public int GetLanguageName(out string bstrName)
        {
            bstrName = "Bicep";
            return VSConstants.S_OK;
        }
    }
}
