// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { MotionPolicy } from "./api";

import { atom } from "jotai";

export const motionPolicyAtom = atom<MotionPolicy>("system");
