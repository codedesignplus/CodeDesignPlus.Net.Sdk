#!/bin/sh
export VAULT_ADDR='http://0.0.0.0:8200'

vault login token=root

# Enable Secret, database and rabbtimq
# unit-test = solution name (software)
vault secrets enable -path=unit-test-keyvalue kv-v2
vault secrets enable -path=unit-test-database database
vault secrets enable -path=unit-test-rabbitmq rabbitmq 
vault secrets enable -path=unit-test-transit transit 

# List Secrets
vault secrets list

# Create Policy
vault policy write full-access /resources/full-access.hcl

# Enable AppRole
vault auth enable approle

# Create AppRole 
vault write auth/approle/role/unit-test-approle policies="full-access"

# Get role_id using grep y awk
role_id=$(vault read auth/approle/role/unit-test-approle/role-id | grep 'role_id' | awk '{print $2}')

# Get secret_id use grep y awk
secret_id=$(vault write -f auth/approle/role/unit-test-approle/secret-id | grep 'secret_id ' | awk '{print $2}')

if [ -z "$role_id" ] || [ -z "$secret_id" ]; then
    echo "Error: Not found role_id or secret_id"
    exit 1
fi

# Save role_id and secret_id to a JSON file
rm -rf /vault/config/credentials.json

cat <<EOF > /vault/config/credentials.json
{
  "role_id": "$role_id",
  "secret_id": "$secret_id"
}
EOF

echo "Role ID: $role_id"
echo "Secret ID: $secret_id"

# Login with AppRole
vault write auth/approle/login role_id=$role_id secret_id=$secret_id

# Create Secret
vault kv put -mount=unit-test-keyvalue my-app Security:ClientId=a74cb192-598c-4757-95ae-b315793bbbca Security:ValidAudiences:0=a74cb192-598c-4757-95ae-b315793bbbca Security:ValidAudiences:1=api://a74cb192-598c-4757-95ae-b315793bbbca

# Get Secret
vault kv get -mount=unit-test-keyvalue my-app

# Create Database
echo "Create Database"
vault write unit-test-database/config/db-my-app \
    plugin_name=mongodb-database-plugin \
    allowed_roles="my-app-mongo-role" \
    connection_url="mongodb://{{username}}:{{password}}@mongo:27017/admin?ssl=false" \
    username="admin" \
    password="password"


vault write unit-test-database/roles/my-app-mongo-role \
    db_name=db-my-app \
    creation_statements='{ "db": "admin", "roles": [{ "role": "readWrite", "db": "db-my-app" }] }' \
    default_ttl="1h" \
    max_ttl="24h"

vault read unit-test-database/creds/my-app-mongo-role

# Create RabbitMQ

# sleep 15 seconds
echo "Sleep 15 seconds"

sleep 15

echo "Create RabbitMQ"

vault write unit-test-rabbitmq/config/connection \
    connection_uri="http://rabbitmq:15672" \
    username="admin" \
    password="password"

vault write unit-test-rabbitmq/roles/my-app-rabbitmq-role \
    vhosts='{"/":{"write": ".*", "read": ".*", "configure": ".*"}}'

vault read unit-test-rabbitmq/creds/my-app-rabbitmq-role

