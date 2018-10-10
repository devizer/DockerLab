rm -rf /opt/dotnet
mkdir -p /opt/dotnet
cd /opt/dotnet
dotnet_2_2_url=https://download.microsoft.com/download/D/5/9/D593CD8F-04E7-425D-962C-86FF4C90B1DA/dotnet-sdk-2.2.100-preview2-009404-linux-arm64.tar.gz
dotnet_2_2_url=https://dotnetcli.blob.core.windows.net/dotnet/Sdk/master/dotnet-sdk-latest-linux-arm64.tar.gz
dotnet_2_1_url=https://download.visualstudio.microsoft.com/download/pr/00038a67-bb86-4c39-88df-7c0998002a9e/97de51fd691c68e18ddd3dcaf3d60181/dotnet-sdk-2.1.403-linux-arm64.tar.gz
for url in $dotnet_2_1_url $dotnet_2_2_url; do
  file=$(basename -- "$url")
  echo downloading $file
  wget --no-check-certificate -O "$file" "$url"
  echo extracting $file
  pv "$file" | tar xzf -
  rm -f "$file"
done

export PATH="/opt/dotnet:$PATH"
dotnet --info

cd /tmp
rm -rf console; mkdir -p console; cd console
dotnet new console
dotnet run

