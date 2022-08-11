@secure()
param kubeConfig string

import kubernetes as k8s {
  namespace: 'default'
  kubeConfig: kubeConfig
}

resource corePersistentVolume_wordpressPv1 'core/PersistentVolume@v1' = {
  metadata: {
    name: 'wordpress-pv-1'
  }
  spec: {
    capacity: {
      storage: '20Gi'
    }
    accessModes: [
      'ReadWriteOnce'
    ]
    gcePersistentDisk: {
      pdName: 'wordpress-1'
      fsType: 'ext4'
    }
  }
}

resource corePersistentVolume_wordpressPv2 'core/PersistentVolume@v1' = {
  metadata: {
    name: 'wordpress-pv-2'
  }
  spec: {
    capacity: {
      storage: '20Gi'
    }
    accessModes: [
      'ReadWriteOnce'
    ]
    gcePersistentDisk: {
      pdName: 'wordpress-2'
      fsType: 'ext4'
    }
  }
}

resource corePersistentVolume_localPv1 'core/PersistentVolume@v1' = {
  metadata: {
    name: 'local-pv-1'
    labels: {
      type: 'local'
    }
  }
  spec: {
    capacity: {
      storage: '20Gi'
    }
    accessModes: [
      'ReadWriteOnce'
    ]
    hostPath: {
      path: '/tmp/data/pv-1'
    }
  }
}

resource corePersistentVolume_localPv2 'core/PersistentVolume@v1' = {
  metadata: {
    name: 'local-pv-2'
    labels: {
      type: 'local'
    }
  }
  spec: {
    capacity: {
      storage: '20Gi'
    }
    accessModes: [
      'ReadWriteOnce'
    ]
    hostPath: {
      path: '/tmp/data/pv-2'
    }
  }
}

resource coreService_wordpressMysql 'core/Service@v1' = {
  metadata: {
    name: 'wordpress-mysql'
    labels: {
      app: 'wordpress'
    }
  }
  spec: {
    ports: [
      {
        port: 3306
      }
    ]
    selector: {
      app: 'wordpress'
      tier: 'mysql'
    }
    clusterIP: 'None'
  }
}

resource corePersistentVolumeClaim_mysqlPvClaim 'core/PersistentVolumeClaim@v1' = {
  metadata: {
    name: 'mysql-pv-claim'
    labels: {
      app: 'wordpress'
    }
  }
  spec: {
    accessModes: [
      'ReadWriteOnce'
    ]
    resources: {
      requests: {
        storage: '20Gi'
      }
    }
  }
}

resource appsDeployment_wordpressMysql 'apps/Deployment@v1' = {
  metadata: {
    name: 'wordpress-mysql'
    labels: {
      app: 'wordpress'
    }
  }
  spec: {
    selector: {
      matchLabels: {
        app: 'wordpress'
        tier: 'mysql'
      }
    }
    strategy: {
      type: 'Recreate'
    }
    template: {
      metadata: {
        labels: {
          app: 'wordpress'
          tier: 'mysql'
        }
      }
      spec: {
        containers: [
          {
            image: 'mysql:5.6'
            name: 'mysql'
            env: [
              {
                name: 'MYSQL_ROOT_PASSWORD'
                valueFrom: {
                  secretKeyRef: {
                    name: 'mysql-pass'
                    key: 'password'
                  }
                }
              }
            ]
            livenessProbe: {
              tcpSocket: {
                port: 3306
              }
            }
            ports: [
              {
                containerPort: 3306
                name: 'mysql'
              }
            ]
            volumeMounts: [
              {
                name: 'mysql-persistent-storage'
                mountPath: '/var/lib/mysql'
              }
            ]
          }
        ]
        volumes: [
          {
            name: 'mysql-persistent-storage'
            persistentVolumeClaim: {
              claimName: 'mysql-pv-claim'
            }
          }
        ]
      }
    }
  }
}

resource coreService_wordpress 'core/Service@v1' = {
  metadata: {
    name: 'wordpress'
    labels: {
      app: 'wordpress'
    }
  }
  spec: {
    ports: [
      {
        port: 80
      }
    ]
    selector: {
      app: 'wordpress'
      tier: 'frontend'
    }
    type: 'LoadBalancer'
  }
}

resource corePersistentVolumeClaim_wpPvClaim 'core/PersistentVolumeClaim@v1' = {
  metadata: {
    name: 'wp-pv-claim'
    labels: {
      app: 'wordpress'
    }
  }
  spec: {
    accessModes: [
      'ReadWriteOnce'
    ]
    resources: {
      requests: {
        storage: '20Gi'
      }
    }
  }
}

resource appsDeployment_wordpress 'apps/Deployment@v1' = {
  metadata: {
    name: 'wordpress'
    labels: {
      app: 'wordpress'
    }
  }
  spec: {
    selector: {
      matchLabels: {
        app: 'wordpress'
        tier: 'frontend'
      }
    }
    strategy: {
      type: 'Recreate'
    }
    template: {
      metadata: {
        labels: {
          app: 'wordpress'
          tier: 'frontend'
        }
      }
      spec: {
        containers: [
          {
            image: 'wordpress:4.8-apache'
            name: 'wordpress'
            env: [
              {
                name: 'WORDPRESS_DB_HOST'
                value: 'wordpress-mysql'
              }
              {
                name: 'WORDPRESS_DB_PASSWORD'
                valueFrom: {
                  secretKeyRef: {
                    name: 'mysql-pass'
                    key: 'password'
                  }
                }
              }
            ]
            ports: [
              {
                containerPort: 80
                name: 'wordpress'
              }
            ]
            volumeMounts: [
              {
                name: 'wordpress-persistent-storage'
                mountPath: '/var/www/html'
              }
            ]
          }
        ]
        volumes: [
          {
            name: 'wordpress-persistent-storage'
            persistentVolumeClaim: {
              claimName: 'wp-pv-claim'
            }
          }
        ]
      }
    }
  }
}