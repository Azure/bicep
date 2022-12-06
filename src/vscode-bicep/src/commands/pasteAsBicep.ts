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
} from "vscode";
import { Command } from "./types";
import { LanguageClient } from "vscode-languageclient/node";
import { findOrCreateActiveBicepFile } from "./findOrCreateActiveBicepFile";
import {
  callWithTelemetryAndErrorHandling,
  IActionContext,
} from "@microsoft/vscode-azext-utils";
import { Disposable } from "../utils";
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

export class PasteAsBicepCommand implements Command {
  public static readonly id = "bicep.pasteAsBicep";
  public readonly id = PasteAsBicepCommand.id;
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
    if (result.errorMessage) {
      context.errorHandling.issueProperties.clipboardText = clipboardText;
      throw new Error(
        `Could not paste clipboard text as Bicep because of an error: ${result.errorMessage}`
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

  public async canPasteAsBicep(
    context: IActionContext,
    jsonContent: string
  ): Promise<boolean> {
    const result = await this.callDecompileForPaste(
      jsonContent,
      true /* queryCanPaste */
    );
    return !!result.pasteType;
  }

  private async callDecompileForPaste(
    jsonContent: string,
    queryCanPaste: boolean
  ): Promise<BicepDecompileForPasteCommandResult> {
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
          const clipboardText = await env.clipboard.readText();

          // This edit was a paste if the clipboard text matches the inserted text (ignoring formatting)
          if (
            clipboardText.length > 1 && // non-trivial changes only
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

            if (canPasteResult.errorMessage || !canPasteResult.bicep) {
              // If we should be able to convert but there were errors in the JSON, show a message to the output window
              this.outputChannelManager.appendToOutputChannel(
                canPasteResult.output
              );
              this.outputChannelManager.appendToOutputChannel(
                `Could not convert pasted text into Bicep because of an error: ${canPasteResult.errorMessage}`
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

            // Don't wait for disclaimer because our telemetry won't fire until we return
            // eslint-disable-next-line @typescript-eslint/no-floating-promises
            this.showDisclaimerWarningIfNeeded(context, canPasteResult);
          }
        }
      }
    );
  }

  private async showDisclaimerWarningIfNeeded(
    context: IActionContext,
    pasteResult: BicepDecompileForPasteCommandResult
  ): Promise<void> {
    if (!pasteResult.decompilationDisclaimer) {
      return;
    }

    if (
      this.disclaimerShownThisSession || 
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
      title: "Disable decompile on paste",
    };

    this.disclaimerShownThisSession = true;
    const result = await context.ui.showWarningMessage(
      pasteResult.decompilationDisclaimer,
      dontShowAgain,
      disable
    );
    if (result === dontShowAgain) {
      await this.suppressedWarningsManager.suppressWarning(
        SuppressedWarningsManager.keys.decompileOnPasteWarning
      );
    } else if (result === disable) {
      this.outputChannelManager.appendToOutputChannel(
        `Automatic decompile on paste has been disabled. You can turn it back on at any time from VS Code settings (${SuppressedWarningsManager.keys.decompileOnPasteWarning})`
      );
      await getBicepConfiguration().update(
        bicepConfigurationKeys.decompileOnPaste,
        false,
        ConfigurationTarget.Global
      );
    }
  }
}
