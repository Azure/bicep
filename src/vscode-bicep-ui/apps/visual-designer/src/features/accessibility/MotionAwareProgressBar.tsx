// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ProgressBar } from "@vscode-bicep-ui/components";
import { useAtomValue } from "jotai";
import { motionPolicyAtom } from "./atoms";

export function MotionAwareProgressBar({
  testId,
  ariaLabel,
}: {
  testId?: string;
  ariaLabel: string;
}) {
  const policy = useAtomValue(motionPolicyAtom);

  return <ProgressBar motionPolicy={policy} testId={testId} ariaLabel={ariaLabel} />;
}
