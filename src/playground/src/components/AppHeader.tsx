// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import React from "react";
import bicepLogoUrl from "../../../icons/bicep-logo-256.png";
import { quickstartsPaths } from "../quickstarts/quickstarts";
import { ColorMode } from "../theme/color-mode";

interface Props {
  activeOperation: string | undefined;
  colorMode: ColorMode;
  copied: boolean;
  sampleLoading: boolean;
  sampleSelectRef: React.RefObject<HTMLSelectElement | null>;
  selectedSample: string | undefined;
  uploadInputRef: React.RefObject<HTMLInputElement | null>;
  onCopyLink(): void;
  onDecompile(file: File): void;
  onSampleChange(path: string): void;
  onToggleColorMode(): void;
}

export const AppHeader: React.FC<Props> = ({
  activeOperation,
  colorMode,
  copied,
  sampleLoading,
  sampleSelectRef,
  selectedSample,
  uploadInputRef,
  onCopyLink,
  onDecompile,
  onSampleChange,
  onToggleColorMode,
}) => {
  const isBusy = activeOperation !== undefined || sampleLoading;

  return (
    <header className="app-header">
      <div className="brand" aria-label="Bicep Playground">
        <img src={bicepLogoUrl} alt="" className="brand-mark" />
        <span>Bicep Playground</span>
      </div>

      <nav className="header-actions" aria-label="Playground actions">
        <label className="visually-hidden" htmlFor="sample-template">
          Sample template
        </label>
        <select
          ref={sampleSelectRef}
          id="sample-template"
          className="sample-select"
          disabled={isBusy}
          value={selectedSample ?? ""}
          onChange={(event) => {
            const path = event.currentTarget.value;
            if (path) {
              onSampleChange(path);
            }
          }}
        >
          <option value="">Choose a sample...</option>
          {quickstartsPaths.map((path) => (
            <option key={path} value={path}>
              {path}
            </option>
          ))}
        </select>
        <input
          ref={uploadInputRef}
          className="visually-hidden"
          type="file"
          accept="application/json,.json"
          aria-label="ARM template JSON file"
          onChange={(event) => {
            const input = event.currentTarget;
            const file = input.files?.[0];
            input.value = "";
            if (file) {
              onDecompile(file);
            }
          }}
        />
        <button
          type="button"
          className="button"
          disabled={isBusy}
          title="Open an ARM template JSON file and decompile it to Bicep"
          onClick={() => uploadInputRef.current?.click()}
        >
          <svg aria-hidden="true" viewBox="0 0 24 24" fill="none">
            <path
              d="M6 3.5h8l4 4V20H6V3.5Z"
              stroke="currentColor"
              strokeWidth="1.7"
              strokeLinejoin="round"
            />
            <path
              d="M14 3.5v4h4M9 11l-1.5 1.5L9 14m6-3 1.5 1.5L15 14m-3.7 1 1.4-5"
              stroke="currentColor"
              strokeWidth="1.5"
              strokeLinecap="round"
              strokeLinejoin="round"
            />
          </svg>
          <span>Decompile</span>
        </button>

        <button
          type="button"
          className="button copy-link-button"
          aria-label={copied ? "Copied" : "Copy Link"}
          onClick={onCopyLink}
        >
          <svg aria-hidden="true" viewBox="0 0 24 24" fill="none">
            <path
              d="M9 15 7 17a3 3 0 0 1-4.2-4.2l3-3A3 3 0 0 1 10 9m5 0 2-2a3 3 0 0 1 4.2 4.2l-3 3A3 3 0 0 1 14 15m-6-3h8"
              stroke="currentColor"
              strokeWidth="1.8"
              strokeLinecap="round"
            />
          </svg>
          <span className="copy-link-label">
            {copied ? "Copied" : "Copy Link"}
          </span>
        </button>

        <button
          type="button"
          className="button icon-button"
          aria-label={`Switch to ${colorMode === "dark" ? "light" : "dark"} theme`}
          title={`Switch to ${colorMode === "dark" ? "light" : "dark"} theme`}
          onClick={onToggleColorMode}
        >
          {colorMode === "dark" ? (
            <svg aria-hidden="true" viewBox="0 0 24 24" fill="none">
              <circle
                cx="12"
                cy="12"
                r="3.5"
                stroke="currentColor"
                strokeWidth="1.8"
              />
              <path
                d="M12 2v2m0 16v2M4.9 4.9l1.4 1.4m11.4 11.4 1.4 1.4M2 12h2m16 0h2M4.9 19.1l1.4-1.4M17.7 6.3l1.4-1.4"
                stroke="currentColor"
                strokeWidth="1.8"
                strokeLinecap="round"
              />
            </svg>
          ) : (
            <svg aria-hidden="true" viewBox="0 0 24 24" fill="none">
              <path
                d="M20.4 15.5A8.5 8.5 0 0 1 8.5 3.6 8.5 8.5 0 1 0 20.4 15.5Z"
                stroke="currentColor"
                strokeWidth="1.8"
                strokeLinejoin="round"
              />
            </svg>
          )}
        </button>
      </nav>
    </header>
  );
};
