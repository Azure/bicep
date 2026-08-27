# Bicep Playground

The Bicep Playground runs the Bicep compiler in a .NET WebAssembly worker and
uses Monaco for the Bicep and generated ARM template editors.

## Prerequisites

- The .NET SDK pinned by [`../../global.json`](../../global.json).
- Node.js and npm.
- The .NET WebAssembly tools workload:

  ```powershell
  dotnet workload install wasm-tools
  ```

The workload is required because the playground development and production
builds explicitly enable native runtime relinking when publishing `Bicep.Wasm`.
It is not required for normal solution builds, packs, or tests.

Install the npm dependencies before running the playground:

```powershell
npm ci
```

## Development

Build and stage the optimized WASM assets, then start the Vite development
server:

```powershell
npm run dev
```

Create a production build:

```powershell
npm run build
```

Run the checks:

```powershell
npm run lint
npx tsc -b
npm run test:e2e
```

## Production build guardrails

The production build runs [`scripts/check-build.mjs`](./scripts/check-build.mjs)
to detect accidental startup-size regressions. It verifies:

- The initial JavaScript bundle and native .NET WASM runtime remain within
  their size budgets.
- Only the explicitly supported compiler and Monaco workers are emitted.
- The optimized .NET WASM runtime is present.
- Invariant globalization remains enabled and ICU assets are not emitted.

The budgets and worker allowlist are guardrails rather than permanent limits.
When a new language or editor feature intentionally requires another worker or
increases startup size, update the corresponding limit or allowlist in the same
pull request and document the measured impact. This keeps intentional growth
possible while preventing unused Monaco features or unoptimized WASM assets
from returning unnoticed.
