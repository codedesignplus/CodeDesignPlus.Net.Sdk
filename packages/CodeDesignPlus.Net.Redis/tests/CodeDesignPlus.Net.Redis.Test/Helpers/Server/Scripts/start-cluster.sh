#!/bin/bash

REDIS0_IP=$(getent hosts redis0 | awk '{ print $1 }')
REDIS1_IP=$(getent hosts redis1 | awk '{ print $1 }')
REDIS2_IP=$(getent hosts redis2 | awk '{ print $1 }')
REDIS3_IP=$(getent hosts redis3 | awk '{ print $1 }')
REDIS4_IP=$(getent hosts redis4 | awk '{ print $1 }')
REDIS5_IP=$(getent hosts redis5 | awk '{ print $1 }')

redis-cli --cluster create $REDIS0_IP:7379 $REDIS1_IP:7380 $REDIS2_IP:7381 $REDIS3_IP:7382 $REDIS4_IP:7383 $REDIS5_IP:7384 --cluster-replicas 1 --tls -a 12345678 --cert /usr/local/etc/redis/redis.crt --key /usr/local/etc/redis/redis.key --cacert /usr/local/etc/redis/ca.crt
