// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * The dev playground's stand-in for the document a language server would compile. The toolbar
 * switches between and mutates these, and `toGraph` turns one into the graph the protocol carries.
 *
 * Only the fields the graph needs are modelled; the rest are derived from the id and type.
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
