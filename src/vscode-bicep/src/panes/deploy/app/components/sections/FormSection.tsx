import { VSCodeDivider } from "@vscode/webview-ui-toolkit/react";
import { FC, PropsWithChildren, useState } from "react";

type FormSectionProps = PropsWithChildren<{
  title: string;
}>;

export const FormSection: FC<FormSectionProps> = ({ title, children, }) => {
  const [open, setOpen] = useState(true);

  return (
    <section className="form-section">
      <h2 onClick={() => setOpen(!open)} style={{ userSelect: 'none', cursor: 'pointer' }}>
        <span className={`codicon codicon-${open ? 'chevron-up' : 'chevron-down'}`} />
        {title}
      </h2>
      {open && children}
      <VSCodeDivider />
    </section>
  );
};
