#!/bin/bash
sleep 2
REDIS0_IP=$(getent hosts redis0 | awk '{ print $1 }')
REDIS1_IP=$(getent hosts redis1 | awk '{ print $1 }')
REDIS2_IP=$(getent hosts redis2 | awk '{ print $1 }')
REDIS3_IP=$(getent hosts redis3 | awk '{ print $1 }')
REDIS4_IP=$(getent hosts redis4 | awk '{ print $1 }')
REDIS5_IP=$(getent hosts redis5 | awk '{ print $1 }')
echo "Redis0 IP: $REDIS0_IP"
echo "Redis1 IP: $REDIS1_IP"
echo "Redis2 IP: $REDIS2_IP"
echo "Redis3 IP: $REDIS3_IP"
echo "Redis4 IP: $REDIS4_IP"
echo "Redis5 IP: $REDIS5_IP"
redis-cli --cluster create $REDIS0_IP:7379 $REDIS1_IP:7380 $REDIS2_IP:7381 $REDIS3_IP:7382 $REDIS4_IP:7383 $REDIS5_IP:7384 --cluster-replicas 1 --tls -a 12345678 --cert /usr/local/etc/redis/redis.crt --key /usr/local/etc/redis/redis.key --cacert /usr/local/etc/redis/ca.crt