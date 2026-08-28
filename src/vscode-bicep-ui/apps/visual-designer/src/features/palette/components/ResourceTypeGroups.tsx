// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReactNode } from "react";
import type { ResourceTypeCatalogEntry } from "../atoms";
import type { ResourceTypeCatalogGroup, ResourceTypeNamespace } from "../types";
import type { PaletteContentProps } from "./PaletteContent";

import { Accordion, AzureIcon, Codicon, useAccordionItem } from "@vscode-bicep-ui/components";
import { useAtomValue, useSetAtom, useStore } from "jotai";
import { motion } from "motion/react";
import { useCallback, useMemo, useState } from "react";
import styled from "styled-components";
import { getErrorMessage } from "@/lib/utils";
import {
  getNamespaceResourceTypesKey,
  namespaceResourceTypesAtomFamily,
  resourceTypeCatalogLoadingCountAtom,
} from "../atoms";

const $Groups = styled.div`
  display: flex;
  flex-direction: column;
  gap: 4px;
  padding: 0 6px 8px;
`;

const $Group = styled.div<{ $active: boolean }>`
  overflow: hidden;
  border-radius: 7px;
  background: ${({ $active }) =>
    $active ? "color-mix(in srgb, var(--vscode-editorWidget-background) 70%, transparent)" : "transparent"};
`;

const $GroupHeader = styled.div`
  display: flex;
  min-height: 30px;
  align-items: center;
  gap: 6px;
  padding: 0 7px;
  border-radius: 6px;
  color: var(--vscode-foreground);
  font-size: 12px;
  font-weight: 600;

  &:hover {
    background: var(--vscode-toolbar-hoverBackground);
  }
`;

const $Chevron = styled.span<{ $active: boolean }>`
  display: inline-flex;
  transform: rotate(${({ $active }) => ($active ? "90deg" : "0deg")});
  transition: transform 100ms ease-out;
`;

const $GroupName = styled.span`
  min-width: 0;
  flex: 1;
  overflow-wrap: anywhere;
`;

const $Count = styled.span`
  color: var(--vscode-descriptionForeground);
  font-size: 11px;
  font-weight: 400;
`;

const $Items = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2px;
  padding: 2px 4px 6px 22px;
`;

const $Item = styled.button`
  display: grid;
  width: 100%;
  min-height: 34px;
  grid-template-columns: 20px minmax(0, 1fr) auto;
  align-items: center;
  gap: 7px;
  padding: 3px 6px;
  border: 1px solid transparent;
  border-radius: 6px;
  color: var(--vscode-foreground);
  text-align: left;
  background: transparent;
  cursor: grab;
  touch-action: none;

  &:hover {
    border-color: var(--vscode-widget-border);
    background: var(--vscode-list-hoverBackground);
  }

  &:active {
    cursor: grabbing;
    color: var(--vscode-list-activeSelectionForeground);
    background: var(--vscode-list-activeSelectionBackground);
  }

  &:active > span:last-child {
    color: inherit;
    background: color-mix(in srgb, currentColor 14%, transparent);
  }

  &:disabled {
    cursor: default;
  }
`;

const $TypeName = styled.span`
  min-width: 0;
  line-height: 15px;
  overflow-wrap: anywhere;
`;

const $Highlight = styled.mark`
  border-radius: 2px;
  color: inherit;
  background: var(--vscode-editor-findMatchHighlightBackground);
  box-shadow: 0 0 0 1px var(--vscode-editor-findMatchHighlightBorder, transparent);
`;

const $Version = styled.span`
  padding: 1px 5px;
  border-radius: 999px;
  color: var(--vscode-badge-foreground);
  background: var(--vscode-badge-background);
  font-size: 10px;
  font-variant-numeric: tabular-nums;
`;

export const PaletteMessage = styled.div`
  padding: 18px 12px;
  color: var(--vscode-descriptionForeground);
  text-align: center;
`;

export const PaletteRetry = styled.button`
  margin-left: 6px;
  padding: 1px 6px;
  border: 1px solid var(--vscode-button-border, transparent);
  border-radius: 2px;
  color: var(--vscode-button-foreground);
  background: var(--vscode-button-background);
  cursor: pointer;

  &:hover {
    background: var(--vscode-button-hoverBackground);
  }
