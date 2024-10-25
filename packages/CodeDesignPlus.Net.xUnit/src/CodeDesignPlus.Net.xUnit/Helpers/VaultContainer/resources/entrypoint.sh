#!/bin/sh

echo "Starting Vault server in dev mode..."
vault server -dev &

# Esperar a que Vault esté disponible
echo "Esperando a que Vault esté disponible..."
sleep 5

echo "Vault is up and running. Executing configuration script..."
sh /resources/config.sh

# Mantener el contenedor en ejecución
wait