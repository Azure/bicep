
import { merge } from '@jsq/seq';
import { compileTemplate } from './src/compileTemplate.js';
import { yieldTemplatesFromRepository } from './src/externalTemplatesIterator.js';
import { repositories } from './src/sources.js';

const externalTemplates = merge(
    ...Object.entries(repositories).map(([clonableUrl, configuration]) => yieldTemplatesFromRepository(
        clonableUrl,
        configuration.commitId,
        configuration.targetBranch,
        configuration.include)));

let succeededCount = 0;
let failedTemplates = [];
const queue = [];

for await (const {name, path} of externalTemplates) {
    const nextPromise = compileTemplate(path).then(
        _ => succeededCount++,
        error => failedTemplates.push({name, error}));
    queue.push(nextPromise);
    nextPromise.finally(_ => {
        queue.splice(queue.indexOf(nextPromise), 1);
    });

    while (queue.length > 10) {
        await Promise.race(queue);
    }
}

await Promise.all(queue);

console.log(`${succeededCount} templates compiled successfully.`);
if (failedTemplates.length > 0) {
    for (const failedTemplate of failedTemplates) {
        console.error(`${failedTemplate.name} compiled with errors: `, failedTemplate.error);
    }
    process.exit(1);
}
