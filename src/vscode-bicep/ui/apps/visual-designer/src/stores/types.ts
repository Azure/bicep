import "zustand/middleware/immer";
import type { Box, Position } from "../math/geometry";
import type { Edge, Graph, Node } from "../math/graph-theory/types";
import type { Diagnostic, DocumentUri, Range } from "vscode-languageserver-protocol";
import type { StateCreator } from "zustand";

export interface CommonNodeData {
  symbolicName: string;
  range: Range;
  diagnostics?: Diagnostic[];
}

export interface ResourceNodeData extends CommonNodeData {
  kind: "resource";
  resourceType: string;
}

export interface ModuleNodeData extends CommonNodeData {
  kind: "module";
  modulePath: string;
  documentUri?: DocumentUri;
}

export type NodeData = ResourceNodeData | ModuleNodeData;

export interface NodeState extends Node {
  boundingBox: Box;
  collapsed?: boolean;
  selected?: boolean;
  padding?: {
    left: number;
    top: number;
    right: number;
    bottom: number;
  };
  data: NodeData;
}

export interface ExplicitDependency {
  kind: 'Explicit';
}

export interface ImplicitDependency {
  kind: 'Implicit';
  sourceProperty: string;
  targetExpression: string;
}

export type Dependency = ExplicitDependency | ImplicitDependency;

export interface EdgeState extends Edge {
  data?: {
    dependency: Dependency;
  };
}

export interface GraphState extends Graph<NodeState, EdgeState> {
  position: Position;
  scale: number;
  data: {
    documentUri: DocumentUri;
    scope?: "Tenant" | "ManagementGroup" | "Subscription" | "ResourceGroup";
    diagnostics?: Diagnostic[];
    dirty?: boolean;
  };

  translateTo: (position: Position) => void;
  scaleTo: (scale: number) => void;

  translateNode: (nodeId: string, dx: number, dy: number) => void;
  addNode: (nodeId: string, position: Position) => void;
}

export type NodeVariant = "Compact" | "Informative";

export type EdgeShape = "Straight" | "Bezier";

export interface ConfigState {
  nodeVariant: NodeVariant;
  edgeShape: EdgeShape;

  setNodeVariant: (nodeVariant: NodeVariant) => void;
  setEdgeShape: (edgeShape: EdgeShape) => void;
}

export type AppState = GraphState & ConfigState;

export type ImmerStateCreator<T> = StateCreator<AppState, [["zustand/immer", never], never], [], T>;
