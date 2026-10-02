<#
.SYNOPSIS
    自動更新の配布ファイル (zip) と manifest.json を作る。本体とマップ情報は別々に作れる。

.EXAMPLE
    # 本体 (この版を付けて publish する)
    ./tools/make-release.ps1 -Component app -Version 1.1.0 -Notes "〇〇を直しました"

.EXAMPLE
    # マップ情報 (data フォルダー全体。zip の version.txt にこの版を書く。リポジトリの data は変えない)
    ./tools/make-release.ps1 -Component maps -Version 100

.NOTES
    出力フォルダー (既定 release/) の中身 (manifest.json と zip) を、そのまま設定 update_url の場所に置く。
    manifest.json の url は zip のファイル名 (manifest.json からの相対) で書くので、同じ場所に置けばよい (-BaseUrl で絶対 URL にもできる)。
    GitHub では .github/workflows/release.yml がタグの push でこれを実行する。
    既存の manifest.json があれば、指定したコンポーネントだけ書き換える (もう一方はそのまま)。
#>
param(
    [Parameter(Mandatory)][ValidateSet('app', 'maps')][string]$Component,
    # 本体だけ 1.2.3-beta.1 のようなプレリリース版も付けられる (その場合は -NoManifest も付ける)
    [Parameter(Mandatory)][ValidatePattern('^\d+(\.\d+)*(-[0-9A-Za-z][0-9A-Za-z.]*)?$')][string]$Version,
    # manifest.json を書き換えない (ベータなど、自動更新で配らない版)
    [switch]$NoManifest,
    [string]$Notes = '',
    # zip の URL の前に付ける (GitHub Releases の download URL など)。空なら manifest.json からの相対
    [string]$BaseUrl = '',
    [string]$Output = (Join-Path $PSScriptRoot '..\release')
)

$ErrorActionPreference = 'Stop'
if ($Version.Contains('-') -and $Component -ne 'app') { throw 'プレリリース版 (1.2.3-beta.1 など) は本体だけです。' }
if ($Version.Contains('-') -and -not $NoManifest) { throw 'プレリリース版は自動更新で配らないので -NoManifest を付けてください。' }
$repo = Resolve-Path (Join-Path $PSScriptRoot '..')
New-Item -ItemType Directory -Force $Output | Out-Null
$Output = Resolve-Path $Output
$staging = Join-Path ([IO.Path]::GetTempPath()) "ggame-release-$([guid]::NewGuid().ToString('N'))"

try {
    if ($Component -eq 'app') {
        dotnet publish (Join-Path $repo 'src\gGameMapOverlay') -c Release -r win-x64 --self-contained false -o $staging -p:Version=$Version
        if ($LASTEXITCODE -ne 0) { throw 'dotnet publish に失敗しました。' }
        # data (マップ情報) は別に配布する。設定・モデルは利用者のもの
        foreach ($excluded in 'data', 'models', 'config.json') {
            $path = Join-Path $staging $excluded
            if (Test-Path $path) { Remove-Item -Recurse -Force $path }
        }
        Get-ChildItem $staging -Filter '*.pdb' -Recurse | Remove-Item
    }
    else {
        New-Item -ItemType Directory $staging | Out-Null
        Copy-Item (Join-Path $repo 'data\*') $staging -Recurse
        Set-Content -NoNewline -Encoding ascii (Join-Path $staging 'version.txt') $Version
    }

    # 本体の正式版は版なしの gGameMapOverlay.zip (releases/latest/download/gGameMapOverlay.zip で常に最新を取れるように)。
    # ベータは gGameMapOverlay-1.2.3-beta.1.zip、マップ情報は gGameMapOverlay-maps-100.zip
    $zipName = if ($Component -ne 'app') { "gGameMapOverlay-$Component-$Version.zip" }
               elseif ($Version.Contains('-')) { "gGameMapOverlay-$Version.zip" }
               else { 'gGameMapOverlay.zip' }
    $zip = Join-Path $Output $zipName
    if (Test-Path $zip) { Remove-Item $zip }
    Compress-Archive -Path (Join-Path $staging '*') -DestinationPath $zip

    if ($NoManifest) {
        Write-Host "作成しました: $zip (manifest.json は書き換えていません)"
        return
    }

    $manifestPath = Join-Path $Output 'manifest.json'
    $manifest = if (Test-Path $manifestPath) { Get-Content -Raw -Encoding utf8 $manifestPath | ConvertFrom-Json } else { [pscustomobject]@{ schema = 1 } }
    $entry = [pscustomobject]@{
        version = $Version
        url     = if ($BaseUrl) { $BaseUrl.TrimEnd('/') + '/' + $zipName } else { $zipName }
        sha256  = (Get-FileHash -Algorithm SHA256 $zip).Hash.ToLowerInvariant()
        size    = (Get-Item $zip).Length
        notes   = $Notes
    }
    # manifest.json のキーはアプリ (UpdateManifest) に合わせる。マップ情報は "data"
    $key = if ($Component -eq 'maps') { 'data' } else { $Component }
    $manifest | Add-Member -NotePropertyName $key -NotePropertyValue $entry -Force
    [IO.File]::WriteAllText($manifestPath, ($manifest | ConvertTo-Json -Depth 5), [Text.UTF8Encoding]::new($false))

    Write-Host "作成しました: $zip"
    Write-Host "更新しました: $manifestPath"
}
finally {
    if (Test-Path $staging) { Remove-Item -Recurse -Force $staging }
}
