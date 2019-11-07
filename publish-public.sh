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

clone=$work/publish/parallel-wait-for
say "Clean parallel-wait-for clone location: [$clone]"
rm -rf $clone; mkdir -p $(dirname $clone)
say "Loading parallel-wait-for working copy"
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

ver=$(date --utc +"%a, %d %b %Y %T GMT")
pushd Properties
bash -e inc-version-info.sh
ver=$(cat full_version.txt)
popd

for r in linux-x64 linux-arm linux-arm64 osx-x64 linux-musl-x64 rhel.6-x64 win-arm win-arm64 win-x86 win-x64; do

  mkdir -p bin/warped-normal bin/warped-agressive
  say "Warping Normal $r [$ver]"
  dotnet-warp -r $r -o bin/warped-normal/parallel-wait-for-$r -l Normal -v 

  say "Warping Normal $r [$ver]"
  dotnet-warp -r $r -o bin/warped-agressive/parallel-wait-for-$r -l Agressive -v 

  say "Building $r [$ver]"
  time dotnet publish -c Release /p:DefineConstants="DUMPS" -o bin/$r --self-contained -r $r
  pushd bin/$r
  for ext in dll so txt a; do eval "ls *.${ext} >/dev/null 2>&1 && chmod 644 *.${ext} || true"; done
  [[ -x WaitFor ]] && chmod 755 WaitFor

  say "Compressing $r [$ver] as GZIP|ZIP"
  echo $ver > VERSION
  if [[ $r == win* ]]; then
    public_name=parallel-wait-for-$r.zip
    7z a ../${public_name}
  else
    public_name=parallel-wait-for-$r.tar.gz 
    compress="pigz -p 8 -b 128"
    time sudo bash -c "tar cf - . | pv | $compress -9 > ../${public_name}"
  fi
  
  sha256sum ../${public_name} | awk '{print $1}' > ../${public_name}.sha256
  cp ../${public_name}* $clone/public/
  # say "Compressing $r [$ver] as XZ"
  # time sudo bash -c "tar cf - w3top | pv | xz -1 -z > ../w3top-$r.tar.xz"
  # say "Compressing $r [$ver] as 7z"
  # 7z a "../w3top-$r.7z" -m0=lzma -mx=1 -mfb=256 -md=256m -ms=on

  popd
done
echo $ver > $clone/public/VERSION
if [ -n "${SKIP_GIT_PUSH:-}" ]; then exit; fi


pushd $clone >/dev/null
git add --all .
say "Commit binaries [$ver]"
git commit -am "Update $ver"
say "Publish binaries [$ver]"
git push
popd >/dev/null


say "Collecting garbage [$ver]"
bash $clone/git-gc/defrag.sh

say "Done [$ver]"
exit

say "Delete bintray versions except stable [$ver]"
export VERSION_STABLE="$ver"
pushd $root/build
bash delete-bintray-versions-except-stable.sh
popd


say "DONE: [$ver]"

