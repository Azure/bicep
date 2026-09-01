// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Codicon } from "@vscode-bicep-ui/components";
import styled from "styled-components";
import { MotionAwareProgressBar } from "@/features/accessibility";

const $StickyControls = styled.div`
  position: sticky;
  top: 0;
  z-index: 1;
  padding-top: 10px;
  background: color-mix(in srgb, var(--vscode-editorWidget-background) 96%, transparent);
  backdrop-filter: blur(12px);
`;

const $Search = styled.label`
  display: flex;
  height: 30px;
  align-items: center;
  gap: 6px;
  margin: 0 10px 8px;
  padding: 0 8px;
  border: 1px solid var(--vscode-input-border, var(--vscode-widget-border));
  border-radius: 6px;
  color: var(--vscode-input-foreground);
  background: var(--vscode-input-background);

  &:focus-within {
    border-color: var(--vscode-focusBorder);
  }
`;

const $SearchInput = styled.input`
  min-width: 0;
  flex: 1;
  border: 0;
  outline: 0;
  color: inherit;
  background: transparent;
  font: inherit;

  &::placeholder {
    color: var(--vscode-input-placeholderForeground);
  }
`;

const $ProgressTrack = styled.div`
  height: 2px;
  overflow: hidden;
`;

export function ResourcePaletteControls({
  query,
  setQuery,
  showProgress,
}: {
  query: string;
  setQuery: (query: string) => void;
  showProgress: boolean;
}) {
  return (
    <$StickyControls>
      <$Search>
        <Codicon name="search" size={14} />
        <$SearchInput
          aria-label="Filter resource types"
          placeholder="Filter resource types"
          value={query}
          onChange={(event) => setQuery(event.target.value)}
        />
      </$Search>
      <$ProgressTrack aria-hidden={!showProgress}>
        {showProgress && (
          <MotionAwareProgressBar
            testId="resource-palette-progress"
            ariaLabel="Loading resource types"
          />
        )}
      </$ProgressTrack>
    </$StickyControls>
  );
}
