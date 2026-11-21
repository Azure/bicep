// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Plan, PlannedStack } from "./api";

export function PlanScope({ plan }: { plan: Plan }) {
  if (plan.stacks.some(stack => !stack.selectionReason)) {
    return <p className="plan-scope">Selection reasons are unavailable for this plan. Predict again with the updated service.</p>;
  }
  const selected = plan.stacks.filter(stack => stack.selectionReason === "Selected").length;
  const prerequisites = plan.stacks.length - selected;
  return <p className="plan-scope">{selected} directly selected · {prerequisites} included as {prerequisites === 1 ? "a prerequisite" : "prerequisites"}</p>;
}

export function SelectionReason({ stack, selectors }: { stack: PlannedStack; selectors: string[] }) {
  if (stack.selectionReason === "Selected") {
    return <p>Directly selected{selectors.length ? " by the label filters" : " (no label filters applied)"}.</p>;
  }
  if (stack.selectionReason === "Prerequisite") {
    return <p>Included as a prerequisite{stack.requiredBy?.length
      ? <> of {stack.requiredBy.map((name, i) => <span key={name}>{i > 0 && ", "}<code>{name}</code></span>)}</>
      : null}.</p>;
  }
  return <p>Selection reason unavailable for this plan.</p>;
}
