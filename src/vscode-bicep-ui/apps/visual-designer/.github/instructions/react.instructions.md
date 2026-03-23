---
applyTo: "src/**/*.tsx"
description: "Use when writing or modifying React components, hooks, or JSX in the visual designer app."
---

# React Best Practices

## Components

- Use function components exclusively. No class components.
- Export one primary component per file. Co-located helpers are fine.
- Prefer named exports over default exports.
- Add `aria-label` and `title` to interactive elements (buttons, inputs).

## Hooks

- Wrap event handlers and callbacks passed to children in `useCallback`.
- Use `useMemo` only for genuinely expensive computations — don't over-memoize.
- Extract shared logic into custom hooks (`use-*.ts`) co-located with the feature.
- Keep hooks side-effect free during render; effects belong in `useEffect`.

## Props & Types

- Use `type` over `interface` for component props unless extending.
- Import types using `import type` to enable proper tree-shaking.
- Colocate types with the code that uses them; only extract to a `types.ts` when shared across files.

## Performance

- Avoid creating objects or arrays inline in JSX props (causes unnecessary re-renders).
- Prefer `useAtomValue` / `useSetAtom` (Jotai) over `useAtom` to minimize subscriptions.
- Wrap expensive subtrees in `<Suspense>` with a fallback when using async atoms.

## File & Folder Naming

- **Components**: PascalCase (`ControlBar.tsx`, `ExportOverlay.tsx`).
- **Non-components** (hooks, utils, atoms, types): kebab-case (`use-fit-view.ts`, `capture-element.ts`, `atoms.ts`).
- **Folders**: kebab-case (`pan-zoom/`, `export/`, `math/`).

## File Organization

- Copyright header: `// Copyright (c) Microsoft Corporation.` + `// Licensed under the MIT License.`
- Imports: type imports first, then library imports, then local imports (handled by Prettier plugin).
- Styled components above the component function.
- Component function at the bottom of the file, exported.
