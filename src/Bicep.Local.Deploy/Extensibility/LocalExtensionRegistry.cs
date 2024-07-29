// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Local.Deploy.Extensibility
{
    public class LocalExtensionRegistry
    {
        public LocalExtensionRegistry(ILocalExtensionFactory factory)
        {
        }

        public void InitializeLocalExtensions()
        {
            RetrieveLocalExtensions();
            RegisterLocalExtensions();
            LoadLocalExtensions();
        }

        private void RetrieveLocalExtensions()
        {

        }

        private void RegisterLocalExtensions()
        {

        }

        private void LoadLocalExtensions()
        {

        }
    }
}
