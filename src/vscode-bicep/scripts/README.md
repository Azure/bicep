# Development Scripts

Run these scripts from any working directory; paths are resolved relative to each script.

## Set up development dependencies

```sh
node ./scripts/setup-development.mjs
```

Installs extension and UI dependencies, builds the UI, and builds the local language and MCP servers.

## Run E2E tests

```sh
node ./scripts/run-e2e-tests.mjs
```

Runs development setup, builds the extension and E2E bootstrap, then tests against minimum and stable VS Code.

## Build a local VSIX

```sh
node ./scripts/build-local-vsix.mjs
```

Runs development setup, copies the local Debug server outputs into the extension, and creates the VSIX and manifest.
