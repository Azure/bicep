import { existsSync } from "fs";
import { writeFile, mkdir, readFile } from "fs/promises";
import path from "path";

export const baselineRecordEnabled = (process.env['BASELINE_RECORD']?.toLowerCase() === 'true');

export async function expectFileContents(filePath: string, contents: string) {
  if (baselineRecordEnabled) {
    await mkdir(path.dirname(filePath), { recursive: true });
    await writeFile(filePath, contents, 'utf-8');
  } else {
    // If these assertions fail, use 'npm run test:fix' to replace the baseline files
    expect(existsSync(filePath)).toBeTruthy();

    const readContents = await readFile(filePath, { encoding: 'utf8' });
    expect(contents.replace(/\r\n/g, '\n')).toBe(readContents.replace(/\r\n/g, '\n'));
  }
}