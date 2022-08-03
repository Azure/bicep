// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Flavor;

namespace Bicep.VSLanguageServerClient.Vsix
{
    [Guid(BicepProjectFactory.BicepProjectFactoryGuid)]
    public class BicepProjectFactory : FlavoredProjectFactoryBase
    {
        public const string BicepProjectFactoryGuid = "229B3E77-97E9-4f6d-9151-E6D103EA4D4A";

        private IServiceProvider site;
        public BicepProjectFactory(IServiceProvider site) : base()
        {
            this.site = site;
        }

        protected override object PreCreateForOuter(IntPtr outerProjectIUnknown)
        {
            return new BicepProject(site);
        }
    }
}
