import { VSCodeTextField, VSCodeCheckbox, VSCodeButton, VSCodeDivider } from "@vscode/webview-ui-toolkit/react";
import { FC } from "react";
import { ParamData, ParamDefinition, ParametersMetadata, TemplateMetadata } from "../models";
import { ParamInputBox } from "../ParamInputBox";

interface ParametersInputViewProps {
  template?: TemplateMetadata;
  parameters?: ParametersMetadata;
  disabled: boolean;
  onValueChange: (name: string, data: ParamData) => void;
  onEnableEditing: () => void;
  onPickParametersFile: () => void;
}

export const ParametersInputView: FC<ParametersInputViewProps> = ({ template, parameters, disabled, onValueChange, onEnableEditing, onPickParametersFile }) => {
  if (!template || !parameters) {
    return null;
  }

  const { parameterDefinitions } = template;
  const { sourceFilePath } = parameters;

  return (
    <section className="form-section">
      <VSCodeDivider />
      <h2>Parameters</h2>
      {sourceFilePath && <VSCodeTextField value={sourceFilePath} disabled={true}>File Path</VSCodeTextField>}
      {sourceFilePath && !sourceFilePath.endsWith('.bicepparam') && <VSCodeCheckbox onChange={onEnableEditing} checked={false}>Edit Parameters?</VSCodeCheckbox>}
      {!sourceFilePath && <VSCodeButton onClick={onPickParametersFile} appearance="secondary">Pick Parameters File</VSCodeButton>}
      {!sourceFilePath && parameterDefinitions.map(definition => (
        <ParamInputBox
          key={definition.name}
          definition={definition}
          data={getParamData(parameters, definition)}
          disabled={disabled}
          onChangeData={data => onValueChange(definition.name, data)}
        />
      ))}
    </section>
  );
};

function getParamData(params: ParametersMetadata, definition: ParamDefinition): ParamData {
  return params.parameters[definition.name] ?? {
    value: definition.defaultValue ?? '',
    useDefault: definition.defaultValue !== undefined,
  };
}