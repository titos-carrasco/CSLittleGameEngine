LGE="../../rcr/lge"
LANGVERSION="6"
SDK="4.5"
TARGET="exe"
OUT="betty.exe"

rm "$OUT"
export MONO_PATH="$LGE"

mcs -pkg:dotnet -lib:"$LGE" -r:"$LGE/lge.dll" -langversion:"$LANGVERSION" -sdk:"$SDK" -target:"$TARGET" -out:"$OUT" *.cs && mono "$OUT"
