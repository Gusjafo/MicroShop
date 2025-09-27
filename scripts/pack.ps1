$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
$Root = Join-Path $Root ".."
$Out = Join-Path $Root ".nuget/feed"
New-Item -ItemType Directory -Force -Path $Out | Out-Null
dotnet pack "$Root/src/BuildingBlocks/Contracts" -c Release -o $Out
dotnet pack "$Root/src/BuildingBlocks/Abstractions" -c Release -o $Out
"Packages in $Out:"; Get-ChildItem $Out | ForEach-Object { " - " + $_.Name }
