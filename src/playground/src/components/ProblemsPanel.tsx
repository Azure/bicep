// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { editor, MarkerSeverity } from "monaco-editor";
import React from "react";

interface Props {
  compilationError: string | undefined;
  diagnostics: editor.IMarkerData[];
  sourceName: string;
  onClose(): void;
  onSelect(diagnostic: editor.IMarkerData): void;
}

export const ProblemsPanel: React.FC<Props> = ({
  compilationError,
  diagnostics,
  sourceName,
  onClose,
  onSelect,
}) => {
  const errorCount = diagnostics.filter(
    (diagnostic) => diagnostic.severity === MarkerSeverity.Error,
  ).length;
  const warningCount = diagnostics.filter(
    (diagnostic) => diagnostic.severity === MarkerSeverity.Warning,
  ).length;
  const informationCount = diagnostics.length - errorCount - warningCount;
  const countLabel = [
    errorCount ? `${errorCount} ${errorCount === 1 ? "error" : "errors"}` : "",
    warningCount
      ? `${warningCount} ${warningCount === 1 ? "warning" : "warnings"}`
      : "",
    informationCount
      ? `${informationCount} ${
          informationCount === 1 ? "information" : "informational messages"
        }`
      : "",
  ]
    .filter(Boolean)
    .join(", ");

  return (
    <section
      id="problems-panel"
      className="problems-panel"
      aria-labelledby="problems-title"
    >
      <header className="problems-header">
        <h2 id="problems-title" className="problems-title">
          Problems
        </h2>
        <span className="problems-count">{countLabel || "Compiler failure"}</span>
        <span className="layout-spacer" />
        <button
          type="button"
          className="pane-action"
          aria-label="Close Problems"
          onClick={onClose}
        >
          <svg aria-hidden="true" viewBox="0 0 24 24" fill="none">
            <path
              d="m7 7 10 10M17 7 7 17"
              stroke="currentColor"
              strokeWidth="1.8"
              strokeLinecap="round"
            />
          </svg>
        </button>
      </header>
      <ul className="problems-list">
        {diagnostics.map((diagnostic, index) => (
          <li key={`${diagnostic.startLineNumber}:${diagnostic.startColumn}:${index}`}>
            <button
              type="button"
              className="problem"
              onClick={() => onSelect(diagnostic)}
            >
              <span
                className={`problem-code ${getSeverityClass(diagnostic.severity)}`}
              >
                {getDiagnosticCode(diagnostic)}
              </span>
              <span className="problem-message">{diagnostic.message}</span>
              <span className="problem-location">
                {sourceName}:{diagnostic.startLineNumber}:
                {diagnostic.startColumn}
              </span>
            </button>
          </li>
        ))}
        {compilationError && diagnostics.length === 0 && (
          <li className="compiler-error">{compilationError}</li>
        )}
      </ul>
    </section>
  );
};

function getDiagnosticCode(diagnostic: editor.IMarkerData): string {
  if (typeof diagnostic.code === "string") {
    return diagnostic.code;
  }

  return diagnostic.code?.value ?? "Bicep";
}

function getSeverityClass(severity: MarkerSeverity): string {
  switch (severity) {
    case MarkerSeverity.Error:
      return "error";
    case MarkerSeverity.Warning:
      return "warning";
    default:
      return "information";
  }
}
