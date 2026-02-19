// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { styled } from "styled-components";
import { FakeMessageChannel, SAMPLE_GRAPHS, GRAPH_MUTATIONS } from "../fake-message-channel";

interface DevToolbarProps {
  channel: FakeMessageChannel;
}

const $Toolbar = styled.div`
  flex: 0 0 auto;
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 6px 8px;
  padding: 8px 12px;
  background: rgba(30, 30, 30, 0.92);
  border-bottom: 1px solid rgba(255, 255, 255, 0.12);
  font-family: system-ui, sans-serif;
  font-size: 12px;
  color: #ccc;
`;

const $Label = styled.span`
  font-weight: 600;
  color: #e0a030;
  margin-right: 4px;
  user-select: none;
`;

const $Separator = styled.div`
  width: 1px;
  height: 20px;
  background: rgba(255, 255, 255, 0.2);
  margin: 0 4px;
`;

const $SectionLabel = styled.span`
  font-size: 11px;
  color: #999;
  user-select: none;
`;

const $Button = styled.button`
  padding: 4px 10px;
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 4px;
  background: rgba(255, 255, 255, 0.08);
  color: #ddd;
  font-size: 12px;
  cursor: pointer;
  white-space: nowrap;

  &:hover {
    background: rgba(255, 255, 255, 0.16);
    border-color: rgba(255, 255, 255, 0.35);
  }

  &:active {
    background: rgba(255, 255, 255, 0.24);
  }
`;

/**
 * A floating toolbar rendered only in dev mode (`npm run dev`).
 * Each button pushes a different sample graph through the
 * {@link FakeMessageChannel}, simulating the extension host
 * sending `deploymentGraph` notifications.
 */
export function DevToolbar({ channel }: DevToolbarProps) {
  const applyMutation = (apply: (graph: import("../../../messages").DeploymentGraph) => import("../../../messages").DeploymentGraph) => {
    const current = channel.getCurrentGraph();
    if (!current) return;
    channel.pushGraph(apply(current));
  };

  return (
    <$Toolbar>
      <$Label>DEV</$Label>
      <$SectionLabel>Graphs</$SectionLabel>
      {Object.entries(SAMPLE_GRAPHS).map(([name, graph]) => (
        <$Button key={name} onClick={() => channel.pushGraph(graph)}>
          {name}
        </$Button>
      ))}
      <$Separator />
      <$SectionLabel>Mutations</$SectionLabel>
      {GRAPH_MUTATIONS.map((mutation) => (
        <$Button key={mutation.label} title={mutation.description} onClick={() => applyMutation(mutation.apply)}>
          {mutation.label}
        </$Button>
      ))}
    </$Toolbar>
  );
}
