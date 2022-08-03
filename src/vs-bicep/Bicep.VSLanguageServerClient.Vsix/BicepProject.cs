// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Flavor;
using Microsoft.VisualStudio.Shell.Interop;

namespace Bicep.VSLanguageServerClient.Vsix
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("FEBF49B8-D18C-4745-ADE0-35BF632E0533")]
    public class BicepProject : FlavoredProjectBase
    {
        public BicepProject(IServiceProvider site)
        {
            this.serviceProvider = site;
        }

        protected override Guid GetGuidProperty(uint itemId, int propId)
        {
            if (propId ==
                (int)__VSHPROPID2.VSHPROPID_AddItemTemplatesGuid)
            {
                return typeof(BicepProjectFactory).GUID;
            }
            return base.GetGuidProperty(itemId, propId);
        }

        protected override int GetProperty(uint itemId, int propId, out object property)
        {
            return base.GetProperty(itemId, propId, out property);
        }
    }
}
