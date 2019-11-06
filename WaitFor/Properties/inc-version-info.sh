#!/bin/bash
builddate=$(date --utc +"%a, %d %b %Y %T GMT"); 
echo "[assembly: Universe.AssemblyBuildDateTime(\"$builddate\")]" | tee AssemblyBuildDate.cs

MYBUILD=$(cat build-number.txt)
MYVERSION=$(cat version-number.txt)
MYBUILD=$((MYBUILD + 1))
echo NEW Build: "${MYBUILD}"
echo ${MYBUILD} > build-number.txt

echo "[assembly: System.Reflection.AssemblyVersion(\"${MYVERSION}.${MYBUILD}\")]" | tee AssemblyVersion.cs
