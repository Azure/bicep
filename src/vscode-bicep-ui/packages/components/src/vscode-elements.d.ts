// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { VscodeProgressBar } from "@vscode-elements/elements/dist/vscode-progress-bar/vscode-progress-bar.js";
import type { DetailedHTMLProps, HTMLAttributes } from "react";

type WebComponentProps<T extends HTMLElement> = DetailedHTMLProps<HTMLAttributes<T>, T> &
  Partial<Omit<T, keyof HTMLElement>>;

declare module "react" {
  namespace JSX {
    interface IntrinsicElements {
      "vscode-progress-bar": WebComponentProps<VscodeProgressBar>;
    }
  }
}
