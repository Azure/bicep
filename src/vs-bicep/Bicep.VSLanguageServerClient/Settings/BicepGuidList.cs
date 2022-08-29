// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.VSLanguageServerClient.Settings
{
    public class BicepGuidList
    {
        public const string LanguageServiceGuidString = "17c12a04-877c-4781-8948-99610a774160";

        public const string EditorFactoryGuidString = "59d159a7-827e-4a7e-9803-6c368485d067";
        public static readonly Guid EditorFactoryGuid = new Guid(EditorFactoryGuidString);

        public const string PackageGuidString = "7bbebedb-0520-44f3-a5e0-e3d8855f1944";
    }
}
