// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import React, { ReactNode } from "react";
import { CompilationStatus } from "./BicepEditor";
import { getBicepVersionLink } from "./version-link";

interface Props {
  compilationStatus: CompilationStatus;
  durationMs: number | undefined;
  errorCount: number;
  hasProblems: boolean;
  problemsOpen: boolean;
  version: string;
  warningCount: number;
  onToggleProblems(): void;
}

export const StatusBar: React.FC<Props> = ({
  compilationStatus,
  durationMs,
  errorCount,
  hasProblems,
  problemsOpen,
  version,
  warningCount,
  onToggleProblems,
}) => {
  const status = getStatus(compilationStatus);
  const summaryContents = (
    <>
      {status.icon}
      <strong>{status.label}</strong>
      {durationMs !== undefined && (
        <span className="status-duration">{Math.round(durationMs)} ms</span>
      )}
    </>
  );
  const versionLink = getBicepVersionLink(version);

  return (
    <footer className="status-bar" aria-label="Compilation status">
      <span className="visually-hidden" role="status" aria-live="polite">
        {status.label}
      </span>
      {hasProblems ? (
        <button
          id="compilation-status-summary"
          type="button"
          className={`status-section status-summary ${status.tone}`}
          aria-expanded={problemsOpen}
          aria-controls="problems-panel"
          onClick={onToggleProblems}
        >
          {summaryContents}
        </button>
      ) : (
        <div
          className={`status-section ${status.tone}`}
          role="status"
          aria-live="polite"
        >
          {summaryContents}
        </div>
      )}
      <div className="status-details">
        <span>
          {errorCount} {errorCount === 1 ? "error" : "errors"}
        </span>
        <span>
          {warningCount} {warningCount === 1 ? "warning" : "warnings"}
        </span>
      </div>
      <span className="layout-spacer" />
      <a
        className="github-link"
        href="https://github.com/Azure/bicep"
        target="_blank"
        rel="noopener noreferrer"
        aria-label="Bicep repository on GitHub (opens in a new tab)"
        title="View Bicep on GitHub"
      >
        <svg aria-hidden="true" viewBox="0 0 24 24" fill="none">
          <path
            d="M12 2.8a9.4 9.4 0 0 0-3 18.3c.5.1.6-.2.6-.5v-1.8c-2.7.6-3.3-1.1-3.3-1.1-.4-1.1-1.1-1.4-1.1-1.4-.9-.6.1-.6.1-.6 1 0 1.5 1 1.5 1 .9 1.5 2.3 1.1 2.8.8.1-.6.4-1.1.7-1.3-2.2-.2-4.5-1.1-4.5-4.7 0-1 .4-1.9 1-2.5-.1-.3-.4-1.2.1-2.5 0 0 .8-.3 2.6 1a9 9 0 0 1 4.8 0c1.8-1.2 2.6-1 2.6-1 .5 1.3.2 2.2.1 2.5.6.6 1 1.5 1 2.5 0 3.6-2.3 4.5-4.5 4.7.4.3.7.9.7 1.7v2.6c0 .3.2.6.7.5A9.4 9.4 0 0 0 12 2.8Z"
            fill="currentColor"
          />
        </svg>
      </a>
      {versionLink.href ? (
        <a
          className="version-link"
          href={versionLink.href}
          target="_blank"
          rel="noopener noreferrer"
          aria-label={versionLink.ariaLabel}
        >
          {versionLink.label}
        </a>
      ) : (
        <span className="version-link">{versionLink.label}</span>
      )}
    </footer>
  );
};

function getStatus(status: CompilationStatus): {
  icon: ReactNode;
  label: string;
  tone: "neutral" | "success" | "warning" | "error";
} {
  const icon = (path: ReactNode) => (
    <svg
      className="status-icon"
      aria-hidden="true"
      viewBox="0 0 24 24"
      fill="none"
    >
      {path}
    </svg>
  );

  switch (status) {
    case "pending":
      return {
        icon: icon(
          <circle
            cx="12"
            cy="12"
            r="7"
            stroke="currentColor"
            strokeWidth="1.8"
          />,
        ),
        label: "Changes pending",
        tone: "warning",
      };
    case "compiling":
      return {
        icon: <span className="spinner small" aria-hidden="true" />,
        label: "Compiling",
        tone: "neutral",
      };
    case "failed":
      return {
        icon: icon(
          <>
            <circle
              cx="12"
              cy="12"
              r="8.5"
              stroke="currentColor"
              strokeWidth="1.8"
            />
            <path
              d="m9 9 6 6m0-6-6 6"
              stroke="currentColor"
              strokeWidth="1.8"
              strokeLinecap="round"
            />
          </>,
        ),
        label: "Compilation failed",
        tone: "error",
      };
    case "upToDate":
      return {
        icon: icon(
          <path
            d="m5 12 4 4L19 6"
            stroke="currentColor"
            strokeWidth="2"
            strokeLinecap="round"
            strokeLinejoin="round"
          />,
        ),
        label: "Compiled",
        tone: "success",
      };
  }
}
