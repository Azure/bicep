
import { dirname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

export function __dirname(importMetaUrl) {
    return dirname(fileURLToPath(importMetaUrl));
}

export function resolveRelative(importMetaUrl, relativePath) {
    return resolve(__dirname(importMetaUrl), relativePath);
}
