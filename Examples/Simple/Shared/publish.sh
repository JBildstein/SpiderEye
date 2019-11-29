#!/bin/bash 

PublishProject()
{
    echo "Publishing for $1..."
    dotnet publish App.$1 -c Release -f netcoreapp3.0 -r $2 -o Publish/$1 >/dev/null
    if [ "$?" -ne "0" ]; then
      echo "Publish failed"
      exit 1
    fi
}

cd ..
# PublishProject Windows win-x64
echo "Windows app can only be built on Windows"
PublishProject Linux linux-x64
PublishProject Mac osx-x64

echo "Done!"
exit 0
