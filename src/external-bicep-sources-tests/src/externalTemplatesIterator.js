import { exec as callbackExec } from 'node:child_process'
import { mkdtemp, readdir } from 'node:fs/promises';
import { tmpdir } from 'node:os';
import { extname, join } from 'node:path';
import { promisify } from 'node:util';
const exec = promisify(callbackExec);

export async function* yieldTemplatesFromRepository(repo, commitId, targetBranch, directoriesToInclude) {
    const cloneTargetDir = await mkdtemp(join(tmpdir(), `${repo.replace(/\W/g, '_')}-`));

    await exec(`cd ${cloneTargetDir} && git init && git remote add origin https://github.com/${repo} && git fetch origin && git checkout ${commitId ?? targetBranch ?? 'main'} --quiet`);

    const directoriesToScan = directoriesToInclude?.map(d => [`${repo}/${d}`, join(cloneTargetDir, d)])
        ?? [[repo, cloneTargetDir]];

    for (const [friendlyName, path] of directoriesToScan) {
        yield* findTemplates(friendlyName, path);
    }
}

async function* findTemplates(directoryFriendlyName, directoryPath) {
    for (const entry of await readdir(directoryPath, {withFileTypes: true})) {
        const entryFriendlyName = `${directoryFriendlyName}/${entry.name}`;
        const entryPath = join(directoryPath, entry.name);

        if (entry.isDirectory()) {
            yield* findTemplates(entryFriendlyName, entryPath);
        } else if (entry.isFile() && extname(entry.name) === ".bicep") {
            yield {name: entryFriendlyName, path: entryPath};
        }
    }
}
