// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


import { it } from "vitest";

export const itif = (condition: boolean) => (condition ? it : it.skip);
