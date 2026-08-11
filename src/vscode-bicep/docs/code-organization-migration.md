# VS Code Extension Code Organization Migration

## Status

Proposed.

## Motivation

The extension is currently organized primarily by implementation type. Commands are placed in
`src/commands`, language client code is placed in `src/language`, webviews are split between
`src/panes` and `src/visualizer`, and unrelated code accumulates in `src/utils`. A single user
workflow therefore spans several top-level folders.

For example, deployment currently spans:

- `src/commands/deploy.ts`
- `src/commands/deployHelper.ts`
- `src/commands/showDeployPane.ts`
- `src/panes/deploy`
- `src/azure`
- `src/utils/AzurePickers.ts`
- deployment contracts in `src/language/protocol.ts`
- deployment notification registration in `src/language/client.ts`
- deployment state in `src/globalState.ts`

This makes ownership unclear, encourages dependencies in the wrong direction, and makes a feature
hard to understand or change without searching the entire extension.

`Bicep.LangServer` recently moved from type-oriented folders to protocol-feature folders. The VS
Code extension should follow the same ownership principle while using a structure appropriate for
a TypeScript VS Code client:

- Organize product behavior by user-visible feature.
- Keep handlers, commands, protocol contracts, state, and feature-only support together.
- Keep only genuinely cross-feature mechanisms outside `features`.
- Prefer cohesive modules over one-class-per-file or one-function-per-file layouts.

This migration is intentionally limited to code organization and the small architectural repairs
required to establish correct boundaries. It does not change command IDs, context keys, protocol
method names, persisted state keys, webview view types, or user-visible behavior.

## Goals

1. Make a feature discoverable from one top-level folder.
2. Make dependency direction explicit and enforceable.
3. Remove `commands`, `language`, `panes`, and `utils` as miscellaneous ownership buckets.
4. Co-locate unit tests with the modules they test.
5. Use kebab-case for all TypeScript file and directory names.
6. Merge trivially fragmented modules when they implement one cohesive concept.
7. Split monolithic modules when their contents belong to different features.
8. Complete the migration in three reviewable PRs, with a two-PR fallback if necessary.

## Non-goals

- Rewriting command implementations.
- Changing public command IDs or package contributions.
- Changing language server protocol payloads or method names.
- Introducing a dependency injection framework.
- Creating an interface for every implementation.
- Reorganizing the behavior of the E2E runner while the test-runner work is in progress.
- Renaming files outside `src` solely for consistency.

## Organization Rules

### Features own behavior

Code belongs under `features/<feature>` when it implements a user workflow or Bicep-specific
capability. Transport does not determine ownership. For example, a command implemented through
`workspace/executeCommand` still belongs to `features/build`, not a generic commands folder.

Feature folders may contain commands, protocol contracts, state, webviews, and feature-only support.
They should remain shallow. Add a child folder only when a feature has a cohesive multi-file
subdomain, such as `deployments/azure` or `deployments/pane`.

### Infrastructure owns mechanisms

Code belongs under `infrastructure` when it implements a cross-feature extension mechanism rather
than product behavior. Examples include command registration, lifecycle ownership, language client
startup, diagnostics dispatch, logging, telemetry, and output channels.

Infrastructure must not import from `features`.

### Avoid generic buckets

Do not introduce generic `types.ts`, `models.ts`, `helpers.ts`, or `common.ts` files.
Types should live with the behavior that gives them meaning. A separate type module is justified
only when the types form a substantial, independently understandable contract.

### Use barrels as public APIs

Every feature and infrastructure domain exposes its public API through an `index.ts` barrel. Code
outside that folder imports from the barrel rather than reaching into implementation modules. This
keeps imports concise and makes each domain's supported surface explicit.

Barrels must contain named re-exports only. Do not add side effects, default exports, wildcard
exports, or implementation logic. Internal modules import siblings directly instead of importing
their own barrel, which avoids self-referential cycles. Do not add a root `features/index.ts` or
`infrastructure/index.ts` that flattens unrelated domains and obscures ownership.

