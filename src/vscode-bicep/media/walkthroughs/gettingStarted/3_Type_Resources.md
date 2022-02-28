# Add resources to your Bicep file

* First, type `'appplan'` to view the 'App Service plan' resource snippet and press Tab or Enter. Press Tab to jump to the `name` attribute and replace its value with the parameter `appPlanName`.

* Next, type `'storage'` to view the 'Storage Account' resource snippet and press Tab or Enter. Replace the `name` attribute's value with `'${appServicePlan.name}storage'` (including the single quotes).

* Save the file.

Feel free to search for other snippets that suit your needs.

[Copy code to clipboard](command:bicep.gettingStarted.copyToClipboardResources)

![Typing resources into Bicep file](3_Type_Resources.gif)
