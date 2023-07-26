import { VSCodeDivider } from "@vscode/webview-ui-toolkit/react";
import { FC, ReactNode } from "react";

interface FormSectionProps {
  title: string;
  children: ReactNode;
}

export const FormSection: FC<FormSectionProps> = ({ title, children, }) => {
  return (
    <section className="form-section">
      <h2>{title}</h2>
      {children}
      <VSCodeDivider />
    </section>
  );
};
