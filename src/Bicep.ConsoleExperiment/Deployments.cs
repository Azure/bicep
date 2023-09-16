// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.ConsoleExperiment
{
    public class Deployments
    {
        public string Name { get; set; }
        public string ResourceType { get; set; }
        public string Status { get; set; }
        public Deployments()
        {
            Name = string.Empty;
            ResourceType = string.Empty;
            Status = string.Empty;
        }
        public Deployments(string name, string resourceType, string status)
        {
            Name = name;
            ResourceType = resourceType;
            Status = status;
        }
    }
}
