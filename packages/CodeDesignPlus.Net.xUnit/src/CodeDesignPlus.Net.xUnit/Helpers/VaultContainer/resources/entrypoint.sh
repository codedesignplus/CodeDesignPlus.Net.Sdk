#!/bin/sh

echo "Starting Vault server in dev mode..."
vault server -dev &

# Esperar a que Vault esté disponible
echo "Esperando a que Vault esté disponible..."
sleep 2

echo "Vault is up and running. Executing configuration script..."
/resources/config.sh

# Mantener el contenedor en ejecución
wait