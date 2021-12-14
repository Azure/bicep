// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Configuration
{
    public interface IBicepConfigChangeHandler
    {
        void RefreshCompilationOfSourceFilesInWorkspace();

        void HandleBicepConfigOpenEvent(DocumentUri documentUri);

        void HandleBicepConfigChangeEvent(DocumentUri documentUri);

        void HandleBicepConfigSaveEvent(DocumentUri documentUri);

        void HandleBicepConfigCloseEvent(DocumentUri documentUri);
    }
}
