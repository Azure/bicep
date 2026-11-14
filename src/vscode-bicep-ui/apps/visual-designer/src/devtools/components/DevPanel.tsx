// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ChangeEvent } from "react";
import type { MotionPolicy, TargetScope } from "@/core";
import type { GraphMutation, SampleGraph } from "../fakes/sample-graphs";

import { Codicon } from "@vscode-bicep-ui/components";
import { useCallback, useState } from "react";
import { styled } from "styled-components";
import { FakeMessageChannel } from "../fakes/fake-message-channel";
import { GRAPH_MUTATIONS, SAMPLE_GRAPHS } from "../fakes/sample-graphs";

interface DevPanelProps {
  channel: FakeMessageChannel;
}

const COLLAPSED_STORAGE_KEY = "visual-designer.dev-panel.collapsed";
const MUTATION_GROUPS: readonly GraphMutation["group"][] = ["Nodes", "Modules", "Edges"];
/** The order actions read in each group's grid, so like actions line up in the same column. */
const ACTION_ORDER = ["Add", "Rename", "Remove", "Remove first", "Remove last", "Error", "Collection"];

// The panel is dev chrome rather than product UI, so it has its own neutral look instead of the app theme, and
// floats over the canvas without taking space from it.
const $Panel = styled.div`
  position: fixed;
  top: 48px;
  left: 16px;
  z-index: 10000;
  display: flex;
  flex-direction: column;
  width: 248px;
  max-height: calc(100vh - 120px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 10px;
  background: rgba(24, 24, 27, 0.94);
  box-shadow:
    0 8px 24px rgba(0, 0, 0, 0.35),
    0 1px 3px rgba(0, 0, 0, 0.3);
  backdrop-filter: blur(12px);
  color: #d4d4d8;
  font-family: system-ui, sans-serif;
  font-size: 12px;
  line-height: 1.4;
  user-select: none;

  &[data-collapsed="true"] {
    width: auto;
  }
`;

const $Header = styled.div`
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 6px 6px 6px 10px;
`;

const $Badge = styled.span`
  padding: 1px 6px;
  border-radius: 4px;
  background: rgba(224, 160, 48, 0.18);
  color: #f0b44c;
  font-size: 10px;
  font-weight: 700;
  letter-spacing: 0.08em;
`;

const $Title = styled.span`
  flex: 1;
  font-weight: 600;
  color: #e4e4e7;
`;

const $IconButton = styled.button`
  display: flex;
  align-items: center;
  justify-content: center;
  width: 22px;
  height: 22px;
  padding: 0;
  border: none;
  border-radius: 5px;
  background: transparent;
  color: #a1a1aa;
  cursor: pointer;

  &:hover {
    background: rgba(255, 255, 255, 0.08);
    color: #f4f4f5;
  }
`;

const $Body = styled.div`
  display: flex;
  flex-direction: column;
  gap: 14px;
  padding: 4px 10px 12px;
  border-top: 1px solid rgba(255, 255, 255, 0.08);
  overflow-y: auto;
`;

const $Section = styled.div`
  display: flex;
  flex-direction: column;
  gap: 6px;
  padding-top: 8px;
`;

const $SectionTitle = styled.h2`
  margin: 0;
  color: #8b8b94;
  font-size: 10px;
  font-weight: 700;
  letter-spacing: 0.08em;
  text-transform: uppercase;
`;

const $Field = styled.label`
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 8px;
  min-height: 24px;
`;

const $Select = styled.select`
  max-width: 128px;
  padding: 2px 4px;
  border: 1px solid rgba(255, 255, 255, 0.14);
  border-radius: 4px;
  background: #27272a;
  color: #e4e4e7;
  font: inherit;
`;

const $Checkbox = styled.input`
  margin: 0;
  accent-color: #f0b44c;
`;

const $Button = styled.button`
  padding: 4px 8px;
  border: 1px solid rgba(255, 255, 255, 0.12);
  border-radius: 5px;
  background: rgba(255, 255, 255, 0.05);
  color: #e4e4e7;
  font: inherit;
  text-align: left;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  cursor: pointer;

  &:hover {
    background: rgba(255, 255, 255, 0.11);
  }

  &:active {
    background: rgba(255, 255, 255, 0.16);
  }
`;

const $GraphButton = styled($Button)`
  border-color: transparent;
  background: transparent;

  &[aria-pressed="true"] {
    border-color: rgba(240, 180, 76, 0.45);
    background: rgba(240, 180, 76, 0.12);
    color: #fbd38d;
  }
`;

const $List = styled.div`
  display: flex;
  flex-direction: column;
  gap: 1px;
`;

const $Grid = styled.div`
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 4px;
`;

const $ActionButton = styled($Button)`
  display: flex;
  align-items: center;
  gap: 6px;

  & > :first-child {
    flex: none;
    color: #a1a1aa;
  }
`;

const $GroupLabel = styled.div`
  margin: 2px 0 4px;
  color: #a1a1aa;
  font-size: 11px;
`;

function readCollapsed() {
  try {
    return localStorage.getItem(COLLAPSED_STORAGE_KEY) === "true";
  } catch {
    return false;
  }
}

function slugify(value: string): string {
  return value
    .normalize("NFKD")
    .replace(/[^a-zA-Z0-9]+/g, "-")
    .replace(/^-+|-+$/g, "")
    .toLowerCase();
}

function Chevron({ isCollapsed }: { isCollapsed: boolean }) {
  return (
    <svg width="14" height="14" viewBox="0 0 16 16" aria-hidden="true">
      <path
        d={isCollapsed ? "M6 4l4 4-4 4" : "M4 6l4 4 4-4"}
        fill="none"
        stroke="currentColor"
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  );
}

