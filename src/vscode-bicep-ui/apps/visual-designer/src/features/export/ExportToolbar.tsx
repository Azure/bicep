// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { DefaultTheme } from "styled-components";
import type { ExportBackgroundMode } from "./types";

import { Codicon } from "@vscode-bicep-ui/components";
import { VscodeOption, VscodeSingleSelect } from "@vscode-elements/react-elements";
import { useAtomValue, useSetAtom, useStore } from "jotai";
import { useCallback, useEffect, useState } from "react";
import { styled } from "styled-components";
import {
  closeExportOverlayAtom,
  exportBackgroundColorAtom,
  exportBackgroundModeAtom,
  exportCanvasElementAtom,
  exportFileStemAtom,
  exportPaddingAtom,
  exportThemeOverrideAtom,
  isExportInProgressAtom,
} from "./atoms";
import { captureGraphElement, saveDataUrl } from "./capture-element";

/* ------------------------------------------------------------------ */
/*  Styled components                                                  */
/* ------------------------------------------------------------------ */

const $Toolbar = styled.div`
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 6px 8px;
  background-color: ${({ theme }) => theme.controlBar.background};
  border: 1px solid ${({ theme }) => theme.controlBar.border};
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  backdrop-filter: blur(8px);
  white-space: nowrap;

  /*
   * Override the VS Code design-token CSS variables consumed by
   * <vscode-single-select> (and any other @vscode-elements widget
   * we may add to this toolbar) so its shadow-DOM styles resolve
   * to the visualizer's curated palette instead of the editor's
   * native theme. The variables listed here are the ones declared
   * by vscode-single-select via @cssprop; any not listed fall back
   * to the library's defaults.
   */
  --vscode-foreground: ${({ theme }) => theme.text.primary};
  --vscode-focusBorder: ${({ theme }) => theme.focusBorder};

  --vscode-settings-dropdownBackground: ${({ theme }) => theme.controlBar.background};
  --vscode-settings-dropdownForeground: ${({ theme }) => theme.text.primary};
  --vscode-settings-dropdownBorder: ${({ theme }) => theme.controlBar.border};
  --vscode-settings-dropdownListBorder: ${({ theme }) => theme.controlBar.border};
  --vscode-settings-checkboxBackground: ${({ theme }) => theme.node.background};

  --vscode-list-hoverBackground: ${({ theme }) => theme.controlBar.hoverBackground};
  --vscode-list-hoverForeground: ${({ theme }) => theme.text.primary};
  --vscode-list-activeSelectionBackground: ${({ theme }) => theme.controlBar.activeBackground};
  --vscode-list-activeSelectionForeground: ${({ theme }) => theme.text.primary};
  --vscode-list-focusOutline: ${({ theme }) => theme.focusBorder};
  --vscode-list-focusHighlightForeground: ${({ theme }) => theme.focusBorder};
  --vscode-list-highlightForeground: ${({ theme }) => theme.focusBorder};

  --vscode-badge-background: ${({ theme }) => theme.text.secondary};
  --vscode-badge-foreground: ${({ theme }) => theme.node.background};

  --vscode-inputValidation-errorBorder: ${({ theme }) => theme.error};
`;

/** Logical group of related controls. */
const $Group = styled.div`
  display: flex;
  align-items: center;
  gap: 4px;
  flex-shrink: 0;
`;

const $ActionsGroup = styled($Group)`
  margin-left: auto;
`;

const $BackgroundSelect = styled(VscodeSingleSelect)`
  width: 100px;
`;

const $ThemeSelect = styled(VscodeSingleSelect)`
  width: 100px;
`;

const $Separator = styled.div`
  width: 1px;
  align-self: stretch;
  margin: 2px 0;
  background-color: ${({ theme }) => theme.controlBar.border};
`;

/* ---- Primary action button -------------------------------------- */

const $ExportButton = styled.button`
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 5px;
  padding: 4px 14px;
  border: none;
  border-radius: 4px;
  background-color: ${({ theme }) => theme.focusBorder};
  color: #ffffff;
  cursor: pointer;
  font-size: 12px;
  font-family: inherit;
  font-weight: 600;
  white-space: nowrap;
  transition:
    opacity 0.15s ease,
    transform 0.1s ease;

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

  &:focus-visible {
    outline: 2px solid ${({ theme }) => theme.focusBorder};
    outline-offset: 2px;
  }
`;

