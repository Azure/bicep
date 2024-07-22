// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
declare module "cytoscape-elk" {
  export default function register(cytoscape: (options?: cytoscape.CytoscapeOptions) => cytoscape.Core): void;
}
