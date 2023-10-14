#!/bin/bash

# Generate some test certificates:

generate_cert() {
    local name=$1
    local cn="$2"
    local opts="$3"
    local conf="$4"

    local keyfile=tests/tls/${name}.key
    local certfile=tests/tls/${name}.crt

    [ -f $keyfile ] || openssl genrsa -out $keyfile 2048
    openssl req \
        -new -sha256 \
        -config $conf \
        -key $keyfile | \
        openssl x509 \
            -req -sha256 \
            -CA tests/tls/ca.crt \
            -CAkey tests/tls/ca.key \
            -CAserial tests/tls/ca.txt \
            -CAcreateserial \
            -days 365 \
            $opts \
            -out $certfile
}

mkdir -p tests/tls

# Create the CA configuration
cat > req-ca.conf <<EOF
[req]
distinguished_name = req_distinguished_name
prompt = no

[req_distinguished_name]
O = CodeDesignPlus
CN = Certificate Authority
C = CO
ST = Bogotá
L = Bogotá
OU = IT Department
EOF

# Generate CA Certificate
[ -f tests/tls/ca.key ] || openssl genrsa -out tests/tls/ca.key 4096
openssl req \
    -x509 -new -nodes -sha256 \
    -key tests/tls/ca.key \
    -days 3650 \
    -config req-ca.conf \
    -out tests/tls/ca.crt

# Create the Redis configuration
cat > req-redis.conf <<EOF
[req]
distinguished_name = req_distinguished_name
prompt = no

[req_distinguished_name]
O = CodeDesignPlus
C = CO
ST = Bogotá
L = Bogotá
OU = IT Department
EOF

cat > tests/tls/openssl.cnf <<_END_
[ server_cert ]
keyUsage = digitalSignature, keyEncipherment
nsCertType = server
[ client_cert ]
keyUsage = digitalSignature, keyEncipherment
nsCertType = client
_END_

generate_cert server "Server-only" "-extfile tests/tls/openssl.cnf -extensions server_cert" "req-redis.conf"
generate_cert client "Client-only" "-extfile tests/tls/openssl.cnf -extensions client_cert" "req-redis.conf"
generate_cert redis "Generic-cert" "" "req-redis.conf"

[ -f tests/tls/redis.dh ] || openssl dhparam -out tests/tls/redis.dh 2048

mkdir ../Certificates
cp -r tests/tls/* ../Certificates
rm -rf tests req-ca.conf req-redis.conf