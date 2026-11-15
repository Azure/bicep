// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useEffect, useRef, useState } from "react";
import type { Stack } from "./api";

export function LabelFilters({ stacks, selectors, disabled, onChange }: {
  stacks: Stack[];
  selectors: string[];
  disabled: boolean;
  onChange: (selectors: string[]) => void;
}) {
  const filters = new Map<string, Set<string>>();
  for (const selector of selectors) {
    const equals = selector.indexOf("=");
    const key = selector.slice(0, equals);
    const values = filters.get(key) ?? new Set<string>();
    selector.slice(equals + 1).split(",").forEach(value => values.add(value));
    filters.set(key, values);
  }
  const [draft, setDraft] = useState<{ key: string; values: string[]; editing: boolean }>();
  const [search, setSearch] = useState("");
  const root = useRef<HTMLDivElement>(null);
  const editor = useRef<HTMLFormElement>(null);
  const opener = useRef<HTMLButtonElement | null>(null);
  const open = draft !== undefined;
  useEffect(() => { setDraft(undefined); }, [selectors]);
  useEffect(() => {
    if (!open) return;
    editor.current?.querySelector<HTMLElement>("select:not(:disabled), input:not(:disabled)")?.focus();
    const dismiss = (event: PointerEvent) => {
      if (event.target instanceof Node && !root.current?.contains(event.target)) setDraft(undefined);
    };
    document.addEventListener("pointerdown", dismiss);
    return () => document.removeEventListener("pointerdown", dismiss);
  }, [open]);
  function close() {
    setDraft(undefined);
    opener.current?.focus();
  }
  function start(button: HTMLButtonElement, key = "") {
    opener.current = button;
    setSearch("");
    setDraft({ key, values: [...filters.get(key) ?? []], editing: filters.has(key) });
  }
  function replace(key: string, values: string[]) {
    onChange([
      ...selectors.filter(selector => selector.slice(0, selector.indexOf("=")) !== key),
      ...(values.length ? [`${key}=${values.join(",")}`] : []),
    ]);
  }
  const keys = [...new Set(stacks.flatMap(stack => Object.keys(stack.labels)))].sort();
  const values = [...new Set([
    ...stacks.flatMap(stack => draft && stack.labels[draft.key] !== undefined ? [stack.labels[draft.key]] : []),
    ...draft?.values ?? [],
  ])].sort();
  const matching = values.filter(value => value.toLowerCase().includes(search.trim().toLowerCase()));
  return <div className="label-filters" role="group" aria-label="Label selectors" ref={root}
    onKeyDown={event => {
      if (event.key === "Escape" && open) { event.stopPropagation(); close(); }
    }}>
    <span>Label selectors</span>
    {[...filters].map(([key, accepted]) => <div className="filter-chip" key={key}>
      <button disabled={disabled} onClick={event => start(event.currentTarget, key)}
        aria-label={`Edit filter ${key}`} aria-expanded={open && draft.key === key}
        title={`${key} equals ${[...accepted].join(" or ")}`}>{key} equals <strong>{[...accepted].join(" or ")}</strong></button>
      <button disabled={disabled} aria-label={`Remove filter ${key}`} onClick={() => { replace(key, []); setDraft(undefined); }}>{"\u00d7"}</button>
    </div>)}
    <button disabled={disabled || keys.every(key => filters.has(key))} aria-expanded={open && !draft.editing}
      onClick={event => start(event.currentTarget)}>+ Add filter</button>
    {selectors.length > 0 && <button disabled={disabled} onClick={() => { onChange([]); setDraft(undefined); }}>Clear filters</button>}
    {draft && <form className="filter-editor" role="dialog" aria-label={draft.editing ? "Edit label filter" : "Add label filter"}
      ref={editor} onSubmit={event => {
        event.preventDefault();
        if (disabled || !draft.key || !draft.values.length) return;
        replace(draft.key, draft.values);
        close();
      }}>
      <h2>{draft.editing ? "Edit label filter" : "Add label filter"}</h2>
      <label>Label <select value={draft.key} disabled={disabled || draft.editing}
        onChange={event => { setDraft({ key: event.target.value, values: [], editing: false }); setSearch(""); }}>
        <option value="">Select a label</option>
        {[...new Set([...keys, ...filters.keys()])].filter(key => key === draft.key || !filters.has(key)).sort()
          .map(key => <option key={key}>{key}</option>)}
      </select></label>
      <label>Operator <select disabled value="equals"><option value="equals">Equals</option></select></label>
      <label>Values <input autoComplete="off" placeholder="Search values..." value={search}
        disabled={!draft.key || disabled} onChange={event => setSearch(event.target.value)} /></label>
      <div className="filter-values" role="group" aria-label="Label values">
        {matching.map(value => <label key={value}><input type="checkbox" checked={draft.values.includes(value)} disabled={disabled}
          onChange={event => setDraft({ ...draft, values: event.target.checked
            ? [...draft.values, value] : draft.values.filter(selected => selected !== value) })} />{value}</label>)}
        {!matching.length && <small>{draft.key ? "No matching values." : "Choose a label to see its values."}</small>}
      </div>
      <small>Any selected value matches. Different label filters must all match. Prerequisites are included in the plan.</small>
      <div className="filter-actions">
        <button type="submit" disabled={disabled || !draft.key || !draft.values.length}>Apply filter</button>
        <button type="button" onClick={close}>Cancel</button>
      </div>
    </form>}
  </div>;
}