/**
 * A floating panel rendered only in dev mode (`npm run dev`) that stands in for the extension host: it changes host
 * settings, loads sample graphs, and mutates the current one through the {@link FakeMessageChannel}.
 */
export function DevPanel({ channel }: DevPanelProps) {
  const [isCollapsed, setIsCollapsed] = useState(readCollapsed);
  const [activeGraph, setActiveGraph] = useState("Module graph");

  const toggleCollapsed = useCallback(() => {
    setIsCollapsed((collapsed) => {
      try {
        localStorage.setItem(COLLAPSED_STORAGE_KEY, String(!collapsed));
      } catch {
        // Storage can be unavailable; the panel just does not remember.
      }
      return !collapsed;
    });
  }, []);

  const changeScope = useCallback(
    (event: ChangeEvent<HTMLSelectElement>) => channel.setTargetScope(event.target.value as TargetScope),
    [channel],
  );
  const changeMotionPolicy = useCallback(
    (event: ChangeEvent<HTMLSelectElement>) => channel.setMotionPolicy(event.target.value as MotionPolicy),
    [channel],
  );
  const changeResourceEditing = useCallback(
    (event: ChangeEvent<HTMLInputElement>) => channel.setResourceEditingEnabled(event.target.checked),
    [channel],
  );
  const changeColorThemeMatching = useCallback(
    (event: ChangeEvent<HTMLInputElement>) => channel.setColorThemeMatched(event.target.checked),
    [channel],
  );
  const changeCatalog = useCallback(() => channel.changeCatalog(), [channel]);

  const loadGraph = (name: string, graph: SampleGraph | null) => {
    setActiveGraph(name);
    channel.pushGraph(graph);
  };
  const applyMutation = (apply: GraphMutation["apply"]) => {
    const current = channel.getCurrentGraph();
    if (current) channel.pushGraph(apply(current));
  };

  return (
    <$Panel role="group" aria-label="Dev panel" data-testid="dev-panel" data-collapsed={isCollapsed}>
      <$Header>
        <$Badge>DEV</$Badge>
        {!isCollapsed && <$Title>Playground</$Title>}
        <$IconButton
          type="button"
          aria-label={isCollapsed ? "Show dev panel" : "Hide dev panel"}
          title={isCollapsed ? "Show dev panel" : "Hide dev panel"}
          aria-expanded={!isCollapsed}
          onClick={toggleCollapsed}
        >
          <Chevron isCollapsed={isCollapsed} />
        </$IconButton>
      </$Header>
      {!isCollapsed && (
        <$Body>
          <$Section role="group" aria-label="Host">
            <$SectionTitle>Host</$SectionTitle>
            <$Field>
              Target scope
              <$Select
                aria-label="Document target scope"
                defaultValue={channel.getTargetScope()}
                onChange={changeScope}
              >
                <option value="resourceGroup">Resource group</option>
                <option value="subscription">Subscription</option>
                <option value="managementGroup">Management group</option>
                <option value="tenant">Tenant</option>
              </$Select>
            </$Field>
            <$Field title="Simulate bicep.visualizer.experimental.enableResourceEditing">
              Resource editing
              <$Checkbox
                type="checkbox"
                aria-label="Resource editing"
                defaultChecked={channel.isResourceEditingEnabled()}
                onChange={changeResourceEditing}
              />
            </$Field>
            <$Field title="Simulate bicep.visualizer.matchColorTheme">
              Match color theme
              <$Checkbox
                type="checkbox"
                aria-label="Match color theme"
                defaultChecked={channel.isColorThemeMatched()}
                onChange={changeColorThemeMatching}
              />
            </$Field>
            <$Field>
              Motion
              <$Select
                aria-label="Motion policy"
                defaultValue={channel.getMotionPolicy()}
                onChange={changeMotionPolicy}
              >
                <option value="animate">Animate</option>
                <option value="reduce">Reduce</option>
                <option value="system">System</option>
              </$Select>
            </$Field>
            <$Button type="button" title="Replace the resource type catalog" onClick={changeCatalog}>
              Change catalog
            </$Button>
          </$Section>

          <$Section role="group" aria-label="Graphs">
            <$SectionTitle>Graphs</$SectionTitle>
            <$List>
              {Object.entries(SAMPLE_GRAPHS).map(([name, graph]) => (
                <$GraphButton
                  key={name}
                  type="button"
                  aria-pressed={activeGraph === name}
                  onClick={() => loadGraph(name, graph)}
                  data-testid={`dev-graph-${slugify(name)}`}
                >
                  {name}
                </$GraphButton>
              ))}
            </$List>
          </$Section>

          <$Section role="group" aria-label="Mutations">
            <$SectionTitle>Mutations</$SectionTitle>
            {MUTATION_GROUPS.map((group) => (
              <div key={group} role="group" aria-label={group}>
                <$GroupLabel aria-hidden="true">{group}</$GroupLabel>
                <$Grid>
                  {GRAPH_MUTATIONS.filter((mutation) => mutation.group === group)
                    .sort((left, right) => ACTION_ORDER.indexOf(left.action) - ACTION_ORDER.indexOf(right.action))
                    .map((mutation) => (
                      <$ActionButton
                        key={mutation.label}
                        type="button"
                        aria-label={mutation.label}
                        title={mutation.description}
                        onClick={() => applyMutation(mutation.apply)}
                        data-testid={`dev-mutation-${slugify(mutation.label)}`}
                      >
                        <Codicon name={mutation.icon} size={14} />
                        {mutation.action}
                      </$ActionButton>
                    ))}
                </$Grid>
              </div>
            ))}
          </$Section>
        </$Body>
      )}
    </$Panel>
  );
}
