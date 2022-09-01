// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.VSLanguageServerClient.ProcessTracker
{
    public class WindowEventArgs : EventArgs
    {
        public WindowEventArgs(WindowsSystemEvents anEvent, IntPtr windowHandle)
        {
            SystemEvent = anEvent;
            WindowHandle = windowHandle;
        }

        public WindowsSystemEvents SystemEvent { get; }
        public IntPtr WindowHandle { get; }
    }
}
