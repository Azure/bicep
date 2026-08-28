---
applyTo: "src/**/*.{ts,tsx}"
description: "Use when working with shared state, atoms, Jotai, or state management patterns in the visual designer app."
---

# State Management (Jotai)

## Core Rules

- Use Jotai as the default for shared feature state.
- Co-locate atoms with the feature they belong to.
- Export only the atoms other layers need through the feature `index.ts` barrel; keep the rest internal.
- Prefer small atoms over one large object atom.
- Use derived atoms for view intent (e.g. `isExportCanvasCoverVisibleAtom`).
- Use action atoms (`open*`, `close*`, `report*`, `reset*`) when the action touches multiple atoms, and
  expose those rather than raw writable atoms across a feature boundary.
- Use `useAtomValue` for reads and `useSetAtom` for writes to reduce accidental subscriptions.

## Project Layout

See `architecture-notes.md` for module structure, dependency direction, and naming. Do not duplicate
that guidance here.

Atom placement follows from it:

- `feature/atoms.ts` — primary atoms, action atoms, derived atoms. It sits at the feature root beside
  `index.ts`, not inside `components/` or `hooks/`. Split into `atoms/` with an `index.ts` only once it
  holds distinct state concerns.
- Components in `feature/components/` read atoms directly where practical.
- Orchestration that reacts to external events and writes atoms belongs in `feature/hooks/`.

## When NOT to Use Atoms

- Purely presentational local toggles that never leave a component.
- One-off temporary values with no cross-component relevance.
- Expensive values better memoized from props inside one component.
- Ephemeral typing state — keep as `useState` for in-progress input UX.
