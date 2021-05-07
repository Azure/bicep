// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
jest.mock(
  "vscode",
  () => ({
    languages: {
      createDiagnosticCollection: jest.fn(),
      registerCodeLensProvider: jest.fn(),
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
