#!/bin/bash

# Check if server's address is provided
if [ "$#" -ne 1 ]; then
    echo "Usage: $0 <server_address>"
    exit 1
fi

SERVER_ADDRESS=$1

while true; do
    # Generate a random number between 1 and 100
    RANDOM_NUM=$(( RANDOM % 100 + 1 ))

    # Curl different paths based on the random number
    if (( RANDOM_NUM <= 80 )); then
        curl http://$SERVER_ADDRESS/weatherforecast; echo
    elif (( RANDOM_NUM <= 85 )); then
        curl http://$SERVER_ADDRESS/fail; echo
    else
        curl http://$SERVER_ADDRESS/notfound; echo
    fi

    # Sleep for a random number of milliseconds between 100 and 500
    SLEEP_TIME=$(( RANDOM % 400 + 100 ))
    sleep 0.$SLEEP_TIME
done
