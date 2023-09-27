// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { Message } from 'vscode-jsonrpc';

let interop: any; /* eslint-disable-line */

export function initializeInterop(self: any): Promise<boolean> { /* eslint-disable */ /* eslint-disable-line */
  return new Promise<boolean>((resolve, reject) => {
    self['LspInitialized'] = (newInterop: any) => {
      interop = newInterop;
      resolve(true);
    }

    // this is necessary to invoke the Blazor startup code - do not remove it!
    const s = document.createElement('script');
    s.setAttribute('src', '_framework/blazor.webassembly.js');
    document.body.appendChild(s);
  });
}

export async function sendLspData(message: Message) {
  const messageString = JSON.stringify(message);
  const lspData = `Content-Length: ${messageString.length}\r\n\r\n${messageString}`;

  return await interop.invokeMethodAsync('SendLspDataAsync', lspData);
}

export function onLspData(callback: (message: Message) => void) {
  (self as any)['ReceiveLspDataAsync'] = async (lspData: string) => {
    while (lspData.length > 0) {
      const headerSplitIndex = lspData.indexOf('\r\n\r\n');
      const header = lspData.substring(0, headerSplitIndex);
      const contentLengthMatch = header.match(/^Content-Length: (?<length>[0-9]+)$/);
      const messageLength = parseInt(contentLengthMatch.groups['length']);
      const messageEnd = headerSplitIndex + 4 + messageLength;

      const messageString = lspData.substring(headerSplitIndex + 4, messageEnd);
      const message: Message = JSON.parse(messageString);

      callback(message);
      lspData = lspData.substring(messageEnd);
    }
  }
}

export function decompile(jsonContent: string): string {
  const { bicepFile, error } = interop.invokeMethod('Decompile', jsonContent);

  if (error) {
    throw error;
  }

  return bicepFile;
}
