import { FC } from "react";
import { ParamData, ParamDefinition, ParamsData, TemplateMetadata } from "../models";
import { ParamInputBox } from "../ParamInputBox";

interface ParametersInputViewProps {
  template?: TemplateMetadata;
  params?: ParamsData;
  disabled: boolean;
  onValueChange: (name: string, data: ParamData) => void;
}

export const ParametersInputView: FC<ParametersInputViewProps> = ({ template, params, disabled, onValueChange }) => {
  if (!template || !params) {
    return null;
  }

  const { parameters } = template;

  return (
    <section>
      {parameters.map(definition => (
        <ParamInputBox
          key={definition.name}
          definition={definition}
          data={getParamData(params, definition)}
          disabled={disabled}
          onChangeData={data => onValueChange(definition.name, data)}
        />
      ))}
    </section>
  );
};

function getParamData(params: ParamsData, definition: ParamDefinition): ParamData {
  return params[definition.name] ?? {
    value: definition.defaultValue ?? '',
    useDefault: definition.defaultValue !== undefined,
  };
}