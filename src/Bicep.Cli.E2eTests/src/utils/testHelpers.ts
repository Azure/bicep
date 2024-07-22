// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export const itif = (condition: boolean) => (condition ? it : it.skip);
