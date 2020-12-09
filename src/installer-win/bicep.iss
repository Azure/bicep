#define MyAppName "Bicep CLI"

#ifndef MyAppVersion
#define MyAppVersion "0.0"
#endif

#define MyAppPublisher "Microsoft Corporation"
#define MyAppURL "https://github.com/Azure/bicep"
#define MyAppExeName "bicep.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{7EF9DE63-59B1-4325-955A-937F3E0A4EA8}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppCopyright=(C) Microsoft Corporation. All rights reserved.
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
LicenseFile=..\..\LICENSE
PrivilegesRequired=lowest
OutputBaseFilename=bicep-setup-win-x64
Compression=lzma
SolidCompression=yes
WizardStyle=modern
ChangesEnvironment=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "bicep\bicep.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "bicep\*.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "SetPath.ps1"; DestDir: "{app}\setup"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Run]
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -Command ""& '{app}\setup\SetPath.ps1' -AppPath '{app}' -Remove $false"" "; WorkingDir: {app}; Flags: runhidden

[UninstallRun]
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -Command ""& '{app}\setup\SetPath.ps1' -AppPath '{app}' -Remove $true"" "; WorkingDir: {app}; Flags: runhidden