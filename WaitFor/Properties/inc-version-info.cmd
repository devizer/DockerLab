@echo off

rem BUILD DATE
for /f "delims=" %%i in ('powershell -command "[System.DateTime]::UtcNow.ToString(\"R\")"') DO set datetime=%%i
echo DATETIME: %datetime%
echo [assembly: Universe.AssemblyBuildDateTime( "%datetime%" )] > AssemblyBuildDate.cs

rem VERSION
for /F %%v in (build-number.txt) DO set MYBUILD=%%v
for /F %%b in (version-number.txt) DO set MYVERSION=%%b
Set /A MYBUILD=%MYBUILD% + 1
Echo NEW Build: "%MYBUILD%"
Echo %MYBUILD% > build-number.txt

echo [assembly: System.Reflection.AssemblyVersion("%MYVERSION%.%MYBUILD%")] > AssemblyVersion.cs
