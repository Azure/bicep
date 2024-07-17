// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// WARNING: The unit tests run in parallel, so this mock is a singleton and shared among all the unit tests.
// So don't use things like expect().toHaveBeenCalledTimes() in the unit tests, because they will interfere with each other.
//
// (This is not the case for the e2e tests, which run sequentially.)
jest.mock(
  "vscode",
  () => ({
    $$_this_is_a_mock_$$: "see vscode/src/test/unit/setup.ts",
    CancellationError: Error,
    ConfigurationTarget: {
      Global: 1,
      Workspace: 2,
      WorkspaceFolder: 3,
    },
    languages: {
      createDiagnosticCollection: jest.fn(() => throwNYI()),
      registerCodeLensProvider: jest.fn(() => throwNYI()),
    },
    ProgressLocation: {
      Notification: 15,
      SourceControl: 1,
      Window: 10,
    },
    StatusBarAlignment: { Left: 1, Right: 2 },
    ThemeColor: jest.fn(),
    ThemeIcon: jest.fn(),
    window: {
      createStatusBarItem: jest.fn(() => ({
        show: jest.fn(() => throwNYI()),
        tooltip: jest.fn(() => throwNYI()),
      })),
      showErrorMessage: jest.fn(() => throwNYI()),
      showWarningMessage: jest.fn(() => throwNYI()),
      createTextEditorDecorationType: jest.fn(() => throwNYI()),
      createOutputChannel: jest.fn(() => throwNYI()),
      showWorkspaceFolderPick: jest.fn(() => throwNYI()),
      onDidChangeActiveTextEditor: jest.fn(() => throwNYI()),
      showInformationMessage: jest.fn(() => throwNYI()),
    },
    workspace: {
      getConfiguration: jest.fn(() => throwNYI()),
      workspaceFolders: [],
      getWorkspaceFolder: jest.fn(() => throwNYI()),
      onDidChangeConfiguration: jest.fn(() => throwNYI()),
      onDidChangeTextDocument: jest.fn(() => throwNYI()),
      onDidChangeWorkspaceFolders: jest.fn(() => throwNYI()),
    },
    OverviewRulerLane: {
      Left: null,
    },
    Uri: {
      file: jest.fn(() => throwNYI()),
      parse: jest.fn(() => throwNYI()),
    },
    Range: jest.fn(() => throwNYI()),
    Diagnostic: jest.fn(() => throwNYI()),
    DiagnosticSeverity: { Error: 0, Warning: 1, Information: 2, Hint: 3 },
    debug: {
      onDidTerminateDebugSession: jest.fn(() => throwNYI()),
      startDebugging: jest.fn(() => throwNYI()),
      registerDebugConfigurationProvider: jest.fn(() => throwNYI()),
    },
    commands: {
      executeCommand: jest.fn(() => throwNYI()),
      registerCommand: jest.fn(() => throwNYI()),
    },
    l10n: {
      t: jest.fn(),
    },
  }),
  { virtual: true },
);

function throwNYI(): void {
  throw new Error("Mock not implemented.");
}
