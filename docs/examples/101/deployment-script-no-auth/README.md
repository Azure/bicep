# Deployment Script with no managed identity

As of apiVersion `2020-10-01`, the `identity` is optional. The permissions of the principal creating the template deployment will be used to provision the ACI and/or storage resources that are required for the script to execute.

Note that if a managed identity is not provided, there will be **no preconfigured authentication to Az powershell or Az CLI** in this case. If you need to reach Azure, we recommend adding a managed identity. 