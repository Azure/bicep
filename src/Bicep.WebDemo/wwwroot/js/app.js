var modelUri = 'file://main.arm';

monaco.languages.register({
    id: 'bicep',
    extensions: ['.arm'],
    aliases: ['bicep'],
});

window.CreateDemoEditors = (appInstance) => {
    var bicepModel = monaco.editor.createModel('', 'bicep', monaco.Uri.parse(modelUri));

    var editorLhs = monaco.editor.create(document.getElementById('editor_lhs'), {
        theme: 'vs-dark',
        automaticLayout: true,
        language: 'bicep',
        minimap: {
            enabled: false,
        },
        model: bicepModel,
    });

    var editorRhs = monaco.editor.create(document.getElementById('editor_rhs'), {
        theme: 'vs-dark',
        automaticLayout: true,
        language: 'json',
        minimap: {
            enabled: false,
        },
        readOnly: true,
    });

    bicepModel.onDidChangeContent((e) => {
        appInstance.invokeMethodAsync('JS_OnContentChanged');
    });

    window.GetLhsContent = () => {
        return editorLhs.getValue();
    }

    window.SetLhsContent = (value) => {
        editorLhs.setValue(value);
    }

    window.SetLhsDiagnostics = (diagnostics) => {
        monaco.editor.setModelMarkers(bicepModel, 'default', diagnostics);
    }

    window.SetRhsContent = (value) => {
        editorRhs.setValue(value);
    }
};