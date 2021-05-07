// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
declare module "cytoscape-elk" {
  const cytoscape = import("cytoscape");

  export default function register(
    cytoscape: (options?: cytoscape.CytoscapeOptions) => cytoscape.Core
  ): void;
}
