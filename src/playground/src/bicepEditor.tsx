// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as monaco from 'monaco-editor';
import React, { createRef, useEffect, useState } from 'react';
import { BaseLanguageClient } from 'vscode-languageclient';

interface Props {
  client: BaseLanguageClient,
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

        props.onBicepChange(text);
    
        setTimeout(async () => {
          const jsonContent: {output?: string} = await props.client.sendRequest(
            "workspace/executeCommand",
            {
              command: "buildActiveCompilation",
              arguments: [{
                bicepUri: model.uri.toString()
              }],
            }
          );
          props.onJsonChange(jsonContent.output ?? "Compilation failed!");
        });
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