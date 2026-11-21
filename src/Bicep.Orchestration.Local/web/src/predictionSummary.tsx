// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Plan, PlannedStack } from "./api";

export function summarizePrediction(stacks: PlannedStack[]) {
  const counts = new Map<string, number>();
  for (const stack of stacks) {
    for (const change of stack.changes) counts.set(change.changeType, (counts.get(change.changeType) ?? 0) + 1);
  }
  const known = ["Create", "Modify", "Delete"];
  const changed = known.reduce((total, kind) => total + (counts.get(kind) ?? 0), 0);
  const uncertain = [...counts].filter(([kind]) => ![...known, "NoChange", "NoEffect"].includes(kind));
  return { counts, known, changed, uncertain };
}

export function PredictionSummary({ plan }: { plan: Plan }) {
  const { counts, known, changed, uncertain } = summarizePrediction(plan.stacks);
  return <section className="prediction-summary" aria-label="Prediction summary">
    <strong>{changed} {uncertain.length ? "known " : ""}predicted resource {changed === 1 ? "change" : "changes"} across {plan.stacks.length} {plan.stacks.length === 1 ? "stack" : "stacks"}</strong>
    <div>{known.filter(kind => counts.has(kind)).map(kind =>
      <span className={`badge ${kind}`} key={kind}>{counts.get(kind)} {kind === "Create" ? "to create" : kind === "Modify" ? "to modify" : "to delete"}</span>)}
      {(counts.get("NoChange") ?? 0) > 0 && <span className="badge">{counts.get("NoChange")} unchanged</span>}
      {uncertain.map(([kind, count]) => <span className="badge prediction-uncertain" key={kind}>{count} {kind} · review required</span>)}
    </div>
  </section>;
}
