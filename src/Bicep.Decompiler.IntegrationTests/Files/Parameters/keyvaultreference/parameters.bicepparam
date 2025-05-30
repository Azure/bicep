using '' /*TODO: Provide a path to a bicep template*/

param adminUsername = 'tim'

param adminPassword = az.getSecret('2fbf906e-1101-4bc0-b64f-adc44e462fff', 'INSTRUCTOR', 'TimKV', 'vm-password', '1.0')

param adminPassword2 = az.getSecret('2fbf906e-1101-4bc0-b64f-adc44e462fff', 'INSTRUCTOR', 'TimKV', 'vm-password')

param dnsLabelPrefix = 'newvm79347a'
