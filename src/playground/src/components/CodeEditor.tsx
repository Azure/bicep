// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as monaco from 'monaco-editor';
import React, { createRef, useEffect, useState } from 'react';
import { DotnetInterop } from '../utils/interop';

interface Props {
  options: monaco.editor.IStandaloneEditorConstructionOptions,
  initialContent: string,
  onContentChange?: (model: monaco.editor.ITextModel, content: string) => void,
}

export const CodeEditor : React.FC<Props> = (props) => {
  const { options, initialContent, onContentChange } = props;
  const editorRef = createRef<HTMLDivElement>();
  const [model, setModel] = useState<monaco.editor.ITextModel>();

  useEffect(() => {
    async function initializeEditor() {
      const editor = monaco.editor.create(editorRef.current!, options);
      const model = editor.getModel()!;
  
      editor.onDidChangeModelContent(async () => {
        if (onContentChange) {
          const text = model.getValue();

          onContentChange(model, text);
        }
      });

      setModel(model);
    }

    initializeEditor();
  }, []);

  useEffect(() => {
    model?.setValue(initialContent);
  }, [initialContent, model]);

  return (
    <div ref={editorRef} style={{height: '100%', width: '100%'}} />
  );
};

export function registerBicep(interop: DotnetInterop) {
  monaco.languages.register({
    id: 'bicep',
    extensions: ['.bicep'],
    aliases: ['bicep'],
  });

  monaco.languages.registerDocumentSemanticTokensProvider('bicep', {
    getLegend: () => interop.getSemanticTokensLegend(),
    provideDocumentSemanticTokens: async (model) => await interop.getSemanticTokens(model.getValue()),
    releaseDocumentSemanticTokens: () => { return; }
  });
}