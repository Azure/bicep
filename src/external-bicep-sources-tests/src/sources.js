import { readFile } from 'node:fs/promises';
import { resolveRelative } from './pathUtils.js';

export const repositories = JSON.parse(await readFile(resolveRelative(import.meta.url, 'sources.json'), {encoding: 'utf-8'}));
