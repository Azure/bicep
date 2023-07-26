import { VSCodeDivider } from "@vscode/webview-ui-toolkit/react";
import { FC, PropsWithChildren } from "react";

type FormSectionProps = PropsWithChildren<{
  title: string;
}>;

export const FormSection: FC<FormSectionProps> = ({ title, children, }) => {
  return (
    <section className="form-section">
      <h2>{title}</h2>
      {children}
      <VSCodeDivider />
    </section>
  );
};
