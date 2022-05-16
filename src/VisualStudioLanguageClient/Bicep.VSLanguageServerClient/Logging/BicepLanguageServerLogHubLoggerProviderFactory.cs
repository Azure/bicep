// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel.Composition;

namespace Bicep.VSLanguageServerClient.Logging
{
    [Export(typeof(BicepLanguageServerLogHubLoggerProviderFactory))]
    public class BicepLanguageServerLogHubLoggerProviderFactory : LogHubLoggerProviderFactoryBase
    {
        [ImportingConstructor]
        public BicepLanguageServerLogHubLoggerProviderFactory(BicepLogHubTraceProvider traceProvider)
            : base(traceProvider)
        {
        }
    }
}
