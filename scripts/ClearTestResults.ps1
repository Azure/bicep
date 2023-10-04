if (!(Test-Path "Bicep.sln")) {
    Write-Error "This script must be run from the repository's root directory";
    exit 1;
}

$testResultsDirName = "TestResults"

function RmRF {
    param (
        [string] $DirectoryName
    )

    if (Test-Path $DirectoryName) {
        Write-Host "Removing $DirectoryName folder and contents"
        Remove-Item $DirectoryName -Recurse
    }
}

RmRF -DirectoryName $testResultsDirName

foreach ($srcSubdir in (Get-ChildItem "src")) {
    RmRF -DirectoryName "$srcSubdir\$testResultsDirName"
}
