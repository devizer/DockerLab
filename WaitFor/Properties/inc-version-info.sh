#!/bin/bash
builddate=$(date --utc +"%a, %d %b %Y %T GMT"); 
echo "[assembly: Universe.AssemblyBuildDateTime(\"$builddate\")]" | tee AssemblyBuildDate.cs

MYBUILD=$(cat build-number.txt)
MYVERSION=$(cat version-number.txt)
MYBUILD=$((MYBUILD + 1))
echo NEW Build: "${MYBUILD}"
echo ${MYBUILD} > build-number.txt
commits=$(git log -n 999999 --date=raw --pretty=format:"%cd" | wc -l)
full_version="${MYVERSION}.${MYBUILD}.${commits}"
echo ${full_version} > full_version.txt 

echo "[assembly: System.Reflection.AssemblyVersion(\"${full_version}\")]" | tee AssemblyVersion.cs
