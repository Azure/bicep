using 'repo.bicep'

// TODO: Fix the below by saving your GH access token:
//         gh auth token > ../secrets/githubtoken
param githubToken = trim(loadTextContent('../secrets/githubtoken'))

param owner = 'anthony-c-martin'
param repoName = 'bicep'
param collaboratorName = 'anthony-c-martin'