For example:

```ts
// extension.ts
import { activateBuildFeature } from "./features/build";

// features/build/index.ts
export { activateBuildFeature, BuildCommand, BuildParamsCommand } from "./commands";
```

### Avoid one-function utility files

A module should represent a cohesive concept, not satisfy a one-export-per-file rule. Related small
functions should be merged into a meaningful module. Feature-specific operations remain inside the
feature even when they are pure functions.

The final `utils` folder, if one remains at all, must contain cohesive sets of generic pure
operations used by multiple features. It must not contain VS Code lifecycle code, services, Azure
operations, logging, output channels, feature state, or feature-specific text processing.

During this migration, prefer eliminating a utility by placing it in its owning domain over creating
a new one-function utility module.

### Naming

All TypeScript file and directory names under `src`, including tests and test infrastructure, use
kebab-case:

- `command-manager.ts`
- `azure-ui-manager.ts`
- `paste-as-bicep.ts`
- `deploy-pane-manager.ts`
- `external-source-content.ts`

Exported TypeScript symbols retain normal PascalCase or camelCase conventions. Acronyms are treated
consistently as acronyms in symbol names. In particular:

- Rename `IAzureUiManager` to `IAzureUIManager`.
- Rename `AzureUiManager` to `AzureUIManager`.
- Name the file `azure-ui-manager.ts`.

Use a temporary filename for case-only renames on Windows so Git records the change reliably.

### Unit tests are co-located

Unit tests live beside production modules:

```text
features/paste-as-bicep/
  paste-as-bicep.ts
  paste-as-bicep.test.ts
  text-formatting.ts
  text-formatting.test.ts
```

E2E tests exercise the assembled extension rather than a single source module, so their final home
is a top-level `e2e` directory beside `src`. The E2E runner, environment, examples, and E2E-only
support code move with them. This physical move is deferred to PR 2; PR 1 only normalizes their file
names to kebab-case.

## Dependency Rules

The intended dependency direction is:

```text
extension.ts
  -> infrastructure
  -> features

features
  -> infrastructure
  -> cohesive generic utilities, when unavoidable

infrastructure
  -X-> features

feature A
  -X-> feature B
```

Feature-to-feature collaboration must happen through composition or an infrastructure mechanism,
not by importing another feature's concrete implementation. If two features need the same behavior,
first decide whether that behavior is truly infrastructure or whether the features should be one
feature. Do not automatically extract a helper.

Feature activation functions receive explicit dependencies and register their own commands, events,
notifications, and disposables:

```ts
export function activatePasteAsBicep(
  extension: Extension,
  commandManager: CommandManager,
  languageClient: LanguageClient,
  outputChannels: OutputChannels,
): void {
  // Register this feature only.
}
```

Do not introduce a service locator. A small typed composition object is acceptable only if passing
individual shared dependencies becomes materially noisy.

## Target Structure

The exact number of modules may change during implementation when existing code reveals a stronger
cohesion boundary. The ownership and dependency rules are fixed.

