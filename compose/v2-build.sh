#!/usr/bin/env bash
echo MSBuildSDKsPath: ${MSBuildSDKsPath}
# dotnet restore --disable-parallel
for prj in WaitFor HelloRest; do
  pushd ../$prj
  rm -rf ../compose/bin/$prj
  dotnet publish -o ../compose/bin/$prj -r linux-x64 --self-contained -c Debug
  popd
done
