// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import {
  env,
  TextDocumentChangeEvent,
  TextDocumentChangeReason,
  Uri,
  window,
  workspace,
  WorkspaceEdit,
  Range,
  MessageItem,
  ConfigurationTarget,
  ProgressLocation,
} from "vscode";
import { Command } from "./types";
import { LanguageClient } from "vscode-languageclient/node";
import { findOrCreateActiveBicepFile } from "./findOrCreateActiveBicepFile";
import {
  callWithTelemetryAndErrorHandling,
  IActionContext,
} from "@microsoft/vscode-azext-utils";
import { areEqualIgnoringWhitespace } from "../utils/areEqualIgnoringWhitespace";
import { getTextAfterFormattingChanges } from "../utils/getTextAfterFormattingChanges";
import { bicepConfigurationKeys, bicepLanguageId } from "../language/constants";
import {
  BicepDecompileForPasteCommandParams,
  BicepDecompileForPasteCommandResult,
} from "../language";
import { OutputChannelManager } from "../utils/OutputChannelManager";
import { getBicepConfiguration } from "../language/getBicepConfiguration";
import { SuppressedWarningsManager } from "./SuppressedWarningsManager";
import { Disposable } from "../utils/disposable";
import { isEmptyOrWhitespace } from "../utils/isEmptyOrWhitespace";
import { withProgressAfterDelay } from "../utils/withProgressAfterDelay";

export class PasteAsBicepCommand implements Command {
  public readonly id = "bicep.pasteAsBicep";
  public disclaimerShownThisSession = false;

  public constructor(
    private readonly client: LanguageClient,
    private readonly outputChannelManager: OutputChannelManager,
    private readonly suppressedWarningsManager: SuppressedWarningsManager
  ) {
    // Nothing to do
  }

  public async execute(
    context: IActionContext,
    documentUri?: Uri
  ): Promise<void> {
    documentUri = await findOrCreateActiveBicepFile(
      context,
      documentUri,
      "Choose which Bicep file to paste into"
    );

    const document = await workspace.openTextDocument(documentUri);
    const editor = await window.showTextDocument(document);
    const clipboardText = await env.clipboard.readText();

    if (editor?.document.languageId !== bicepLanguageId) {
      return;
    }

    const result = await this.callDecompileForPaste(
      clipboardText,
      false /* queryCanPaste */
    );

    if (!result.pasteType) {
      throw new Error(
        `The clipboard text does not appear to be valid JSON or is not in a format that can be pasted as Bicep.`
      );
    }

    if (result.errorMessage) {
      context.errorHandling.issueProperties.clipboardText = clipboardText;
      throw new Error(
        `Could not paste clipboard text as Bicep: ${result.errorMessage}`
      );
    }

    await editor.edit((builder) => {
      builder.insert(editor.selection.active, result.bicep ?? "");
    });
  }

  public registerForPasteEvents(extension: Disposable) {
    extension.register(
      workspace.onDidChangeTextDocument(this.onDidChangeTextDocument.bind(this))
    );
  }

  private async callDecompileForPaste(
    jsonContent: string,
    queryCanPaste: boolean
  ): Promise<BicepDecompileForPasteCommandResult> {
    return await withProgressAfterDelay(
      {
        location: ProgressLocation.Notification,
        title:
          "Decompiling clipboard text into Bicep is taking longer than expected...",
      },
      async () => {
        const decompileParams: BicepDecompileForPasteCommandParams = {
          jsonContent,
          queryCanPaste,
        };
        const decompileResult: BicepDecompileForPasteCommandResult =
          await this.client.sendRequest("workspace/executeCommand", {
            command: "decompileForPaste",
            arguments: [decompileParams],
          });

        return decompileResult;
      }
    );
  }

  public isExperimentalPasteAsBicepEnabled(): boolean {
    return (
      getBicepConfiguration().get<boolean>(
        bicepConfigurationKeys.experimentalEnablePasteOnBicep
      ) ?? false
    );
  }

  private isAutoConvertOnPasteEnabled(): boolean {
    return (
      getBicepConfiguration().get<boolean>(
        bicepConfigurationKeys.decompileOnPaste
      ) ?? true
    );
  }

