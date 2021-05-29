<#
The script creates BIcep release notes.

Note: Non-MSFT users do not have permissions to execute this script successfully.

Prerequisites:
- GitHub CLI installed on the machine
- Completed login procecures for GH CLI

Parameters:
- WorkingDir - Directory used for temporary work. Contents will be deleted.
- TagName    - The tag name of the GitHub release release. Should be in the format v<x>.<y>.<z> where <x>, <y>, and <z> are numbers.
- BuildId    - Set it to a DevOps build ID to use a specific signed build instead of the latest.

#>
[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)]
    [string] $FromTag,

    [Parameter(Mandatory = $true)]
    [string] $ToTag
)

$ErrorActionPreference = 'Stop';

function ValidateTag([string] $tag) {
  if($tag -notmatch 'v\d+\.\d+\.\d+')
  {
    Write-Error "The specified tag name '$($tag)' is not in the expected format 'v<x>.<y>.<z>' where <x>, <y>, and <z> are numbers.";
  }
}

ValidateTag -tag $FromTag;
ValidateTag -tag $ToTag;

$shortLogOutput = git shortlog "$FromTag..$ToTag";
if($lastExitCode -ne 0)
{
  Write-Output $shortLogOutput;
  Write-Error "Failed to obtain the short log";
}

$currentAuthor = $null;

[Regex] $regex = '\(\#(?<pr>\d+)\)';
foreach ($line in $shortLogOutput) {
  $match = $regex.Match($line);
  if($match.Success)
  {
    $pullRequestNumber = $match.Groups['pr'].Value
    $pullRequestInfo = gh pr view $pullRequestNumber --json author,title
    $json = $pullRequestInfo | ConvertFrom-Json;
    
    if($json.author.login -ne $currentAuthor)
    {
      $currentAuthor = $json.author.login;
      Write-Output '';
      Write-Output "@$currentAuthor";
    }

    Write-Output "* $($json.title) (#$pullRequestNumber)"
  }
}
