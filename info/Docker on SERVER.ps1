Install-WindowsFeature -Name Containers
Uninstall-WindowsFeature Windows-Defender
# Restart-Computer -Force 

Install-Module DockerMsftProvider -Force
Install-Package Docker -ProviderName DockerMsftProvider -Force

docker run mcr.microsoft.com/powershell:nanoserver pwsh -c `$PSVersionTable


