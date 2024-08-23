var unterminatedMultilineString = '''
hello!''
//@[04:31) [no-unused-vars (Warning)] Variable "unterminatedMultilineString" is declared but never used. (CodeDescription: Linter(https://aka.ms/bicep/linter/no-unused-vars)) |unterminatedMultilineString|
//@[34:46) [BCP140 (Error)] The multi-line string at this location is not terminated. Terminate it with "'''". (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP140)) |'''\nhello!''|
