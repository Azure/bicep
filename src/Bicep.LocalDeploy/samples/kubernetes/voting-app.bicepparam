using 'voting-app.bicep'

// TODO: Fix the below by copying your kubeconfig (~/.kube/config):
//         cat ~/.kube/config > ../secrets/kubeconfig.yml
param kubeConfig = base64(loadTextContent('../secrets/kubeconfig.yml'))
