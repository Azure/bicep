// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReactNode } from "react";
import type { ResourceTypeCatalogEntry } from "../atoms";
import type { ResourceTypeCatalogGroup, ResourceTypeNamespace } from "../types";
import type { PaletteContentProps } from "./PaletteContent";

import { Accordion, Codicon, useAccordionItem } from "@vscode-bicep-ui/components";
import { useAtomValue, useSetAtom, useStore } from "jotai";
import { motion } from "motion/react";
import { memo, useCallback, useMemo, useState } from "react";
import styled from "styled-components";
import { getErrorMessage } from "@/utils";
import {
  getNamespaceResourceTypesKey,
  namespaceResourceTypesAtomFamily,
  resourceTypeCatalogLoadingCountAtom,
} from "../atoms";
import { useProgressiveBudget } from "../hooks/use-progressive-budget";
import { allocateProgressiveRows } from "../progressive-rows";
import { ResourceTypeItem } from "./ResourceTypeItem";

const $Groups = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2px;
  padding: 4px 8px 8px;
`;

/** Marks the end of what is rendered; reaching it renders the next batch of groups and rows. */
const $MoreSentinel = styled.div`
  height: 1px;
`;

const $Group = styled.div`
  & > button:focus-visible {
    border-radius: 4px;
    outline: 1px solid ${({ theme }) => theme.focusBorder};
    outline-offset: -1px;
  }
`;

const $GroupHeader = styled.div`
  display: flex;
  min-height: 28px;
  align-items: center;
  gap: 8px;
  padding: 0 8px;
  border: 1px solid transparent;
  border-radius: 4px;
  font-weight: 600;

  &:hover {
    border-color: var(--vscode-contrastActiveBorder, transparent);
    background: ${({ theme }) => theme.iconButton.hoverBackground};
  }
`;

const $Chevron = styled.span<{ $active: boolean }>`
  display: inline-flex;
  color: ${({ theme }) => theme.text.secondary};
  transform: rotate(${({ $active }) => ($active ? "180deg" : "0deg")});
  transition: transform 150ms ease-out;
`;

const $GroupName = styled.span`
  min-width: 0;
  flex: 1;
  overflow-wrap: anywhere;
`;

const $Items = styled.div`
  display: flex;
  flex-direction: column;
  padding: 2px 0 6px;
`;

const $Highlight = styled.mark`
  border-radius: 2px;
  color: inherit;
  background: var(--vscode-editor-findMatchHighlightBackground);
  box-shadow: 0 0 0 1px var(--vscode-editor-findMatchHighlightBorder, transparent);
`;

export const PaletteMessage = styled.div`
  padding: 16px;
  color: ${({ theme }) => theme.text.secondary};
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

/**
 * Memoized on primitive props, so growing the progressive budget renders only the newly revealed rows instead of
 * re-rendering every row already on screen.
 */
const ResourceTypeRow = memo(function ResourceTypeRow({
  group,
  resourceType,
  apiVersion,
  highlightQuery,
  loadVersions,
  onResourceTypePointerDown,
}: {
  group: string;
  resourceType: string;
  apiVersion: string;
  highlightQuery?: string;
  loadVersions: PaletteContentProps["loadVersions"];
  onResourceTypePointerDown?: PaletteContentProps["onResourceTypePointerDown"];
}) {
  return (
    <ResourceTypeItem
      fullyQualifiedType={`${group}/${resourceType}`}
      defaultVersion={apiVersion}
      loadVersions={loadVersions}
      onResourceTypePointerDown={onResourceTypePointerDown}
    >
      <HighlightMatches text={resourceType} query={highlightQuery} />
    </ResourceTypeItem>
  );
});

function ResourceTypeItems({
  group,
  resourceTypes,
  rowLimit,
  highlightQuery,
  loadVersions,
  onResourceTypePointerDown,
}: {
  group: string;
  resourceTypes: ResourceTypeCatalogEntry[];
  rowLimit: number;
  highlightQuery?: string;
  loadVersions: PaletteContentProps["loadVersions"];
  onResourceTypePointerDown?: PaletteContentProps["onResourceTypePointerDown"];
}) {
  return (
    <$Items>
      {resourceTypes.slice(0, rowLimit).map(({ resourceType, apiVersion }) => (
        <ResourceTypeRow
          key={`${resourceType}@${apiVersion}`}
          group={group}
          resourceType={resourceType}
          apiVersion={apiVersion}
          highlightQuery={highlightQuery}
          loadVersions={loadVersions}
          onResourceTypePointerDown={onResourceTypePointerDown}
        />
      ))}
    </$Items>
  );
}

