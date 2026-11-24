// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PendingApproval } from "./api";
import { WhatIfDiff } from "./whatif";
import { PlanScope, SelectionReason } from "./planScope";
import { PredictionSummary } from "./predictionSummary";
import { VersionChange } from "./sourceLinks";

export function Approvals({ pending, selectedId, onSelect, onApprove, onReject, busy, active }: {
  pending: PendingApproval[];
  selectedId: string | undefined;
  onSelect: (id: string) => void;
  onApprove: (id: string) => void;
  onReject: (id: string) => void;
  busy: boolean;
  active: boolean;
}) {
  const selected = pending.find(item => item.plan.id === selectedId) ?? pending[0];
  return <section className="approvals" aria-label="Pending approvals">
    <h2>Pending approvals <span className="badge">{pending.length}</span></h2>
    <p>Submission does not deploy anything. Review a plan, then approve it to start deployment.</p>
    <p className="approval-notice">Local prototype: any connected client can approve or reject. No separate approver identity or role is enforced.
      Pending plans are held in memory and are lost when the host restarts.</p>
    {!pending.length ? <p>No plans pending approval.</p> : <div className="approval-layout">
      <ul className="approval-list">
        {pending.map(({ plan, submittedAt }) => <li key={plan.id} data-plan={plan.id}>
          <button aria-pressed={selected?.plan.id === plan.id} onClick={() => onSelect(plan.id)}>
            <strong>Review plan {plan.id.slice(0, 8)}</strong>
            <code>{plan.source}</code>
            <span>{plan.stacks.length} stacks · {plan.selectors.join(" · ") || "All stacks"}</span>
            <time dateTime={submittedAt}>Submitted {new Date(submittedAt).toLocaleString()}</time>
          </button>
        </li>)}
      </ul>
      {selected && <section className="approval-review" aria-label="Plan review">
        <div className="approval-heading">
          <h2>Plan {selected.plan.id.slice(0, 8)}</h2>
          <p>Plan ID: <code>{selected.plan.id}</code></p>
        </div>
        <div className="approval-section">
          <dl className="approval-metadata">
            <div><dt>Source:</dt><dd><code>{selected.plan.source}</code></dd></div>
            <div><dt>Deployment version:</dt><dd>{selected.plan.version}</dd></div>
            <div><dt>Scope:</dt><dd>{selected.plan.selectors.join(" · ") || "All stacks"} · {selected.plan.stacks.length} stacks, including prerequisites</dd></div>
          </dl>
          <PlanScope plan={selected.plan} />
        </div>
        <div className="approval-section">
          <h3>Prediction summary</h3>
          <PredictionSummary plan={selected.plan} />
          <p className="approval-guidance">Approval revalidates source, parameters, and observed state. Stale plans cannot deploy; reject them and predict a new plan.</p>
        </div>
        <div className="approval-actions">
          <button className="primary" disabled={busy || active} onClick={() => onApprove(selected.plan.id)}>Approve &amp; deploy</button>
          <button disabled={busy} onClick={() => onReject(selected.plan.id)}>Reject plan</button>
        </div>
        {active && <p>A deployment is running. Approval is available after it completes.</p>}
        <div className="approval-section approval-changes"><h3>Submitted resource changes</h3>
        {selected.plan.stacks.map(stack => <details key={stack.name} open={selected.plan.stacks.length === 1}>
          <summary>{stack.name}</summary>
          <SelectionReason stack={stack} selectors={selected.plan.selectors} />
          <VersionChange previousVersion={stack.previousVersion} version={selected.plan.version} />
          <WhatIfDiff changes={stack.changes} />
        </details>)}
        </div>
      </section>}
    </div>}
  </section>;
}
