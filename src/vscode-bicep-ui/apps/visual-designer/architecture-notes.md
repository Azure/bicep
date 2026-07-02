# Visual Designer Architecture Notes

This note captures the intended direction for organizing the visual designer app as it grows. It is not a rewrite plan; use it as a guide for future refactors when a touched area already needs cleanup.

## Organizing Principle

Keep the app split into three broad layers:

- `app/`: top-level composition, providers, global style, and registration of graph node renderers.
- `features/`: user-facing product surfaces and workflows.
- `lib/`: app-local foundations such as graph infrastructure, protocol code, theming, and utilities.

A file should live near the behavior it owns. Avoid moving code into shared folders only because it is visually reusable; shared folders should hold infrastructure that multiple features actually use. Introduce `ui/` only when the app has enough reusable visual primitives to justify a separate UI infrastructure layer.

## Suggested Future Shape

```text
src/
  app/
    App.tsx
    providers.tsx
    global-style.ts
    node-config.ts

  features/
    canvas/
      GraphCanvas.tsx
      fit-view.ts
      pan-zoom.ts
    nodes/
      ResourceNode.tsx
      ModuleNode.tsx
      node-data.ts
      node-styles.ts
    edges/
      StraightEdge.tsx
      CurvedEdge.tsx
      OrthogonalEdge.tsx
      edge-shapes.ts
    controls/
      ControlBar.tsx
      use-reset-layout.ts
      atoms.ts
    export/
    status/
    devtools/

  lib/
    graph/
      atoms/
      components/
      hooks/
      model.ts
      geometry.ts
    protocol/
      messages.ts
      use-graph-update.ts
      layout-invalidation.ts
      use-visual-graph.ts
    theming/
      themes.ts
      use-theme.ts
    utils/
      math/
      strings.ts

  ui/          # optional later
    primitives/
```

## Folder Roles

### `features/nodes`

Owns Bicep-specific graph node presentation, such as resource and module cards. These components know about symbolic names, resource types, module paths, collection state, error state, and Azure icons. They should stay out of generic `ui/` because they are semantic app surfaces, not primitives.

### `features/edges`

Use this if the app grows multiple edge presentations. Straight, curved, orthogonal, animated, dependency-highlighted, or error-state edges are user-facing graph visuals and fit better as a feature than as generic graph infrastructure.

Keep route math and low-level geometry helpers in `lib/graph` or `lib/utils/math`; keep the actual rendered edge shapes and edge-specific affordances in `features/edges`.

### `features/controls`

Owns toolbar actions and other control-surface behavior. Hooks that exist only to support control actions, such as reset-layout single-flight guarding, belong here.

### `features/export`, `features/status`, `features/devtools`

Keep workflow-owned UI and state in the corresponding feature folder. Do not move export preview UI, status presentation, or dev-only graph tooling into shared `ui/` unless the code becomes a reusable primitive.

### `lib/graph`

Owns graph state and rendering infrastructure that should not know Bicep semantics: nodes, edges, boxes, bounds, drag state, graph atoms, graph hooks, and generic graph components.

### `lib/protocol`

A good future home for the server-driven visual graph protocol client. The current `lib/messaging` code is not generic messaging anymore; it owns visual graph update/layout requests, patch application, and protocol invalidation rules.

### `lib/theming`

Owns theme tokens, theme objects, styled-components theme typing, and theme hooks. Keep this in `lib` unless a broader `ui/` layer is introduced and theme infrastructure moves with other visual primitives.

### `lib/utils`

Keep this boring and generic. If a helper knows about graph state, protocol patches, Bicep declarations, or visual designer workflows, it probably belongs somewhere more specific.

### `ui/`

Optional. Use this only if the app intentionally introduces a UI infrastructure layer. Good candidates are reusable visual controls such as icon buttons, tooltips, dividers, and floating panels. `ui/theming` can also make sense later, but only if theming moves alongside a broader UI primitives layer.

## Incremental Refactor Path

Prefer small moves when touching related code:

1. Rename `features/visualization` to `features/nodes`.
2. Introduce `features/edges` when adding a second edge shape or edge presentation mode.
3. Rename `lib/messaging` to `lib/protocol` once the visual graph protocol is the only messaging responsibility left there.
4. Keep `lib/theming` in place unless introducing a broader `ui/` layer.
5. Leave `lib/graph` in place until there is a clear split between graph state, graph rendering primitives, and app-specific node/edge visuals.
