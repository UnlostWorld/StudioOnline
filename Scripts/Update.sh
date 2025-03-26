#! /usr/bin/bash
echo "Updating Studio Online..."
cd ..
echo "Stopping Server"
pkill dotnet
echo "Getting latest"
git pull
echo "building"
dotnet publish --configuration Release
echo "Starting Server"
(dotnet ./bin/StudioOnline.dll &)