```text
src/
  extension.ts

  infrastructure/
    commands/
      command-manager.ts
      command-manager.test.ts
    configuration/
      extension-configuration.ts
    editor/
      bicep-documents.ts
    language-client/
      language-client.ts
      diagnostics-router.ts
      diagnostics-router.test.ts
    lifecycle/
      disposable.ts
    logging/
      logging.ts
      logging.test.ts
      output-channels.ts
      output-channels.test.ts
    timing/
      timing.ts
      timing.test.ts

  features/
    build/
      commands.ts

    configuration/
      create-configuration.ts
      protocol.ts

    decompile/
      commands.ts
      editor-context.ts

    deployments/
      azure/
        azure-clients.ts
        azure-pickers.ts
        azure-ui-manager.ts
      pane/
        deploy-pane.ts
        deploy-pane-manager.ts
        deploy-pane-state.ts
      commands.ts
      deployment-output.ts
      deployment-scope.ts
      protocol.ts

    external-source/
      commands.ts
      external-source-content.ts
      protocol.ts

    import-kubernetes-manifest/
      import-kubernetes-manifest.ts
      protocol.ts

    insert-resource/
      insert-resource.ts
      protocol.ts

    mcp/
      mcp-server-provider.ts

    module-restore/
      commands.ts

    parameters/
      commands.ts

    paste-as-bicep/
      paste-as-bicep.ts
      paste-as-bicep.test.ts
      suppressed-warnings.ts
      suppressed-warnings.test.ts
      text-formatting.ts
      text-formatting.test.ts
      protocol.ts

    refactoring/
      post-extraction.ts

    surveys/
      surveys.ts
      surveys.test.ts
      survey-state.ts

    visualization/
      commands.ts
      protocol.ts
      visualizer.ts
      visualizer-manager.ts

    walkthrough/
      commands.ts

e2e/
  examples/
  support/
  commands.ts
  environment.ts
  index.ts
  run-tests.ts
  runner.ts
  setup.ts
  test-reporter.ts
  *.test.ts
```

### Deliberate module merges

#### Command infrastructure

Merge `src/commands/types.ts` into `command-manager.ts`. The `Command` interface exists solely to
support command registration and does not warrant a separate type bucket.

#### Paste text formatting

Merge these files into `features/paste-as-bicep/text-formatting.ts`:

- `utils/areEqualIgnoringWhitespace.ts`
- `utils/getTextAfterFormattingChanges.ts`
- `utils/isEmptyOrWhitespace.ts`
- `utils/isWhitespaceChar.ts`
- `utils/removeWhitespace.ts`

Merge their existing unit tests into `text-formatting.test.ts`. These operations jointly implement
format-aware paste reconciliation and are not general extension utilities.

Move `utils/withProgressAfterDelay.ts` into the paste feature unless another production feature uses
it by migration time. Its current behavior exists to support the paste workflow.

#### Output handling

Merge the cross-feature output channel mechanism into
`infrastructure/logging/output-channels.ts`:

- `utils/AzExtOutputChannel.ts`
- `utils/OutputChannelManager.ts`

Move deployment-specific output cleanup and redaction out of that infrastructure module and into
`features/deployments/deployment-output.ts`:

- `commands/deployHelper.ts`
- `utils/removePropertiesWithPossibleUserInfo.ts`

This removes feature policy from the generic output mechanism.

#### Walkthrough commands

Merge the three small walkthrough command classes into `features/walkthrough/commands.ts`. They are
registered and maintained as one feature and do not need one file per class.

#### Build and parameter commands

Place `build.ts` and `buildParams.ts` together in `features/build/commands.ts`. Place
`generateParams.ts` in `features/parameters/commands.ts`, matching the language server's distinction
between build and parameter generation.

#### Decompile commands

Merge `decompile.ts` and `decompileParams.ts` into `features/decompile/commands.ts` if doing so keeps
the module readable. Their shared editor context behavior moves from `updateUiContext.ts` to
`features/decompile/editor-context.ts`.

### Deliberate module splits

#### Language protocol

Delete the monolithic `src/language/protocol.ts`. Move each request, response, and protocol type to
the feature that owns it:

| Protocol ownership                                     | Destination                                       |
| ------------------------------------------------------ | ------------------------------------------------- |
| Visual graph update, layout, and node source           | `features/visualization/protocol.ts`              |
| Deployment data, deployment commands, and local deploy | `features/deployments/protocol.ts`                |
| External module source                                 | `features/external-source/protocol.ts`            |
| Recommended configuration location                     | `features/configuration/protocol.ts`              |
| Decompile for paste                                    | `features/paste-as-bicep/protocol.ts`             |
| Insert resource                                        | `features/insert-resource/protocol.ts`            |
| Import Kubernetes manifest                             | `features/import-kubernetes-manifest/protocol.ts` |

