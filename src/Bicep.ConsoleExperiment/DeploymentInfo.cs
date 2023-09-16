// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.ConsoleExperiment
{
    public class DeploymentInfo
    {
        public string Name { get; set; }
        public string ResourceGroup { get; set; }
        public string PortalLink { get; set; }
        public string CorrelationID { get; set; }
        public DeploymentInfo() {
            Name = string.Empty;
            ResourceGroup = string.Empty;
            PortalLink = string.Empty;
            CorrelationID = string.Empty;
        }
        public DeploymentInfo(string name, string resourceGroup, string portalLink, string correlationId) {
            Name = name;
            ResourceGroup = resourceGroup;
            PortalLink = portalLink;
            CorrelationID = correlationId;
        }

    }
}