  // Handle automatically converting to Bicep on paste
  private async onDidChangeTextDocument(
    e: TextDocumentChangeEvent
  ): Promise<void> {
    await callWithTelemetryAndErrorHandling(
      "autoPasteAsBicep",
      async (context) => {
        context.telemetry.suppressIfSuccessful = true;

        if (!this.isExperimentalPasteAsBicepEnabled()) {
          return;
        }
        if (!this.isAutoConvertOnPasteEnabled()) {
          return;
        }

        if (
          e.reason !== TextDocumentChangeReason.Redo &&
          e.reason !== TextDocumentChangeReason.Undo &&
          e.document === window.activeTextEditor?.document &&
          e.document.languageId === bicepLanguageId &&
          e.contentChanges.length === 1
        ) {
          const contentChange = e.contentChanges[0];

          // Ignore deletions and trivial changes
          if (
            contentChange.text.length < 2 ||
            isEmptyOrWhitespace(contentChange.text)
          ) {
            return;
          }

          const clipboardText = await env.clipboard.readText();

          // This edit was a paste if the clipboard text matches the inserted text (ignoring formatting)
          if (
            clipboardText.length > 1 &&
            !isEmptyOrWhitespace(clipboardText) && // ... non-trivial changes only
            areEqualIgnoringWhitespace(clipboardText, contentChange.text)
          ) {
            // See if we can paste this text as Bicep
            const canPasteResult = await this.callDecompileForPaste(
              clipboardText,
              true // queryCanPaste
            );
            if (!canPasteResult.pasteType) {
              // Nothing we know how to convert
              return;
            }

            context.telemetry.suppressIfSuccessful = false;
            context.telemetry.properties.pasteType = canPasteResult.pasteType;
            context.telemetry.properties.decompileId =
              canPasteResult.decompileId;
            context.telemetry.properties.jsonSize = String(
              clipboardText.length
            );

            if (canPasteResult.pasteType === "bicepValue") {
              // If the input was already a valid Bicep expression (i.e., the conversion looks the same as the original, once formatting
              //   changes are ignored), then skip the conversion, otherwise the user will see formatting changes when copying Bicep values
              //   to Bicep (e.g. [1] would get changed to a multi-line array).
              // This will mainly happen with single-line arrays and objects, especially since the Newtonsoft parser accepts input that is
              //   JavaScript but not technically JSON, such as '{ abc: 1, def: 'def' }, but which also happens to be valid Bicep.
              return;
            }

            if (canPasteResult.errorMessage || !canPasteResult.bicep) {
              // If we should be able to convert but there were errors in the JSON, show a message to the output window
              this.outputChannelManager.appendToOutputChannel(
                canPasteResult.output
              );
              this.outputChannelManager.appendToOutputChannel(
                `Could not convert pasted text into Bicep: ${canPasteResult.errorMessage}`
              );

              // ... and register telemetry for the failure (don't show the error to the user again)
              context.telemetry.maskEntireErrorMessage = true;
              context.errorHandling.suppressDisplay = true;
              throw new Error("Decompile error");
            }

            context.telemetry.properties.bicepSize = String(
              canPasteResult.bicep.length
            );

            // While we were awaiting async calls, the pasted text may have been formatted in the editor.
            // Need to figure out the length of the formatted text so we can properly replace it.
            const formattedPastedText = getTextAfterFormattingChanges(
              contentChange.text,
              e.document.getText(),
              contentChange.rangeOffset
            );
            if (
              !formattedPastedText ||
              !areEqualIgnoringWhitespace(formattedPastedText, clipboardText)
            ) {
              // Some other editor change must have happened, abort the conversion to Bicep
              context.errorHandling.suppressDisplay = true;
              throw new Error("Editor changed");
            }

            // All systems go - replace pasted JSON with Bicep
            const edit = new WorkspaceEdit();
            const rangeOfFormattedPastedText = new Range(
              e.document.positionAt(contentChange.rangeOffset),
              e.document.positionAt(
                contentChange.rangeOffset + formattedPastedText.length
              )
            );
            edit.replace(
              e.document.uri,
              rangeOfFormattedPastedText,
              canPasteResult.bicep
            );
            const success = await workspace.applyEdit(edit);
            if (!success) {
              throw new Error(
                "Applying edit failed while converting pasted JSON to Bicep"
              );
            }

            // Don't wait for disclaimer/warning because our telemetry won't fire until we return
            // eslint-disable-next-line @typescript-eslint/no-floating-promises
            this.showWarning(context, canPasteResult);
          }
        }
      }
    );
  }

  private async showWarning(
    context: IActionContext,
    pasteResult: BicepDecompileForPasteCommandResult
  ): Promise<void> {
    try {
      // Always show this message
      this.outputChannelManager.appendToOutputChannel(
        "The JSON pasted into the editor was automatically decompiled to Bicep. Use undo to revert.",
        true /*noFocus*/
      );

      if (!pasteResult.disclaimer) {
        return;
      }

      // Show disclaimer only once per session
      if (this.disclaimerShownThisSession) {
        return;
      }

      // Always show disclaimer in output window
      this.outputChannelManager.appendToOutputChannel(
        pasteResult.disclaimer,
        true /*noFocus*/
      );

      // Show disclaimer in a dialog until disabled
      if (
        this.suppressedWarningsManager.isWarningSuppressed(
          SuppressedWarningsManager.keys.decompileOnPasteWarning
        )
      ) {
        return;
      }

      const dontShowAgain: MessageItem = {
        title: "Never show again",
      };
      const disable: MessageItem = {
        title: "Disable automatic decompile on paste",
      };

      const result = await context.ui.showWarningMessage(
        pasteResult.disclaimer,
        dontShowAgain,
        disable
      );
      if (result === dontShowAgain) {
        await this.suppressedWarningsManager.suppressWarning(
          SuppressedWarningsManager.keys.decompileOnPasteWarning
        );
      } else if (result === disable) {
        await getBicepConfiguration().update(
          bicepConfigurationKeys.decompileOnPaste,
          false,
          ConfigurationTarget.Global
        );

        // Don't wait for this to finish
        window.showWarningMessage(
          `Automatic decompile on paste has been disabled. You can turn it back on at any time from VS Code settings (${SuppressedWarningsManager.keys.decompileOnPasteWarning}). You can also still use the "Paste as Bicep" command from the command palette.`
        );
      }
    } finally {
      this.disclaimerShownThisSession = true;
    }
  }
}
