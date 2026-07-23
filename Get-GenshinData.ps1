<#
.SYNOPSIS
    Fetches Genshin Impact player data via Enka.Network API (handles redirects).
.PARAMETER Uid
    Your UID.
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$Uid
)

# Правильный URL — без слеша в конце
$ApiUrl = "https://enka.network/api/uid/$Uid"
$UserAgent = "PowerShell-Genshin-Data/1.0"

Write-Host "Requesting data for UID: $Uid ..." -ForegroundColor Cyan

try {
    # Используем Invoke-WebRequest с разрешением редиректов
    $response = Invoke-WebRequest -Uri $ApiUrl -Method Get -UserAgent $UserAgent -MaximumRedirection 5 -ErrorAction Stop
    $content = $response.Content
    $PlayerData = $content | ConvertFrom-Json
} catch {
    Write-Host "Error: " -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    if ($_.Exception.Response) {
        $statusCode = [int]$_.Exception.Response.StatusCode
        Write-Host "HTTP Status: $statusCode" -ForegroundColor Yellow
    }
    return
}

# Сохраняем полный JSON
$jsonFile = "GenshinData_$Uid.json"
$PlayerData | ConvertTo-Json -Depth 10 | Set-Content -Path $jsonFile -Encoding UTF8
Write-Host "Full data saved to $jsonFile" -ForegroundColor Green

# Вывод информации
if ($PlayerData.playerInfo) {
    $p = $PlayerData.playerInfo
    Write-Host "`n=== PLAYER INFO ===" -ForegroundColor Green
    Write-Host "Name: $($p.nickname)"
    Write-Host "Adventure Rank: $($p.level)"
    Write-Host "World Level: $($p.worldLevel)"
    Write-Host "Signature: $($p.signature)"
}

if (-not $PlayerData.avatarInfoList -or $PlayerData.avatarInfoList.Count -eq 0) {
    Write-Host "No characters in showcase or details hidden." -ForegroundColor Yellow
    Write-Host "Make sure you enabled 'Display Details' for each character." -ForegroundColor Yellow
    return
}

Write-Host "`n=== CHARACTERS ($($PlayerData.avatarInfoList.Count)) ===" -ForegroundColor Green

foreach ($av in $PlayerData.avatarInfoList) {
    Write-Host "`n--- Character ID: $($av.avatarId) ---" -ForegroundColor Magenta
    Write-Host "  Level: $($av.level) | Ascension: $($av.ascension) | Rarity: $($av.rarity)"

    # Weapon
    if ($av.weapon) {
        $w = $av.weapon
        Write-Host "  Weapon: ID=$($w.weaponId) | Level=$($w.level) | Ascension=$($w.ascension) | Refinement=$($w.refinement)"
    } else {
        Write-Host "  Weapon: none"
    }

    # Artifacts
    if ($av.reliquaries -and $av.reliquaries.Count -gt 0) {
        Write-Host "  Artifacts:"
        foreach ($art in $av.reliquaries) {
            $type = switch ($art.reliquaryId % 10) {
                1 { "Flower" }
                2 { "Plume" }
                3 { "Sands" }
                4 { "Goblet" }
                5 { "Circlet" }
                default { "Unknown" }
            }
            Write-Host "    $type : ID=$($art.reliquaryId) | Level=$($art.level) | Rarity=$($art.rarity)"
        }
    } else {
        Write-Host "  Artifacts: none"
    }

    # Статы
    if ($av.fightPropMap) {
        $f = $av.fightPropMap
        Write-Host "  Main Stats:"
        if ($f.'1') { Write-Host "    HP: $($f.'1')" }
        if ($f.'2') { Write-Host "    ATK: $($f.'2')" }
        if ($f.'3') { Write-Host "    DEF: $($f.'3')" }
        if ($f.'20') { Write-Host "    Crit Rate: $($f.'20')%" }
        if ($f.'22') { Write-Host "    Crit DMG: $($f.'22')%" }
        if ($f.'23') { Write-Host "    ER: $($f.'23')%" }
        if ($f.'26') { Write-Host "    EM: $($f.'26')" }
        if ($f.'27') { Write-Host "    Phys Bonus: $($f.'27')%" }
        if ($f.'28') { Write-Host "    Anemo Bonus: $($f.'28')%" }
        if ($f.'29') { Write-Host "    Geo Bonus: $($f.'29')%" }
        if ($f.'30') { Write-Host "    Electro Bonus: $($f.'30')%" }
        if ($f.'31') { Write-Host "    Hydro Bonus: $($f.'31')%" }
        if ($f.'32') { Write-Host "    Pyro Bonus: $($f.'32')%" }
        if ($f.'33') { Write-Host "    Cryo Bonus: $($f.'33')%" }
        if ($f.'34') { Write-Host "    Dendro Bonus: $($f.'34')%" }
    }

    if ($av.constellationList) {
        Write-Host "  Constellations unlocked: $($av.constellationList.Count)"
    }
}

Write-Host "`n=== DONE ===" -ForegroundColor Green