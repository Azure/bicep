// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { commands, window } from "vscode";
import { DecompileCommand } from "./commands/decompile";

export async function updateUiContext(): Promise<void> {
  let cannotDecompile = false; // Only disable if the current editor is JSON but not an ARM template.
  const currentDocument = window.activeTextEditor?.document;

  if (currentDocument) {
    switch (currentDocument.languageId) {
      case "arm-template":
      case "json":
      case "jsonc":
        cannotDecompile = !(await DecompileCommand.mightBeArmTemplate(
          currentDocument.uri
        ));
        break;
    }
  }

  commands.executeCommand(
    "setContext",
    "bicep.cannotDecompile",
    cannotDecompile
  );
}
