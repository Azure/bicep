// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
export interface TemplateMetadata {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  template: any;
  parameters: ParamDefinition[];
}

export interface ParamDefinition {
  type: string;
  name: string;
  defaultValue?: string;
}

export interface ParamData {
  useDefault: boolean;
  value: string;
}
