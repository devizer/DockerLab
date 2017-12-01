#!/bin/bash
builddate=$(date --utc +"%a, %d %b %Y %T GMT"); 
echo "[assembly: Universe.AssemblyBuildDateTime(\"$builddate\")]" | tee Properties/AssemblyBuildDate.cs
