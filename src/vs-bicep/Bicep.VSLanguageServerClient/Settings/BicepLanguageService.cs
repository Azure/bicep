// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Bicep.VSLanguageServerClient.Settings
{
    [Guid(BicepGuids.LanguageServiceGuidString)]
    public class BicepLanguageService : IVsLanguageInfo
    {
        public int GetLanguageName(out string bstrName)
        {
            bstrName = BicepLanguageServerClientConstants.LanguageName;
            return VSConstants.S_OK;
        }

        public int GetFileExtensions(out string pbstrExtensions)
        {
            pbstrExtensions = ".bicep";
            return VSConstants.S_OK;
        }

        public int GetColorizer(IVsTextLines pBuffer, out IVsColorizer? ppColorizer)
        {
            ppColorizer = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetCodeWindowManager(IVsCodeWindow pCodeWin, out IVsCodeWindowManager? ppCodeWinMgr)
        {
            ppCodeWinMgr = null;
            return VSConstants.E_NOTIMPL;
        }
    }
}
