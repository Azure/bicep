
export function expectLinesInLog(log: string, expectedLines: string[]) {
  expectedLines.forEach(expectedLine => {
    expect(log).toContain(expectedLine);
  });
}
