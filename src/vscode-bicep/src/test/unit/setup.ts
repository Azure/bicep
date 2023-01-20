// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// WARNING: The unit tests run in parallel, so this mock is a singleton and shared among all the unit tests.
// (This is not the case for the e2e tests, which run sequentially.)
jest.mock(
  "vscode",
  () => ({
    $$_this_is_a_mock_$$: "see vscode/src/test/unit/setup.ts",
    ConfigurationTarget: {
      Global: 1,
      Workspace: 2,
      WorkspaceFolder: 3,
    },
    languages: {
      createDiagnosticCollection: jest.fn(),
      registerCodeLensProvider: jest.fn(),
    },
    ProgressLocation: {
      SourceControl: 1,
      Window: 10,
      Notification: 15,
    },
    StatusBarAlignment: { Left: 1, Right: 2 },
    window: {
      createStatusBarItem: jest.fn(() => ({
        show: jest.fn(),
        tooltip: jest.fn(),
      })),
      showErrorMessage: jest.fn(),
      showWarningMessage: jest.fn(),
      createTextEditorDecorationType: jest.fn(),
      createOutputChannel: jest.fn(),
      showWorkspaceFolderPick: jest.fn(),
      onDidChangeActiveTextEditor: jest.fn(),
      showInformationMessage: jest.fn(),
    },
    workspace: {
      getConfiguration: jest.fn(),
      workspaceFolders: [],
      getWorkspaceFolder: jest.fn(),
      onDidChangeConfiguration: jest.fn(),
      onDidChangeTextDocument: jest.fn(),
      onDidChangeWorkspaceFolders: jest.fn(),
    },
    OverviewRulerLane: {
      Left: null,
    },
    Uri: {
      file: jest.fn(),
      parse: jest.fn(),
    },
    Range: jest.fn(),
    Diagnostic: jest.fn(),
    DiagnosticSeverity: { Error: 0, Warning: 1, Information: 2, Hint: 3 },
    debug: {
      onDidTerminateDebugSession: jest.fn(),
      startDebugging: jest.fn(),
      registerDebugConfigurationProvider: jest.fn(),
    },
    commands: {
      executeCommand: jest.fn(),
      registerCommand: jest.fn(),
    },
    CodeLen: jest.fn(),
  }),
  { virtual: true }
);
