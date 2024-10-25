export VAULT_ADDR="http://0.0.0.0:8200"

vault login token=root

# Enable Secret, database and rabbtimq
# unit-test = solution name (software)

# Enable AppRole
echo "1. Enabling auth methods..."
vault auth enable approle

# Enable Secret, database and rabbtimq
echo "2. Enabling secrets engines..."
vault secrets enable -path=unit-test-keyvalue kv-v2
vault secrets enable -path=unit-test-database database
vault secrets enable -path=unit-test-rabbitmq rabbitmq
vault secrets enable -path=unit-test-transit transit

# Create policies
echo "3. Applying policies..."
vault policy write full-access /resources/full-access.hcl

# Create roles
echo "4. Creating roles..."
vault write auth/approle/role/unit-test-approle policies="full-access"

role_id=$(vault read auth/approle/role/unit-test-approle/role-id | grep 'role_id' | awk '{print $2}')

secret_id=$(vault write -f auth/approle/role/unit-test-approle/secret-id | grep 'secret_id ' | awk '{print $2}')

if [ -z "$role_id" ] || [ -z "$secret_id" ]; then
    echo "Error: Not found role_id or secret_id"
    exit 1
fi

file="$FILE_CREDENTIAL-credentials.json"

cat <<EOF > /vault/config/$file
{
  "role_id": "$role_id",
  "secret_id": "$secret_id"
}
EOF

echo "Role ID: $role_id"
echo "Secret ID: $secret_id"

# Login with approle
echo "5. Login with approle..."
vault write auth/approle/login role_id=$role_id secret_id=$secret_id

# Write secrets
echo "6. Writing secrets..."
vault kv put -mount=unit-test-keyvalue my-app Security:ClientId=a74cb192-598c-4757-95ae-b315793bbbca Security:ValidAudiences:0=a74cb192-598c-4757-95ae-b315793bbbca Security:ValidAudiences:1=api://a74cb192-598c-4757-95ae-b315793bbbca
vault kv get -mount=unit-test-keyvalue my-app

# Write database configuration
echo "7. Writing database configuration..."
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

# Write rabbitmq configuration
echo "8. Writing rabbitmq configuration..."

sleep 15

vault write unit-test-rabbitmq/config/connection \
    connection_uri="http://rabbitmq:15672" \
    username="admin" \
    password="password"

vault write unit-test-rabbitmq/roles/my-app-rabbitmq-role \
    vhosts='{"/":{"write": ".*", "read": ".*", "configure": ".*"}}'

vault read unit-test-rabbitmq/creds/my-app-rabbitmq-role