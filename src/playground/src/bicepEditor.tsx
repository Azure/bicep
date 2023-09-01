// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as monaco from 'monaco-editor';
import React, { createRef, useEffect, useState } from 'react';
import { compileAndEmitDiagnostics, getSemanticTokens, getSemanticTokensLegend } from './lspInterop';

interface Props {
  initialContent: string,
  onBicepChange: (bicepContent: string) => void,
  onJsonChange: (jsonContent: string) => void,
}

const editorOptions: monaco.editor.IStandaloneEditorConstructionOptions = {
  language: 'bicep',
  theme: 'vs-dark',
  scrollBeyondLastLine: false,
  automaticLayout: true,
  minimap: {
    enabled: false,
  },
  insertSpaces: true,
  tabSize: 2,
  suggestSelection: 'first',
  suggest: {
    snippetsPreventQuickSuggestions: false,
    showWords: false,
  },
  'semanticHighlighting.enabled': true,
};

export const BicepEditor : React.FC<Props> = (props) => {
  const editorRef = createRef<HTMLDivElement>();
  const [model, setModel] = useState<monaco.editor.ITextModel>();

  useEffect(() => {
    async function initializeEditor() {
      monaco.languages.register({
        id: 'bicep',
        extensions: ['.bicep'],
        aliases: ['bicep'],
      });
    
      monaco.languages.registerDocumentSemanticTokensProvider('bicep', {
        getLegend: () => getSemanticTokensLegend(),
        provideDocumentSemanticTokens: async (model) => await getSemanticTokens(model.getValue()),
        releaseDocumentSemanticTokens: () => { return; }
      });

      const editor = monaco.editor.create(editorRef.current, editorOptions);
      const model = editor.getModel();

      // @ts-expect-error: Using a private method on editor
      editor._themeService._theme.getTokenStyleMetadata = (type) => {
        // see 'monaco-editor/esm/vs/editor/standalone/common/themes.js' to understand these indices
        switch (type) {
          case 'keyword':
            return { foreground: 12 };
          case 'comment':
            return { foreground: 7 };
          case 'parameter':
            return { foreground: 2 };
          case 'property':
            return { foreground: 3 };
          case 'type':
            return { foreground: 8 };
          case 'member':
            return { foreground: 6 };
          case 'string':
            return { foreground: 5 };
          case 'variable':
            return { foreground: 4 };
          case 'operator':
            return { foreground: 9 };
          case 'function':
            return { foreground: 13 };
          case 'number':
            return { foreground: 15 };
          case 'class':
          case 'enummember':
          case 'event':
          case 'modifier':
          case 'label':
          case 'typeParameter':
          case 'macro':
          case 'interface':
          case 'enum':
          case 'regexp':
          case 'struct':
          case 'namespace':
            return { foreground: 0 };
        }
      };
  
      editor.onDidChangeModelContent(async () => {
        const text = model.getValue();

        const { template, diagnostics } = await compileAndEmitDiagnostics(text);
        monaco.editor.setModelMarkers(model, 'default', diagnostics);
        props.onBicepChange(text);
        props.onJsonChange(template);
      });

      setModel(model);
    }

    initializeEditor();
  }, []);

  useEffect(() => {
    model?.setValue(props.initialContent);
  }, [props.initialContent, model]);

  return <div ref={editorRef} style={{height: '100%', width: '100%'}} />;
};