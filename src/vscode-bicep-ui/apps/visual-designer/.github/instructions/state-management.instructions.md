---
applyTo: "src/**/*.{ts,tsx}"
description: "Use when working with shared state, atoms, Jotai, or state management patterns in the visual designer app."
---

# State Management (Jotai)

## Core Rules

- Use Jotai as the default for shared feature state.
- Co-locate atoms with the feature they belong to.
- Export feature atoms through the feature `index.ts` barrel.
- Prefer small atoms over one large object atom.
- Use derived atoms for view intent (e.g. `isExportCanvasCoverVisibleAtom`).
- Use action atoms (`open*`, `close*`, `reset*`) when the action touches multiple atoms.
- Use `useAtomValue` for reads and `useSetAtom` for writes to reduce accidental subscriptions.

## Project Layout

Core libraries: `src/lib/` (`graph/`, `messaging/`, `theming/`, `utils/`).
Feature slices: `src/features/` (`controls/`, `export/`, `layout/`, `status/`, `visualization/`, `devtools/`).

Per feature:

- `feature/atoms.ts` — primary atoms, action atoms, derived atoms.
- `feature/components/*` — use atoms directly where practical.
- `feature/hooks/*` — orchestration logic that reacts to external events and writes atoms.

## When NOT to Use Atoms

- Purely presentational local toggles that never leave a component.
- One-off temporary values with no cross-component relevance.
- Expensive values better memoized from props inside one component.
- Ephemeral typing state — keep as `useState` for in-progress input UX.
