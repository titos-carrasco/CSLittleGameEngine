TARGET="library"
OUT="lge.dll"

rm "$OUT"
mcs -pkg:dotnet -target:"$TARGET" -out:"$OUT" *.cs
