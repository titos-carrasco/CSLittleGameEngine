LGE="../../rcr/lge"
TARGET="exe"
OUT="demo02.exe"

rm "$OUT"
export MONO_PATH="$LGE"

mcs -pkg:dotnet -r:"$LGE/lge.dll" -target:"$TARGET" -out:"$OUT" *.cs && mono "$OUT"