Delete `src/language/index.ts`; it is a broad legacy barrel spanning unrelated responsibilities.
Each replacement feature and infrastructure domain exposes a narrow `index.ts` public API.

#### Global state

Delete the generic `src/globalState.ts` after moving ownership:

- Survey state and the synced-state registration belong to `features/surveys/survey-state.ts`.
- Deploy pane state belongs to `features/deployments/pane/deploy-pane-state.ts`.
- A small shared `Memento` test fake may remain under `src/test/fakes`.

Persisted key strings must not change during the move.

#### Language client

`infrastructure/language-client/language-client.ts` owns only language client construction, process
startup, runtime acquisition, client-wide telemetry, and client-wide completion triggering.

Deployment notification registration moves to the deployment feature. The language client must not
import deployment output code.

The MCP executable lookup and provider registration move to `features/mcp/mcp-server-provider.ts`.
Shared .NET executable acquisition remains in language client infrastructure.

#### Azure types

Delete `src/azure/types.ts` rather than recreating a generic type file:

- Move `DeploymentScope` and `DeploymentScopeType` to
  `features/deployments/deployment-scope.ts`.
- Move `IAzureUIManager` beside `AzureUIManager` in
  `features/deployments/azure/azure-ui-manager.ts`.
- Rename the interface from `IAzureUiManager` to `IAzureUIManager`.
- Rename the implementation from `AzureUiManager` to `AzureUIManager` for consistency.

Keep the interface because the deploy pane consumes a mockable Azure interaction boundary. Do not
split the interface into a separate file from its only implementation.

## Current-to-Target Ownership Map

| Current path                                           | Target ownership                                                                        |
| ------------------------------------------------------ | --------------------------------------------------------------------------------------- |
| `extension.ts`                                         | Remains the composition root; reduce it to lifecycle and feature activation             |
| `commands/commandManager.ts`, `commands/types.ts`      | `infrastructure/commands/command-manager.ts`                                            |
| `commands/findOrCreateActiveBicepFile.ts`              | `infrastructure/editor/bicep-documents.ts`                                              |
| `commands/build.ts`, `commands/buildParams.ts`         | `features/build/commands.ts`                                                            |
| `commands/generateParams.ts`                           | `features/parameters/commands.ts`                                                       |
| `commands/createConfigurationFile.ts`                  | `features/configuration/create-configuration.ts`                                        |
| `commands/decompile.ts`, `commands/decompileParams.ts` | `features/decompile/commands.ts`                                                        |
| `updateUiContext.ts`                                   | `features/decompile/editor-context.ts`                                                  |
| `commands/pasteAsBicep.ts`                             | `features/paste-as-bicep/paste-as-bicep.ts`                                             |
| `commands/SuppressedWarningsManager.ts`                | `features/paste-as-bicep/suppressed-warnings.ts`                                        |
| Paste-related text utilities                           | `features/paste-as-bicep/text-formatting.ts`                                            |
| `commands/deploy.ts`                                   | `features/deployments/commands.ts`                                                      |
| `commands/deployHelper.ts`                             | `features/deployments/deployment-output.ts`                                             |
| `commands/showDeployPane.ts`                           | `features/deployments/commands.ts`                                                      |
| `panes/deploy/view.ts`                                 | `features/deployments/pane/deploy-pane.ts`                                              |
| `panes/deploy/viewManager.ts`                          | `features/deployments/pane/deploy-pane-manager.ts`                                      |
| `panes/deploy/models.ts`, `panes/deploy/messages.ts`   | Merge into the pane module or `deploy-pane-state.ts` according to ownership             |
| `panes/deploy/index.ts`                                | Replace with the narrow `features/deployments/index.ts` public API                      |
| `azure/azureClients.ts`                                | `features/deployments/azure/azure-clients.ts`                                           |
| `azure/AzureUiManager.ts`                              | `features/deployments/azure/azure-ui-manager.ts`                                        |
| `azure/types.ts`                                       | Split between `deployment-scope.ts` and `azure-ui-manager.ts`                           |
| `utils/AzurePickers.ts`                                | `features/deployments/azure/azure-pickers.ts`                                           |
| `visualizer/view.ts`                                   | `features/visualization/visualizer.ts`                                                  |
| `visualizer/viewManager.ts`                            | `features/visualization/visualizer-manager.ts`                                          |
| Visualizer commands                                    | `features/visualization/commands.ts`                                                    |
| `visualizer/index.ts`                                  | Replace with the narrow `features/visualization/index.ts` public API                    |
| External source provider, decoder, and command         | `features/external-source`                                                              |
| `commands/insertResource.ts`                           | `features/insert-resource/insert-resource.ts`                                           |
| `commands/importKubernetesManifest.ts`                 | `features/import-kubernetes-manifest/import-kubernetes-manifest.ts`                     |
| `commands/forceModulesRestore.ts`                      | `features/module-restore/commands.ts`                                                   |
| `commands/PostExtractionCommand.ts`                    | `features/refactoring/post-extraction.ts`                                               |
| `commands/gettingStarted/*`                            | `features/walkthrough/commands.ts`                                                      |
| `feedback/surveys.ts`                                  | `features/surveys/surveys.ts`                                                           |
| `globalState.ts`                                       | Split between survey and deployment pane state                                          |
| `language/client.ts`                                   | Split between language client infrastructure, deployments, and MCP                      |
| `language/protocol.ts`                                 | Split among owning features                                                             |
| `language/constants.ts`                                | Split among editor metadata, configuration, paste, and surveys as described below       |
| `language/getBicepConfiguration.ts`                    | `infrastructure/configuration/extension-configuration.ts`                               |
| `language/index.ts`                                    | Replace with feature and infrastructure-domain barrels                                  |
| `utils/disposable.ts`                                  | `infrastructure/lifecycle/disposable.ts`                                                |
| `utils/logger.ts`, `utils/telemetry.ts`                | `infrastructure/logging/logging.ts` where cohesion permits                              |
| Output channel utilities                               | Split between logging infrastructure and deployment output policy                       |
| `utils/time.ts`                                        | `infrastructure/timing/timing.ts`                                                       |
| `utils/compareStringsOrdinal.ts`                       | Delete; use private ordinal comparators in `bicep-documents.ts` and deployment commands |

