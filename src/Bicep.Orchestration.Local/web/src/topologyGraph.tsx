// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useMemo, useState } from "react";
import type { Stack } from "./api";
import { SourceLinks } from "./sourceLinks";

export function declaration(name: string) {
  return name.replace(/\[\d+\]/g, "[*]");
}

type Group = { name: string; stacks: Stack[] };
type Edge = { from: string; to: string; count: number };

function groupTopology(stacks: Stack[]) {
  const groups = new Map<string, Group>();
  const byName = new Map(stacks.map(stack => [stack.name, stack]));
  for (const stack of stacks) {
    const name = declaration(stack.name);
    const group = groups.get(name) ?? { name, stacks: [] };
    group.stacks.push(stack);
    groups.set(name, group);
  }

  const edges = new Map<string, Map<string, Edge>>();
  for (const stack of stacks) {
    for (const dependency of stack.dependencies) {
      const prerequisite = byName.get(dependency);
      if (!prerequisite) continue;
      const from = declaration(prerequisite.name);
      const to = declaration(stack.name);
      const outgoing = edges.get(from) ?? new Map<string, Edge>();
      const edge = outgoing.get(to) ?? { from, to, count: 0 };
      edge.count++;
      outgoing.set(to, edge);
      edges.set(from, outgoing);
    }
  }
  return { groups: [...groups.values()], edges: [...edges.values()].flatMap(outgoing => [...outgoing.values()]) };
}

export function TopologyGraph({ stacks, selected, onSelect, status, version }: {
  stacks: Stack[];
  selected?: string;
  onSelect: (name: string) => void;
  status: (stack: Stack) => string;
  version: (stack: Stack) => string;
}) {
  const [selectedGroup, setSelectedGroup] = useState<string>();
  const graph = useMemo(() => {
    const { groups, edges } = groupTopology(stacks);
    const rank = new Map(groups.map(group => [group.name, 0]));
    const pending = new Map(groups.map(group => [group.name, 0]));
    for (const edge of edges) {
      if (edge.from !== edge.to) pending.set(edge.to, (pending.get(edge.to) ?? 0) + 1);
    }
    const queue = groups.filter(group => pending.get(group.name) === 0).map(group => group.name);
    for (let i = 0; i < queue.length; i++) {
      const name = queue[i];
      for (const edge of edges.filter(edge => edge.from === name && edge.from !== edge.to)) {
        rank.set(edge.to, Math.max(rank.get(edge.to) ?? 0, (rank.get(name) ?? 0) + 1));
        const remaining = (pending.get(edge.to) ?? 0) - 1;
        pending.set(edge.to, remaining);
        if (remaining === 0) queue.push(edge.to);
      }
    }

    const rows = new Map<number, number>();
    const positions = new Map<string, { x: number; y: number }>();
    for (const group of groups) {
      const column = rank.get(group.name) ?? 0;
      const row = rows.get(column) ?? 0;
      positions.set(group.name, { x: 24 + column * 260, y: 24 + row * 116 });
      rows.set(column, row + 1);
    }
    return {
      groups, edges, positions,
      width: Math.max(260, 48 + (Math.max(0, ...rank.values()) * 260) + 210),
      height: Math.max(120, 48 + Math.max(0, ...rows.values()) * 116),
    };
  }, [stacks]);
  const group = graph.groups.find(item => item.name === selectedGroup);
  const displayName = (name: string) => {
    const short = name.split("::").at(-1) ?? name;
    return graph.groups.filter(item => item.name.split("::").at(-1) === short).length === 1 ? short : name;
  };

  return <section className="topology-graph" aria-label="Stack dependency overview">
    <p>Arrows point from prerequisites to dependents. Groups combine stack instances; a connection means at least one instance depends on another, not necessarily every instance.</p>
    {!graph.groups.length ? <p>No stacks to display.</p> : <>
      <div className="graph-scroll" tabIndex={0} role="region" aria-label="Dependency diagram">
        <div className="graph-canvas" style={{ width: graph.width, height: graph.height }}>
          <svg width={graph.width} height={graph.height} aria-hidden="true">
            <defs><marker id="dependency-arrow" viewBox="0 0 10 10" refX="9" refY="5" markerWidth="7" markerHeight="7" orient="auto-start-reverse">
              <path d="M 0 0 L 10 5 L 0 10 z" />
            </marker></defs>
            {graph.edges.map(edge => {
              const from = graph.positions.get(edge.from);
              const to = graph.positions.get(edge.to);
              if (!from || !to) return null;
              const x1 = from.x + 210, y1 = from.y + 42, x2 = to.x, y2 = to.y + 42;
              const path = edge.from === edge.to
                ? `M ${x1} ${y1} C ${x1 + 35} ${y1 - 55}, ${x2 - 35} ${y2 - 55}, ${x2} ${y2}`
                : `M ${x1} ${y1} C ${x1 + 50} ${y1}, ${x2 - 50} ${y2}, ${x2} ${y2}`;
              return <path key={`${edge.from}\0${edge.to}`} d={path} className="graph-edge" markerEnd="url(#dependency-arrow)" />;
            })}
          </svg>
          {graph.groups.map(item => {
            const position = graph.positions.get(item.name);
            if (!position) return null;
            const regions = new Set(item.stacks.map(stack => stack.region)).size;
            return <button key={item.name} className="graph-node" style={{ left: position.x, top: position.y }}
              title={item.name} aria-pressed={selectedGroup === item.name} onClick={() => setSelectedGroup(item.name)}>
              <strong>{displayName(item.name)}</strong>
              <span>{item.stacks.length} {item.stacks.length === 1 ? "stack" : "stacks"} · {regions} {regions === 1 ? "region" : "regions"}</span>
            </button>;
          })}
        </div>
      </div>
      <details className="graph-relationships">
        <summary>Dependency relationships ({graph.edges.length})</summary>
        {graph.edges.length ? <ul>{graph.edges.map(edge => <li key={`${edge.from}\0${edge.to}`}>
          <strong>{edge.from}</strong> → <strong>{edge.to}</strong> · {edge.count} {edge.count === 1 ? "dependency" : "dependencies"}
        </li>)}</ul> : <p>No dependencies.</p>}
      </details>
      {group && <section className="graph-instances" aria-label={`${group.name} instances`}>
        <h3>{group.name} instances</h3>
        <p>Select an instance to inspect its exact dependencies and resources.</p>
        <SourceLinks source={group.stacks[0].source} />
        <div>{group.stacks.map(stack => <button key={stack.name} className="graph-instance"
          aria-pressed={selected === stack.name} onClick={() => onSelect(stack.name)}>
          <strong>{stack.name}</strong><span>{stack.region} · {status(stack)} · Version: {version(stack)}</span>
        </button>)}</div>
      </section>}
    </>}
  </section>;
}
