import { VSCodeButton, VSCodeCheckbox, VSCodeDropdown, VSCodeOption, VSCodeTextArea, VSCodeTextField } from "@vscode/webview-ui-toolkit/react";
import { FC } from "react";
import { ParamData, ParamDefinition } from "../../models";

interface ParamInputBoxProps {
  definition: ParamDefinition;
  data: ParamData;
  disabled: boolean;
  onChangeData: (data: ParamData) => void;
}

export const ParamInputBox: FC<ParamInputBoxProps> = (props) => {
  const { definition, data, disabled, onChangeData } = props;
  const { name, defaultValue, type } = definition;
  const { value } = data;

  function handleValueChange(value: any) {
    onChangeData({ ...data, value });
  }

  function handleResetToDefaultClick() {
    handleValueChange(defaultValue);
  }

  function getInputBox() {
    switch (type) {
      case 'bool':
        return (
          <VSCodeCheckbox
            checked={!!value}
            onChange={() => handleValueChange(!value)}
            disabled={disabled}>
            {name}
          </VSCodeCheckbox>
        );
      case 'int':
        return (
          <VSCodeTextField
            value={`${value ?? 0}`}
            onChange={e => handleValueChange(parseInt((e.currentTarget as HTMLInputElement).value, 10))}
            disabled={disabled}>
            {name}
          </VSCodeTextField>
        );
      case 'string':
        if (definition.allowedValues) {
          const dropdownHtmlId = `param-input-${name.toLowerCase()}`;
          return (
            <div className="dropdown-container">
              <label htmlFor={dropdownHtmlId}>{name}</label>
              <VSCodeDropdown
                id={dropdownHtmlId}
                onChange={e => handleValueChange((e.currentTarget as HTMLSelectElement).value)}
                disabled={disabled}>
                {definition.allowedValues.map(option => (
                  <VSCodeOption key={option} selected={value === option}>
                    {option}
                  </VSCodeOption>
                ))}
              </VSCodeDropdown>
            </div>
          );
        } else {
          return (
            <VSCodeTextField
              value={`${value ?? ''}`}
              onChange={e => handleValueChange((e.currentTarget as HTMLInputElement).value)}
              disabled={disabled}>
              {name}
            </VSCodeTextField>
          );
        }
      default:
        return (
          <VSCodeTextArea
            className="code-textarea-container"
            resize="vertical"
            value={value ? JSON.stringify(value, null, 2) : ''}
            onChange={e => handleValueChange(JSON.parse((e.currentTarget as HTMLInputElement).value))}
            disabled={disabled}>
            {name}
          </VSCodeTextArea>
        );
    }
  }

  return (
    <span className="input-row">
      {getInputBox()}
      {(defaultValue !== undefined && value !== defaultValue) && (
        <VSCodeButton onClick={handleResetToDefaultClick} disabled={disabled}>
          Reset to default
        </VSCodeButton>
      )}
    </span>
  );
}