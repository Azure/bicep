// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReactNode } from "react";
import type { ResourceTypeCatalogEntry, ResourceTypeGroup } from "../types";
import type { PaletteContentProps } from "./PaletteContent";

import { Accordion, Codicon, useAccordionItem } from "@vscode-bicep-ui/components";
import { memo } from "react";
import styled from "styled-components";
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

export const PaletteAction = styled.button`
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

/**
 * The catalog's groups as an accordion, rendered progressively: headers and rows are added in batches as the
 * end of what is rendered scrolls into reach. A new `budgetKey` starts over from the first batch.
 */
export function ResourceTypeGroups({
  groups,
  expandedGroups,
  setExpandedGroups,
  budgetKey,
  highlightQuery,
  keepHeadersMounted = false,
  loadVersions,
  onResourceTypePointerDown,
}: {
  groups: readonly ResourceTypeGroup[];
  expandedGroups: readonly string[];
  setExpandedGroups: (groups: readonly string[]) => void;
  budgetKey: string;
  highlightQuery?: string;
  /** Keep the headers already revealed mounted when opening a large group uses up the budget. */
  keepHeadersMounted?: boolean;
  loadVersions: PaletteContentProps["loadVersions"];
  onResourceTypePointerDown?: PaletteContentProps["onResourceTypePointerDown"];
}) {
  const { budget, sentinelRef } = useProgressiveBudget(budgetKey);
  const { rowsPerGroup, hasMore, truncatedGroupIndex } = allocateProgressiveRows(
    groups.map(({ group, resourceTypes }) => ({
      rowCount: resourceTypes.length,
      expanded: expandedGroups.includes(group),
    })),
    budget,
    keepHeadersMounted ? Math.min(budget, groups.length) : 0,
  );
  const sentinel = <$MoreSentinel ref={sentinelRef} aria-hidden="true" data-testid="resource-palette-more" />;

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
              {index === truncatedGroupIndex && sentinel}
            </ResourceTypeGroupFrame>
          </Accordion.Item>
        ))}
      </Accordion>
      {hasMore && truncatedGroupIndex === undefined && sentinel}
    </$Groups>
  );
}