### Constants ownership

Delete `src/language/constants.ts` and move each value to the domain that owns it:

| Current value                             | Destination                                                                   |
| ----------------------------------------- | ----------------------------------------------------------------------------- |
| `bicepFileExtension`                      | `infrastructure/editor/bicep-documents.ts`                                    |
| `bicepLanguageId`                         | `infrastructure/editor/bicep-documents.ts`                                    |
| `bicepParamLanguageId`                    | `infrastructure/editor/bicep-documents.ts`                                    |
| `bicepConfigurationPrefix`                | `infrastructure/configuration/extension-configuration.ts`                     |
| `bicepConfigurationKeys.decompileOnPaste` | Private paste setting constant in `features/paste-as-bicep/paste-as-bicep.ts` |
| `bicepConfigurationKeys.enableSurveys`    | Private survey setting constant in `features/surveys/surveys.ts`              |

Language client infrastructure may import the Bicep language ID from editor infrastructure. The
paste and walkthrough features may import document language IDs and the file extension from editor
infrastructure. Feature-specific configuration key names must not be promoted back into a shared
constants module.

### Ordinal comparison

Delete `utils/compareStringsOrdinal.ts`. The function is too small to justify a shared module and is
currently used by only two domains. Keep a private ordinal path comparator in
`infrastructure/editor/bicep-documents.ts` and another private comparator in the deployment command
module. This small duplication is preferable to a dependency on a one-function utility bucket and
keeps each sort's ordering rule visible at its call site.

## Test Organization

Change the unit Jest configuration to discover co-located tests:

```js
testMatch: ["<rootDir>/src/**/*.test.ts"],
```

