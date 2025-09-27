#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.."; pwd)"
OUT="$ROOT/.nuget/feed"
mkdir -p "$OUT"
dotnet pack "$ROOT/src/BuildingBlocks/Contracts" -c Release -o "$OUT"
dotnet pack "$ROOT/src/BuildingBlocks/Abstractions" -c Release -o "$OUT"
echo "Packages in $OUT:"; ls -1 "$OUT" | sed 's/^/ - /'
