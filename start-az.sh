#!/bin/bash

docker run -v ${HOME}:/root  -v /usr/local/bin:/tools -it azuresdk/azure-cli-python:latest