After E2E moves outside `src`, unit Jest discovery is naturally isolated to co-located source tests.
Retain the unit setup file under `src/test/unit/setup.ts` during the migration unless moving it is
required by Jest configuration. Move existing unit tests as follows:

| Current test                                   | Destination                                                               |
| ---------------------------------------------- | ------------------------------------------------------------------------- |
| Formatting utility tests                       | `features/paste-as-bicep/text-formatting.test.ts`                         |
| `SuppressedWarningsManager.test.ts`            | `features/paste-as-bicep/suppressed-warnings.test.ts`                     |
| `withProgressAfterDelay.test.ts`               | Paste feature or timing infrastructure, matching final ownership          |
| `surveys.unit.test.ts`                         | `features/surveys/surveys.test.ts`                                        |
| `logger.test.ts`                               | `infrastructure/logging/logging.test.ts`                                  |
| `removePropertiesWithPossibleUserInfo.test.ts` | `features/deployments/deployment-output.test.ts`                          |
| `packageJson.test.ts`                          | Keep under `src/test/unit`; it validates the assembled extension manifest |

Rename all moved tests to kebab-case. Rename all E2E test and harness files to kebab-case during PR
1, including `runTests.ts`, `testReporter.ts`, `expectedNewConfigFileContents.ts`, `testScope.ts`,
and the current camel-cased feature test files. Update script and configuration entry points in the
same commit. This is a naming-only change; runner behavior and ownership remain unchanged until the
top-level move in PR 2.

Update `coveragePathIgnorePatterns` before moving visualization. The current `/visualizer/` exclusion
is path-dependent and would silently stop applying after the move. Preserve or deliberately revise
the coverage policy rather than changing it accidentally.

## Required Architectural Repairs

These repairs are part of the migration because moving files without fixing them would preserve an
invalid dependency boundary.

### Diagnostics routing

The deploy pane manager and visualizer manager both replace
`languageClient.clientOptions.middleware.handleDiagnostics`. Each manager captures the previous
handler and clears the middleware when disposed. Behavior therefore depends on construction and
disposal order.

Introduce one `DiagnosticsRouter` in language client infrastructure. It owns the single diagnostics
middleware and lets features subscribe to diagnostic changes. Deployment and visualization register
subscriptions and dispose only their own subscriptions.

Add focused unit tests for:

- notifying multiple subscribers;
- preserving the language client's `next` callback;
- removing one subscriber without removing others;
- subscriber disposal order.

### Deployment output ownership

Remove the mutable module-global output channel manager from `deployHelper.ts`. The deployment
feature should register the `deploymentComplete` notification with an explicitly captured output
channel dependency. Notification registration must occur before the language client starts accepting
relevant notifications.

### Feature activation

Each feature should expose an activation function or cohesive activator module. `extension.ts`
creates shared infrastructure and activates features. It should not construct every individual
command and view implementation itself.

Activation must preserve serializer registration early enough for VS Code to revive existing
webviews.

## Three-PR Migration

### PR 1: Conventions, infrastructure, and low-risk features

Purpose: establish the destination structure and testing convention without touching the two large
webview features.

Changes:

1. Add organization guidance to the project documentation.
2. Update Jest to discover co-located unit tests while excluding E2E tests.
3. Move command registration to `infrastructure/commands/command-manager.ts` and merge the command
   interface into it.
4. Move `Disposable` to `infrastructure/lifecycle/disposable.ts`.
5. Move configuration access, document selection, logging, output channel infrastructure, and timing
   into cohesive infrastructure modules.
6. Co-locate existing infrastructure unit tests.
7. Move low-risk features: build, parameters, configuration, module restore, insert resource, import
   Kubernetes manifest, refactoring, surveys, and walkthrough.
8. Apply kebab-case to every file moved in this PR.
9. Rename the remaining test and E2E harness TypeScript files to kebab-case without changing runner
   behavior.
10. Introduce feature activation functions for moved features while preserving command behavior.

