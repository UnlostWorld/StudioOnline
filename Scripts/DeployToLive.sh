#! /usr/bin/bash
echo "Updating Studio Online..."

# attach to the tmux session
tmux attach -t StudioOnline

# stop the server?
kill $!

# get latest from git
git pull

#build latest
dotnet publish --configuration Release

dotnet ./bin/StudioOnline.dll