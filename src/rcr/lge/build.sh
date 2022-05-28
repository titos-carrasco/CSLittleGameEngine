LANGVERSION="6"
SDK="4.5"
TARGET="library"
OUT="lge.dll"

rm "$OUT"
mcs -pkg:dotnet -langversion:"$LANGVERSION" -sdk:"$SDK"  -target:"$TARGET" -out:"$OUT" *.cs
