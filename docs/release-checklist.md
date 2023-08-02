# Bicep Release Checklist

1. (**end-of-month releases only**) Update Bicep Az types:
    1. Run the [Update Types](https://github.com/Azure/bicep-types-az/actions/workflows/update-types.yml) GitHub Action to generate the latest type definitions.
        1. Wait ~3hrs for it to complete. Ensure it runs successfully and that it generates + merges a PR (example [here](https://github.com/Azure/bicep-types-az/pull/1299)).
    1. Run the Official Build for BicepMirror-Types-Az(see [this README](https://msazure.visualstudio.com/One/_git/BicepMirror-Types-Az) for instructions).
    1. Publish Bicep.Types.Az NuGet packages to nuget.org. Follow the latter half of the readme [here](https://dev.azure.com/msazure/One/_git/BicepMirror-Types-Az) and the below steps.
        1. Find your build [here](https://dev.azure.com/msazure/One/_build?definitionId=179851&_a=summary) and wait for it to finish successfully. Then click on it and for the `drop_build_main`, download the artifacts.
        1. Follow instructions to download the nuget.exe from [here](https://learn.microsoft.com/en-us/nuget/install-nuget-client-tools)
        1. Run `./scripts/UploadPackages.ps1 -PackageDirectory <downloads>/drop_build_main -NuGetPath <nuget_tool_directory>`
        1. You need to be part of the armdeployments org on nuget.org. (Ask one of the admins to be added) You must generate an API key and then use that as the password for when the popup window appears after running the above command. (Username can be anything)
    1. Bump the Bicep.Types.Az NuGet package version in this project in this [file](https://github.com/Azure/bicep/blob/main/src/Bicep.Core/Bicep.Core.csproj) by creating and merging a PR
        1. Might need to run a `dotnet restore` to update the packages.lock.json files
        1. Might also need to update baseline tests (run `bicep/scripts/SetBaseline.ps1`)
1. Verify the latest build on the `main` branch is green: ![Build on main](https://github.com/Azure/bicep/actions/workflows/build.yml/badge.svg?branch=main).
1. Review history for changes to [bicepconfig.schema.json](https://github.com/Azure/bicep/commits/main/src/vscode-bicep/schemas/bicepconfig.schema.json). Raise an issue for any recently-added linter rules which do not have public documentation.
1. (**end-of-month releases only**) Update Bicep:
    1. Bump the version number by incrementing the minor version number in [this file](https://github.com/Azure/bicep/blob/main/version.json) (example [here](https://github.com/Azure/bicep/pull/9698))
    1. Run the Official Build for BicepMirror (see [this README](https://msazure.visualstudio.com/One/_git/BicepMirror) for instructions).
1. Push the version tag for the commit used to generate the official build.
    1. Obtain the version number from official build. This should be of format `vXX.YY.ZZ` - e.g `v0.14.85`.
        1. Look at either the official build artifacts or 
        2. Look at random logs of the the build from [here](https://msazure.visualstudio.com/One/_build?definitionId=182734&_a=summary). One example might be bicep_windows -> Copy Bicep registry module tool package to output (windows_build_container)
    1. In the Bicep repo, run `git tag v<new_release_number> <commit_hash>`, where `<commit_hash>` is the git commit hash used for the official build (ex: `git tag v0.15.31 3ba6e06a8d412febd182e607d0f5fe607ec90274`).
    1. Run `git push origin v<new_release_number>` to push the tag (ex: `git push origin v0.15.31`).
1. [Create a draft release](https://github.com/Azure/bicep/releases/new) for the new tag and set release title to the tag name. Use the "Save draft" button to save the changes without publishing it.
1. Run `bicep/scripts/CreateReleaseNotes -FromTag <previous tag> -ToTag <new tag>` and set the output as the release description.
    1. Give the output of this script to a PM, and ask them to clean up the notes for the release.
    1. Once they have cleaned up the notes, copy + paste them into the draft notes, and hit "Save draft" again.
1. Run `BicepMirror/scripts/UploadSignedReleaseArtifacts.ps1` to add official artifacts to the release.
    * `-WorkingDir` can be any empty temporary directory that you create
    * `-BuildId` is only needed if the latest official build is NOT the official build you are trying to release
1. Validate VSCode extension and Bicep CLI manually on Windows, Mac & Linux:
    1. Download `vscode-bicep.vsix` from the draft release, and [Install it from VSIX](https://code.visualstudio.com/docs/editor/extension-marketplace#_install-from-a-vsix). Verify that you can open a Bicep file, that text is correctly colorized, and that error messages show up as expected.
    1. Download the appropriate Bicep executable for your platform (e.g. `bicep-linux-x64`). Verify you can invoke it with e.g. `bicep-linux-x64 --version`, and that it prints the expected output.
1. Publish the release on GitHub.
1. Upload copyleft dependency source to 3rd party disclosure site. See [instructions](https://msazure.visualstudio.com/One/_wiki/wikis/Azure%20Deployments%20Team%20Wiki/369910/Bicep-release-step-Upload-copyleft-source-to-3rd-party-disclosure-site).
1. Upload vscode-bicep.VSIX to the VS marketplace [here](https://marketplace.visualstudio.com/manage).
    1. Click on the ... for Bicep, then Update, then upload the .vsix file. The site will verify it then the version number should be updated to the right one. 
1. Upload vs-bicep.VSIX to VS marketplace
    1. Click on the ... for Bicep for Visual Studio, then Edit. Once on the new page, upload the new vs-bicep.VSIX file at the top. This should update the Version number automatically for you. Verify that it does then scroll to the bottom and hit Save and Upload.
1. Upload NuGet packages to nuget.org via `BicepMirror/scripts/PublishPackages.ps1`. This is an almost identical process to publishing the BicepMirror-Types-Az nuget packages so look at that previous step above. (Make sure to include CLI packages.) This can be done one of two ways:
    1. Easiest is to use the `__assets` directory created by the `UploadSignedReleaseArtifacts.ps1` script.)
    2. You can also download all the files from the published release into a separate folder and run the script using that folder. (The script looks for files ending in *.nupkg)
1. Update homebrew by going here [here](https://github.com/Azure/homebrew-bicep/actions/workflows/update-homebrew.yml) and clicking on `Run workflow`
    * A PR will be auto created by this action (example [here](https://github.com/Azure/homebrew-bicep/pull/40)). Approve and merge it.
