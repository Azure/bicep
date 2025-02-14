// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as monaco from 'monaco-editor';
import React, { createRef, useEffect, useState } from 'react';
import { DotnetInterop } from '../utils/interop';
import { useColorMode } from '../utils/colorModes';

interface Props {
  options: monaco.editor.IStandaloneEditorConstructionOptions,
  initialContent: string,
  onContentChange?: (model: monaco.editor.ITextModel, content: string) => void,
}

export const CodeEditor : React.FC<Props> = (props) => {
  const { options, initialContent, onContentChange } = props;
  const editorRef = createRef<HTMLDivElement>();
  const [model, setModel] = useState<monaco.editor.ITextModel>();
  const [editor, setEditor] = useState<monaco.editor.IStandaloneCodeEditor>();
  const colorMode = useColorMode();

  useEffect(() => {
    async function initializeEditor() {
      const editor = monaco.editor.create(editorRef.current!, {
        ...options,
        theme: colorMode === 'dark' ? 'vs-dark' : 'vs',
      });
      const model = editor.getModel()!;
  
      editor.onDidChangeModelContent(async () => {
        if (onContentChange) {
          const text = model.getValue();

          onContentChange(model, text);
        }
      });

      setEditor(editor);
      setModel(model);
    }

    initializeEditor();
  }, []);

  useEffect(() => {
    model?.setValue(initialContent);
  }, [initialContent, model]);

  useEffect(() => {
    editor?.updateOptions({
      ...options,
      theme: colorMode === 'dark' ? 'vs-dark' : 'vs',
    });
  }, [colorMode]);

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