function ResourceTypeGroupFrame({
  group,
  highlightQuery,
  children,
}: {
  group: string;
  highlightQuery?: string;
  children: ReactNode;
}) {
  const { active } = useAccordionItem();

  return (
    <$Group>
      <Accordion.ItemCollapse>
        <$GroupHeader>
          <$GroupName>
            <HighlightMatches text={group} query={highlightQuery} />
          </$GroupName>
          <$Chevron $active={active}>
            <Codicon name="chevron-down" size={14} />
          </$Chevron>
        </$GroupHeader>
      </Accordion.ItemCollapse>
      <Accordion.ItemContent>{children}</Accordion.ItemContent>
    </$Group>
  );
}

function LazyResourceTypeGroup({
  catalogId,
  namespace,
  rowLimit,
  loadNamespace,
  loadVersions,
  onResourceTypePointerDown,
}: {
  catalogId: string;
  namespace: ResourceTypeNamespace;
  rowLimit: number;
  loadNamespace: PaletteContentProps["loadNamespace"];
  loadVersions: PaletteContentProps["loadVersions"];
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
      <ResourceTypeGroupFrame group={namespace.name}>
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
              loadVersions={loadVersions}
              group={namespace.name}
              resourceTypes={state.resourceTypes}
              rowLimit={rowLimit}
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
  loadVersions,
  setExpandedGroups,
  onResourceTypePointerDown,
}: {
  groups: ResourceTypeCatalogGroup[];
  expandedGroups: readonly string[];
  highlightQuery: string;
  loadVersions: PaletteContentProps["loadVersions"];
  setExpandedGroups: (groups: readonly string[]) => void;
  onResourceTypePointerDown?: PaletteContentProps["onResourceTypePointerDown"];
}) {
  const { budget, sentinelRef } = useProgressiveBudget(`search:${highlightQuery}`);
  const { rowsPerGroup, hasMore } = allocateProgressiveRows(
    groups.map(({ group, resourceTypes }) => ({
      rowCount: resourceTypes.length,
      expanded: expandedGroups.includes(group),
    })),
    budget,
  );

  return (
    <$Groups>
      <Accordion multiple value={expandedGroups} onValueChange={(value) => setExpandedGroups(value.map(String))}>
        {groups.slice(0, rowsPerGroup.length).map(({ group, resourceTypes }, index) => (
          <Accordion.Item key={group} itemId={group}>
            <ResourceTypeGroupFrame group={group} highlightQuery={highlightQuery}>
              <ResourceTypeItems
                loadVersions={loadVersions}
                group={group}
                resourceTypes={resourceTypes}
                rowLimit={rowsPerGroup[index] ?? 0}
                highlightQuery={highlightQuery}
                onResourceTypePointerDown={onResourceTypePointerDown}
              />
            </ResourceTypeGroupFrame>
          </Accordion.Item>
        ))}
      </Accordion>
      {hasMore && <$MoreSentinel ref={sentinelRef} aria-hidden="true" data-testid="resource-palette-more" />}
    </$Groups>
  );
}

export function LazyResourceTypeGroups({
  catalogId,
  namespaces,
  loadNamespace,
  loadVersions,
  onResourceTypePointerDown,
}: {
  catalogId: string;
  namespaces: ResourceTypeNamespace[];
  loadNamespace: PaletteContentProps["loadNamespace"];
  loadVersions: PaletteContentProps["loadVersions"];
  onResourceTypePointerDown?: PaletteContentProps["onResourceTypePointerDown"];
}) {
  const [expandedGroups, setExpandedGroups] = useState<readonly string[]>([]);
  const { budget, sentinelRef } = useProgressiveBudget(`browse:${catalogId}`);
  const { rowsPerGroup, hasMore } = allocateProgressiveRows(
    namespaces.map((namespace) => ({
      rowCount: namespace.resourceTypeCount,
      expanded: expandedGroups.includes(namespace.name),
    })),
    budget,
  );

  return (
    <$Groups>
      <Accordion multiple value={expandedGroups} onValueChange={(value) => setExpandedGroups(value.map(String))}>
        {namespaces.slice(0, rowsPerGroup.length).map((namespace, index) => (
          <LazyResourceTypeGroup
            loadVersions={loadVersions}
            key={namespace.name}
            catalogId={catalogId}
            namespace={namespace}
            rowLimit={rowsPerGroup[index] ?? 0}
            loadNamespace={loadNamespace}
            onResourceTypePointerDown={onResourceTypePointerDown}
          />
        ))}
      </Accordion>
      {hasMore && <$MoreSentinel ref={sentinelRef} aria-hidden="true" data-testid="resource-palette-more" />}
    </$Groups>
  );
}
