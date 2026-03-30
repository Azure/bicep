# Visual Designer State Management Guidelines

Use Jotai as the default for shared feature state. Keep component-local state only for transient UI input that should not be shared.

## Core rules

1. Co-locate atoms with the feature they belong to.
2. Export feature atoms through the feature `index.ts` barrel.
3. Prefer small atoms over one large object atom.
4. Use derived atoms for view intent (for example: `isExportCanvasCoverVisibleAtom`).
5. Keep imperative writes in action atoms (`open*`, `close*`, `reset*`) when the action touches multiple atoms.
6. Use `useAtomValue` for reads and `useSetAtom` for writes to reduce accidental subscriptions.
7. Keep ephemeral typing state local (`useState`) only when needed for in-progress input UX.

## Recommended layout

Core libraries live under `src/lib/` (`graph/`, `messaging/`, `theming/`, `utils/`).
User-facing feature slices live under `src/features/` (`control/`, `export/`, `layout/`, `status/`, `visualization/`, `devtools/`).

- `feature/atoms.ts`: primary atoms, action atoms, derived atoms.
- `feature/components/*`: use atoms directly where practical.
- `feature/hooks/*`: orchestration logic that reacts to external events and writes atoms.

## Patterns in this app

1. Export flow:

- Source of truth in `features/export/atoms.ts`.
- UI visibility via derived atoms.
- Export execution reads atoms directly in toolbar.

2. Theming:

- VS Code body theme is observed once via `theming/atoms.ts` (`activeThemeAtom.onMount`).
- Consumers use `useTheme()` backed by the shared atom.

3. Status (`features/status/`):

- Owns graph metadata atoms (`errorCountAtom`, `hasNodesAtom`) written by the messaging layer.
- `graphStatusAtom` derives semantic status (`errors | empty | ready`).
- `StatusBar` component lives here as it renders diagnostic status.
- Kept separate from `graph/`, which only handles rendering and geometry.

4. Control (`features/controls/`):

- `graphControlAvailabilityAtom` derives which controls are actionable from `hasNodesAtom`.
- `ControlBar` component lives here as it orchestrates graph interactions.
- Depends on `status` (availability), `export` (open overlay), `graph/` (fit view), `layout` (reset).

## When NOT to use atoms

1. Purely presentational local toggles that never leave a component.
2. One-off temporary values with no cross-component relevance.
3. Expensive values better memoized from props inside one component.
