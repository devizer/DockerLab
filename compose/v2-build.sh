#!/usr/bin/env bash
echo MSBuildSDKsPath: ${MSBuildSDKsPath}
for prj in WaitFor HelloRest; do
  pushd ../$prj
  rm -rf ../compose/bin/$prj
  dotnet publish -o ../compose/bin/$prj -r linux-x64 --self-contained
  popd
done
