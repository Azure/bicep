// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { FC, PropsWithChildren } from "react";

import { Codicon } from "@vscode-bicep-ui/components";
import { VscodeDivider } from "@vscode-elements/react-elements";
import { useState } from "react";
import styled from "styled-components";

type FormSectionProps = PropsWithChildren<{
  title: string;
  hidden?: boolean;
}>;

const FormTitle = styled.div`
  display: flex;
  user-select: none;
  cursor: pointer;
`;

const FormContent = styled.div`
  display: grid;
  grid-auto-flow: row;
  grid-row-gap: 0.5rem;
  padding: 0.75rem;
`;

const FormTitleH3 = styled.h3`
  margin: 0;
  text-transform: uppercase;
  font-size: 13px;
  min-width: 3ch;
`;

export const FormSection: FC<FormSectionProps> = ({ title, hidden, children }) => {
  const [open, setOpen] = useState(true);

  return (
    <section style={hidden ? { display: "none" } : {}}>
      <VscodeDivider />
      <FormTitle onClick={() => setOpen(!open)}>
        <Codicon name={open ? "chevron-up" : "chevron-down"} size={13} />
        <FormTitleH3>{title}</FormTitleH3>
      </FormTitle>
      {open && <FormContent>{children}</FormContent>}
    </section>
  );
};
