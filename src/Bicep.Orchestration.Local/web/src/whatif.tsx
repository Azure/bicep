// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import React from "react";
import type { Change } from "./api";

type PropertyChange = { path?: string; propertyChangeType: string; before?: unknown; after?: unknown; children?: PropertyChange[] };

const resourceSymbols: Record<string, string> = { Create: "+", Delete: "-", Modify: "~", Deploy: "!", NoChange: "=", Ignore: "*", Unsupported: "x" };
const propertySymbols: Record<string, string> = { Create: "+", Delete: "-", Modify: "~", Array: "~", NoEffect: "x" };
const propertyOrder = ["Delete", "Create", "Modify", "Array", "NoEffect"];
const resourceOrder = ["Delete", "Create", "Modify", "Deploy", "Unsupported", "NoChange", "Ignore"];
const verbs: Record<string, string> = { Create: "to create", Delete: "to delete", Modify: "to modify", Deploy: "to deploy", NoChange: "no change", Unsupported: "unsupported" };

// Mirrors `bicep what-if`: show the resource ID relative to its deployment scope.
const relative = (id: string) => {
  const index = id.toLowerCase().lastIndexOf("/providers/");
  return index < 0 ? id : id.slice(index + "/providers/".length);
};
const isLeaf = (value: unknown) => value === null || typeof value !== "object";
const json = (value: unknown) => value === undefined ? "null" : JSON.stringify(value, null, 2);
const sorted = (changes: PropertyChange[]) => [...changes].sort((a, b) =>
  propertyOrder.indexOf(a.propertyChangeType) - propertyOrder.indexOf(b.propertyChangeType) || (a.path ?? "").localeCompare(b.path ?? ""));

function Value({ value, kind }: { value: unknown; kind: string }) {
  return isLeaf(value)
    ? <span className={`wi-${kind}`}>{json(value)}</span>
    : <pre className={`wi-block wi-${kind}`}>{json(value)}</pre>;
}

function Property({ change }: { change: PropertyChange }) {
  const kind = change.propertyChangeType;
  const children = change.children ?? [];
  const head = <><span className={`wi-symbol wi-${kind}`}>{propertySymbols[kind] ?? "?"}</span> {change.path && <>{change.path}: </>}</>;
  if ((kind === "Modify" || kind === "Array") && children.length) {
    const array = kind === "Array";
    return <li>{head}{array && "["}
      <ul className="wi-nested">{sorted(children).map((child, i) => <Property key={`${child.path}-${i}`} change={child} />)}</ul>
      {array && "]"}</li>;
  }
  if (kind === "Modify") {
    return <li>{head}<Value value={change.before} kind="Delete" /> =&gt; <Value value={change.after} kind="Create" /></li>;
  }
  const value = kind === "Delete" ? change.before : change.after;
  return <li>{head}<Value value={value} kind={kind === "NoEffect" ? "NoEffect" : kind} /></li>;
}

export function WhatIfDiff({ changes }: { changes: Change[] }) {
  const visible = changes.filter(c => c.changeType !== "Ignore")
    .sort((a, b) => resourceOrder.indexOf(a.changeType) - resourceOrder.indexOf(b.changeType) || a.resourceId.localeCompare(b.resourceId));
  if (!visible.length) return <div className="whatif"><span className="wi-NoChange">No resource changes.</span></div>;
  const counts = Object.entries(visible.reduce<Record<string, number>>((all, c) => ({ ...all, [c.changeType]: (all[c.changeType] ?? 0) + 1 }), {}));
  return <div className="whatif" aria-label="What-if changes">
    <ul>{visible.map(change => {
      const delta = (change.delta as PropertyChange[] | undefined) ?? [];
      return <li key={change.resourceId} className="wi-resource" title={change.resourceId}>
        <span className={`wi-symbol wi-${change.changeType}`}>{resourceSymbols[change.changeType] ?? "?"}</span> <span className={`wi-${change.changeType}`}>{relative(change.resourceId)}</span>
        {change.changeType === "Create" && change.after !== undefined ? <pre className="wi-block wi-Create">{json(change.after)}</pre>
          : change.changeType === "Delete" && change.before !== undefined ? <pre className="wi-block wi-Delete">{json(change.before)}</pre>
            : delta.length > 0 && <ul className="wi-nested">{sorted(delta).map((property, i) => <Property key={`${property.path}-${i}`} change={property} />)}</ul>}
      </li>;
    })}</ul>
    <p className="wi-summary">Resource changes: {counts.map(([kind, count]) => `${count} ${verbs[kind] ?? kind}`).join(", ")}.</p>
  </div>;
}
