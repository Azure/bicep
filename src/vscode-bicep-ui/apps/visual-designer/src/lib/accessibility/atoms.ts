// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { MotionPolicy } from "@/lib/messaging";

import { atom } from "jotai";

export const motionPolicyAtom = atom<MotionPolicy>("system");
