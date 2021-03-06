#!/usr/bin/env bash
echo MSBuildSDKsPath: ${MSBuildSDKsPath}
# dotnet restore --disable-parallel
for prj in HelloRest WaitFor; do
  pushd ../$prj
  rm -rf ../compose/bin/$prj
  # dotnet publish -o ../compose/bin/$prj -r linux-x64 --self-contained -c Debug
  dotnet publish -o ../compose/bin/$prj -c Debug
  popd
done

# echo runtimeconfig.json:
# cat bin/WaitFor/*runtimeconfig.json

