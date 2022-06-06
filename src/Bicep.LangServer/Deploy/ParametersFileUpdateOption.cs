// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.LanguageServer.Deploy
{
    public enum ParametersFileUpdateOption
    {
        // If the user did not provide a parameters file to be used during deployment and chose to create one at the end of deployment flow
        Create = 1,
        // User select "No" to create/update parameters file at the end of deployment flow
        None = 2,
        // If the user did not provide a parameters file to be used in deployment, but chose to create one at the end of the flow and
        // file with name <bicep_file_name>.parameters.json already exists in the same folder as bicep file
        Overwrite = 3,
        // If the user provided a file to be used during deployment and chose to update it with valuses from current deployment at the
        // end of deployment flow
        Update = 4,
    }
}
