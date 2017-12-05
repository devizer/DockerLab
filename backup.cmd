for /f "delims=;" %%i in ('powershell -command "[System.DateTime]::Now.ToString(\"yyyy-MM-dd,HH-mm-ss\")"') DO set datetime=%%i

"C:\Program Files\7-Zip\7zG.exe" a -t7z -mx=9 -mfb=128 -md=128m -ms=on -xr!.git -xr!bin -xr!obj -xr!.vs ^
  "C:\Users\Backups on Google Drive\DockerLab (%datetime%).7z" .
