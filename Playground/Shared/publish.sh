#!/bin/bash 

PublishProject()
{
  echo "Publishing for $1..."
  dotnet publish SpiderEye.Playground.$1 -c Release -f netcoreapp3.0 -r $2 -o Publish/$1 >/dev/null
  if [ "$?" -ne "0" ]; then
    echo "Publish failed"
    exit 1
  fi
}

cd ../SpiderEye.Playground.Core
echo "Installing node packages..."
npm i &>/dev/null
if [ "$?" -ne "0" ]; then
  echo "Installing node packages failed"
  exit 1
fi

echo "Linting files..."
npm run lint >/dev/null
if [ "$?" -ne "0" ]; then
  echo "Linting failed"
  exit 1
fi

echo "Building Angular..."
npm run build:prod >/dev/null
if [ "$?" -ne "0" ]; then
  echo "Angular build failed"
  exit 1
fi

cd ..
# PublishProject Windows win-x64
echo "Windows app can only be built on Windows"
PublishProject Linux linux-x64
PublishProject Mac osx-x64

echo "Done!"
exit 0