`;

function HighlightMatches({ text, query }: { text: string; query?: string }) {
  const normalizedQuery = query?.trim().toLocaleLowerCase();
  if (!normalizedQuery) {
    return text;
  }

  const normalizedText = text.toLocaleLowerCase();
  const segments: ReactNode[] = [];
  let offset = 0;
  let matchIndex = normalizedText.indexOf(normalizedQuery, offset);

  while (matchIndex >= 0) {
    if (matchIndex > offset) {
      segments.push(text.slice(offset, matchIndex));
    }
    const matchEnd = matchIndex + normalizedQuery.length;
    segments.push(<$Highlight key={matchIndex}>{text.slice(matchIndex, matchEnd)}</$Highlight>);
    offset = matchEnd;
    matchIndex = normalizedText.indexOf(normalizedQuery, offset);
  }

  if (segments.length === 0) {
    return text;
  }
  if (offset < text.length) {
    segments.push(text.slice(offset));
  }

  return segments;
}

function ResourceTypeItems({
  group,
  resourceTypes,
  highlightQuery,
  onResourceTypeActivate,
  onResourceTypePointerDown,
}: {
  group: string;
  resourceTypes: ResourceTypeCatalogEntry[];
  highlightQuery?: string;
  onResourceTypeActivate?: PaletteContentProps["onResourceTypeActivate"];
  onResourceTypePointerDown?: PaletteContentProps["onResourceTypePointerDown"];
}) {
  return (
    <$Items>
      {resourceTypes.map(({ resourceType, apiVersion }) => {
        const fullyQualifiedType = `${group}/${resourceType}`;
        return (
          <$Item
            key={`${resourceType}@${apiVersion}`}
            disabled={!onResourceTypeActivate && !onResourceTypePointerDown}
            onKeyDown={(event) => {
              if ((event.key === "Enter" || event.key === " ") && onResourceTypeActivate) {
                event.preventDefault();
                onResourceTypeActivate({ fullyQualifiedType, apiVersion });
              }
            }}
            onPointerDown={(event) => onResourceTypePointerDown?.({ fullyQualifiedType, apiVersion }, event)}
          >
            <AzureIcon resourceType={fullyQualifiedType} size={18} />
            <$TypeName>
              <HighlightMatches text={resourceType} query={highlightQuery} />
            </$TypeName>
            <$Version>{apiVersion}</$Version>
          </$Item>
        );
      })}
    </$Items>
  );
}

function ResourceTypeGroupFrame({
  group,
  count,
  highlightQuery,
  children,
}: {
  group: string;
  count: number;
  highlightQuery?: string;
  children: ReactNode;
}) {
  const { active } = useAccordionItem();

  return (
    <$Group $active={active}>
      <Accordion.ItemCollapse>
        <$GroupHeader>
          <$Chevron $active={active}>
            <Codicon name="chevron-right" size={14} />
          </$Chevron>
          <$GroupName>
            <HighlightMatches text={group} query={highlightQuery} />
          </$GroupName>
          <$Count>{count}</$Count>
        </$GroupHeader>
      </Accordion.ItemCollapse>
      <Accordion.ItemContent>{children}</Accordion.ItemContent>
    </$Group>
  );
}

function LazyResourceTypeGroup({
  catalogId,
  namespace,
  loadNamespace,
  onResourceTypeActivate,
  onResourceTypePointerDown,
}: {
  catalogId: string;
  namespace: ResourceTypeNamespace;
  loadNamespace: PaletteContentProps["loadNamespace"];
  onResourceTypeActivate?: PaletteContentProps["onResourceTypeActivate"];
  onResourceTypePointerDown?: PaletteContentProps["onResourceTypePointerDown"];
}) {
  const stateAtom = useMemo(
    () => namespaceResourceTypesAtomFamily(getNamespaceResourceTypesKey(catalogId, namespace.name)),
    [catalogId, namespace.name],
  );
  const state = useAtomValue(stateAtom);
  const setState = useSetAtom(stateAtom);
  const setLoadingCount = useSetAtom(resourceTypeCatalogLoadingCountAtom);
  const store = useStore();

  const load = useCallback(
    async (force = false) => {
      const current = store.get(stateAtom);
      if ((!force && current.status !== "idle") || current.status === "loading") {
        return;
      }

      setState({ status: "loading" });
      setLoadingCount((count) => count + 1);
      try {
        const catalog = await loadNamespace(namespace.name);
        const group = catalog.groups.find(
          (candidate) => candidate.group.toLocaleLowerCase() === namespace.name.toLocaleLowerCase(),
        );
        setState({ status: "loaded", resourceTypes: group?.resourceTypes ?? [] });
      } catch (error) {
        setState({ status: "error", message: getErrorMessage(error, "Failed to load resource types.") });
      } finally {
        setLoadingCount((count) => Math.max(0, count - 1));
      }
    },
    [loadNamespace, namespace.name, setLoadingCount, setState, stateAtom, store],
  );

  return (
    <Accordion.Item itemId={namespace.name} onActiveChange={(active) => active && void load()}>
      <ResourceTypeGroupFrame group={namespace.name} count={namespace.resourceTypeCount}>
        {state.status === "error" ? (
          <PaletteMessage>
            {state.message}
            <PaletteRetry onClick={() => void load(true)}>Retry</PaletteRetry>
          </PaletteMessage>
        ) : state.status === "loaded" && state.resourceTypes.length === 0 ? (
          <PaletteMessage>No resource types available.</PaletteMessage>
        ) : state.status === "loaded" ? (
          <motion.div
            initial={{ opacity: 0, y: -4 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.16, ease: [0.2, 0.8, 0.2, 1] }}
          >
            <ResourceTypeItems
              group={namespace.name}
              resourceTypes={state.resourceTypes}
              onResourceTypeActivate={onResourceTypeActivate}
              onResourceTypePointerDown={onResourceTypePointerDown}
            />
          </motion.div>
        ) : null}
      </ResourceTypeGroupFrame>
    </Accordion.Item>
  );
}

export function SearchResourceTypeGroups({
  groups,
  expandedGroups,
  highlightQuery,
  setExpandedGroups,
  onResourceTypeActivate,
  onResourceTypePointerDown,
}: {
  groups: ResourceTypeCatalogGroup[];
  expandedGroups: readonly string[];
  highlightQuery: string;
  setExpandedGroups: (groups: readonly string[]) => void;
  onResourceTypeActivate?: PaletteContentProps["onResourceTypeActivate"];
  onResourceTypePointerDown?: PaletteContentProps["onResourceTypePointerDown"];
}) {
  return (
    <$Groups>
      <Accordion multiple value={expandedGroups} onValueChange={(value) => setExpandedGroups(value.map(String))}>
        {groups.map(({ group, resourceTypes }) => (
          <Accordion.Item key={group} itemId={group}>
            <ResourceTypeGroupFrame group={group} count={resourceTypes.length} highlightQuery={highlightQuery}>
              <ResourceTypeItems
                group={group}
                resourceTypes={resourceTypes}
                highlightQuery={highlightQuery}
                onResourceTypeActivate={onResourceTypeActivate}
                onResourceTypePointerDown={onResourceTypePointerDown}
              />
            </ResourceTypeGroupFrame>
          </Accordion.Item>
        ))}
      </Accordion>
    </$Groups>
  );
}

export function LazyResourceTypeGroups({
  catalogId,
  namespaces,
  loadNamespace,
  onResourceTypeActivate,
  onResourceTypePointerDown,
}: {
  catalogId: string;
  namespaces: ResourceTypeNamespace[];
  loadNamespace: PaletteContentProps["loadNamespace"];
  onResourceTypeActivate?: PaletteContentProps["onResourceTypeActivate"];
  onResourceTypePointerDown?: PaletteContentProps["onResourceTypePointerDown"];
}) {
  const [expandedGroups, setExpandedGroups] = useState<readonly string[]>([]);

  return (
    <$Groups>
      <Accordion multiple value={expandedGroups} onValueChange={(value) => setExpandedGroups(value.map(String))}>
        {namespaces.map((namespace) => (
          <LazyResourceTypeGroup
            key={namespace.name}
            catalogId={catalogId}
            namespace={namespace}
            loadNamespace={loadNamespace}
            onResourceTypeActivate={onResourceTypeActivate}
            onResourceTypePointerDown={onResourceTypePointerDown}
          />
        ))}
      </Accordion>
    </$Groups>
  );
}