Avoid changing deployment, visualization, decompile, paste, and external source ownership in this
PR except for import updates required by infrastructure moves.

Exit criteria:

- No new generic type, helper, or model modules; each moved domain has a narrow public barrel.
- Moved unit tests execute from their co-located paths.
- All existing command IDs remain unchanged.
- `npm run lint`
- `npm run test:unit`
- `npm run build:prod`
- Build, build-params, generate-params, create-config, and survey E2E tests pass.

### PR 2: Language workflows and protocol ownership

Purpose: remove the generic language and utility buckets and establish feature-owned protocol
contracts.

Changes:

1. Move decompile and its editor context handling to `features/decompile`.
2. Make `paste-as-bicep` a top-level feature.
3. Merge paste formatting functions and tests into cohesive modules.
4. Move suppressed warning state and delayed progress behavior into the paste feature.
5. Move external source content, URI decoding, command handling, and protocol to
   `features/external-source`.
6. Split `language/protocol.ts` among all owning features, including protocol destinations needed by
   deployment and visualization in PR 3.
7. Split `language/constants.ts` by ownership.
8. Delete the broad `language/index.ts` and use narrow feature and infrastructure-domain barrels.
9. Separate MCP provider ownership from language client startup.
10. Reduce language client infrastructure to client-wide behavior only.
11. Move `src/test/e2e` to top-level `e2e`, including its runner, environment, examples, and
    E2E-only support code. Update TypeScript, Jest, package script, and VS Code task paths together.
12. Apply kebab-case to every file moved in this PR.

Exit criteria:

- `src/language/protocol.ts` and `src/language/index.ts` no longer exist.
- E2E tests and their runner live under top-level `e2e`, beside `src`.
- Language client infrastructure does not import any feature implementation except the temporary
  deployment notification dependency scheduled for PR 3.
- Paste formatting no longer exists under `utils`.
- `npm run lint`
- `npm run test:unit`
- `npm run build:prod`
- Decompile, paste-as-Bicep, and external-source E2E tests pass.

### PR 3: Deployment, visualization, composition, and enforcement

Purpose: migrate the highest-risk features, repair middleware ownership, and enforce the final
architecture.

Changes:

1. Move all deployment code, Azure integration, deploy pane code, state, output policy, and protocol
   contracts to `features/deployments`.
2. Rename `IAzureUiManager` to `IAzureUIManager` and `AzureUiManager` to `AzureUIManager`.
3. Split and delete `azure/types.ts`.
4. Remove the mutable deployment output channel global and register deployment notifications in the
   deployment feature.
5. Move visualization commands, views, manager, and protocol to `features/visualization`.
6. Add the diagnostics router and migrate both webview features to subscriptions.
7. Replace the legacy deployment and visualization barrels with explicit feature public APIs.
8. Complete feature activation and reduce `extension.ts` to composition and lifecycle.
9. Delete empty `commands`, `language`, `panes`, `azure`, `feedback`, `visualizer`, and `utils`
   directories.
10. Add ESLint import restrictions enforcing `infrastructure -X-> features` and preventing direct
    sibling-feature imports.
11. Update webpack path exclusions and Jest coverage paths so moved files retain existing behavior.
12. Apply kebab-case to all remaining TypeScript files touched by the migration.

Exit criteria:

- A feature can be located from one top-level folder.
- Infrastructure has no imports from `features`.
- Features have no direct imports from sibling features.
- No broad cross-domain barrel, generic type, model, helper, or one-function utility files remain
  from the old layout.
- Command IDs, context keys, protocol method names, persisted state keys, and webview view types are
  byte-for-byte unchanged.
- Webview serializers revive existing deploy and visualization panels.
- Both webviews receive diagnostics regardless of activation and disposal order.
- `npm run lint`
- `npm run test:unit`
- `npm run build:prod`
- Full E2E suite passes.
- Manually validate one authenticated deployment flow because it is not fully represented by the
  automated deploy pane tests.

