#!/usr/bin/env bash
for prj in WaitFor HelloRest; do
  pushd ../$prj
  # mkdir -p ../compose/bin/$prj
  dotnet publish -o ../compose/bin/$prj
  popd
done
