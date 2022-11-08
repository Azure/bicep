import { exec } from 'node:child_process'
import { dirname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

const __dirname = dirname(fileURLToPath(import.meta.url));

const bicepCli = resolve(
    __dirname,
    process.env.BICEP_CLI_EXECUTABLE ||
    "../../Bicep.Cli/bin/Debug/net6.0/bicep"
);

export function compileTemplate(templatePath) {
    return new Promise((resolve, reject) => {
        exec(`${bicepCli} build --stdout ${templatePath}`, (err, stdout, stderr) => {
            if (err) {
                reject(err);
            }

            if (stderr) {
                console.error(stderr);
            }

            resolve(stdout);
        });
    });
}