## Two-PR Fallback

If three PRs create excessive import churn, combine PR 1 and PR 2. Do not combine PR 2 and PR 3:
deployment and visualization need a dedicated review because they contain middleware mutation,
webview serialization, persisted state, Azure authentication, webpack asset paths, and language
client notification registration.

The two-PR shape is therefore:

1. Infrastructure, low-risk features, language workflows, test co-location, and protocol split.
2. Deployment, visualization, diagnostics routing, final composition, and boundary enforcement.

## Deferred Package-Root Cleanup

Organization outside `src` is deliberately deferred until after this migration. A follow-up should
consider consolidating tracked package resources by purpose:

```text
resources/
  icons/
  language/
  configuration/
  walkthrough/

docs/
  assets/

scripts/
```

Candidate moves include the current `icons`, `media`, `schemas`, `syntaxes`, `vscode-snippets`,
`readme-links`, and `npm-install.*` paths. Generated language server, MCP server, E2E log, coverage,
and build output should also use an explicit ignored artifacts layout rather than accumulating at the
package root.

Keep conventional tool entry points at the package root, including `package.json`, TypeScript,
Jest, webpack, ESLint, and Prettier configuration. Resource moves must update `package.json`,
webpack copy paths, README links, `.gitignore`, and `.vscodeignore` together. They must preserve the
final VSIX paths expected by VS Code contributions and runtime server lookup.

The `docs` directory is repository-only engineering documentation and is excluded from the VSIX by
`.vscodeignore`.

## Compatibility Invariants

The migration must preserve these string contracts:

- Commands beginning with `bicep.` in `package.json` and command implementations.
- Context keys including `bicep.cannotDecompile`, `bicep.cannotDecompileParams`,
  `bicepVisualizerFocus`, and `deployPaneFocus`.
- Webview view types `bicep.visualizer` and `bicep.deployPane`.
- Global state keys `bicep.surveys.annualSurveyState` and `bicep.deployPane.configState`.
- All language server request, notification, and execute-command method names.
- Environment variables `BICEP_LANGUAGE_SERVER_PATH` and `BICEP_MCP_SERVER_PATH`.
- Packaged server and webview asset paths.

Add or retain manifest tests for command declarations and use constants local to each feature for
the contracts it owns. Moving a constant must never change its value.

## Review Guidance

Keep mechanical moves distinguishable from behavior repairs. Within each PR:

1. Move and rename a cohesive group.
2. Repair imports and co-locate its tests.
3. Run focused validation.
4. Make the smallest required boundary repair.
5. Run the PR-level validation matrix.

Do not combine unrelated implementation refactoring with a move merely because a file is already
being touched. The exceptions are the diagnostics router, deployment notification ownership, and
feature activation changes described above; those are required to make the target dependency graph
valid.

For large Git move diffs, reviewers should inspect with rename detection enabled and review in this
order:

1. Target ownership and names.
2. Import direction.
3. String contract preservation.
4. Architectural repairs.
5. Test movement and validation evidence.

## Completion Checklist

- [ ] All production TypeScript files use kebab-case.
- [ ] All feature unit tests are co-located.
- [ ] E2E tests and their runner live under top-level `e2e`, beside `src`.
- [ ] E2E tests remain isolated from unit Jest discovery.
- [ ] `IAzureUIManager` uses the required acronym casing.
- [ ] Deployment scope types are no longer in a generic Azure type bucket.
- [ ] Protocol contracts are feature-owned.
- [ ] Deployment and visualization use one diagnostics router.
- [ ] Language client infrastructure does not depend on deployment.
- [ ] Deployment output registration has no mutable module-global state.
- [ ] `extension.ts` is a small composition root.
- [ ] Old horizontal ownership folders are deleted.
- [ ] Import boundaries are enforced by ESLint.
- [ ] Unit coverage exclusions remain intentional after path changes.
- [ ] Lint, unit tests, production build, E2E tests, and manual deployment validation pass.