/* ---- Icon button ------------------------------------------------ */

const $IconButton = styled.button`
  display: flex;
  align-items: center;
  justify-content: center;
  width: 24px;
  height: 24px;
  padding: 0;
  border: none;
  border-radius: 4px;
  background-color: transparent;
  color: ${({ theme }) => theme.controlBar.icon};
  cursor: pointer;
  transition: background-color 0.15s ease;

  &:hover {
    background-color: ${({ theme }) => theme.controlBar.hoverBackground};
  }

  &:active {
    background-color: ${({ theme }) => theme.controlBar.activeBackground};
  }

  &:focus-visible {
    outline: 2px solid ${({ theme }) => theme.focusBorder};
    outline-offset: 1px;
  }
`;

/* ---- Labels ----------------------------------------------------- */

const $Label = styled.label`
  font-size: 11px;
  color: ${({ theme }) => theme.text.secondary};
  user-select: none;
  white-space: nowrap;
`;

const $BackgroundLabel = styled($Label)`
  margin-left: 4px;
`;

/* ---- Padding stepper -------------------------------------------- */

const $StepperGroup = styled.div`
  display: inline-flex;
  align-items: center;
  border-radius: 4px;
  overflow: hidden;
  border: 1px solid ${({ theme }) => theme.controlBar.border};
`;

const $StepButton = styled.button`
  display: flex;
  align-items: center;
  justify-content: center;
  width: 22px;
  height: 22px;
  padding: 0;
  border: none;
  background-color: transparent;
  color: ${({ theme }) => theme.text.primary};
  cursor: pointer;
  transition: background-color 0.15s ease;

  &:hover {
    background-color: ${({ theme }) => theme.controlBar.hoverBackground};
  }

  &:active {
    background-color: ${({ theme }) => theme.controlBar.activeBackground};
  }

  &:focus-visible {
    outline: 2px solid ${({ theme }) => theme.focusBorder};
    outline-offset: -2px;
  }
`;

const $PaddingInput = styled.input`
  width: 32px;
  padding: 2px 0;
  border: none;
  border-left: 1px solid ${({ theme }) => theme.controlBar.border};
  border-right: 1px solid ${({ theme }) => theme.controlBar.border};
  border-radius: 0;
  background-color: transparent;
  color: ${({ theme }) => theme.text.primary};
  font-size: 11px;
  font-family: inherit;
  text-align: center;

  &:focus {
    outline: none;
    background-color: ${({ theme }) => theme.controlBar.hoverBackground};
  }

  /* Hide spinner arrows */
  appearance: textfield;
  -moz-appearance: textfield;
  &::-webkit-outer-spin-button,
  &::-webkit-inner-spin-button {
    -webkit-appearance: none;
    margin: 0;
  }
`;

/* ------------------------------------------------------------------ */
/*  Constants                                                          */
/* ------------------------------------------------------------------ */

const BACKGROUND_OPTIONS: { label: string; value: ExportBackgroundMode }[] = [
  { label: "Transparent", value: "transparent" },
  { label: "Solid", value: "solid" },
];

const THEME_OPTIONS: { label: string; value: DefaultTheme["name"] | null }[] = [
  { label: "Current", value: null },
  { label: "Light", value: "light" },
  { label: "Dark", value: "dark" },
  { label: "Dark HC", value: "high-contrast" },
  { label: "Light HC", value: "high-contrast-light" },
];

const STEP = 10;

/* ------------------------------------------------------------------ */
/*  Component                                                          */
/* ------------------------------------------------------------------ */

