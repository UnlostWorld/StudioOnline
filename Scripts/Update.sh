#! /usr/bin/bash
echo "Updating Studio Online..."
cd ./StudioOnline/
echo "Stopping Server"
pkill dotnet
echo "Getting latest"
git pull
echo "Starting Server"
(dotnet run &)