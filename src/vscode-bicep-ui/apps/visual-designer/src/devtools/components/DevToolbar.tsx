// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ChangeEvent } from "react";
import type { TargetScope } from "@/features/canvas";

import { useCallback } from "react";
import { styled } from "styled-components";
import { FakeMessageChannel, GRAPH_MUTATIONS, SAMPLE_GRAPHS } from "../fakes/fake-message-channel";

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
 * {@link FakeMessageChannel}, simulating extension graph-change notifications.
 */
export function DevToolbar({ channel }: DevToolbarProps) {
  const changeScope = useCallback(
    (event: ChangeEvent<HTMLSelectElement>) => channel.setTargetScope(event.target.value as TargetScope),
    [channel],
  );
  const changeCatalog = useCallback(() => channel.changeCatalog(), [channel]);
  const applyMutation = (
    apply: (graph: import("../fakes/sample-graph").SampleGraph) => import("../fakes/sample-graph").SampleGraph,
  ) => {
    const current = channel.getCurrentGraph();
    if (!current) return;
    channel.pushGraph(apply(current));
  };

  return (
    <$Toolbar data-testid="dev-toolbar">
      <$Label>DEV</$Label>
      <select
        aria-label="Document target scope"
        title="Document target scope"
        defaultValue={new URLSearchParams(window.location.search).get("targetScope") ?? "resourceGroup"}
        onChange={changeScope}
      >
        <option value="resourceGroup">Resource group</option>
        <option value="subscription">Subscription</option>
        <option value="managementGroup">Management group</option>
        <option value="tenant">Tenant</option>
      </select>
      <$Button title="Replace the resource type catalog" onClick={changeCatalog}>
        Change catalog
      </$Button>
      <$SectionLabel>Graphs</$SectionLabel>
      {Object.entries(SAMPLE_GRAPHS).map(([name, graph]) => (
        <$Button key={name} onClick={() => channel.pushGraph(graph)} data-testid={`dev-graph-${slugify(name)}`}>
          {name}
        </$Button>
      ))}
      <$Separator />
      <$SectionLabel>Mutations</$SectionLabel>
      {GRAPH_MUTATIONS.map((mutation) => (
        <$Button
          key={mutation.label}
          title={mutation.description}
          onClick={() => applyMutation(mutation.apply)}
          data-testid={`dev-mutation-${slugify(mutation.label)}`}
        >
          {mutation.label}
        </$Button>
      ))}
    </$Toolbar>
  );
}

function slugify(value: string): string {
  return value
    .normalize("NFKD")
    .replace(/[^a-zA-Z0-9]+/g, "-")
    .replace(/^-+|-+$/g, "")
    .toLowerCase();
}
