#!/bin/bash

docker run --rm -d -p 6379:6379 redis
docker run --rm -d -p 2379:2379 quay.io/coreos/etcd:v3.4.14 etcd --listen-client-urls http://0.0.0.0:2379 --advertise-client-urls http://127.0.0.1:2379