import { VSCodeCheckbox, VSCodeTextField } from "@vscode/webview-ui-toolkit/react";
import { FC, FormEvent } from "react";
import { ParamData, ParamDefinition } from "./models";

interface ParamInputBoxProps {
  definition: ParamDefinition;
  data: ParamData;
  disabled: boolean;
  onChangeData: (data: ParamData) => void;
}

export const ParamInputBox : FC<ParamInputBoxProps> = (props) => {
  const { definition, data, disabled, onChangeData } = props;
  function handleValueInput(event: FormEvent<HTMLInputElement>) {
    onChangeData({
      ...data,
      value: event.currentTarget.value,
    });
  }
  function handleDefaultValueChange() {
    onChangeData({
      value: definition.defaultValue ?? '',
      useDefault: !data.useDefault,
    });
  }

  const { name, defaultValue } = definition;
  const { value, useDefault } = data;

  return (
    <span className="input-row">
      <VSCodeTextField
        value={value ?? ''}
        onInput={e => handleValueInput(e as FormEvent<HTMLInputElement>)}
        disabled={disabled || useDefault}>
        {name}
      </VSCodeTextField>
      {(defaultValue !== undefined) && (
        <VSCodeCheckbox
          checked={useDefault}
          onChange={handleDefaultValueChange}
          disabled={disabled}>
          Use Default?
        </VSCodeCheckbox>
      )}
    </span>
  );
}