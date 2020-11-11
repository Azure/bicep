import example_101_vm_simple_windows from '../../../docs/examples/101/vm-simple-windows/main.bicep'
import example_101_logic_app_create from '../../../docs/examples/101/logic-app-create/main.bicep'
import example_101_storage_blob_container from '../../../docs/examples/101/storage-blob-container/main.bicep'
import example_101_vnet_two_subnets from '../../../docs/examples/101/vnet-two-subnets/main.bicep'
import example_101_cosmosdb_webapp from '../../../docs/examples/101/cosmosdb-webapp/main.bicep'
import example_101_key_vault_create from '../../../docs/examples/101/key-vault-create/main.bicep'
import example_101_databricks_workspace from '../../../docs/examples/101/databricks-workspace/main.bicep'
import example_101_databricks_all_in_one_template_for_vnet_injection from '../../../docs/examples/101/databricks-all-in-one-template-for-vnet-injection/main.bicep'
import example_101_aks from '../../../docs/examples/101/aks/main.bicep'
import example_101_monitor_action_groups from '../../../docs/examples/101/monitor-action-groups/main.bicep'
import example_101_vm_simple_linux from '../../../docs/examples/101/vm-simple-linux/main.bicep'
import example_101_key_vault_secret_only from '../../../docs/examples/101/key-vault-secret-only/main.bicep'
import example_101_website_with_container from '../../../docs/examples/101/website-with-container/main.bicep'
import example_101_1vm_2nics_2subnets_1vnet from '../../../docs/examples/101/1vm-2nics-2subnets-1vnet/main.bicep'
import example_201_vm_windows_with_custom_script_extension from '../../../docs/examples/201/vm-windows-with-custom-script-extension/main.bicep'
import example_201_key_vault_secret_create from '../../../docs/examples/201/key-vault-secret-create/main.bicep'
import example_201_vm_push_cert_windows from '../../../docs/examples/201/vm-push-cert-windows/main.bicep'
import example_201_decrypt_running_windows_vm_without_aad from '../../../docs/examples/201/decrypt-running-windows-vm-without-aad/main.bicep'
import example_201_iot_with_storage from '../../../docs/examples/201/iot-with-storage/main.bicep'
import example_201_vnet_peering from '../../../docs/examples/201/vnet-peering/main.bicep'
import example_201_sql from '../../../docs/examples/201/sql/main.bicep'
import example_201_web_app_loganalytics from '../../../docs/examples/201/web-app-loganalytics/main.bicep'
import example_101_deployment_script_with_storage from '../../../docs/examples/101/deployment-script-with-storage/main.bicep'

export const examples = {
  'blank': '',
  '101/vm-simple-windows': example_101_vm_simple_windows,
  '101/logic-app-create': example_101_logic_app_create,
  '101/storage-blob-container': example_101_storage_blob_container,
  '101/vnet-two-subnets': example_101_vnet_two_subnets,
  '101/cosmosdb-webapp': example_101_cosmosdb_webapp,
  '101/key-vault-create': example_101_key_vault_create,
  '101/databricks-workspace': example_101_databricks_workspace,
  '101/databricks-all-in-one-template-for-vnet-injection': example_101_databricks_all_in_one_template_for_vnet_injection,
  '101/aks': example_101_aks,
  '101/monitor-action-groups': example_101_monitor_action_groups,
  '101/vm-simple-linux': example_101_vm_simple_linux,
  '101/key-vault-secret-only': example_101_key_vault_secret_only,
  '101/website-with-container': example_101_website_with_container,
  '101/1vm-2nics-2subnets-1vnet': example_101_1vm_2nics_2subnets_1vnet,
  '201/vm-windows-with-custom-script-extension': example_201_vm_windows_with_custom_script_extension,
  '201/key-vault-secret-create': example_201_key_vault_secret_create,
  '201/vm-push-cert-windows': example_201_vm_push_cert_windows,
  '201/decrypt-running-windows-vm-without-aad': example_201_decrypt_running_windows_vm_without_aad,
  '201/iot-with-storage': example_201_iot_with_storage,
  '201/vnet-peering': example_201_vnet_peering,
  '201/sql': example_201_sql,
  '201/201-web-app-loganalytics': example_201_web_app_loganalytics,
  '101/deployment-script-with-storage': example_101_deployment_script_with_storage,
}