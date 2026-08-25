// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import React, { ReactNode } from "react";

export type StatusTone = "neutral" | "success" | "warning" | "error";

interface Props {
  children: ReactNode;
  id: string;
  labelId: string;
  status?: string;
  statusTone?: StatusTone;
  subtitle: string;
  title: string;
  actions?: ReactNode;
  ariaBusy?: boolean;
  isStale?: boolean;
  pane: "bicep" | "arm";
}

export const EditorPane: React.FC<Props> = ({
  children,
  id,
  labelId,
  status,
  statusTone,
  subtitle,
  title,
  actions,
  ariaBusy,
  isStale,
  pane,
}) => (
  <section
    id={id}
    className={`editor-pane${isStale ? " is-stale" : ""}`}
    data-pane={pane}
    role="tabpanel"
    aria-busy={ariaBusy}
    aria-labelledby={labelId}
  >
    <header className="pane-header">
      <div className="pane-title-group">
        <h2 id={labelId} className="pane-title">
          {title}
        </h2>
        <span className="pane-subtitle" title={subtitle}>
          {subtitle}
        </span>
      </div>
      <div className="pane-tools">
        {status && statusTone && (
          <span className={`status-pill ${statusTone}`}>
            <span className="status-dot" aria-hidden="true" />
            {status}
          </span>
        )}
        {actions}
      </div>
    </header>
    <div className="editor-surface">{children}</div>
  </section>
);
