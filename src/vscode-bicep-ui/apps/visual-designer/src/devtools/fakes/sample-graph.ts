// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * The dev playground's stand-in for the document a language server would compile.
 *
 * The real protocol never carries a whole graph — the server sends patches and the webview submits
 * what it has rendered. The fake needs a whole-graph model anyway, because that is what the toolbar
 * lets you switch between and mutate, and `diffGraph` turns the difference into the patches the
 * protocol does carry.
 *
 * Only the fields that survive the trip are modelled: anything else would be written here and
 * dropped at the boundary.
 */
export interface SampleGraph {
  nodes: SampleGraphNode[];
  edges: SampleGraphEdge[];
  errorCount: number;
}

export interface SampleGraphNode {
  id: string;
  type: string;
  isCollection: boolean;
  hasChildren: boolean;
  hasError: boolean;
}

export interface SampleGraphEdge {
  sourceId: string;
  targetId: string;
}
