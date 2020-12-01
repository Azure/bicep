import example_101_wvd_backplane from '../../../docs/examples/101/wvd-backplane/main.bicep'
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
import example_101_web_app_windows from '../../../docs/examples/101/web-app-windows/main.bicep'
import example_201_vm_windows_with_custom_script_extension from '../../../docs/examples/201/vm-windows-with-custom-script-extension/main.bicep'
import example_201_key_vault_secret_create from '../../../docs/examples/201/key-vault-secret-create/main.bicep'
import example_201_vm_push_cert_windows from '../../../docs/examples/201/vm-push-cert-windows/main.bicep'
import example_201_decrypt_running_windows_vm_without_aad from '../../../docs/examples/201/decrypt-running-windows-vm-without-aad/main.bicep'
import example_201_iot_with_storage from '../../../docs/examples/201/iot-with-storage/main.bicep'
import example_201_vnet_peering from '../../../docs/examples/201/vnet-peering/main.bicep'
import example_201_sql from '../../../docs/examples/201/sql/main.bicep'
import example_201_web_app_loganalytics from '../../../docs/examples/201/web-app-loganalytics/main.bicep'
import example_101_deployment_script_with_storage from '../../../docs/examples/101/deployment-script-with-storage/main.bicep'
import example_101_front_door_basic from '../../../docs/examples/101/front-door-basic/main.bicep'
import example_101_front_door_custom_domain from '../../../docs/examples/101/front-door-custom-domain/main.bicep'
import example_101_front_door_redirect from '../../../docs/examples/101/front-door-redirect/main.bicep'
import example_101_mg_policy from '../../../docs/examples/101/mg-policy/main.bicep'
import example_101_aks_vmss_systemassigned_identity from '../../../docs/examples/101/aks-vmss-systemassigned-identity/main.bicep'
import example_101_app_config from '../../../docs/examples/101/app-config/main.bicep'
import example_101_basic_publicip from '../../../docs/examples/101/basic-publicip/main.bicep'
import example_101_container_registry from '../../../docs/examples/101/container-registry/main.bicep'
import example_101_create_rg_lock_role_assignment from '../../../docs/examples/101/create-rg-lock-role-assignment/main.bicep'
import example_101_function_app_create from '../../../docs/examples/101/function-app-create/main.bicep'
import example_101_redis_cache from '../../../docs/examples/101/redis-cache/main.bicep'
import example_101_data_factory_v2_blob_to_blob_copy from '../../../docs/examples/101/data-factory-v2-blob-to-blob-copy/main.bicep'
import example_201_1vm_2nics_2subnets_1vnet from '../../../docs/examples/201/1vm-2nics-2subnets-1vnet/main.bicep'
import example_201_aci_wordpress from '../../../docs/examples/201/aci-wordpress/main.bicep'
import example_201_anchored_proximity_placement_group from '../../../docs/examples/201/anchored-proximity-placement-group/main.bicep'
import example_201_cyclecloud from '../../../docs/examples/201/cyclecloud/main.bicep'
import example_201_firewall_with_ip_from_prefix from '../../../docs/examples/201/firewall-with-ip-from-prefix/main.bicep'
import example_201_log_analytics_with_solutions_and_diagnostics from '../../../docs/examples/201/log-analytics-with-solutions-and-diagnostics/main.bicep'
import example_201_policy_with_initiative_definition_and_assignment from '../../../docs/examples/201/policy-with-initiative-definition-and-assignment/main.bicep'
import example_201_vnet_to_vnet_bgp from '../../../docs/examples/201/vnet-to-vnet-bgp/main.bicep'
import example_201_vwan_shared_services from '../../../docs/examples/201/vwan-shared-services/main.bicep'
import example_301_nested_vms_in_virtual_network from '../../../docs/examples/301/nested-vms-in-virtual-network/main.bicep'

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
  '101/web-app-windows': example_101_web_app_windows,
  '101/data-factory-v2-blob-to-blob-copy': example_101_data_factory_v2_blob_to_blob_copy,
  '101/wvd-backplane': example_101_wvd_backplane,  
  '201/vm-windows-with-custom-script-extension': example_201_vm_windows_with_custom_script_extension,
  '201/key-vault-secret-create': example_201_key_vault_secret_create,
  '201/vm-push-cert-windows': example_201_vm_push_cert_windows,
  '201/decrypt-running-windows-vm-without-aad': example_201_decrypt_running_windows_vm_without_aad,
  '201/iot-with-storage': example_201_iot_with_storage,
  '201/vnet-peering': example_201_vnet_peering,
  '201/sql': example_201_sql,
  '201/201-web-app-loganalytics': example_201_web_app_loganalytics,
  '101/deployment-script-with-storage': example_101_deployment_script_with_storage,
  '101/front-door-basic': example_101_front_door_basic,
  '101/front-door-custom-domain': example_101_front_door_custom_domain,
  '101/front-door-redirect': example_101_front_door_redirect,
  '101/mg-policy': example_101_mg_policy,
  '101/aks-vmss-systemassigned-identity': example_101_aks_vmss_systemassigned_identity,
  '101/app-config': example_101_app_config,
  '101/basic-publicip': example_101_basic_publicip,
  '101/container-registry': example_101_container_registry,
  '101/create-rg-lock-role-assignment': example_101_create_rg_lock_role_assignment,
  '101/function-app-create': example_101_function_app_create,
  '101/redis-cache': example_101_redis_cache,
  '201/1vm-2nics-2subnets-1vnet': example_201_1vm_2nics_2subnets_1vnet,
  '201/aci-wordpress': example_201_aci_wordpress,
  '201/anchored-proximity-placement-group': example_201_anchored_proximity_placement_group,
  '201/cyclecloud': example_201_cyclecloud,
  '201/firewall-with-ip-from-prefix': example_201_firewall_with_ip_from_prefix,
  '201/log-analytics-with-solutions-and-diagnostics': example_201_log_analytics_with_solutions_and_diagnostics,
  '201/policy-with-initiative-definition-and-assignment': example_201_policy_with_initiative_definition_and_assignment,
  '201/vnet-to-vnet-bgp': example_201_vnet_to_vnet_bgp,
  '201/vwan-shared-services': example_201_vwan_shared_services,
  '301/nested-vms-in-virtual-network': example_301_nested_vms_in_virtual_network
}