export function ExportToolbar() {
  const store = useStore();
  const canvasElement = useAtomValue(exportCanvasElementAtom);
  const backgroundMode = useAtomValue(exportBackgroundModeAtom);
  const padding = useAtomValue(exportPaddingAtom);
  const exportThemeName = useAtomValue(exportThemeOverrideAtom);
  const exportFileStem = useAtomValue(exportFileStemAtom);
  const exportBackgroundColor = useAtomValue(exportBackgroundColorAtom);
  const exporting = useAtomValue(isExportInProgressAtom);
  const setBackgroundMode = useSetAtom(exportBackgroundModeAtom);
  const setTheme = useSetAtom(exportThemeOverrideAtom);
  const setPadding = useSetAtom(exportPaddingAtom);
  const setExportInProgress = useSetAtom(isExportInProgressAtom);
  const closeExportOverlay = useSetAtom(closeExportOverlayAtom);

  const handleExport = useCallback(async () => {
    if (!canvasElement || exporting) {
      return;
    }

    setExportInProgress(true);

    try {
      const backgroundColor = backgroundMode === "solid" ? exportBackgroundColor : undefined;
      const dataUrl = await captureGraphElement(canvasElement, store, padding, backgroundColor);
      const fileStem = exportFileStem.trim() || "bicep-graph";
      await saveDataUrl(dataUrl, `${fileStem}.png`);
    } catch (error) {
      console.error("Export failed:", error);
    } finally {
      setExportInProgress(false);
    }
  }, [
    canvasElement,
    exporting,
    setExportInProgress,
    store,
    backgroundMode,
    padding,
    exportBackgroundColor,
    exportFileStem,
  ]);

  const [paddingText, setPaddingText] = useState(String(padding));

  useEffect(() => {
    setPaddingText(String(padding));
  }, [padding]);

  const handlePaddingChange = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>) => {
      const raw = e.target.value;
      if (raw.includes("-")) return;
      setPaddingText(raw);
      const value = parseInt(raw, 10);
      if (!isNaN(value) && value >= 0) {
        setPadding(value);
      }
    },
    [setPadding],
  );

  const handlePaddingBlur = useCallback(() => {
    const value = parseInt(paddingText, 10);
    const clamped = isNaN(value) || value < 0 ? 0 : value;
    setPadding(clamped);
    setPaddingText(String(clamped));
  }, [paddingText, setPadding]);

  const stepPadding = useCallback(
    (delta: number) => {
      const next = Math.max(0, padding + delta);
      setPadding(next);
      setPaddingText(String(next));
    },
    [padding, setPadding],
  );

  const handleBackgroundSelect = useCallback(
    (e: Event) => {
      setBackgroundMode((e.currentTarget as HTMLSelectElement).value as ExportBackgroundMode);
    },
    [setBackgroundMode],
  );

  const handleThemeSelect = useCallback(
    (e: Event) => {
      const value = (e.currentTarget as HTMLSelectElement).value;
      setTheme(value === "" ? null : (value as DefaultTheme["name"]));
    },
    [setTheme],
  );

  return (
    <$Toolbar role="toolbar" aria-label="Export settings">
      {/* Background */}
      <$Group>
        <$BackgroundLabel>Background</$BackgroundLabel>
        <$BackgroundSelect onChange={handleBackgroundSelect}>
          {BACKGROUND_OPTIONS.map((bg) => (
            <VscodeOption key={bg.value} value={bg.value} selected={bg.value === backgroundMode}>
              {bg.label}
            </VscodeOption>
          ))}
        </$BackgroundSelect>
      </$Group>

      <$Separator />

      {/* Theme */}
      <$Group>
        <$Label>Theme</$Label>
        <$ThemeSelect onChange={handleThemeSelect}>
          {THEME_OPTIONS.map((t) => (
            <VscodeOption key={t.label} value={t.value ?? ""} selected={t.value === exportThemeName}>
              {t.label}
            </VscodeOption>
          ))}
        </$ThemeSelect>
      </$Group>

      <$Separator />

      {/* Padding */}
      <$Group>
        <$Label>Padding</$Label>
        <$StepperGroup>
          <$StepButton onClick={() => stepPadding(-STEP)} title="Decrease padding" aria-label="Decrease padding">
            <Codicon name="remove" size={12} />
          </$StepButton>
          <$PaddingInput
            type="number"
            value={paddingText}
            onChange={handlePaddingChange}
            onBlur={handlePaddingBlur}
            min={0}
            max={200}
            step={STEP}
            title="Padding (px)"
            aria-label="Padding in pixels"
          />
          <$StepButton onClick={() => stepPadding(STEP)} title="Increase padding" aria-label="Increase padding">
            <Codicon name="add" size={12} />
          </$StepButton>
        </$StepperGroup>
      </$Group>

      <$ActionsGroup>
        <$ExportButton onClick={handleExport} disabled={exporting}>
          {exporting ? "Exporting\u2026" : "Export"}
        </$ExportButton>
        <$IconButton onClick={() => closeExportOverlay()} title="Close" aria-label="Close export toolbar">
          <Codicon name="close" size={14} />
        </$IconButton>
      </$ActionsGroup>
    </$Toolbar>
  );
}
