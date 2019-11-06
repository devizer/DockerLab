#!/usr/bin/env bash
set -e
set -u

export DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0
function header() {
  if [[ $(uname -s) != Darwin ]]; then
    startAt=${startAt:-$(date +%s)}; elapsed=$(date +%s); elapsed=$((elapsed-startAt)); elapsed=$(TZ=UTC date -d "@${elapsed}" "+%_H:%M:%S");
  fi
  LightGreen='\033[1;32m'; Yellow='\033[1;33m'; RED='\033[0;31m'; NC='\033[0m'; LightGray='\033[1;2m';
  printf "${LightGray}${elapsed:-}${NC} ${LightGreen}$1${NC} ${Yellow}$2${NC}\n"; 
}
counter=0;
function say() { counter=$((counter+1)); header "STEP $counter" "$1"; }

work=$HOME/transient-builds
if [[ -d "/transient-builds" ]]; then work=/transient-builds; fi
if [[ -d "/ssd" ]]; then work=/ssd/transient-builds; fi

clone=$work/publish/WaitFor
say "Clean WaitFor clone location: [$clone]"
rm -rf $clone; mkdir -p $(dirname $clone)
say "Loading WaitFor working copy"
if [ -n "${SKIP_GIT_PUSH:-}" ]; then waitForBinRepo=https://github.com/devizer/parallel-wait-for; else waitForBinRepo=git@github.com:devizer/parallel-wait-for; fi
git clone ${waitForBinRepo} $clone



work=$work/publish/DockerLab;
say "Loading source to [$work]"
# work=/mnt/ftp-client/KernelManagementLab;
mkdir -p "$(dirname $work)"
cd $(dirname $work);
rm -rf $work;
git clone https://github.com/devizer/DockerLab;
cd DockerLab
root=$(pwd)
cd WaitFor
dir=$(pwd)

ver=not-changed

for r in linux-musl-x64 rhel.6-x64 linux-x64 linux-arm linux-arm64 osx-x64; do

  say "Building $r [$ver]"
  time SKIP_CLIENTAPP=true dotnet publish -c Release /p:DefineConstants="DUMPS" -o bin/$r --self-contained -r $r
  pushd bin/$r
  chmod 644 *.dll
  chmod 755 WaitFor

  say "Compressing $r [$ver] as GZIP"
  echo $ver > VERSION
  compress="pigz -p 8 -b 128"
  time sudo bash -c "tar cf - . | pv | $compress -9 > ../parallel-wait-for-$r.tar.gz"
  sha256sum ../parallel-wait-for-$r.tar.gz | awk '{print $1}' > ../parallel-wait-for-$r.tar.gz.sha256
  cp ../parallel-wait-for-$r.tar.gz* $clone/public/
  # say "Compressing $r [$ver] as XZ"
  # time sudo bash -c "tar cf - w3top | pv | xz -1 -z > ../w3top-$r.tar.xz"
  # say "Compressing $r [$ver] as 7z"
  # 7z a "../w3top-$r.7z" -m0=lzma -mx=1 -mfb=256 -md=256m -ms=on

  popd
done

if [ -n "${SKIP_GIT_PUSH:-}" ]; then exit; fi



pushd $clone >/dev/null
git add --all .
say "Commit binaries [$ver]"
git commit -am "Update $ver"
say "Publish binaries [$ver]"
git push
popd >/dev/null

exit

say "Collecting garbage"
bash $clone/git-gc/defrag.sh

say "Delete bintray versions except stable [$ver]"
export VERSION_STABLE="$ver"
pushd $root/build
bash delete-bintray-versions-except-stable.sh
popd


say "DONE: [$ver]"

