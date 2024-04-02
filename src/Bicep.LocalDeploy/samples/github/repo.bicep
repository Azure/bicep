@secure()
param githubToken string

param owner string
param repoName string
param collaboratorName string

provider 'github@0.0.1' with {
  token: githubToken
}

resource repo 'Repository' existing = {
  owner: owner
  name: repoName
}

resource collaborator 'Collaborator' existing = {
  owner: owner
  name: repoName
  user:collaboratorName
}

output repo object = repo
output collaborator object = collaborator
