// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { BaseLanguageClient, CloseAction, Disposable, ErrorAction } from 'vscode-languageclient';
import { MonacoLanguageClient } from 'monaco-languageclient';
import { AbstractMessageReader, AbstractMessageWriter, DataCallback, Message, MessageReader, MessageWriter } from 'vscode-jsonrpc';
import { onLspData, sendLspData } from './lspInterop';
import * as monaco from 'monaco-editor'

class LspMessageReader extends AbstractMessageReader implements MessageReader {
  listen(callback: DataCallback): Disposable {
    onLspData(data => callback(data));
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    return Disposable.create(() => {});
  }
}

class LspMessageWriter extends AbstractMessageWriter implements MessageWriter {
  async write(msg: Message): Promise<void> {
    sendLspData(msg);
  }
  // eslint-disable-next-line @typescript-eslint/no-empty-function
  end(): void { }
}

function createStream() {
  const reader = new LspMessageReader();
  const writer = new LspMessageWriter();

  return [reader, writer] as const;
}

export function createLanguageClient(): BaseLanguageClient {
  monaco.languages.register({
    id: 'bicep',
    extensions: ['.bicep'],
    aliases: ['bicep'],
    configuration: require('../../textmate/language-configuration.json'),
  });

  const [reader, writer] = createStream();

  const client = new MonacoLanguageClient({
    name: "Bicep Monaco Client",
    clientOptions: {
      documentSelector: [{ language: 'bicep' }],
      errorHandler: {
        error: () => ({ action: ErrorAction.Continue }),
        closed: () => ({ action: CloseAction.DoNotRestart }),
      }
    },
    connectionProvider: {
      get: () => Promise.resolve({ reader, writer }),
    }
  });

  return client;
}
