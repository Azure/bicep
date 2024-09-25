// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import hljs from "highlight.js"
import bicep from './bicep';

// eslint-disable-next-line jest/require-hook
hljs.registerLanguage('bicep', bicep);