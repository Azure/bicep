import { exec as callbackExec } from 'node:child_process';
import { promisify } from 'node:util';
import { resolveRelative } from './pathUtils.js';
const exec = promisify(callbackExec);

const bicepCli = resolveRelative(import.meta.url,
    process.env.BICEP_CLI_EXECUTABLE || "../../Bicep.Cli/bin/Debug/net6.0/bicep");

export async function compileTemplate(templatePath) {
    const {stdout, stderr} = await exec(`${bicepCli} build --stdout ${templatePath}`);

    if (stderr) {
        console.error(stderr);
    }

    return stdout;
}
