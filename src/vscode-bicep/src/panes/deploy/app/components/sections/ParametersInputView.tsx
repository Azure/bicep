import { VSCodeTextField, VSCodeButton } from "@vscode/webview-ui-toolkit/react";
import { FC } from "react";
import { ParamData, ParamDefinition, ParametersMetadata, TemplateMetadata } from "../../../models";
import { ParamInputBox } from "../ParamInputBox";
import { FormSection } from "./FormSection";

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
    <FormSection title="Parameters">
      {sourceFilePath && <VSCodeTextField value={sourceFilePath} disabled={true}>File Path</VSCodeTextField>}
      {sourceFilePath && !sourceFilePath.endsWith('.bicepparam') && <VSCodeButton onClick={onEnableEditing}>Edit Parameters</VSCodeButton>}
      {!sourceFilePath && <VSCodeButton onClick={onPickParametersFile}>Pick Parameters File</VSCodeButton>}
      {!sourceFilePath && parameterDefinitions.map(definition => (
        <ParamInputBox
          key={definition.name}
          definition={definition}
          data={getParamData(parameters, definition)}
          disabled={disabled}
          onChangeData={data => onValueChange(definition.name, data)}
        />
      ))}
    </FormSection>
  );
};

function getParamData(params: ParametersMetadata, definition: ParamDefinition): ParamData {
  return params.parameters[definition.name] ?? {
    value: definition.defaultValue,
  };
}