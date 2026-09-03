param([string]$GameDir='C:\Program Files (x86)\Steam\steamapps\common\TheLongDark',[string]$Configuration='Release')
$ErrorActionPreference='Stop'; $repo=Split-Path -Parent $MyInvocation.MyCommand.Path; $out=Join-Path $repo 'artifacts'
New-Item -ItemType Directory -Force -Path $out | Out-Null
dotnet build $repo -c $Configuration -p:TLDPath=$GameDir -p:OutputPath=$out
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }; Write-Host "Compilación terminada: $out"
