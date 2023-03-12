// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { editor, languages } from 'monaco-editor/esm/vs/editor/editor.api';

let interop: any; /* eslint-disable-line */

export function initializeInterop(self: any): Promise<boolean> { /* eslint-disable */ /* eslint-disable-line */
  return new Promise<boolean>((resolve, reject) => {
    self['BicepInitialize'] = (newInterop: any) => {
      interop = newInterop;
      resolve(true);
    }

    // this is necessary to invoke the Blazor startup code - do not remove it!
    const test = require('../../Bicep.Wasm/bin/Release/net7.0/wwwroot/_framework/blazor.webassembly.js');
  });
}

export function getSemanticTokensLegend(): languages.SemanticTokensLegend {
  return interop.invokeMethod('GetSemanticTokensLegend');
}

export async function getSemanticTokens(content: string): Promise<languages.SemanticTokens> {
  return await interop.invokeMethodAsync('GetSemanticTokens', content);
}

export async function compileAndEmitDiagnostics(content: string): Promise<{template: string, diagnostics: editor.IMarkerData[]}> {
  return await interop.invokeMethodAsync('CompileAndEmitDiagnostics', content);
}

export async function decompile(jsonContent: string): Promise<string> {
  const { bicepFile, error } = await interop.invokeMethodAsync('Decompile', jsonContent);

  if (error) {
    throw error;
  }

  return bicepFile;
}
