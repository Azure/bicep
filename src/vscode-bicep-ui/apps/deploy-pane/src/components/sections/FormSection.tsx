// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { FC, PropsWithChildren } from "react";

import { Codicon } from "@vscode-bicep-ui/components";
import { VscodeDivider } from "@vscode-elements/react-elements";
import { useState } from "react";

type FormSectionProps = PropsWithChildren<{
  title: string;
}>;

export const FormSection: FC<FormSectionProps> = ({ title, children }) => {
  const [open, setOpen] = useState(true);

  return (
    <section className="form-section">
      <VscodeDivider />
      <div className="form-title" onClick={() => setOpen(!open)}>
        <Codicon name={open ? "chevron-up" : "chevron-down"} size={13} />
        <h3>{title}</h3>
      </div>
      {open && <div className="form-content">{children}</div>}
    </section>
  );
};
