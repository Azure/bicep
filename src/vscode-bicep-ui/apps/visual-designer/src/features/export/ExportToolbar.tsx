// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ExportFormat } from "./types";

import { Codicon } from "@vscode-bicep-ui/components";
import { useCallback, useState } from "react";
import { styled } from "styled-components";

const $Toolbar = styled.div`
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 12px;
  background-color: ${({ theme }) => theme.controlBar.background};
  border: 1px solid ${({ theme }) => theme.controlBar.border};
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  backdrop-filter: blur(8px);
`;

const $FormatButton = styled.button<{ $active: boolean }>`
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 4px 12px;
  border: 1px solid ${({ theme, $active }) => ($active ? theme.focusBorder : theme.controlBar.border)};
  border-radius: 4px;
  background-color: ${({ theme, $active }) =>
    $active ? theme.controlBar.hoverBackground : "transparent"};
  color: ${({ theme }) => theme.text.primary};
  cursor: pointer;
  font-size: 12px;
  font-family: inherit;
  transition: all 0.15s ease;

  &:hover {
    background-color: ${({ theme }) => theme.controlBar.hoverBackground};
  }
`;

const $ExportButton = styled.button`
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  min-width: 110px;
  padding: 6px 16px;
  border: none;
  border-radius: 4px;
  background-color: ${({ theme }) => theme.focusBorder};
  color: #ffffff;
  cursor: pointer;
  font-size: 12px;
  font-family: inherit;
  font-weight: 600;
  transition: all 0.15s ease;

  &:hover {
    opacity: 0.9;
  }

  &:active {
    transform: scale(0.97);
  }

  &:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }
`;

const $IconButton = styled.button`
  display: flex;
  align-items: center;
  justify-content: center;
  width: 28px;
  height: 28px;
  padding: 0;
  border: none;
  border-radius: 4px;
  background-color: transparent;
  color: ${({ theme }) => theme.controlBar.icon};
  cursor: pointer;
  transition: all 0.15s ease;

  &:hover {
    background-color: ${({ theme }) => theme.controlBar.hoverBackground};
  }

  &:active {
    background-color: ${({ theme }) => theme.controlBar.activeBackground};
    transform: scale(0.95);
  }
`;

const $Separator = styled.div`
  width: 1px;
  height: 20px;
  background-color: ${({ theme }) => theme.controlBar.border};
`;

const $Label = styled.span`
  font-size: 12px;
  color: ${({ theme }) => theme.text.secondary};
  user-select: none;
`;

const $PaddingInput = styled.input`
  width: 44px;
  padding: 3px 6px;
  border: 1px solid ${({ theme }) => theme.controlBar.border};
  border-radius: 4px;
  background-color: transparent;
  color: ${({ theme }) => theme.text.primary};
  font-size: 12px;
  font-family: inherit;
  text-align: center;

  &:focus {
    outline: none;
    border-color: ${({ theme }) => theme.focusBorder};
  }

  /* Hide spinner arrows */
  -moz-appearance: textfield;
  &::-webkit-outer-spin-button,
  &::-webkit-inner-spin-button {
    -webkit-appearance: none;
    margin: 0;
  }
`;

const $SizeLabel = styled.span`
  font-size: 12px;
  color: ${({ theme }) => theme.text.primary};
  user-select: none;
  font-variant-numeric: tabular-nums;
`;

const FORMATS: ExportFormat[] = ["svg", "png", "jpeg"];

export interface ExportToolbarProps {
  onExport: (format: ExportFormat) => void;
  onClose: () => void;
  padding: number;
  onPaddingChange: (padding: number) => void;
  canvasWidth: number;
  canvasHeight: number;
  exporting?: boolean;
}

export function ExportToolbar({
  onExport,
  onClose,
  padding,
  onPaddingChange,
  canvasWidth,
  canvasHeight,
  exporting,
}: ExportToolbarProps) {
  const [format, setFormat] = useState<ExportFormat>("png");

  const handleExport = useCallback(() => {
    onExport(format);
  }, [onExport, format]);

  const handlePaddingChange = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>) => {
      const value = parseInt(e.target.value, 10);
      if (!isNaN(value) && value >= 0) {
        onPaddingChange(value);
      }
    },
    [onPaddingChange],
  );

  return (
    <$Toolbar>
      <$Label>Format:</$Label>
      {FORMATS.map((f) => (
        <$FormatButton key={f} $active={f === format} onClick={() => setFormat(f)}>
          {f.toUpperCase()}
        </$FormatButton>
      ))}
      <$Separator />
      <$Label>Padding:</$Label>
      <$PaddingInput
        type="number"
        value={padding}
        onChange={handlePaddingChange}
        min={0}
        max={200}
        step={10}
        title="Padding around graph (px)"
        aria-label="Padding"
      />
      <$Label>px</$Label>
      <$Separator />
      <$SizeLabel>{canvasWidth} &times; {canvasHeight}</$SizeLabel>
      <$Separator />
      <$ExportButton onClick={handleExport} disabled={exporting}>
        <Codicon name="desktop-download" size={14} />
        {exporting ? "Saving…" : "Save As"}
      </$ExportButton>
      <$Separator />
      <$IconButton onClick={onClose} title="Close Export View" aria-label="Close Export View">
        <Codicon name="close" size={16} />
      </$IconButton>
    </$Toolbar>
  );
}
