// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.LanguageServer.Configuration
{
    public interface IBicepConfigChangeHandler
    {
        void RefreshCompilationOfSourceFilesInWorkspace();
    }
}
